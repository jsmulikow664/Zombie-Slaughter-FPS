using UnityEngine;
using System.Collections;

public class Medic : MonoBehaviour
{
    public float hitPoints = 50.0f;
    public AudioClip sound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessageUpwards("Medic", hitPoints, SendMessageOptions.DontRequireReceiver);
            AudioSource.PlayClipAtPoint(sound, transform.position);
            Destroy(gameObject);
        }
    }
}