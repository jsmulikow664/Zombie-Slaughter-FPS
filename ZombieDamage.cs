using UnityEngine;
using System.Collections;

public class ZombieDamage : MonoBehaviour
{

    public float maximumHitPoints = 100.0f;
    public float hitPoints = 100.0f;
    public Rigidbody deadReplacement;
    public GameObject GOPos;
    public ScoreManager scoreManager;

    void Start(){	
	scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
}

    void ApplyDamage(float damage)
    {
        if (hitPoints <= 0.0f)
            return;

        // Apply damage
        hitPoints -= damage;
        scoreManager.DrawCrosshair();
        // Are we dead?
        if (hitPoints <= 0.0f)
            Replace();
    }

    void Replace()
    {

        // If we have a dead barrel then replace ourselves with it!
        if (deadReplacement)
        {
            Rigidbody dead = Instantiate(deadReplacement, GOPos.transform.position, GOPos.transform.rotation) as Rigidbody;
            scoreManager.addScore(20);
            // For better effect we assign the same velocity to the exploded barrel
            dead.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            dead.angularVelocity = GetComponent<Rigidbody>().angularVelocity;
        }
        // Destroy ourselves
        Destroy(gameObject);
    }


}