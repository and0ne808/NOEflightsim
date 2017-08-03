using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chopper3 : MonoBehaviour {

     float forwardAccel; // Forward Acceleration
     public float forwardVelocity; // Velocity in the forward direction
    public float forwardVelocityMultiplier; // Multiplier for forward velocity 0.0005 is working great
    public float forwardFrictionMultiplier; // 0.2 seems to work great
    public float maxForwardVelocity;

     float sideAccel; // Side Acceleration
     float sideVelocity; // Positive Means left, negative means right
    public float sideVelocityMultiplier;
    public float sideFrictionMultiplier;

     float upAccel; // Upwards Acceleration
     float upVelocity; // 
    public float upVelocityMultiplier;
    public float upFrictionMultiplier;

    public float dangerZone; // The point at which you get a warning y ~30
    public float killZone; // The point at wchich you get gunned down y ~50

    public AudioClip dontFlyHigh;
    public AudioSource warningAudio;

    public float timePassed;
    public float playerScore;

    public float distanceFromWinCube;
    public Transform winCubeTransform;

    string scoreText = "SCORE: ";
    string altitudeText = "ALTITUDE: ";
    string radarDetectionText = "RADAR DANGER: ";
    string radarDetectionLevel;


    // Use this for initialization
    void Start () {
	}

    private void OnGUI()
    {
        
        GUI.Label(new Rect(0, 0, 300, 100), scoreText + playerScore.ToString());
        GUI.Label(new Rect(0, 20, 300, 100), altitudeText + Mathf.Ceil(transform.position.y) + " meters");

        //Calculate Radar Detection
        if (Mathf.Ceil(transform.position.y) < 50)
        {
            radarDetectionLevel = "VERY HIGH";
        }
        if (Mathf.Ceil(transform.position.y) < 40)
        {
            radarDetectionLevel = "HIGH";
        }
        if (Mathf.Ceil(transform.position.y) < 30)
        {
            radarDetectionLevel = "MEDIUM";
        }
        if (Mathf.Ceil(transform.position.y) < 20)
        {
            radarDetectionLevel = "LOW";
        }
        if (Mathf.Ceil(transform.position.y) < 10)
        {
            radarDetectionLevel = "VERY LOW";
        }


        GUI.Label(new Rect(0, 40, 300, 100), radarDetectionText + radarDetectionLevel);
    }

    // Update is called once per frame
	void Update () {

        //Determine forward and backward acceleration
        if (Input.GetKey(KeyCode.W)/* && (forwardVelocity < maxForwardVelocity)*/)
        {
            upAccel = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            upAccel = -1;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            upAccel = 0;
        }
        else
        {
            upAccel = 0;
        }

        //Determine sideways acceleration. Positive = left, Negative = right
        if (Input.GetKey(KeyCode.A))
        {
            sideAccel = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            sideAccel = -1;
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            sideAccel = 0;
        }
        else
        {
            sideAccel = 0;
        }

        //Determine upwards acceleration. Positive = left, Negative = right
        /*
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            upAccel = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightControl))
        {
            upAccel = -1;
        }
        else if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightControl)))
        {
            upAccel = 0;
        }
        else
        {
            upAccel = 0;
        }
        */

        //Apply Friction

        //if chopper is in dead zone
        //Forward
        if (forwardVelocity < forwardVelocityMultiplier && forwardVelocity > -forwardVelocityMultiplier)
        {
            forwardVelocity = 0;
        }

        //if chopper is flying forward
        else if (forwardVelocity > forwardVelocityMultiplier)
        {
            forwardVelocity -= forwardVelocityMultiplier * forwardFrictionMultiplier;
        }

        //if chopper is flying backwards
        else if (forwardVelocity < -forwardVelocityMultiplier)
        {
            forwardVelocity += forwardVelocityMultiplier * forwardFrictionMultiplier;
        }

        //Sideways
        if (sideVelocity < sideVelocityMultiplier && sideVelocity > -sideVelocityMultiplier)
        {
            sideVelocity = 0;
        }

        //if chopper is flying left
        else if (sideVelocity > sideVelocityMultiplier)
        {
            sideVelocity -= sideVelocityMultiplier * sideFrictionMultiplier;
        }

        //if chopper is flying right
        else if (sideVelocity < -sideVelocityMultiplier)
        {
            sideVelocity += sideVelocityMultiplier * sideFrictionMultiplier;
        }

        //Upwards
        if (upVelocity < upVelocityMultiplier && upVelocity > -upVelocityMultiplier)
        {
            upVelocity = 0;
        }

        //if chopper is flying up
        else if (upVelocity > upVelocityMultiplier)
        {
            upVelocity -= upVelocityMultiplier * upFrictionMultiplier;
        }

        //if chopper is flying down
        else if (upVelocity < -upVelocityMultiplier)
        {
            upVelocity += upVelocityMultiplier * upFrictionMultiplier;
        }

        //Update chopper speed
        //forwardVelocity += (forwardVelocityMultiplier*forwardAccel);
        forwardVelocity = maxForwardVelocity; // I'll make the player move forward at a constant speed
        sideVelocity += (sideVelocityMultiplier * sideAccel);
        upVelocity += (upVelocityMultiplier * upAccel);

        //Move the chopper
        transform.Translate(Vector3.forward * Time.deltaTime * forwardVelocity);
        transform.Translate(Vector3.left * Time.deltaTime * sideVelocity);
        transform.Translate(Vector3.up * Time.deltaTime * upVelocity);

        //Check for chopper kill zone
        if (transform.position.y > killZone)
        {
            Debug.Log("too high!");
            SceneManager.LoadScene("gunned", LoadSceneMode.Single);
        }

        //Check for chopper danger zone
        if (transform.position.y > dangerZone && !warningAudio.isPlaying)
        {
            warningAudio.PlayOneShot(dontFlyHigh);
        }

        //Calculate Score
        timePassed += Time.deltaTime;
        if (timePassed > 1)
        {
            playerScore += Mathf.Ceil(dangerZone - transform.position.y);
            timePassed = 0;
        }

        //Calculate Distance from Win Cuble
        distanceFromWinCube = Vector3.Distance(winCubeTransform.position, transform.position);
        if(distanceFromWinCube < 50)
        {
            PlayerPrefs.SetInt("Player Score", (int) playerScore);
            SceneManager.LoadScene("success", LoadSceneMode.Single);
        }
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit!");
        SceneManager.LoadScene("crashed", LoadSceneMode.Single);

    }

    void AddScore()
    {

    }


}
