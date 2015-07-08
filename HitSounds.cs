using UnityEngine;
using System.Collections;

public class HitSounds : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().Play();
    }
}