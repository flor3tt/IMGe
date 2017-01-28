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

    //Radar
    public Texture radarBG;
    public Texture radarDot;
    public Texture radarDotUp;
    public Texture radarDotDown;

    //Enemy control
    private Enemy[] enemies;
    public GameObject enemyPrefab;
    public float spawnCooldown;
    private float remainingSpawnCooldown;

    // Use this for initialization
    void Start ()
    {


        hitpoints = 100;
        shieldpoints = 200;
        shield = GameObject.FindGameObjectWithTag("Shield");
        enemies = FindObjectsOfType<Enemy>();
        remainingSpawnCooldown = spawnCooldown;
	}

    //FixedUpdate is called in fixed time intervals
    void FixedUpdate()
    {
        speed += (0.5f - controller.getSlider1()) * 2 * maxAcceleration;
        speed = Mathf.Min(speed, maxSpeed);
        speed = Mathf.Max(speed, 0);

        //Adjust camera FOV
        Camera cam = FindObjectOfType<Camera>();
        cam.fieldOfView = 60 + 20 * speed / maxSpeed;
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

        //Spawn and Destroy Enemies
        enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy e in enemies)
        {
            if(Vector3.Distance(transform.position, e.transform.position) > 5000)
            {
                Destroy(e);
            }
        }
        if(remainingSpawnCooldown <= 0)
        {
            remainingSpawnCooldown = spawnCooldown;

            //spawn new enemy
            Instantiate(enemyPrefab, transform.position + transform.forward * Random.Range(-1000, 1000) + transform.right * Random.Range(-1000, 1000) + transform.up * Random.Range(-1000, 1000), transform.rotation);
        }
        else
        {
            remainingSpawnCooldown -= Time.deltaTime;
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

        GUI.DrawTexture(new Rect(Screen.width - 110, 10, 100, 100), radarBG);

        //Enemy position to radar point transformation
        Plane flightPlane = new Plane(transform.up, transform.position);

        foreach(Enemy e in enemies)
        {
            float height;//height of enemy BELOW player, because sending the Raycast along the enemey.transform.up axis
            float distance = Vector3.Distance(transform.position, e.transform.position);
            Vector3 projection = Vector3.ProjectOnPlane(e.transform.position - transform.position, transform.up);
            float angle1 = Vector3.Angle(transform.forward, projection);
            float angle2 = Vector3.Angle(transform.right, projection);
            if(angle2 >= 90)
            {
                angle1 = 360 - angle1;
            }
            angle1 -= 90;//for simpler sin/cos use

            Ray ray = new Ray(e.transform.position, e.transform.up);
            
            flightPlane.Raycast(ray, out height);
                        
            float xPos = Screen.width - 65 + Mathf.Cos(angle1*Mathf.Deg2Rad) * 50 * Mathf.Min(1, distance / 400);
            float yPos = 55 + Mathf.Sin(angle1 * Mathf.Deg2Rad) * 50 * Mathf.Min(1, distance / 400);

            if(height / distance < -0.25f)
            {
                GUI.DrawTexture(new Rect(xPos, yPos, 10, 10), radarDotUp);
            }
            else if(height / distance > 0.25f)
            {
                GUI.DrawTexture(new Rect(xPos, yPos, 10, 10), radarDotDown);
            }
            else
            {
                GUI.DrawTexture(new Rect(xPos, yPos, 10, 10), radarDot);
            }
        }

        
        
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
