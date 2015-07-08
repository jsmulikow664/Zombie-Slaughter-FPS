using UnityEngine;
using System.Collections;

public class Detach : MonoBehaviour
{

    public GameObject[] unparentWheels;
    public float hitPoints = 100;
    public Transform explosion;
    public GameObject body;
    public GameObject trigger;

    public void ApplyDamage(float damage)
    {
        if (hitPoints <= 0.0f)
            return;

        // Apply damage
        hitPoints -= damage;

        // Are we dead?
        if (hitPoints <= 0.0f) Detonate();
    }
    void Detonate()
    {

        /*Component[] coms = GetComponentsInChildren<MonoBehaviour>();
        foreach(var b in coms) {
            MonoBehaviour p = b as MonoBehaviour;
            if (p)
                p.enabled = false;
        }*/
        trigger.SetActive(false);
        for (int i = 0; i < unparentWheels.Length; i++)
        {
            unparentWheels[i].transform.parent = null;
            unparentWheels[i].AddComponent<MeshCollider>();
            unparentWheels[i].AddComponent<Rigidbody>();
            //unparentWheels[i].transform.position.y += 1;

        }
        Instantiate(explosion, body.transform.position, body.transform.rotation);
        transform.DetachChildren();

    }

}