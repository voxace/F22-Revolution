using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileScript : MonoBehaviour {

    Rigidbody rb;
    float startTime;
    public float startVelocity = 20f;
    public float endVelocity = 50f;
    public GameObject explosion;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        float duration = 2f;
        float timeSinceStart = Time.time - startTime;
        float offset = timeSinceStart / duration;

        Vector3 velocity = Vector3.Lerp(new Vector3(startVelocity, 0f, 0f), new Vector3(endVelocity, 0f, 0f), offset);
        transform.position = Vector3.Lerp(new Vector3(transform.position.x, -0.3f, transform.position.z), new Vector3(transform.position.x, 0.6f, transform.position.z), offset);
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject explosion01 = (GameObject)Instantiate(
                explosion,
                transform.position,
                new Quaternion(0, 0f, 0f, 0));
            explosion01.GetComponent<ParticleSystem>().Play();
            explosion01.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Destroy(explosion01, 5.0f);
            Destroy(this.gameObject, 0.1f);
        }
    }
}
