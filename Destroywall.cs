using UnityEngine;
using System.Collections;

public class Destroywall : MonoBehaviour {
public float timeOut = 3.0f;
public Rigidbody explosion;
   
   void OnTriggerEnter(Collider other) 
   {
    if(other.gameObject.tag == "RPG Destroy")
	{
      Rigidbody clone;
       clone = Instantiate(explosion, transform.position, transform.rotation) as Rigidbody;
            clone.velocity = transform.TransformDirection(Vector3.forward * 10);
       Destroy(other.gameObject);
    }
    
}
    void Start()
    {
        Invoke("Kill", timeOut);
    }

    
    void Kill()
    {
        // Stop emitting particles in any children
        ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
        if (emitter)
            emitter.emit = false;

        // Detach children - We do this to detach the trail rendererer which should be set up to auto destruct
        transform.DetachChildren();

        // Destroy the projectile
        Destroy(gameObject);
    }
} 
