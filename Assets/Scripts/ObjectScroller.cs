using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScroller : MonoBehaviour {

    public float terrainSpeed = 15f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        foreach (Transform child in transform)
        {
            child.transform.Translate(terrainSpeed * Time.deltaTime, 0f, 0f);

            if (child.transform.position.x > 200)
            {
                Destroy(child.gameObject, 2f);
            }
        }


        
    }
}
