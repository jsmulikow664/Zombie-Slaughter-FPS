using UnityEngine;
using System.Collections;

#pragma warning disable 0219
#pragma warning disable 0414

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RigidController : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/
    public float proneSpeed = 1.0f;
    public float crouchSpeed = 2.0f;
    public float walkSpeed = 8.0f;
    public float runSpeed = 20.0f;
    public float runSkillLevel = 0.0f;
    public float gravitySkillLevel = 0.0f;

    public int fallDamageMultiplier = 2;
    public GameObject fallAnimGO;
    public float inAirControl = 0.1f;
    public float gravity = 20.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    public AudioClip fallSound;
    public GameObject playerWeapons;
    //@HideInInspector
    public bool grounded = false;
    private bool sliding = false;
    private float speed = 10.0f;
    private bool limitDiagonalSpeed = true;
    private float normalHeight = 0.5f;
    private float crouchHeight = -0.2f;
    private float crouchingHeight = 0.3f;
    public float proneHeight = -0.7f;
    private RaycastHit hit;
    private Transform myTransform;
    private float rayDistance;
    private GameObject mainCameraGO;
    private GameObject weaponCameraGO;
    public int state = 0;
    public float moveSpeed = 2.0f;
    public Vector3 targetVelocity = Vector3.zero;
    public bool onLadder = false;
    public float climbSpeed = 10.0f;
    public bool canClimb = false;

    private CapsuleCollider coll;

    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
        myTransform = transform;
        mainCameraGO = GameObject.FindWithTag("MainCamera");
        weaponCameraGO = GameObject.FindWithTag("WeaponCamera");
        coll = GetComponent<CapsuleCollider>();
        rayDistance = coll.height * .5f + coll.radius;
        playerWeapons.GetComponent<Animation>().wrapMode = WrapMode.Loop;
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if (grounded)
        {


            if (state == 0)
            {
                if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) > 30)
                    {
                        sliding = true;
                        GetComponent<Rigidbody>().AddRelativeForce(-Vector3.up * 500);
                    }
                    else
                    {
                        sliding = false;

                    }
                }
            }

            // Calculate how fast we should be moving
            targetVelocity = new Vector3(inputX * inputModifyFactor, 0.0f, inputY * inputModifyFactor);
            targetVelocity = myTransform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0.0f;
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);


            if (canJump && Input.GetButtonDown("Jump") && state == 0)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }

            if (Input.GetButton("Use") && onLadder)
            {
                canClimb = true;
                GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }

            if (state == 0)
            {
                if (grounded && Input.GetButton("Run") && Input.GetKey("w"))
                {
                    speed = runSpeed + runSkillLevel;
                }
                else
                {
                    speed = walkSpeed;
                }
            }
            else if (state == 1)
            {
                speed = crouchSpeed;
            }
            else if (state == 2)
            {
                speed = proneSpeed;
            }

        }
        else
        {

            if (onLadder && canClimb)
            {
                //if(Input.GetAxis("Vertical")){

                targetVelocity = new Vector3(0.0f, Input.GetAxis("Vertical") * inputModifyFactor, 0.0f);
                targetVelocity *= climbSpeed;
                targetVelocity = myTransform.TransformDirection(targetVelocity);

                Vector3 velChange = (targetVelocity - GetComponent<Rigidbody>().velocity);
                velChange.x = Mathf.Clamp(velChange.x, -maxVelocityChange, maxVelocityChange);
                velChange.y = Mathf.Clamp(velChange.y, -maxVelocityChange, maxVelocityChange);
                velChange.z = 0.0f;

                GetComponent<Rigidbody>().AddForce(velChange, ForceMode.VelocityChange);
                //}
                /*
                // Calculate how fast we should be moving
                targetVelocity = new Vector3(inputX * inputModifyFactor, inputY * inputModifyFactor, 0.0f);
                targetVelocity = myTransform.TransformDirection(targetVelocity);
                targetVelocity *= climbSpeed;
			
                // Apply a force that attempts to reach our target velocity
                FIXME_VAR_TYPE velChange= (targetVelocity - rigidbody.velocity);
                velChange.x = Mathf.Clamp(velChange.x, -maxVelocityChange, maxVelocityChange);
                velChange.y = Mathf.Clamp(velChange.y, -maxVelocityChange, maxVelocityChange);
                velChange.z = 0.0f;
                rigidbody.AddForce(velChange, ForceMode.VelocityChange);
                */
            }
            else
            {
                // AirControl 
                targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
                GetComponent<Rigidbody>().AddForce(targetVelocity, ForceMode.VelocityChange);
            }
        }

        if (onLadder == false)
        {
            canClimb = false;
        }

        if (canClimb == false)
        {
            // Gravity 
            GetComponent<Rigidbody>().AddForce(new Vector3(0, (-gravity + gravitySkillLevel) * GetComponent<Rigidbody>().mass, 0));
        }

        grounded = false;
        onLadder = false;
    }

    void OnCollisionStay(Collision col)
    {

        foreach (ContactPoint contact in col.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45)
            {
                grounded = true;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            onLadder = true;
            grounded = false;
        }
    }

    void HitJumpPad(float velocity)
    {
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z + velocity);
        //rigidbody.velocity.z += velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (grounded == false)
        {
            fallAnimGO.GetComponent<Animation>().CrossFadeQueued("Fall", 0.3f, QueueMode.PlayNow);
            float currSpeed = collision.relativeVelocity.magnitude;

            if (currSpeed > 25)
            {
                float damage = currSpeed * fallDamageMultiplier;
                Debug.Log("FallDamage" + damage);
                SendMessage("PlayerDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    void Update()
    {
        if (grounded)
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < (walkSpeed + 2) && GetComponent<Rigidbody>().velocity.magnitude > (walkSpeed - 2) && !Input.GetButton("Fire2"))
            {
                playerWeapons.GetComponent<Animation>().CrossFade("Walk");

            }
            else if (GetComponent<Rigidbody>().velocity.magnitude > (runSpeed - 2))
            {
                playerWeapons.GetComponent<Animation>().CrossFade("Run");

            }
            else if (GetComponent<Rigidbody>().velocity.magnitude < (crouchSpeed + 1) && GetComponent<Rigidbody>().velocity.magnitude > (crouchSpeed - 1) && Input.GetButton("Fire2"))
            {
                playerWeapons.GetComponent<Animation>().CrossFade("CrouchAim");

            }
            else
            {
                playerWeapons.GetComponent<Animation>().CrossFade("IdleAnim");
            }
        }
        else
        {
            playerWeapons.GetComponent<Animation>().CrossFade("IdleAnim");
        }

        if (mainCameraGO.transform.localPosition.y > normalHeight)
        {
            //mainCameraGO.transform.localPosition.y = normalHeight;
            mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, normalHeight, mainCameraGO.transform.localPosition.z);
        }
        else if (mainCameraGO.transform.localPosition.y < proneHeight)
        {
            //mainCameraGO.transform.localPosition.y = proneHeight;
            mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, proneHeight, mainCameraGO.transform.localPosition.z);
        }

        weaponCameraGO.transform.localPosition = mainCameraGO.transform.localPosition;


        if (Input.GetButtonDown("Crouch"))
        {
            if (state == 0 || state == 2)
            {
                state = 1;
            }
            else if (state == 1)
            {
                state = 0;
            }
        }

        if (state == 0)
        { //Stand Position
            coll.direction = 1;
            coll.height = 2.0f;
            coll.center = new Vector3(0, 0, 0);
            if (mainCameraGO.transform.localPosition.y < normalHeight)
            {
                //mainCameraGO.transform.localPosition.y += Time.deltaTime * moveSpeed;
                mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, mainCameraGO.transform.localPosition.y + Time.deltaTime * moveSpeed, mainCameraGO.transform.localPosition.z);
            }



        }
        else if (state == 1)
        { //Crouch Position
            coll.direction = 1;
            coll.height = 1.5f;
            coll.center = new Vector3(0, -0.25f, 0);
            if (mainCameraGO.transform.localPosition.y > crouchHeight)
            {
                if (mainCameraGO.transform.localPosition.y - (crouchingHeight * Time.deltaTime / .1) < crouchHeight)
                {
                    //mainCameraGO.transform.localPosition.y = crouchHeight;
                    mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, crouchHeight, mainCameraGO.transform.localPosition.z);
                }
                else
                {
                    //mainCameraGO.transform.localPosition.y -= crouchingHeight * Time.deltaTime / .1;
                    mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, mainCameraGO.transform.localPosition.y - Time.deltaTime / .1f, mainCameraGO.transform.localPosition.z);
                }
            }

            if (mainCameraGO.transform.localPosition.y < crouchHeight)
            {
                if (mainCameraGO.transform.localPosition.y - (crouchingHeight * Time.deltaTime / .1) > crouchHeight)
                {
                    //mainCameraGO.transform.localPosition.y = crouchHeight;
                    mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, crouchHeight, mainCameraGO.transform.localPosition.z);
                }
                else
                {
                    //mainCameraGO.transform.localPosition.y += crouchingHeight * Time.deltaTime / .1;
                    mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, mainCameraGO.transform.localPosition.y + Time.deltaTime / .1f, mainCameraGO.transform.localPosition.z);
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                state = 0;
            }

        }
        else if (state == 2)
        { //Prone Position
            coll.direction = 2;
            coll.height = 0.5f;
            coll.center = new Vector3(0, -0.5f, 0);
            if (mainCameraGO.transform.localPosition.y > proneHeight)
            {
                //mainCameraGO.transform.localPosition.y += proneHeight * Time.deltaTime ;
                mainCameraGO.transform.localPosition = new Vector3(mainCameraGO.transform.localPosition.x, mainCameraGO.transform.localPosition.y + proneHeight * Time.deltaTime, mainCameraGO.transform.localPosition.z);
            }


            if (Input.GetButtonDown("Jump"))
            {
                state = 1;
            }
        }

        if (Input.GetButtonDown("GoProne"))
        {
            if (state == 0 || state == 1)
            {
                state = 2;
            }
            else if (state == 2)
            {
                state = 0;
            }
        }
    }

    void Accelerate(float accelerateY, float accelerateZ)
    {
        grounded = false;
        GetComponent<Rigidbody>().AddRelativeForce(0, accelerateY, accelerateZ);
    }

    void OnGUI()
    {
        //if (onLadder && !canClimb)
         //   GUI.Label(new Rect(Screen.width - (Screen.width / 1.7f), Screen.height - (Screen.height / 1.4f), 800, 100), "Press key >>E<< to Climb");
    }
}