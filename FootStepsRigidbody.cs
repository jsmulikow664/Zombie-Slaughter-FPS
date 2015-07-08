using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FootStepsRigidbody : MonoBehaviour
{
    /**
    * Script made by OMA [www.armedunity.com]
    **/

    public AudioClip[] concrete;
    public AudioClip[] wood;
    public AudioClip[] dirt;
    public AudioClip[] metal;
    private bool step = true;
    public float audioStepLengthCrouch = 0.65f;
    public float audioStepLengthWalk = 0.45f;
    public float audioStepLengthRun = 0.25f;
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;

    public RigidController controller;

    void OnCollisionStay(Collision col)
    {
        if (controller.grounded && GetComponent<Rigidbody>().velocity.magnitude > 1 && step)
        {
            if (col.transform.tag == "Wood")
            {
                StartCoroutine(OneStep(wood));
            }
            else if (col.transform.tag == "Dirt")
            {
                StartCoroutine(OneStep(dirt));
            }
            else if (col.transform.tag == "Metal")
            {
                StartCoroutine(OneStep(metal));
            }
            else
            {
                StartCoroutine(OneStep(concrete));
            }
        }

    }

    IEnumerator OneStep(AudioClip[] clip)
    {
        step = false;
        GetComponent<AudioSource>().clip = clip[Random.Range(0, clip.Length)];
        if (GetComponent<Rigidbody>().velocity.magnitude > (runSpeed - 2))
        {
            GetComponent<AudioSource>().volume = 1;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(audioStepLengthRun);
        }
        else if (controller.state == 0)
        {
            GetComponent<AudioSource>().volume = 0.8f;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(audioStepLengthWalk);
        }
        else if (controller.state == 1)
        {
            GetComponent<AudioSource>().volume = 0.6f;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(audioStepLengthCrouch);
        }
        step = true;
    }

}