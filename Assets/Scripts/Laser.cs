using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public float speed;

	// Use this for initialization
	void Start ()
    {
        transform.Rotate(Vector3.right, 90);

        //Destroy after 15 sec
        Destroy(gameObject, 15);
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Move
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
