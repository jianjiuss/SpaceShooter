using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shot;
    public Transform shotSpan;

    public float fireRate;
    private float nextFire = 1;

    public CameraView cameraView;

    public JoystickController joystick;

    private void Update()
    {
        if (fire)
        {
            Fire();
        }

        boundary.xMin = cameraView.lowerCorners[0].x;
        boundary.xMax = cameraView.lowerCorners[1].x;
        boundary.zMax = cameraView.lowerCorners[0].z;
        boundary.zMin = cameraView.lowerCorners[2].z;
    }

    public void Fire()
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpan.position, shotSpan.rotation);
            this.GetComponent<AudioSource>().Play();
        }
    }

    private bool fire = false;
    public void ButtonDown()
    {
        fire = true;
    }

    public void ButtonUp()
    {
        fire = false;
    }

    private void FixedUpdate()
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();

        float moveHorizontal = joystick.GetAxis("axisX");
        float movoVertical = joystick.GetAxis("axisY");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, movoVertical);
        rigidbody.velocity = movement * speed;

        rigidbody.position = new Vector3
        (
            Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
        );

        rigidbody.rotation = Quaternion.Euler(0.0f , 0.0f, rigidbody.velocity.x * -tilt);
    }
}
