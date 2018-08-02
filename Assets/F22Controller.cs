using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class F22Controller : MonoBehaviour {

    // Physics variables
    public float maxRoll = 30f;
    public float maxRollSpeed = 100f;
    public float cruiseSpeed = 100f;
    public float yawSpeed = 1f;
    public float boundaryDamper = 1f;
    public float maxRollDamper = 1f;

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
        transform.Translate(0f, 0f, cruiseSpeed * Time.deltaTime);

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
        if (transform.position.x < 190)
        {
            // damper = 0 when position = 100
            // damper = 1 when position = 150
            float offset = (transform.position.x - 100) / 50f;
            boundaryDamper = Mathf.Abs(Mathf.Lerp(0f, 1f, offset));

            // Allow full roll back the other way
            if (horizontalThrow < 0f)
            {
                transform.Rotate(Vector3.forward, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                transform.Rotate(Vector3.forward, -horizontalThrow * Time.deltaTime * maxRollSpeed * 2f * maxRollDamper);
            }
        }
        else if (transform.position.x > 350)
        {
            // damper = 0 when position = 400
            // damper = 1 when position = 350
            float offset = (400 - transform.position.x) / 50f;
            boundaryDamper = Mathf.Abs(Mathf.Lerp(0f, 1f, offset));

            // Allow full roll back the other way
            if (horizontalThrow > 0f)
            {
                transform.Rotate(Vector3.forward, -horizontalThrow * Time.deltaTime * maxRollSpeed * boundaryDamper * maxRollDamper);
            }
            else
            {
                transform.Rotate(Vector3.forward, -horizontalThrow * Time.deltaTime * maxRollSpeed * 2f * maxRollDamper);
            }
        }
        else
        {
            boundaryDamper = 1f;
            transform.Rotate(Vector3.forward, -horizontalThrow * Time.deltaTime * maxRollSpeed * maxRollDamper);
        }

        // Horizontal speed determined by amount of roll
        // Damper settings also affect the speed here
        if (transform.position.x > 100 && roll > 0f)
        {
            if (horizontalThrow > 0f)
            {
                transform.Translate(yawSpeed * -roll * rotation.z * Time.deltaTime * boundaryDamper, 0f, 0f, Space.World);
            }
            else
            {
                transform.Translate(yawSpeed * -roll * rotation.z * Time.deltaTime, 0f, 0f, Space.World);
            }
        }
        if (transform.position.x < 400 && roll > 0f)
        {
            if (horizontalThrow < 0f)
            {
                transform.Translate(yawSpeed * -roll * rotation.z * Time.deltaTime * boundaryDamper, 0f, 0f, Space.World);
            }
            else
            {
                transform.Translate(yawSpeed * -roll * rotation.z * Time.deltaTime, 0f, 0f, Space.World);
            }
        }

        // Re-centre F22 slowly if it is near edges
        if ((horizontalThrow < 0.25f && horizontalThrow > -0.25f) || boundaryDamper < 0.5f)
        {
            transform.Rotate(Vector3.forward, -roll * rotation.z * Time.deltaTime * yawSpeed);
            transform.Translate(((250f - transform.position.x) / 10f) * Time.deltaTime, 0f, 0f, Space.World);
        }
    }
}
