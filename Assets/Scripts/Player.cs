using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    //Controller Object
    public ControllerScript controller;

    //Shield
    public GameObject shield;

    //Laser Prefab
    public GameObject laserPrefab;

    //Public control variables
    public float speed;
    public float maxSpeed;
    public float maxAcceleration;
    public float rotationSpeed;
    public float laserCooldown;


    //Private control variables
    float remainingLaserCooldown;
    float hitpoints;
    float shieldpoints;

    // Use this for initialization
    void Start ()
    {
        hitpoints = 100;
        shieldpoints = 200;
        shield = GameObject.FindGameObjectWithTag("Shield");
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
        //Check Game Over
        if(hitpoints <= 0)
        {
            Destroy(gameObject);

            //ToDo Game Over Scene
        }

        //(de-)aktivate shield Renderer
        if(shield != null)
        {
            shield.GetComponent<Renderer>().enabled = shieldpoints > 0;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        /**
         * Rotation ohne Accelerometer
        //Rotation Horizontal
        if(controller.getButton5())
        {
            transform.Rotate(Vector3.up, -1 * rotationSpeed * Time.deltaTime);
        }
        if(controller.getButton6())
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        //Rotation Vertikal

        transform.Rotate(Vector3.right, (0.5f - controller.getSlider2smooth()) * -1 * rotationSpeed * Time.deltaTime);
        */

        //Rotation mit Accelerometer
        float accelX = controller.getAccelXsmooth();
        float accelY = controller.getAccelYsmooth();


        float rotVertikal = accelX * rotationSpeed * Time.deltaTime;
        float rotHorizontal = accelY * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.right, rotVertikal);
        transform.Rotate(Vector3.up, rotHorizontal);


        //Shoot da LAAASOOOOOOOR
        if (controller.getButton2())
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

    void OnGUI()
    {
        GUI.Label(new Rect(10, 0, 100, 20), "HitPoints:\t" + hitpoints);
        GUI.Label(new Rect(10, 20, 100, 20), "Shield:\t" + shieldpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        float damage = 0;
        if(other.tag == "Enemy")
        {
            damage = 100;
        }
        else if(other.tag == "Laser")
        {
            damage = 25;
            Destroy(other.gameObject);
        }

        if(shieldpoints > 0)
        {
            shieldpoints = Mathf.Max(0, shieldpoints - damage);
        }
        else
        {
            hitpoints -= damage;
        }
    }
}
