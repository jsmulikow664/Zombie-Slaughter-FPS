using UnityEngine;
using System.Collections;

public class HelicopterController : MonoBehaviour
{

    public GameObject main_Rotor_GameObject;				// gameObject to be animated
    public GameObject tail_Rotor_GameObject;				// gameObject to be animated

    public float max_Rotor_Force = 22241.1081f;			// newtons
    public static float max_Rotor_Velocity = 7200;		// degrees per second
    public float rotor_Velocity = 0.0f;					// value between 0 and 1
    private float rotor_Rotation = 0.0f; 			// degrees... used for animating rotors

    public float max_tail_Rotor_Force = 15000.0f; 		// newtons
    public float max_Tail_Rotor_Velocity = 2200.0f; 		// degrees per second
    private float tail_Rotor_Velocity = 0.0f; 		// value between 0 and 1
    private float tail_Rotor_Rotation = 0.0f; 		// degrees... used for animating rotors

    public float forward_Rotor_Torque_Multiplier = 0.5f;	// multiplier for control input
    public float sideways_Rotor_Torque_Multiplier = 0.5f;	// multiplier for control input

    public static bool main_Rotor_Active = true;		// bool  for determining if a prop is active
    public static bool tail_Rotor_Active = true;		// bool  for determining if a prop is active

    public float fuel = 100.0f;
    public GameObject noFuelSoundGO;
    public AudioClip rotatorSound;
    public GameObject rotatorTextureGO1;
    public GameObject rotatorTextureGO2;
    public GameObject bladesTextureGO;
    public float alpha;
    public GUISkin mySkin;

    public bool grounded;
    [HideInInspector]
    public bool damaged = false;
    [HideInInspector]
    public bool controlsEnabled = false;

    public RaycastHit hit;
    public float rayDistance = 3.0f;
    public GameObject PlayerInCar;

    void Awake()
    {
        rotatorTextureGO1.GetComponent<Renderer>().enabled = false;
        rotatorTextureGO2.GetComponent<Renderer>().enabled = false;
    }

    void Start()
    {
        GetComponent<AudioSource>().clip = rotatorSound;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
    }

