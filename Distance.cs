using UnityEngine;
using System.Collections;

public class Distance : MonoBehaviour
{
    public Transform target;
    private Transform myTransform;
    public TextMesh distanceTextMesh;
    public string currentDistance = "";

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        if (target)
        {
            float distance = (target.position - myTransform.position).magnitude;
            currentDistance = distance.ToString("F0");
            distanceTextMesh.text = currentDistance;
        }
    }
}