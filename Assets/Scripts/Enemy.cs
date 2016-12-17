using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    GameObject explosion;

    Transform player;
    private float playerSpeed;

    //Laser Prefab
    public GameObject laserPrefab;

    public float hitpoints;
    public float speed;
    
    //Flight control
    private bool isClose;
    Quaternion currentRotation;
    Quaternion toRotation;
    public float lerpTime;

    //Weapon control
    public float laserCooldown;
    float remainingLaserCooldown;

    // Use this for initialization
    void Start ()
    {
        currentRotation = transform.rotation;
        //To-Do: move player detection to Radar-Like stuff
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player.position - player.forward * playerSpeed);
        toRotation = transform.rotation;
        transform.rotation = currentRotation;
        lerpTime = 0;
	}

    void FixedUpdate()
    {
                
        lerpTime += Time.fixedDeltaTime;
        if(player != null)
        {
            playerSpeed = player.gameObject.GetComponentInParent<Player>().speed;

            currentRotation = transform.rotation;

            /**
             * Fliegt auf einen Punkt vor dem Spieler zu, abhängig von der Geschwindigkeit des Spielers
             * Ab einer Distanz von 75 beginnt ein Ausweichmanöver, das knapp über den Spieler hinwegführt
             * 
             */
            if (Vector3.Distance(transform.position, player.transform.position) < 100 && !isClose)
            {
                isClose = true;
                StartCoroutine(close());
                lerpTime = 0;
                
                transform.LookAt(player.position - player.forward * playerSpeed);
                transform.Rotate(Vector3.right, -10);
                toRotation = transform.rotation;

                transform.rotation = currentRotation;
            }

            if(isClose)
            {
                transform.rotation = Quaternion.Lerp(currentRotation, toRotation, lerpTime / 5);
            }
            else
            {
                transform.LookAt(player.position - player.forward * playerSpeed);
                toRotation = transform.rotation;

                transform.rotation = currentRotation;

                transform.rotation = Quaternion.Lerp(currentRotation, toRotation, lerpTime / 30);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Check Death
		if(hitpoints <= 0)
        {
            //ToDo INstantiate Explosion
            Destroy(gameObject);

            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        //Move
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        //Different behaviour for flying towards / away from the player
        if(isClose)
        {
        }
        else
        {
            //Shoot while actually flying towards the player, NOT while still turning
            if (Mathf.Abs(Quaternion.Angle(transform.rotation, toRotation)) < 15)
            {
                if (remainingLaserCooldown <= 0)
                {
                    remainingLaserCooldown = laserCooldown;
                    Instantiate(laserPrefab, transform.position + transform.forward * 2 + transform.right * 0.35f - transform.up * 1f, transform.rotation);
                    Instantiate(laserPrefab, transform.position + transform.forward * 2 - transform.right * 0.35f - transform.up * 1f, transform.rotation);
                }
            }
        }

        //Handle Weapon Cooldowns
        remainingLaserCooldown -= Time.deltaTime;
    }

    //Collision Detection
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Laser")
        {
            hitpoints -= 25;
            Destroy(other.gameObject);
        }
        else if(other.tag == "Enemy")
        {
            hitpoints = 0;
        }
        else if(other.tag == "Player")
        {
            hitpoints = 0;
        }
    }

    public IEnumerator close()
    {
        yield return new WaitForSeconds(5f);
        isClose = false;

        if(player != null)
        {
            currentRotation = transform.rotation;
            transform.LookAt(player.position - player.forward * playerSpeed);
            toRotation = transform.rotation;
            transform.rotation = currentRotation;

            lerpTime = 0;
        }
    }
}
