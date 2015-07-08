using UnityEngine;
using System.Collections;

#pragma warning disable 0219
#pragma warning disable 0414

public class AIzombie : MonoBehaviour
{
    public float speed = 3.0f;
    public float rotationSpeed = 5.0f;
    public float shootRange = 15.0f;
    public float attackRange = 30.0f;
    public float shootAngle = 4.0f;
    public float dontComeCloserRange = 5.0f;
    public float delayHitTime = 0.35f;
    public float pickNextWaypointDistance = 2.0f;
    public Transform target;
    public float HitDistance = 0.0f;
    public float damage = 0.0f;
    public AudioClip attack;
    private float lastShot = -10.0f;
    private float walkSpeed = 2;
    public GameObject anim;

    void Start()
    {
        // Auto setup player as target through tags
        if (target == null && GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
        StartCoroutine(Patrol());
        // walkSpeed = speed;
        //  anim.animation.wrapMode = WrapMode.Loop;
        //  anim.animation["Fury"].wrapMode = WrapMode.Once;
    }

    public IEnumerator Patrol()
    {
        AutoWayPoint curWayPoint = AutoWayPoint.FindClosest(transform.position);
        while (true)
        {
            Vector3 waypointPosition = curWayPoint.transform.position;
            // Are we close to a waypoint? -> pick the next one!
            if (Vector3.Distance(waypointPosition, transform.position) < pickNextWaypointDistance)
                curWayPoint = PickNextWaypoint(curWayPoint);
            // Attack the player and wait until
            // - player is killed
            // - player is out of sight      
            if (CanSeeTarget())
                StartCoroutine("AttackPlayer");

            // Move towards our target
            MoveTowards(waypointPosition);
            yield return 0;
        }
    }
    public bool CanSeeTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > attackRange)
            return false;

        RaycastHit hit;
        if (Physics.Linecast(transform.position, target.position, out hit))
            return hit.transform == target;

        return false;
    }
    public IEnumerator Attack()
    {
        // Start shoot animation
        //   animation.CrossFade("hit", 0.3f);
        anim.GetComponent<Animation>().Rewind("Fury");
        yield return new WaitForSeconds(delayHitTime);
        target.SendMessage("PlayerDamage", damage);
        // Wait until half the animation has played

        //   AudioSource.PlayClipAtPoint(attack, transform.position);
        // Fire gun
        //BroadcastMessage("Fire");


        // Wait for the rest of the animation to finish
        //  yield return new WaitForSeconds(animation["hit"].length - delayShootTime);
        yield return new WaitForSeconds(0.2f);
        //  animation.CrossFade("hit", 0.3f);
    }

    public IEnumerator AttackPlayer()
    {
        Vector3 lastVisiblePlayerPosition = target.position;
        while (true)
        {
            if (CanSeeTarget())
            {
                // Target is dead - stop hunting
                if (target == null)
                    yield break;

                // Target is too far away - give up   
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > shootRange * 3)
                    yield break;

                lastVisiblePlayerPosition = target.position;
                if (distance > dontComeCloserRange)
                    MoveTowards(lastVisiblePlayerPosition);
                else
                    RotateTowards(lastVisiblePlayerPosition);

                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 targetDirection = lastVisiblePlayerPosition - transform.position;
                targetDirection.y = 0;

                float angle = Vector3.Angle(targetDirection, forward);

                // Start shooting if close and play is in sight
                if (distance < HitDistance && angle < shootAngle)
                    StartCoroutine("Attack");
            }
            else
            {
                StartCoroutine("SearchPlayer", lastVisiblePlayerPosition);
                // Player not visible anymore - stop attacking
                if (!CanSeeTarget())
                    yield break;
            }
            yield return 0;
        }
    }

    public IEnumerator SearchPlayer(Vector3 position)
    {
        // Run towards the player but after 3 seconds timeout and go back to Patroling
        float timeout = 10.0f;
        while (timeout > 0.0f)
        {
            MoveTowards(position);

            // We found the player
            if (CanSeeTarget())
                yield break;

            timeout -= Time.deltaTime;
            yield return 0;
        }
    }
    public void RotateTowards(Vector3 position)
    {
        //   SendMessage("SetSpeed", 0.0f);

        Vector3 direction = position - transform.position;
        direction.y = 0;
        if (direction.magnitude < 0.1f)
            return;

        // Rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void MoveTowards(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
        if (direction.magnitude < 0.5f)
        {
            anim.GetComponent<Animation>().CrossFade("Idle");
            return;
        }

        // Rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // Modify speed so we slow down when we are not facing the target
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float speedModifier = Vector3.Dot(forward, direction.normalized);
        speedModifier = Mathf.Clamp01(speedModifier);

        // Move the character
        direction = forward * speed * speedModifier;
        GetComponent<CharacterController>().SimpleMove(direction);
    }

    AutoWayPoint PickNextWaypoint(AutoWayPoint currentWaypoint)
    {
        // We want to find the waypoint where the character has to turn the least

        // The direction in which we are walking
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // The closer two vectors, the larger the dot product will be.
        AutoWayPoint best = currentWaypoint;
        float bestDot = -10.0f;
        foreach (AutoWayPoint cur in currentWaypoint.connected)
        {
            Vector3 direction = Vector3.Normalize(cur.transform.position - transform.position);
            float dot = Vector3.Dot(direction, forward);
            if (dot > bestDot && cur != currentWaypoint)
            {
                bestDot = dot;
                best = cur;
            }
        }

        return best;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitDistance);
    }

}