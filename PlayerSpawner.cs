using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] destination;
    public GameObject Player;

    void SpawnPlayer()
    {
        GameObject player = Instantiate(Player, Vector3.zero, Quaternion.identity) as GameObject;
        player.name = "Player";
        player.transform.position = destination[Random.Range(0, destination.Length)].transform.position;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

}