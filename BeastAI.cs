using UnityEngine;
using System.Collections;

#pragma warning disable 0219
#pragma warning disable 0414

public class BeastAI : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3;
    public float rotationSpeed = 3;
    public float attackRange = 1.5f;
    public float chaseRange = 10; // distance within which to start chasing
    public float giveUpRange = 20; // distance beyond which AI gives up
    public float attackRepeatTime = 1;
    public GameObject anim;
    public float dontComeCloserRange = 3;
    public float maxDamage = 5.0f;
    public float minDamage = 5.0f;
    public AudioClip attack;
    public string idleAnim = "idle";
    public string walkAnim = "walk";
    public string attackAnim = "attack";
    public string attackAnim2 = "crouchLook";
    public string hitAnim = "hit";
    private bool chasing = false;
    private float attackTime;
    private bool checking = false;
    public float attackdelay = 0.8f;
    public float delayBeforeJump = 1.0f;
    public AudioClip[] walkSound;
    public float audioStepLength = 0.25f;
    private bool isPlaying = false;
    private bool attemptToJump = false;

    public float maximumHitPoints = 100.0f;
    public float hitPoints = 100.0f;
    private float gotHitTimer = -1.0f;
    public float detonationDelay = 0.0f;
    public Rigidbody deadReplacement;
    public bool grounded = false;
    public float gravity = 10;
    [HideInInspector]
    public ScoreManager scoreManager;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform; //cache transform data for easy access/preformance
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        anim.GetComponent<Animation>().wrapMode = WrapMode.Loop;
        anim.GetComponent<Animation>()[attackAnim].wrapMode = WrapMode.Once;
        anim.GetComponent<Animation>()[hitAnim].wrapMode = WrapMode.Once;
        anim.GetComponent<Animation>()[attackAnim].layer = 2;
        anim.GetComponent<Animation>()[hitAnim].layer = 1;
        anim.GetComponent<Animation>().Stop();
        GameObject GO = GameObject.FindWithTag("ScoreManager");
        scoreManager = GO.GetComponent<ScoreManager>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            // check distance to target (every frame)
            float distance = (target.position - myTransform.position).magnitude;

            if (distance < dontComeCloserRange)
            {
                moveSpeed = 0;
            }
            else
            {
                moveSpeed = Random.Range(3, 6);
            }

            if (chasing)
            {

                //rotate to look at the player
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                if (distance > attackRange && !attemptToJump)
                {
                    myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
                    if (grounded)
                    {
                        anim.GetComponent<Animation>().CrossFade(walkAnim);
                        if (!isPlaying)
                        {
                            StartCoroutine(playWalkSounds());
                        }
                    }
                }

                // give up
                if (distance > giveUpRange)
                {
                    chasing = false;
                    GetComponent<AudioSource>().Stop();
                }

                // attack
                if (distance < attackRange)
                {
                    anim.GetComponent<Animation>().CrossFade(attackAnim2);
                    if (Time.time > attackTime)
                    {
                        checkInDelay();
                        attackTime = Time.time + attackRepeatTime;
                    }
                }
            }
            else
            {
                anim.GetComponent<Animation>().CrossFade(idleAnim);
                GetComponent<AudioSource>().Stop();
                // start chasing if target comes close enough
                if (distance < chaseRange)
                {
                    chasing = true;
                }
            }

            // Gravity 
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
            grounded = false;
        }
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    IEnumerator checkInDelay()
    {
        if (checking)
            yield break;

        checking = true;
        attemptToJump = true;
        yield return new WaitForSeconds(delayBeforeJump);
        GetComponent<Rigidbody>().AddRelativeForce(0, 30000, 40000);
        anim.GetComponent<Animation>().CrossFade(attackAnim);
        yield return new WaitForSeconds(attackdelay);
        attemptToJump = false;
        if ((target.position - myTransform.position).magnitude < 1.5f)
        {
            target.SendMessage("PlayerDamage", Random.Range(minDamage, maxDamage));
            GetComponent<AudioSource>().PlayOneShot(attack, 1.0f / GetComponent<AudioSource>().volume);
            GetComponent<Rigidbody>().AddRelativeForce(0, 2000, -10000);
        }
        else
        {
            checking = false;
            yield break;
        }
        checking = false;
    }

    void ApplyDamage(float damage)
    {
        if (hitPoints <= 0.0f)
            return;

        hitPoints -= damage;
        scoreManager.DrawCrosshair();
        if (hitPoints <= 0.0f)
            Invoke("Detonate", detonationDelay);
    }

    void Detonate()
    {
        scoreManager.addScore(10);
        // Destroy ourselves
        Destroy(gameObject);
        gameObject.SetActive(false);

        // If we have a dead barrel then replace ourselves with it!
        if (deadReplacement)
        {
            Rigidbody dead = Instantiate(deadReplacement, transform.position, transform.rotation) as Rigidbody;

            // For better effect we assign the same velocity to the exploded barrel
            dead.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            dead.angularVelocity = GetComponent<Rigidbody>().angularVelocity;
        }
    }

    IEnumerator playWalkSounds()
    {
        isPlaying = true;
        GetComponent<AudioSource>().clip = walkSound[Random.Range(0, walkSound.Length)];
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(audioStepLength);
        isPlaying = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

}