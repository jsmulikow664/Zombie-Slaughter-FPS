using UnityEngine;
using System.Collections;

public class Activate : MonoBehaviour
{
    public GameObject GO;

    public void ApplyDamage()
    {
        Action();
    }

    void Action()
    {
        GO.SendMessage("Action", SendMessageOptions.DontRequireReceiver);
    }
}