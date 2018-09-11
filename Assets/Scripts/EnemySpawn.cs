using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public GameObject interceptor;
    private List<SpawnInfo> spawnItems = new List<SpawnInfo>();
    private SpawnInfo currentSpawn;
    //private int spawnIndex = 0;

	// Use this for initialization
	void Start () {
        spawnItems.Add(new SpawnInfo(interceptor, 1f, -1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 1f, 1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 3f, -2f));
        spawnItems.Add(new SpawnInfo(interceptor, 3f, 2f));
        spawnItems.Add(new SpawnInfo(interceptor, 5f, -3f));
        spawnItems.Add(new SpawnInfo(interceptor, 5f, 3f));
        spawnItems.Add(new SpawnInfo(interceptor, 7f, -1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 7f, 1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 9f, -2f));
        spawnItems.Add(new SpawnInfo(interceptor, 9f, 2f));
        spawnItems.Add(new SpawnInfo(interceptor, 11f, -3f));
        spawnItems.Add(new SpawnInfo(interceptor, 11f, 3f));
        spawnItems.Add(new SpawnInfo(interceptor, 13f, -1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 13f, 1.5f));
        spawnItems.Add(new SpawnInfo(interceptor, 15f, -2f));
        spawnItems.Add(new SpawnInfo(interceptor, 15f, 2f));
        spawnItems.Add(new SpawnInfo(interceptor, 17f, -3f));
        spawnItems.Add(new SpawnInfo(interceptor, 17f, 3f));
    }
	
	// Update is called once per frame
	void Update () {

        float integerTime = Mathf.FloorToInt(Time.time);
        currentSpawn = spawnItems[0];

        if(currentSpawn.spawnTime == integerTime)
        {
            // Create the Bullet from the Bullet Prefab
            GameObject spawnItem = (GameObject)Instantiate(
                currentSpawn.itemToSpawn,
                new Vector3(-200f,0f,currentSpawn.zpos),
                new Quaternion(0f, 0f, 0f, 0f));           

            spawnItems.RemoveAt(0);
        }
        
    }    

}

class SpawnInfo
{
    public GameObject itemToSpawn;
    public float spawnTime;
    public float zpos;

    public SpawnInfo(GameObject itemToSpawn, float spawnTime, float zpos)
    {
        this.itemToSpawn = itemToSpawn;
        this.spawnTime = spawnTime;
        this.zpos = zpos;
    }
}
