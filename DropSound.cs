using UnityEngine;
using System.Collections;

public class DropSound : MonoBehaviour
{
    public AudioClip[] sound;

    public void OnCollisionEnter(Collision collision)
    {
        plauDropSound();
    }

    public void plauDropSound()
    {
        GetComponent<AudioSource>().clip = sound[Random.Range(0, sound.Length)];
        GetComponent<AudioSource>().volume = .7f;
        GetComponent<AudioSource>().Play();
    }
}