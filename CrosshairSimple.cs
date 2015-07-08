using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CrosshairSimple : MonoBehaviour
{
    public Texture2D crosshair;

    void OnGUI()
    {
        float w = crosshair.width / 2;
        float h = crosshair.height / 2;
        Rect position = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);

        if (!Input.GetButton("Fire2"))
        {
            GUI.DrawTexture(position, crosshair);
        }
    }
}