using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class F22Controller : MonoBehaviour {

    #region Variables

    // Game variables
    public int health = 100;

    // Physics variables
    public float maxRoll = 30f;
    public float maxRollSpeed = 1f;
    public float yawSpeed = 1f;
    public float boundaryDamper = 1f;
    public float maxRollDamper = 1f;
    public float boundWidth = 6f;
    float speedMultiplier = -0.1f;

    // Flaps
    public GameObject leftInnerFlap;
    public GameObject leftOuterFlap;
    public GameObject rightInnerFlap;
    public GameObject rightOuterFlap;

    // Weapons
    public GameObject missilePrefab;
    public Transform missileSpawnLocation;
    public GameObject bulletPrefab;

    // Timing
    public float keyDelay = 0.1f;
    public float missileDelay = 0.6f;
    private float timePassed = 0f;
    private float timePassedMissile = 0f;

    // Sounds
    public AudioSource hit;
    public AudioSource enemyHit;
    public AudioSource missileSound;
    public AudioSource minigun;
    public AudioSource thrusters;

    #endregion

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        // Timing
        timePassed += Time.deltaTime;
        timePassedMissile += Time.deltaTime;

        // Input / Control
        MissileControl();
        BulletControl();
        MovementController();
    }

    private void BulletControl()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (timePassed >= keyDelay)
            {
                FireBullets();
                timePassed = 0f;
            }
            if (!minigun.isPlaying)
            {
                minigun.Play();
            }
        }
    }

    private void MissileControl()
    {
        if (Input.GetKeyDown(KeyCode.Space) && timePassedMissile >= missileDelay)
        {
            Fire();
            timePassedMissile = 0f;
        }
    }

    private void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        GameObject missile = (GameObject)Instantiate(
            missilePrefab,
            missileSpawnLocation.position,
            new Quaternion(90f,0f,0f,90f));

        missileSound.Play();

        // Destroy the bullet after 10 seconds
        Destroy(missile, 7.0f);
    }

    private void FireBullets()
    {
        // Create the Bullet from the Bullet Prefab
        GameObject bullet = (GameObject)Instantiate(
            bulletPrefab,
            missileSpawnLocation.position,
            new Quaternion(0, 0f, 0f, 0));

        // Destroy the bullet after 10 seconds
        Destroy(bullet, 5.0f);
    }

    private void MovementController()
    {
        // Get cross-platform input
        float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        // Play Thruster Sounds
        ThrusterSounds(horizontalThrow);

        // Calculate rotation
        float roll;
        Vector3 rotation;
        transform.rotation.ToAngleAxis(out roll, out rotation);

        // Animate flaps
        AnimateFlaps(roll, rotation);

        // Limit amount of roll
        LimitRollAmount(roll);

        // Dimish amount of roll after certain X positions until zero
        // This will then slow down movement at the edges and restrict f22 to the map
        ReduceRollAtEdges(horizontalThrow);

        // Horizontal speed determined by amount of roll
        // Damper settings also affect the speed here
        CalculateHorizontalSpeed(horizontalThrow, roll, rotation);

        // Re-centre F22 slowly if it is near edges
        RecentreF22(horizontalThrow, roll, rotation);
    }

    private void RecentreF22(float horizontalThrow, float roll, Vector3 rotation)
    {
        if ((horizontalThrow < 0.25f && horizontalThrow > -0.25f) || boundaryDamper < 0.5f || maxRollDamper < 0.5f)
        {
            transform.Rotate(Vector3.right, -roll * rotation.x * Time.deltaTime * yawSpeed);
            transform.Translate(0f, 0f, (-transform.position.z / 3f) * Time.deltaTime, Space.World);
        }
    }

    private void CalculateHorizontalSpeed(float horizontalThrow, float roll, Vector3 rotation)
    {
        if (transform.position.z < 5 && roll > 0f)
        {
            if (horizontalThrow > 0f)
            {
                transform.Translate(0f, 0f, yawSpeed * -roll * rotation.x * Time.deltaTime * boundaryDamper * speedMultiplier, Space.World);
            }
            else
            {
                // gets rid of boundary damper when pressing right
                transform.Translate(0f, 0f, yawSpeed * -roll * rotation.x * Time.deltaTime * speedMultiplier, Space.World);
            }
        }
        if (transform.position.z > -5 && roll > 0f)
        {
            if (horizontalThrow < 0f)
            {
                transform.Translate(0f, 0f, yawSpeed * -roll * rotation.x * Time.deltaTime * boundaryDamper * speedMultiplier, Space.World);
            }
            else
            {
                transform.Translate(0f, 0f, yawSpeed * -roll * rotation.x * Time.deltaTime * speedMultiplier, Space.World);
            }
        }
    }

    private void ReduceRollAtEdges(float horizontalThrow)
    {
        if (transform.position.z < -(boundWidth - 1f))
        {
            // damper = 0 when position = -3
            // damper = 1 when position = -4
            float offset = Mathf.Abs(Mathf.Clamp(transform.position.z, -boundWidth, -(boundWidth - 1f)) + (boundWidth - 1f));
            boundaryDamper = Mathf.Lerp(1f, 0f, offset);

            // Allow full roll back the other way
            if (horizontalThrow < 0f)
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * 1f * maxRollDamper);
            }
        }
        else if (transform.position.z > (boundWidth - 1f))
        {
            // damper = 0 when position = 5
            // damper = 1 when position = 6
            float offset = Mathf.Abs(boundWidth - (Mathf.Clamp(transform.position.z, (boundWidth - 1f), boundWidth)));
            boundaryDamper = Mathf.Lerp(0f, 1f, offset);

            // Allow full roll back the other way
            if (horizontalThrow > 0f)
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                // gets rid of boundary damper when pressing left
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * 1f * maxRollDamper);
            }
        }
        else
        {
            boundaryDamper = 1f;
            transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * maxRollDamper);
        }
    }

    private void LimitRollAmount(float roll)
    {
        if (Mathf.Abs(roll) > maxRoll)
        {
            // damper = 0 when rotation = maxRoll
            // damper = 1 when rotation = maxRoll + 10
            float offset = (maxRoll + 10f - Mathf.Abs(roll)) / 10f;
            maxRollDamper = Mathf.Abs(Mathf.Lerp(0f, 1f, offset));
        }
        else
        {
            maxRollDamper = 1f;
        }
    }

    private void ThrusterSounds(float horizontalThrow)
    {
        thrusters.volume = (Mathf.Abs(horizontalThrow) / 2f) + 0.3f;
        //thrusters.panStereo = -horizontalThrow / 2f; // OLD WAY
        thrusters.panStereo = transform.position.z / 8f;
    }

    private void AnimateFlaps(float roll, Vector3 rotation)
    {
        leftInnerFlap.transform.localEulerAngles = new Vector3(roll * rotation.x / 3f, 0f, 0f);
        leftOuterFlap.transform.localEulerAngles = new Vector3(roll * rotation.x, 0f, 0f);
        rightInnerFlap.transform.localEulerAngles = new Vector3(-roll * rotation.x / 3f, 0f, 0f);
        rightOuterFlap.transform.localEulerAngles = new Vector3(-roll * rotation.x, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.tag == "EnemyMissile")
        {
            health -= 10;
            hit.Play();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            health -= 10;
            //enemyHit.Play();
        }
    }
}
