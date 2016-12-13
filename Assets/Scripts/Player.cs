using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //Controller Object
    public ControllerScript controller;

    //Laser Prefab
    public GameObject laserPrefab;

    //Public control vriables
    public float speed;
    public float maxSpeed;
    public float maxAcceleration;
    public float rotationSpeedHorizontal;
    public float rotationSpeedVertikal;
    public float laserCooldown;


    //Private control variables
    float remainingLaserCooldown;

    // Use this for initialization
    void Start ()
    {
	}

    //FixedUpdate is called in fixed time intervals
    void FixedUpdate()
    {
        speed += (0.5f - controller.getSlider1()) * 2 * maxAcceleration;
        speed = Mathf.Min(speed, maxSpeed);
        speed = Mathf.Max(speed, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);

        //Rotation Horizontal
        if(controller.getButton5())
        {
            transform.Rotate(Vector3.up, -1 * rotationSpeedHorizontal * Time.deltaTime);
        }
        if(controller.getButton6())
        {
            transform.Rotate(Vector3.up, rotationSpeedHorizontal * Time.deltaTime);
        }
        //Rotation Vertikal
        transform.Rotate(Vector3.right, (0.5f - controller.getSlider2smooth()) * -1 * rotationSpeedVertikal * Time.deltaTime);
        
        //Shoot da LAAASOOOOOOOR
        if(controller.getButton2())
        {
            if(remainingLaserCooldown <= 0)
            {
                remainingLaserCooldown = laserCooldown;
                Instantiate(laserPrefab, transform.position + transform.right * 1.8f + transform.up * 0.8f, transform.rotation);
                Instantiate(laserPrefab, transform.position - transform.right * 1.8f + transform.up * 0.8f, transform.rotation);
            }
        }

        //Handle Weapon Cooldowns
        remainingLaserCooldown -= Time.deltaTime;
    }
}
