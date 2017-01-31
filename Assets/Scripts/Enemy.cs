using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    GameObject explosion;

    public Transform player;
    private float playerSpeed;

    //Laser Prefab
    public GameObject laserPrefab;

    public float hitpoints = 50;
    public float speed = 75;

    //Flight control
    public Vector3 searchAngles;
    bool isClose;
    Quaternion currentRotation;
    Quaternion toRotation;
    public float lerpTime;

    //Weapon control
    public float laserCooldown = 1;
    float remainingLaserCooldown;

    // Use this for initialization
    void Start ()
    {
        currentRotation = transform.rotation;
        //zufällige Winkel für eine runde Flugbahn um den Spieler zu "suchen"
        searchAngles.x = Random.Range(0, 20);
        searchAngles.y = Random.Range(0, 20);

        /**
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player.position - player.forward * playerSpeed);
        toRotation = transform.rotation;
        transform.rotation = currentRotation;
        lerpTime = 0;
        */
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
            player.gameObject.GetComponentInParent<Player>().increaseScore(50);
            Destroy(gameObject);

            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        //Move
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        //Different behaviour for having (not) found the player
        if(player == null)
        {
            transform.Rotate(searchAngles * Time.deltaTime);
            LayerMask mask = 8;
            RaycastHit hit;

            Quaternion up = Quaternion.AngleAxis(5, Vector3.right);
            Quaternion down = Quaternion.AngleAxis(-5, Vector3.right);
            Quaternion right = Quaternion.AngleAxis(5, Vector3.up);
            Quaternion left = Quaternion.AngleAxis(-5, Vector3.up);
            Vector3 vector = new Vector3(0, 0, 0);
            vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;

            //Cast a bunch of rays in a cone forward to find the player
            if (Physics.Raycast(transform.position,                 transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, up *            transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, up * right *    transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, right *         transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, down * right *  transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, down *          transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, down * left *   transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, left *          transform.forward, out hit, 300) ||
                Physics.Raycast(transform.position, up * left *     transform.forward, out hit, 300) )
            {
                player = hit.transform;
            }
            else
            {
            }
            /**
            //Raycast debug:
            Debug.DrawRay(transform.position, transform.forward * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (up * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (up * right * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (right * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (down * right * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (down * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (down * left * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (left * transform.forward) * 300, Color.blue, 1);
            Debug.DrawRay(transform.position, (up * left * transform.forward) * 300, Color.blue, 1);
            */
        }
        else
        {
            //Different behaviour for flying towards / away from the player
            if (isClose)
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
        }

        //Handle Weapon Cooldowns
        remainingLaserCooldown -= Time.deltaTime;
    }

    //Collision Detection
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
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
