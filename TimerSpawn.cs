using UnityEngine;
using System.Collections;

public class TimerSpawn : MonoBehaviour {

public GameObject gamemanager; 

	void Awake () {
	// gamemanager = gameObject;
	  StartCoroutine("Wait");
	}

IEnumerator Wait() {
yield return new WaitForSeconds(50.0F);
 gamemanager.GetComponent<GameManager>().enabled = false;
}
}