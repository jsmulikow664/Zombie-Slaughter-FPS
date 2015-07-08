using UnityEngine;
using System.Collections;

public class LiftTriggerScript : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    public GameObject lift;
    public AudioClip liftSound;
    private bool canUse = false;
    public float waitTime; //Time to Open door (animation lenght)
    private bool IRuzspiests = false;

    void Down()
    {
        lift.GetComponent<Animation>().CrossFade("LiftDown");
        GetComponent<AudioSource>().PlayOneShot(liftSound, 1.0f / GetComponent<AudioSource>().volume);
    }

    void Up()
    {
        lift.GetComponent<Animation>().CrossFade("LiftUp");
        GetComponent<AudioSource>().PlayOneShot(liftSound, 1.0f / GetComponent<AudioSource>().volume);
    }

    void ApplyDamage()
    {
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        if (!canUse && !IRuzspiests)
        {
            Up();
            canUse = true;
            IRuzspiests = true;
            yield return new WaitForSeconds(waitTime);
            IRuzspiests = false;

        }
        else
        {
            if (canUse && !IRuzspiests)
            {
                Down();
                canUse = false;
                IRuzspiests = true;
                yield return new WaitForSeconds(waitTime);
                IRuzspiests = false;
            }
        }
    }
}