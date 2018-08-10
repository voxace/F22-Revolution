using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour {

    Rigidbody rb;
    float startTime;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-5f, 0.5f, 0f);
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        float duration = 2f;
        float timeSinceStart = Time.time - startTime;
        float offset = timeSinceStart / duration;

        Vector3 velocity = Vector3.Lerp(new Vector3(-5f, 0.5f, 0f), new Vector3(-20f, 0f, 0f), offset);
        rb.velocity = velocity;
    }
}
