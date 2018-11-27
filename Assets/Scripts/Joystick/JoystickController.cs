using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour
{

    private GameObject handler;
    private RectTransform handlerTrans;
    private RectTransform joystickTrans;

    private float MaxHorizontal;
    private float MinHorizontal;

    private float MaxVertical;
    private float MinVertical;

    private Vector2 output;

    private int touchIndex = -1;

    private Canvas canvas;
    private RectTransform canvasRect;

    private void Start () {
        handler = GameObject.Find("Handler");
        handlerTrans = handler.GetComponent<RectTransform>();
        joystickTrans = GetComponent<RectTransform>();

        MaxHorizontal = joystickTrans.rect.width / 2;
        MinHorizontal = -MaxHorizontal;

        MaxVertical = joystickTrans.rect.height / 2;
        MinVertical = -MaxVertical;

        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.gameObject.GetComponent<RectTransform>();
	}

    public void HandleDragBegan(BaseEventData baseEventData)
    {
        var rect = new Rect(joystickTrans.anchoredPosition - new Vector2(joystickTrans.rect.size.x / 2, joystickTrans.rect.size.y / 2), joystickTrans.rect.size);

        for(int i = 0; i < Input.touchCount; i ++)
        {
            var touch = Input.GetTouch(i);
            var actualPos = GetPosInCanvasPos(touch.position);
            if (rect.Contains(actualPos))
            {
                touchIndex = i;
            }
        }
    }

    public void HandleDrag(BaseEventData baseEventData)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (touchIndex == -1)
        {
            return;
        }
#endif

        Vector3 movePos = Vector3.zero;

#if UNITY_EDITOR
        movePos = GetPosInCanvasPos(Input.mousePosition);
#elif UNITY_ANDROID
        movePos = GetPosInCanvasPos(Input.GetTouch(touchIndex).position);
#endif

        var dis = Vector3.Distance(movePos, joystickTrans.anchoredPosition);
        if (Vector3.Distance(movePos, joystickTrans.anchoredPosition) < MaxHorizontal)
        {
            handlerTrans.anchoredPosition = (Vector2)movePos - joystickTrans.anchoredPosition;
        }
        else
        {
            var dragPosRelativeToPivot1 = movePos - (Vector3)joystickTrans.anchoredPosition;
            handlerTrans.anchoredPosition = dragPosRelativeToPivot1.normalized * MaxHorizontal;
        }

        Vector2 tempOutput = new Vector2(handlerTrans.anchoredPosition.x / MaxHorizontal, handlerTrans.anchoredPosition.y / MaxVertical);

        output = CircleToSquare(tempOutput);
    }

    private Vector2 GetPosInCanvasPos(Vector2 input)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, input, canvas.worldCamera, out pos);
        var x = canvasRect.rect.width / 2;
        var y = canvasRect.rect.height / 2;
        return new Vector2(x, y) + pos;
    }

    public void HandleDragEnd(BaseEventData baseEventData)
    {
        handlerTrans.anchoredPosition = Vector3.zero;
        output = Vector2.zero;
        touchIndex = -1;
    }

    public float GetAxis(string axisName)
    {
        if(axisName.Equals("axisX"))
        {
            return output.x;
        }
        else if(axisName.Equals("axisY"))
        {
            return output.y;
        }
        else
        {
            return 0;
        }
    }

    private Vector2 CircleToSquare(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = (Mathf.Sqrt(2 + Mathf.Pow(input.x, 2) - Mathf.Pow(input.y, 2) + 2 * Mathf.Sqrt(2) * input.x) / 2) - (Mathf.Sqrt(2 + Mathf.Pow(input.x, 2) - Mathf.Pow(input.y, 2) - 2 * Mathf.Sqrt(2) * input.x) / 2);
        output.y = (Mathf.Sqrt(2 - Mathf.Pow(input.x, 2) + Mathf.Pow(input.y, 2) + 2 * Mathf.Sqrt(2) * input.y) / 2) - (Mathf.Sqrt(2 - Mathf.Pow(input.x, 2) + Mathf.Pow(input.y, 2) - 2 * Mathf.Sqrt(2) * input.y) / 2);

        return output;
    }
}
