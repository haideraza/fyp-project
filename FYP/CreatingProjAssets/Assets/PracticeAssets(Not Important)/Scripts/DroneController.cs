using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float vertical_force, horizontal_force;
    public float fan_speed;
    public Rigidbody rb;
    public Joystick joystick1;
    public Joystick joystick2;
    private GameObject[] fans;
    private float fan_speed_incrementer;
    private bool drone_started=false;
    private Vector3 initial_drone_position;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fan_speed_incrementer = fan_speed;
        initial_drone_position = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

            if (drone_started)
            {
                if (fan_speed_incrementer < 1997f)
                {
                    fan_speed_incrementer += 3;
                }

                else
                {
                    DroneControls();
                }
                
            }

            else
            {
                if (fan_speed_incrementer >= 0)
                {
                    fan_speed_incrementer -= 3;
                }
                
            }

            RotateFans(fan_speed_incrementer);

            transform.rotation=Quaternion.identity;

        
    }


    public void ResetDrone() {

        transform.position = initial_drone_position;
        transform.rotation = Quaternion.identity;
        drone_started = false;
    }


    public void ToggleOn()
    {

        drone_started = true;

    }

    public void ToggleOff()
    {

        drone_started = false;

    }

  

    void DroneControls() {

        if(joystick1.Vertical>=0.2f)
        {
            rb.AddForce(transform.forward * horizontal_force);

        }

        if (joystick1.Vertical <= -0.2f)
        {
            rb.AddForce(-transform.forward * horizontal_force);

        }

        if (joystick1.Horizontal >= 0.2f)
        {

            rb.AddForce(transform.right * horizontal_force);

        }

        if (joystick1.Horizontal <= -0.2f)
        {

            rb.AddForce(-transform.right * horizontal_force);

        }

        if (joystick2.Vertical >= 0.2f)
        {
            rb.AddForce(transform.up * vertical_force);

        }

        if (joystick2.Vertical <= -0.2f)
        {
            rb.AddForce(-transform.up * vertical_force);

        }

    }


    void RotateFans(float speed) {

        fans = GameObject.FindGameObjectsWithTag("RotateFan");

        foreach (GameObject fan in fans)
        {
            fan.transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }

    }
}
