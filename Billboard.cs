using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public Camera lookAtCamera;

    void Update()
    {
        if (lookAtCamera)
        {
            Vector3 v = lookAtCamera.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(lookAtCamera.transform.position - v);
        }
    }
}