using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptorScript : MonoBehaviour {

    Rigidbody rb;
    public GameObject explosion1;
    public GameObject explosion2;
    public int health = 100;
    int startTime;
    bool isDead = false;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Mathf.FloorToInt(Time.time);
        rb.velocity = new Vector3(20f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
        int integerTime = Mathf.FloorToInt(Time.time);
        int timeSinceSpawn = integerTime - startTime;

        switch (timeSinceSpawn)
        {
            case 2:
                rb.velocity = new Vector3(20f, 0f, 2f);
                break;
            case 4:
                rb.velocity = new Vector3(20f, 0f, -4f);
                break;
            case 6:
                rb.velocity = new Vector3(20f, 0f, 2f);
                break;
            case 8:
                rb.velocity = new Vector3(20f, 0f, 0f);
                break;
        }

        if (transform.position.x > 0)
        {
            Destroy(this.gameObject, 0.0f);
        }

        if(health <= 0 && isDead == false)
        {
            GameObject explosion01 = (GameObject)Instantiate(
                explosion1,
                transform.position,
                new Quaternion(0, 0f, 0f, 0));
            explosion01.GetComponent<ParticleSystem>().Play();
            Destroy(explosion01, 5.0f);

            GameObject explosion02 = (GameObject)Instantiate(
                explosion2,
                transform.position,
                new Quaternion(0, 0f, 0f, 0));
            explosion02.GetComponent<ParticleSystem>().Play();
            Destroy(explosion02, 5.0f);

            rb.AddExplosionForce(1000f, transform.position, 20f);

            isDead = true;
            rb.useGravity = true;
            Destroy(this.gameObject, 3.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= 4;
            Destroy(collision.collider.gameObject, 0f);
        }
        if (collision.gameObject.tag == "Missile")
        {
            health = 0;
            Destroy(collision.collider.gameObject, 0f);
        }
    }


    
}
