using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    GameObject explosion;

    public float hitpoints;

	// Use this for initialization
	void Start ()
    {
        hitpoints = 100;
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
}
