using UnityEngine;
using System.Collections;

public class PlayerPickup : MonoBehaviour {

public Inventory inventory;

 private void OnTriggerEnter(Collider other) {
if (other.tag == "Item")
{
inventory.AddItem(other.GetComponent<Item>());
}
}
}
