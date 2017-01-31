using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField]
    Transform player;
    
    //Enemie Control
    public GameObject enemyPrefab;
    private Enemy[] enemies;
    public float enemySpawnCooldown;
    private float remainingSpawnCooldown;
    

    //Item Control
    public GameObject[] items;
    private Item[] existingItems;

	// Use this for initialization
	void Start () {
        enemies = FindObjectsOfType<Enemy>();
        remainingSpawnCooldown = enemySpawnCooldown;
    }
	
	// Update is called once per frame
	void Update () {
        //Spawn and Destroy Enemies and items
        enemies = FindObjectsOfType<Enemy>();
        existingItems = FindObjectsOfType<Item>();
        foreach (Enemy e in enemies)
        {
            if (Vector3.Distance(transform.position, e.transform.position) > 5000)
            {
                Destroy(e);
            }
        }
        foreach (Item e in existingItems)
        {
            if (Vector3.Distance(transform.position, e.transform.position) > 5000)
            {
                Destroy(e);
            }
        }
        if (remainingSpawnCooldown <= 0)
        {
            remainingSpawnCooldown = enemySpawnCooldown;

            //spawn new enemy
            GameObject spawnedEnemy = Instantiate(enemyPrefab, transform.position + transform.forward * Random.Range(-1000, 1000) + transform.right * Random.Range(-1000, 1000) + transform.up * Random.Range(-1000, 1000), transform.rotation);


            //spawn random item
            GameObject spawnedItem = Instantiate(items[Random.Range(0, items.Length)], transform.position + transform.forward * Random.Range(-300, 300) + transform.right * Random.Range(-300, 300) + transform.up * Random.Range(-300, 300), transform.rotation);
            spawnedItem.GetComponent<Item>().player = this.player;
        }
        else
        {
            remainingSpawnCooldown -= Time.deltaTime;
        }


    }


}
