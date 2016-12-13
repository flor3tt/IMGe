using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public float speed;
    float spawnTime;
	// Use this for initialization
	void Start ()
    {
        //transform.RotateAround(transform.position, transform.right, 90);
        //transform.RotateAround(transform.position, transform.up, 180);
        transform.Rotate(Vector3.right, 90);
        spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Destroy after 20 seconds
        if(Time.time - spawnTime > 20)
        {
            Destroy(gameObject);
        }

        //Move
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
