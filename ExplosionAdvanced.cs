using UnityEngine;
using System.Collections;

public class ExplosionAdvanced : MonoBehaviour
{
    public float explosionRadius = 5.0f;
    public float explosionPower = 10.0f;
    public float explosionDamage = 100.0f;
    public float explosionTimeout = 2.0f;
    public bool destryObject = true;
    public AudioClip[] explosionSounds;

    IEnumerator Start()
    {
        PlaySounds();
        Vector3 explosionPosition = transform.position;

        // Apply damage to close by objects first
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (var hit in colliders)
        {
            // Calculate distance from the explosion position to the closest point on the collider
            Vector3 closestPoint = hit.ClosestPointOnBounds(explosionPosition);
            float distance = Vector3.Distance(closestPoint, explosionPosition);

            // The hit points we apply fall decrease with distance from the explosion point
            float hitPoints = 1.0f - Mathf.Clamp01(distance / explosionRadius);
            hitPoints *= explosionDamage;

            // Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
            hit.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
            hit.SendMessageUpwards("PlayerDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
            hit.SendMessageUpwards("Exploasion", hitPoints, SendMessageOptions.DontRequireReceiver);
        }

        // Apply explosion forces to all rigidbodies
        // This needs to be in two steps for ragdolls to work correctly.
        // (Enemies are first turned into ragdolls with ApplyDamage then we apply forces to all the spawned body parts)
        colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Rigidbody>())
                hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 3.0f);
        }

        // stop emitting particles
        if (GetComponent<ParticleEmitter>())
        {
            GetComponent<ParticleEmitter>().emit = true;
            yield return new WaitForSeconds(0.5f);
            GetComponent<ParticleEmitter>().emit = false;
        }

        // destroy the explosion after a while
        if (GetComponent<AudioSource>() && explosionSounds.Length > 0)
        {
            yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
            Destroy(gameObject);
        }
        else
        {
            if (destryObject) Destroy(gameObject, explosionTimeout);
        }
    }

    public IEnumerator ApplyForce(Rigidbody body)
    {
        yield return new WaitForSeconds(.4f);
        Vector3 direction = body.transform.position - transform.position;
        body.AddForceAtPosition(direction.normalized, transform.position);
    }

    void PlaySounds()
    {
        if (explosionSounds.Length > 0)
        {
            GetComponent<AudioSource>().clip = explosionSounds[Random.Range(0, explosionSounds.Length)];
            GetComponent<AudioSource>().Play();
        }
    }
}