using UnityEngine;
using System.Collections;

public class RotateBullet : MonoBehaviour
{

    void Start()
    {
        transform.Rotate(new Vector3(0, Random.Range(-180.0f, 180.0f), 0));
    }
}