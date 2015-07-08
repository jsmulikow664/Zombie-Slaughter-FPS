using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

public float timer;
public float flashTimer;
public float flashDuration;
public float spinSpeed;
public float rotation;
	
	// Update is called once per frame
	void Update () 
	{
	timer += Time.deltaTime;
	if (timer > 10f)
	{
	rotation += spinSpeed * Time.deltaTime;
	} 
	else if (timer > 0)
	{
	flashTimer -= Time.deltaTime;
	if(flashTimer <= 0)
	{
	flashTimer = flashDuration;
	Flash();
	}
		rotation += spinSpeed  * 2 * Time.deltaTime;
	}
	else 
	{
	Destroy (gameObject);
	}
	transform.rotation = Quaternion.Euler (0, rotation, 0);
	}
	
	void Flash ()
	{
	MeshRenderer renderer = GetComponentInParent<MeshRenderer>();
	renderer.enabled = !renderer.enabled;
	Light light = GetComponentInParent<Light> ();
	light.enabled =!light.enabled;
	}
	
	public void DisablePickup()
	{
	MeshRenderer renderer = GetComponentInParent<MeshRenderer>();
	renderer.enabled = false;
	Light light = GetComponentInParent<Light> ();
	light.enabled = false;
	BoxCollider collider = GetComponentInParent<BoxCollider>();
	collider.enabled = false;
	
	Destroy(gameObject, 2f);
	}
	}

