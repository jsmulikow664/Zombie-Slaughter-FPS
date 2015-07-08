using UnityEngine;
using System.Collections;

public class LiftScript : MonoBehaviour
{

    public Transform platform;
    public Transform player;

    void OnTriggerEnter()
    {
        GameObject temp = GameObject.Find("Player");
        player = temp.transform;
        Transform GO = platform;
        Transform GO1 = player;
        GO1.transform.parent = GO.transform;
    }

    void OnTriggerExit()
    {
        Transform GO1 = player;
        GO1.transform.parent = null;
    }
}