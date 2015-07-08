using UnityEngine;
using System.Collections;

public class DeadPlayerCamera : MonoBehaviour
{
    public GameObject damage;

    void Update()
    {
        damage.GetComponent<GUITexture>().enabled = true;
    }
}