using UnityEngine; 
using System.Collections;

public class ZombieAI1 : MonoBehaviour {


NavMeshAgent nav;
Transform player;
Animator controller;
float health;
GameManager game;
public AudioClip deathzombie;
public AudioClip awakezombie;
CapsuleCollider capsuleCollider;
Animator anim;
bool isDead = false;
float damage = 5;
int pointValue = 10;
public float attackTimer;
public float timeToAttack;
 public Rigidbody projectile;

//public AudioClip zombieAttackClip;


	// Use this for initialization
	void Awake () {
	nav = GetComponent <NavMeshAgent> ();
	player = GameObject.FindGameObjectWithTag ("Player").transform;
	controller = GetComponentInParent<Animator> ();
	game = FindObjectOfType <GameManager> ();
    health = 200 + (1.25f * game.round);
	capsuleCollider = GetComponent <CapsuleCollider> ();
	anim = GetComponent <Animator> ();
}
	
	
	// Update is called once per frame
	void Update () {
	
	if (!isDead)
	{
nav.SetDestination (player.position);
	controller.SetFloat ("speed", Mathf.Abs(nav.velocity.x) + Mathf.Abs (nav.velocity.z));
	float distance = Vector3.Distance(transform.position, player.position);
	if(distance < 3)
	{
	attackTimer += Time.deltaTime;
	}
	else if (attackTimer > 0)
	{
	attackTimer -= Time.deltaTime*2;
	}
	else 
	attackTimer = 0;
	}
}

bool Attack ()
{
if(attackTimer > timeToAttack)
{
//sound
attackTimer = 0;
return true;
}
return false;
}

//Send message to player to die!
void Death ()
{	
// the enemy is dead
isDead = true;
  GetComponent<AudioSource>().Stop();
 GetComponent<AudioSource>().PlayOneShot(deathzombie);
nav.Stop();
//Turn the controller into a trigger so shots can pass through it.
capsuleCollider.isTrigger = true;

//anim.SetTrigger ("isDead");
//anim.SetTrigger ("Dead");
//GameManager.score += 10;	
//GameManager.money += 10;		
// Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing)
//enemyAudio.clip = deathClip;
//enemyAudio.Play();
 Rigidbody clone;
 clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;

 GameManager.zombiesLeftInRound -= 1;
 Destroy (gameObject);

	}


// Apply damage to player game object
	void ApplyDamage (float damage)
	{
health -= damage;

if(!isDead)
{
GameManager.AddPoints(pointValue);
if (health <= 0)
{
Death();

}
}	


}
      void OnTriggerEnter(Collider other) 
	  {
	
if(other.gameObject.tag == "Player")
{
//Debug.Log("PlayerDamage" + damage);
other.GetComponent<Collider>().SendMessageUpwards("PlayerDamage", damage, SendMessageOptions.RequireReceiver);
}
if(other.gameObject.tag == "Bullet")
{
 Rigidbody clone;
 clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
 Destroy(gameObject);
 }
}




}



