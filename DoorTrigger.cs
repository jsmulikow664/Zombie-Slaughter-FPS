using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    public GameObject door;
    private bool opened = false;
    public float waitTime; 				//Time to Open door (animation time)
    private bool IRuzspiests = false;

    void Close()
    {
        door.GetComponent<Animation>().CrossFade("Close");
    }

    void Open()
    {
        door.GetComponent<Animation>().CrossFade("Open");
    }

    public IEnumerator Action()
    {
        if (!opened && !IRuzspiests)
        {
            Open();
            opened = true;
            IRuzspiests = true;
            yield return new WaitForSeconds(waitTime);
            IRuzspiests = false;
        }
        else if (opened && !IRuzspiests)
        {
            Close();
            opened = false;
            IRuzspiests = true;
            yield return new WaitForSeconds(waitTime);
            IRuzspiests = false;
        }
    }

    public void ApplyDamage()
    {
        StartCoroutine(Action());
    }
}