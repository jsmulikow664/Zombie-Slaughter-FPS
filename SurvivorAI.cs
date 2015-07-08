using UnityEngine;
using System.Collections;

public class SurvivorAI : MonoBehaviour {

Transform Follow;
float AISpeed = 2;
float MaxDistance = 10;
float MinDistance = 2;

	// Use this for initialization
	void Start () {
    Follow = GameObject.FindGameObjectWithTag("Survivor AI").transform;
	}
	
	// Update is called once per frame
	void Update () {
	transform.LookAt(Follow);
	AI();
	}

	void AI()
	{
	if (Vector3.Distance (transform.position, Follow.position) >= MinDistance)
	{
	transform.position += transform.forward*AISpeed*Time.deltaTime;
	
	if (Vector3.Distance (transform.position, Follow.position) >= MaxDistance)
	{
//	AIDie();
	}
	}
	}
	
	
	}
