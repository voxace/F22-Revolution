using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    public GameObject terrain1;
    public GameObject terrain2;
    public float terrainSpeed = 15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        terrain1.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);
        terrain2.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);

        if (terrain1.transform.position.x > 100)
        {
            terrain1.transform.position = new Vector3(-100f, 0f, 0f);
        }
        if (terrain2.transform.position.x > 100)
        {
            terrain2.transform.position = new Vector3(-100, 0, 0f);
        }

    }
}
