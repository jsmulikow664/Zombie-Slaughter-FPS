using UnityEngine;
using System.Collections;

public class TriggerLevel : MonoBehaviour {
public int LoadLevel;


  void OnTriggerEnter(Collider other) {
          if(other.gameObject.tag == "Player")
	{
		Application.LoadLevel(LoadLevel);
    }
}
}