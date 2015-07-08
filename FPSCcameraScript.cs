using UnityEngine;
using System.Collections;

public class FPSCcameraScript : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    public float maxRayDistance = 2.0f;
    public LayerMask layerMask;
    public GUISkin mySkin;
    public bool showGui = false;

    void Update()
    {
        Vector3 direction = gameObject.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Vector3 position = transform.position;
        if (Physics.Raycast(position, direction, out hit, maxRayDistance, layerMask.value))
        {
            showGui = true;
            if (Input.GetButtonDown("Use"))
            {
                GameObject target = hit.collider.gameObject;
                target.BroadcastMessage("Action");
            }
        }
        else
        {
            showGui = false;
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;
        if (showGui)
        {
            //GUI.Label(new Rect(Screen.width - (Screen.width / 1.7f), Screen.height - (Screen.height / 1.4f), 800, 100), "Press key >>E<< to Use");
        }
    }
}