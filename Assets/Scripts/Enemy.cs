using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    GameObject explosion;

    Transform player;

    public float hitpoints;
    public float speed;

    [SerializeField]
    private bool isClose;
    Quaternion startRotation;
    Quaternion toRotation;
    float lerpTime;

	// Use this for initialization
	void Start ()
    {
        startRotation = transform.rotation;
        //To-Do: move player detection to Radar-Like stuff
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player);
        toRotation = transform.rotation;
        transform.rotation = startRotation;
        lerpTime = 0;
	}

    void FixedUpdate()
    {
        lerpTime += Time.fixedDeltaTime;
        if(player != null)
        {
            if (Vector3.Distance(transform.position, player.position) < 100 && !isClose)
            {
                isClose = true;
                StartCoroutine(close());

                startRotation = transform.rotation;
                transform.LookAt(player);
                transform.Rotate(transform.right, 20);
                toRotation = transform.rotation;

                transform.rotation = startRotation;
                lerpTime = 0;
            }

            //rotate the other direction if enemy is close to the player
            if (isClose)
            {
                transform.rotation = Quaternion.Lerp(startRotation, toRotation, lerpTime);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(startRotation, toRotation, lerpTime);
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
        
    }

    //Collision Detection
    private void OnTriggerEnter(Collider other)
    {
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
            startRotation = transform.rotation;
            transform.LookAt(player);
            toRotation = transform.rotation;
            transform.rotation = startRotation;

            lerpTime = 0;
        }
    }
}
