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

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
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

        missile.transform.localScale = new Vector3(80f, 80f, 80f);

        // Add velocity to the bullet
        missile.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 40f);

        // Destroy the bullet after 2 seconds
        Destroy(missile, 10.0f);
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
        leftInnerFlap.transform.localEulerAngles = new Vector3(roll * rotation.z / 3f, 0f, 0f);
        leftOuterFlap.transform.localEulerAngles = new Vector3(roll * rotation.z, 0f, 0f);
        rightInnerFlap.transform.localEulerAngles = new Vector3(-roll * rotation.z / 3f, 0f, 0f);
        rightOuterFlap.transform.localEulerAngles = new Vector3(-roll * rotation.z, 0f, 0f);

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

        print("Roll:" + roll);

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
