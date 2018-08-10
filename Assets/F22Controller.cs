using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class F22Controller : MonoBehaviour {

    // Physics variables
    public float maxRoll = 30f;
    public float maxRollSpeed = 1f;
    //public float cruiseSpeed = 100f;
    public float yawSpeed = 1f;
    public float boundaryDamper = 1f;
    public float maxRollDamper = 1f;
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
    private float timePassed = 0f;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        timePassed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }

         if (Input.GetKey(KeyCode.LeftControl) && timePassed >= keyDelay)
        {
            FireBullets();
            timePassed = 0f;
        }

        MovementController();

    }

    private void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        GameObject missile = (GameObject)Instantiate(
            missilePrefab,
            missileSpawnLocation.position,
            new Quaternion(90f,0f,0f,90f));

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
        // Constantly move forward
        //transform.Translate(0f, 0f, cruiseSpeed * Time.deltaTime);

        // Get cross-platform input
        float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        // Calculate rotation
        float roll;
        Vector3 rotation;
        transform.rotation.ToAngleAxis(out roll, out rotation);

        // Animate flaps
        leftInnerFlap.transform.localEulerAngles = new Vector3(roll * rotation.x / 3f, 0f, 0f);
        leftOuterFlap.transform.localEulerAngles = new Vector3(roll * rotation.x, 0f, 0f);
        rightInnerFlap.transform.localEulerAngles = new Vector3(-roll * rotation.x / 3f, 0f, 0f);
        rightOuterFlap.transform.localEulerAngles = new Vector3(-roll * rotation.x, 0f, 0f);

        // Limit amount of roll
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

        // Dimish amount of roll after certain X positions until zero
        // This will then slow down movement at the edges and restrict f22 to the map
        if (transform.position.z < -3)
        {
            // damper = 0 when position = -3
            // damper = 1 when position = -4
            float offset = Mathf.Abs(Mathf.Clamp(transform.position.z,-4f,-3f) + 3);
            boundaryDamper = Mathf.Lerp(1f, 0f, offset);

            // Allow full roll back the other way
            if (horizontalThrow < 0f)
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * 2f * maxRollDamper);
            }
        }
        else if (transform.position.z > 3)
        {
            // damper = 0 when position = 3
            // damper = 1 when position = 4
            float offset = Mathf.Abs(4 - (Mathf.Clamp(transform.position.z,3f,4f)));
            boundaryDamper = Mathf.Lerp(0f, 1f, offset);

            // Allow full roll back the other way
            if (horizontalThrow > 0f)
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * 2f * maxRollDamper);
            }
        }
        else
        {
            boundaryDamper = 1f;
            transform.Rotate(Vector3.left, -horizontalThrow * Time.deltaTime * maxRollSpeed * maxRollDamper);
        }

        // Horizontal speed determined by amount of roll
        // Damper settings also affect the speed here
        if (transform.position.z < 5 && roll > 0f)
        {
            if (horizontalThrow > 0f)
            {
                transform.Translate(0f, 0f, yawSpeed * -roll * rotation.x * Time.deltaTime * boundaryDamper * speedMultiplier, Space.World);
            }
            else
            {
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

        // Re-centre F22 slowly if it is near edges
        if ((horizontalThrow < 0.25f && horizontalThrow > -0.25f) || boundaryDamper < 0.5f || maxRollDamper < 0.5f)
        {
            transform.Rotate(Vector3.right, -roll * rotation.x * Time.deltaTime * yawSpeed);
            transform.Translate(0f, 0f, (-transform.position.z / 3f) * Time.deltaTime, Space.World);
        }
    }
}
