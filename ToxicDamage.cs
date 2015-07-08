using UnityEngine;
using System.Collections;

public class ToxicDamage : MonoBehaviour {

Transform player;
float damage = 0.1f;
void Awake ()
{
player = GameObject.FindGameObjectWithTag ("Player").transform;
}

   void OnTriggerStay (Collider other) 
	  {
	
if(other.gameObject.tag == "Player")
{
//Debug.Log("PlayerDamage" + damage);
other.GetComponent<Collider>().SendMessageUpwards("PlayerDamage", damage, SendMessageOptions.RequireReceiver);
}
}
}