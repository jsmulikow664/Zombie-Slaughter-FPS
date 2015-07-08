using UnityEngine;
using System.Collections;

public class LandMineDamage : MonoBehaviour
{
    public float hitPoints = 300.0f;
    public Transform explosion;
    private bool callFunction = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
            StartCoroutine(Explosion());
        }

        if (other.CompareTag("Player"))
        {
            other.SendMessageUpwards("PlayerDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
            StartCoroutine(Explosion());
        }
    }

    public IEnumerator ApplyDamage()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Explosion());
    }

    public IEnumerator Explosion()
    {
        if (callFunction)
            yield break;
        callFunction = true;
        yield return new WaitForSeconds(0.1f);
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }


}