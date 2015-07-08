using UnityEngine;
using System.Collections;

public class AIRigidbody : MonoBehaviour
{
    // These variables are for adjusting in the inspector how the object behaves
    public float maxSpeed = 7.000f;
    public float force = 8.000f;

    // Don't let the Physics Engine rotate this physics object so it doesn't fall over when running
    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
    }

    // This is called every physics frame
    void FixedUpdate()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
        {
            GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward);
            GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.right);
        }
    }
}