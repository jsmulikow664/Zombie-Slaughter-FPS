	// ping-pong animate background color
using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour {
    public Color color1 = Color.white;
    public Color color2 = Color.black;
    public float duration = 300.0F;
    
    Camera camera;
    
    void Start() {
    	camera = GetComponent<Camera>();
    	camera.clearFlags = CameraClearFlags.SolidColor;
    }
    
    void Update() {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        camera.backgroundColor = Color.Lerp(color1, color2, t);
    }
}