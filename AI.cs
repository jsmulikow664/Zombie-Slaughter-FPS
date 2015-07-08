using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
    public Transform target; //the enemy's target
    public float moveSpeed = 3; //move speed
    public float rotationSpeed = 3; //speed of turning
    public float attackRange = 3; // distance within which to attack
    public float chaseRange = 10; // distance within which to start chasing
    public float giveUpRange = 20; // distance beyond which AI gives up
    public float attackRepeatTime = 1.5f; // delay between attacks when within range
    public GameObject anim;
    public float maximumHitPoints = 5.0f;
    public float hitPoints = 5.0f;
    public AudioClip attack;
    private bool chasing = false;
    private float attackTime;
    public bool checkRay = false;
    public string idleAnim = "idle";
    public string walkAnim = "walk";
    public string attackAnim = "attack";
    public int dontComeCloserRange = 4;

    private Transform myTransform; //current transform data of this enemy

    void Awake()
    {
        myTransform = transform; //cache transform data for easy access/preformance
        anim.GetComponent<Animation>().wrapMode = WrapMode.Loop;
        anim.GetComponent<Animation>()[attackAnim].wrapMode = WrapMode.Once;
        anim.GetComponent<Animation>()[attackAnim].layer = 2;
        anim.GetComponent<Animation>().Stop();
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        // check distance to target every frame:
        float distance = (target.position - myTransform.position).magnitude;

        if (distance < dontComeCloserRange)
        {
            moveSpeed = 0;

            anim.GetComponent<Animation>()[idleAnim].speed = .4f;
            anim.GetComponent<Animation>().CrossFade(idleAnim);
        }
        else
        {
            moveSpeed = Random.Range(4, 6);
            anim.GetComponent<Animation>().CrossFade(walkAnim);
        }

        if (chasing)
        {
            //move towards the player
            myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;


            //rotate to look at the player
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            // give up, if too far away from target:
            if (distance > giveUpRange)
            {
                chasing = false;
            }

            // attack, if close enough, and if time is OK:
            if (distance < attackRange && Time.time > attackTime)
            {
                anim.GetComponent<Animation>()[attackAnim].speed = 2.0f;
                anim.GetComponent<Animation>().CrossFade(attackAnim);
                target.SendMessage("PlayerDamage", maximumHitPoints);
                attackTime = Time.time + attackRepeatTime;
                GetComponent<AudioSource>().PlayOneShot(attack, 1.0f / GetComponent<AudioSource>().volume);
            }

        }
        else
        {
            // not currently chasing.
            anim.GetComponent<Animation>()[idleAnim].speed = .4f;
            anim.GetComponent<Animation>().CrossFade(idleAnim);
            // start chasing if target comes close enough
            if (distance < chaseRange)
            {
                chasing = true;
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

}