    void FixedUpdate()
    {
        Vector3 torqueValue = Vector3.zero;
        Vector3 controlTorque = Vector3.zero;

        if (controlsEnabled)
        {
            controlTorque = new Vector3(Input.GetAxis("HeliVertical1") * forward_Rotor_Torque_Multiplier, 1.0f, -Input.GetAxis("HeliHorizontal2") * sideways_Rotor_Torque_Multiplier);
        }
        // Now check if the main rotor is active, if it is, then add it's torque to the "Torque Value", and apply the forces to the body of the 
        // helicopter.
        if (main_Rotor_Active == true)
        {
            torqueValue += (controlTorque * max_Rotor_Force * rotor_Velocity);

            // Now the force of the prop is applied. The main rotor applies a force direclty related to the maximum force of the prop and the 
            // prop velocity (a value from 0 to 1)
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * max_Rotor_Force * rotor_Velocity / 3);

            // This is simple code to help stabilize the helicopter. It essentially pulls the body back towards neutral when it is at an angle to
            // prevent it from tumbling in the air.
            if (Vector3.Angle(Vector3.up, transform.up) < 80)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * rotor_Velocity * 2);
            }
        }

        // Now we check to make sure the tail rotor is active, if it is, we add it's force to the "Torque Value"
        if (tail_Rotor_Active == true)
        {
            torqueValue -= (Vector3.up * max_tail_Rotor_Force * tail_Rotor_Velocity);
        }

        // And finally, apply the torques to the body of the helicopter.
        GetComponent<Rigidbody>().AddRelativeTorque(torqueValue);
        grounded = false;
    }

    void OnCollisionStay(Collision col)
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
        {
            grounded = true;
        }
    }
    /*
    void OnCollisionEnter ( Collision collision  ){
        if (Physics.Raycast(transform.position, -Vector3.up, hit, rayDistance)) {
	
            if(collision.gameObject.tag != "Player"){
                int currSpeed = collision.relativeVelocity.magnitude;
                Debug.Log ("speed" + currSpeed);
                if (currSpeed > 5) {
                    float damage = currSpeed * 30;
                //Debug.Log("tag" + collision.gameObject.tag);
                Debug.Log ("FallDamage" + damage);
			
                    BroadcastMessage ("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
        }	
    }	
    */
    void Update()
    {
        // This line simply changes the pitch of the attached audio emitter to match the speed of the main rotor.
        GetComponent<AudioSource>().pitch = rotor_Velocity;
        alpha = rotor_Velocity;
        bladesTextureGO.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.2f - alpha);
        if (fuel > 0 && rotor_Velocity > 0.2f)
        {
            fuel -= rotor_Velocity / 3 * Time.deltaTime;
        }
        if (alpha > 0.2f)
        {
            rotatorTextureGO1.GetComponent<Renderer>().enabled = true;
            rotatorTextureGO2.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            rotatorTextureGO1.GetComponent<Renderer>().enabled = false;
            rotatorTextureGO2.GetComponent<Renderer>().enabled = false;
        }

        // Now we animate the rotors, simply by setting their rotation to an increasing value multiplied by the helicopter body's rotation.
        if (main_Rotor_Active == true)
        {
            main_Rotor_GameObject.transform.rotation = transform.rotation * Quaternion.Euler(0, rotor_Rotation / 2, 0);
        }
        if (tail_Rotor_Active == true)
        {
            tail_Rotor_GameObject.transform.rotation = transform.rotation * Quaternion.Euler(tail_Rotor_Rotation, 0, 0);
        }

        // this just increases the rotation value for the animation of the rotors.
        rotor_Rotation += max_Rotor_Velocity * rotor_Velocity * Time.deltaTime;
        tail_Rotor_Rotation += max_Tail_Rotor_Velocity * rotor_Velocity * Time.deltaTime;

        // here we find the velocity required to keep the helicopter level. With the rotors at this speed, all forces on the helicopter cancel 
        // each other out and it should hover as-is.
        float hover_Rotor_Velocity = (GetComponent<Rigidbody>().mass * Mathf.Abs((Physics.gravity.y) / max_Rotor_Force));
        float hover_Tail_Rotor_Velocity = (max_Rotor_Force * rotor_Velocity) / max_tail_Rotor_Force;

        if (damaged)
        {
            controlsEnabled = false;
        }

        if (controlsEnabled)
        {
            if (Input.GetAxis("HeliVertical2") > 0.2f && fuel > 0.3f)
            {
                rotor_Velocity += Input.GetAxis("HeliVertical2") * 0.001f;
            }
            else if (Input.GetAxis("HeliVertical2") == 0 && fuel > 0.3f)
            {
                rotor_Velocity = Mathf.Lerp(rotor_Velocity, Random.Range(0.796f, 0.797f), Time.deltaTime / 6);
            }
            if (Input.GetAxis("HeliVertical2") < -0.2f)
            {
                rotor_Velocity = Mathf.Lerp(rotor_Velocity, hover_Rotor_Velocity, Time.deltaTime / 3);
            }
            if (rotor_Velocity > 0.4f)
            {
                tail_Rotor_Velocity = hover_Tail_Rotor_Velocity - Input.GetAxis("HeliHorizontal1");
            }
            else
            {
                tail_Rotor_Velocity = hover_Tail_Rotor_Velocity;
            }

        }
        else
        {
            if (!damaged)
            {
                rotor_Velocity = Mathf.Lerp(rotor_Velocity, 0, Time.deltaTime / 20);
                tail_Rotor_Velocity = 0;
            }
            else
            {
                tail_Rotor_Velocity = hover_Tail_Rotor_Velocity - Input.GetAxis("HeliHorizontal1");
            }
        }

        if (fuel < 0.3f)
        {
            rotor_Velocity = Mathf.Lerp(rotor_Velocity, 0, Time.deltaTime / 15);
            noFuelSoundGO.GetComponent<AudioSource>().enabled = true;
        }
        if (grounded && rotor_Velocity < 0.1f && fuel <= 0.0f)
        {
            noFuelSoundGO.GetComponent<AudioSource>().enabled = false;
        }
        // now we set velocity limits. The multiplier for rotor velocity is fixed to a range between 0 and 1. You can limit the tail rotor velocity 
        // too, but this makes it more difficult to balance the helicopter variables so that the helicopter will fly well.
        if (rotor_Velocity > 1.0f)
        {
            rotor_Velocity = 1.0f;
        }
        else if (rotor_Velocity < 0.0f)
        {
            rotor_Velocity = 0.0f;
        }

        if (fuel < 0.1f)
        {
            fuel = 0;
        }

        if (controlsEnabled)
        {
            PlayerInCar.SetActive(true);
        }
        else
        {
            PlayerInCar.SetActive(false);
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];

        if (controlsEnabled)
        {
            GUI.Label(new Rect(40, 40, 150, 80), " Fuel: ");
            GUI.Label(new Rect(90, 40, 150, 80), "" + fuel.ToString("f2"), style1);
        }
    }
}