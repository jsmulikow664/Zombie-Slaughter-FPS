using UnityEngine;
using System.Collections;

#pragma warning disable 0219
#pragma warning disable 0414

[ExecuteInEditMode]
public class PlayerDamageNew : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    public Rigidbody projectile;
    public float hitPoints;
	public float damage;
    public int regenerationSkill;
    public int shieldSkill;
    public AudioClip painSound;
    public AudioClip die;
    public Transform deadReplacement;
    public GUISkin mySkin;
    public GameObject explShake;
    private GameObject radar;
    public float maxHitPoints;
    public Texture damageTexture;
    private float time = 0.0f;
    private float alpha;
    private bool callFunction = false;
    private ScoreManager scoreManager;
    public int resetLevel;
    void Start()
    {
        maxHitPoints = hitPoints;
        alpha = 0;
    }

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        alpha = time;
        if (hitPoints <= maxHitPoints)
        {
            hitPoints += Time.deltaTime * regenerationSkill;
        }

        if (hitPoints > maxHitPoints)
        {
            float convertToScore = hitPoints - maxHitPoints;
            //scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            //scoreManager.addScore(convertToScore);
            hitPoints = maxHitPoints;
        }
    }

IEnumerator Wait() {
        Debug.Log("start waiting");
        yield return new WaitForSeconds(4.0F);
         Rigidbody clone;
 clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
		 Application.LoadLevel(resetLevel);
		Debug.Log("finished waiting");
    }

	
	
	void Die ()
{	
//palyer is dead
 Rigidbody clone;
 clone = Instantiate(projectile, transform.position, transform.rotation) as Rigidbody;
  StartCoroutine("Wait");
}

	
    void PlayerDamage(float damage)
	{
hitPoints -= damage;
if (hitPoints <= 90)
{
GetComponent<AudioSource>().clip = painSound;
 GetComponent<AudioSource>().PlayOneShot(painSound);
}
if (hitPoints <= 60)
{
GetComponent<AudioSource>().clip = painSound;
 GetComponent<AudioSource>().PlayOneShot(painSound);
}
if (hitPoints <= 30)
{
GetComponent<AudioSource>().clip = painSound;
 GetComponent<AudioSource>().PlayOneShot(painSound);
}
if (hitPoints <= 5)
{
GetComponent<AudioSource>().clip = painSound;
 GetComponent<AudioSource>().PlayOneShot(painSound);
}

if (hitPoints <= 0)
{
GetComponent<AudioSource>().clip = die;
 GetComponent<AudioSource>().PlayOneShot(die);
Die();

}
}	

void OnGUI()
    {
        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];
        GUI.Label(new Rect(40, Screen.height - 50, 60, 60), " Health: ");
        GUI.Label(new Rect(100, Screen.height - 50, 60, 60), "" + hitPoints.ToString("F0"), style1);

        GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha); //Color (r,g,b,a)
      //  GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), damageTexture);
    }
}
  