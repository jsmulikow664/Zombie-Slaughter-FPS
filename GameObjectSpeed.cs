using UnityEngine;
using System.Collections;

public class GameObjectSpeed : MonoBehaviour
{

    public float speed = 0;
    private Vector3 lastPosition = Vector3.zero;

    void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (speed > 0.2f)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Metal"))
            {
                other.gameObject.BroadcastMessage("ApplyDamage", speed * 3000, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}