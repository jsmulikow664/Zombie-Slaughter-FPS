using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

public int LoadLevel;

public void ChangeToScene (int sceneToChangeTo) {
Application.LoadLevel(sceneToChangeTo);
}
}