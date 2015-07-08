using UnityEngine;
using System.Collections;

public class StabilizeKickback : MonoBehaviour
{
    public float returnSpeed = 2.0f;

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
    }
}