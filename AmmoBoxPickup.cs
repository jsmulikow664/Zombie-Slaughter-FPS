using UnityEngine;
using System.Collections;

public class AmmoBoxPickup : MonoBehaviour 
{
public AudioClip pickupClip;

void OnTriggerEnter(Collider other)
{
if(other.gameObject.tag == "Player")
{
WeaponManager wManager = FindObjectOfType<WeaponManager>();
wManager.FillAmmo ();

GetComponent<AudioSource>().clip = pickupClip;
GetComponent<AudioSource>().Play();
PickupItem item = GetComponentInParent<PickupItem>();
StartCoroutine("Wait");
}
}

IEnumerator Wait() {
yield return new WaitForSeconds(0.2F);
Destroy(gameObject);
}

}
