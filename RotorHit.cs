using UnityEngine;
using System.Collections;

public class RotorHit : MonoBehaviour
{
    public GameObject hitPrefab;

    void OnTriggerEnter()
    {


        //hit effect
        Instantiate(hitPrefab, transform.position, transform.rotation);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Instantiate(hitPrefab, contact.point, Quaternion.LookRotation(Vector3.up, contact.normal));   //contact.point; //this is the Vector3 position of the point of contact
    }
}