using UnityEngine;
using System.Collections;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] bulletSounds;
    public float audioRicochetteLength = 0.2f;

    void Start()
    {
        StartCoroutine(PlaySounds());
    }

    public IEnumerator PlaySounds()
    {
        GetComponent<AudioSource>().clip = bulletSounds[Random.Range(0, bulletSounds.Length)];
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(10);
        //yield return new WaitForSeconds(audio.clip.length);
        Destroy(gameObject);
    }
}