using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{
    public Light linkedLight;

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            linkedLight.enabled = !linkedLight.enabled;
        }
    }

    void LightOff()
    {
        linkedLight.enabled = false;
    }
}