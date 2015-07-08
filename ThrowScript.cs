using UnityEngine;
using System.Collections;

public class ThrowScript : MonoBehaviour
{

    private float thePower;
    private bool increasing = false;
    private bool shooting = false;

    //speed to increment the bar
    public float barSpeed = 25;
    public Rigidbody ball;
    public Transform spawnPos;
    public float shotForce = 5;
    public AudioClip throwSound;

    void Update()
    {

        //if we are not currently shooting and Jump key is pressed down
        if (!shooting && Input.GetButtonDown("Fire"))
        {
            //set the increasing part of Update() below to start adding power
            increasing = true;
        }

        // detect if Jump key is released and then call the Shoot function, passing current 
        // value of 'thePower' variable into its 'power' argument
        if (!shooting && Input.GetButtonUp("Fire"))
        {
            //reset increasing to stop charge of the power bar
            increasing = false;
            //call the custom function below with current value of thePower fed to its argument
            Shoot(thePower);
        }

        if (increasing)
        {
            //add to thePower variable using Time.deltaTime multiplied by barSpeed
            thePower += Time.deltaTime * barSpeed;



            //set the pitch of the audio tone to the power var but step it down with division
            GetComponent<AudioSource>().pitch = thePower / 30;
        }
    }

    // start the 'Shoot' custom function, and establish a 
    // float argument to recieve 'thePower' when function is called
    void Shoot(float power)
    {
        shooting = true;

        //play the sound of the cannon blast in a new object to avoid interfering
        //with the current sound assignment and loop setup
        AudioSource.PlayClipAtPoint(throwSound, transform.position);

        //create a ball, assign the newly created ball to a var called pFab
        Rigidbody pFab = Instantiate(ball, spawnPos.position, spawnPos.rotation) as Rigidbody;

        //find the forward direction of the object assigned to the spawnPos variable
        Vector3 fwd = spawnPos.forward;
        pFab.AddForce(fwd * power * shotForce);

        thePower = 0;

        //allow shooting to occur again
        shooting = false;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(40, Screen.height - 100, 60, 60), " Power: ");
        GUI.Label(new Rect(100, Screen.height - 100, 60, 60), "" + thePower);
    }

    void DrawWeapon()
    {
        print("drawing");
    }
}