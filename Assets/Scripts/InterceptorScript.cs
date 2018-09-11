using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptorScript : MonoBehaviour {

    Rigidbody rb;
    public GameObject explosion1;
    public GameObject explosion2;
    public GameObject missilePrefab;
    public Transform missileSpawnLocation;
    public AudioSource explosionSound;
    public int health = 100;
    int startTime;
    bool isDead = false;
    bool missileLaunched = false;

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

        // Gives the aircraft its flight pattern
        FlightPattern(timeSinceSpawn);

        // Shoot a missile at a certain point in the map
        if (transform.position.x > -60f && missileLaunched == false)
        {
            spawnMissile();
        }

        // Destroy aircraft after it goes past map bounds
        if (transform.position.x > 0)
        {
            Destroy(this.gameObject, 0.0f);
        }

        // Explode aircraft when health reaches 0
        if (health <= 0 && isDead == false)
        {
            ExplodeAircraft();
        }
    }

    private void FlightPattern(int timeSinceSpawn)
    {
        if (!isDead)
        {
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
        }
    }

    private void ExplodeAircraft()
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

        rb.AddForce(new Vector3(0f, 100f, 0f));        

        rb.AddExplosionForce(
            UnityEngine.Random.Range(800f, 1600f), 
            transform.position - new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
            UnityEngine.Random.Range(0.5f, 1.5f), 
            UnityEngine.Random.Range(-0.1f, 0.1f)), 20f);

        isDead = true;
        rb.useGravity = true;
        explosionSound.Play();
        Destroy(this.gameObject, 4f);
    }

    private void spawnMissile()
    {
        missileLaunched = true;

        // Create the Bullet from the Bullet Prefab
        GameObject bullet = (GameObject)Instantiate(
            missilePrefab,
            missileSpawnLocation.position + new Vector3(5f,-0.3f,0f),
            new Quaternion(0, 0f, 0f, 0));

        // Destroy the bullet after 10 seconds
        Destroy(bullet, 10.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<BulletScript>().damage;
            Destroy(collision.collider.gameObject, 0f);
        }
        if (collision.gameObject.tag == "Missile")
        {
            health = 0;
            Destroy(collision.collider.gameObject, 0f);
        }
        if (collision.gameObject.tag == "Player")
        {
            health = 0;            
        }
    }

}
