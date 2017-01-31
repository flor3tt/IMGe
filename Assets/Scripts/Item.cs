using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public int itemType = 0;
    
    public Transform player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(player, player.up);
	}

    void OnTriggerEnter(Collider other)
    {  
        if(other.tag == "Player")
        {
            other.gameObject.SendMessageUpwards("applyItem", itemType, SendMessageOptions.RequireReceiver);
            Destroy(this.gameObject);
        }
    }
}
