using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    public GameObject terrain1;
    public GameObject terrain2;
    public GameObject buildings1;
    public GameObject buildings2;
    public GameObject terrain3;
    public float terrainSpeed = 15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        terrain1.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);
        terrain2.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);
        terrain3.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);

        if (terrain1.transform.position.x > 200)
        {
            terrain1.transform.position = new Vector3(-200f, 0f, 0f);
        }
        if (terrain2.transform.position.x > 200)
        {
            terrain2.transform.position = new Vector3(-200, 0, 0f);
        }
        if (terrain3.transform.position.x > 200)
        {
            terrain3.transform.position = new Vector3(-200, 0, 0f);
        }

        buildings1.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);
        buildings2.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);

        if (buildings1.transform.position.x > 300)
        {
            buildings1.transform.position = new Vector3(-300, 0, 0f);
        }
        if (buildings2.transform.position.x > 300)
        {
            buildings2.transform.position = new Vector3(-300, 0, 0f);
        }

    }
}
