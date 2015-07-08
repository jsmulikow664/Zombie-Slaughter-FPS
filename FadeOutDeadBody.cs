using UnityEngine;
using System.Collections;

public class FadeOutDeadBody : MonoBehaviour
{
    public GameObject target;
    public float fadeDuration = 3.0f;

    void Update()
    {

        if (target.GetComponent<Renderer>().material.color.a > 0)
            //target.renderer.material.color.a -= Time.deltaTime / fadeDuration;
            target.GetComponent<Renderer>().material.color = new Color(target.GetComponent<Renderer>().material.color.r, target.GetComponent<Renderer>().material.color.g, target.GetComponent<Renderer>().material.color.b, target.GetComponent<Renderer>().material.color.a - Time.deltaTime / fadeDuration);
    }

}