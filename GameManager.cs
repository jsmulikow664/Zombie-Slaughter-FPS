using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

public int round = 1;
int zombiesInRound = 10;
int zombiesSpawnedInRound = 0;
float zombieSpawnTimer = 0;
public Transform[] zombieSpawnPoints;
public GameObject zombieEnemy;
public GameObject zombieEnemy2;
public GameObject zombieEnemy3;
public GameObject zombieEnemy4;

public static int zombiesLeftInRound = 10;
float countdown = 0;


static int playerScore = 0;
static int playerCash = 0;

public GUISkin mySkin;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
	if(zombiesSpawnedInRound < zombiesInRound && countdown == 0)
	{
	if(zombieSpawnTimer > 5)
	{
	SpawnZombie ();
	zombieSpawnTimer = 0;
	}
	else
	{
	zombieSpawnTimer+=Time.deltaTime;
	}
}
else if (zombiesLeftInRound == 0)
{
StartNextRound ();
}


if (countdown > 0)
countdown -= Time.deltaTime;
else
countdown = 0;


}

public static void AddPoints(int pointValue)
{
playerScore += pointValue;
playerCash += pointValue;
}

void SpawnZombie ()
{
Vector3 randomSpawnPoint = zombieSpawnPoints[Random.Range (0,zombieSpawnPoints.Length)].position;
Instantiate(zombieEnemy,randomSpawnPoint,Quaternion.identity);
zombiesSpawnedInRound ++;
{
Vector3 randomSpawnPoint2 = zombieSpawnPoints[Random.Range (0,zombieSpawnPoints.Length)].position;
Instantiate(zombieEnemy2,randomSpawnPoint2,Quaternion.identity);
zombiesSpawnedInRound ++;
{
Vector3 randomSpawnPoint3 = zombieSpawnPoints[Random.Range (0,zombieSpawnPoints.Length)].position;
Instantiate(zombieEnemy3,randomSpawnPoint3,Quaternion.identity);
zombiesSpawnedInRound ++;
{
Vector3 randomSpawnPoint4 = zombieSpawnPoints[Random.Range (0,zombieSpawnPoints.Length)].position;
Instantiate(zombieEnemy4,randomSpawnPoint4,Quaternion.identity);
zombiesSpawnedInRound ++;
}
}
}
}

void StartNextRound()
{
zombiesInRound = zombiesLeftInRound = round * 10;
zombiesSpawnedInRound = 0;
countdown = 15;
round++;



}

void OnGUI()
{
GUI.skin = mySkin;
GUIStyle style1 = mySkin.customStyles[0];

GUI.Label(new Rect(50, Screen.height - 80, 100, 60), "SCORE :");
GUI.Label(new Rect(100, Screen.height - 80, 100, 60), "" + playerScore);

GUI.Label(new Rect(60, Screen.height - 113, 100, 60), "$ :");
GUI.Label(new Rect(80, Screen.height - 110, 100, 60), "" + playerCash, style1);
GUI.Label(new Rect(300, Screen.height - 850, 300, 200), "Zombie Sounds: http://www.sidneyturner.com/");
}
}