using UnityEngine;
using System.Collections;

public class MeleeScript : MonoBehaviour
{

    //ANIMATION OF YOUR WEAPONS
    public GameObject weaponAnim;
    public string idleAnim = "Idle";

    public float meleeAttackRange = 3.0f;       		 //how far you can hit someone or something (i suggest to use from 1 to 3).
    public float meleeDamage = 300.0f;             	 //how much damage will receive enemy.
    public AudioClip meleeSlash;
    public GameObject meleeHitUntagged;
    public GameObject meleeHitEnemy;
    private bool inAttack = false;

    //MELEE ANIMATIONS
    public string axeHit = "HitAnim";
    public string DrawAnimation = "DrawAXE";
    public AudioClip soundDraw;
    public AudioClip soundAxeSlash;

    //CROSSHAIR
    public Texture2D crosshair;

    //GUI
    public GUISkin mySkin;

    public float force = 400;
    public bool selected = false;
    public LayerMask layerMask;

    void Start()
    {
        weaponAnim.GetComponent<Animation>().wrapMode = WrapMode.Loop;
        weaponAnim.GetComponent<Animation>()[axeHit].wrapMode = WrapMode.Once;
        weaponAnim.GetComponent<Animation>()[axeHit].layer = 1;
        inAttack = false;
    }

    void Update()
    {
        if (selected)
        {

            if (Input.GetButtonDown("Fire"))
            {
                StartCoroutine(MeleeAttack());
            }

            if (!inAttack)
            {
                weaponAnim.GetComponent<Animation>().CrossFade(idleAnim);
            }
        }
    }

    void OnGUI()
    {

        if (crosshair != null)
        {
            float w = crosshair.width / 2;
            float h = crosshair.height / 2;
            Rect position = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
            GUI.DrawTexture(position, crosshair);
        }

    }

    public IEnumerator MeleeAttack()
    {
        if (inAttack)
            yield break;
        inAttack = true;
        if (axeHit != "")
        {
            weaponAnim.GetComponent<Animation>()[axeHit].speed = weaponAnim.GetComponent<Animation>()[axeHit].clip.length / 1.0f;
            weaponAnim.GetComponent<Animation>().Play(axeHit);
        }
        yield return new WaitForSeconds(0.3f);
        GetComponent<AudioSource>().clip = soundAxeSlash;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(MeleeAttackHit());
    }


    IEnumerator MeleeAttackHit()
    {
        Vector3 direction = transform.TransformDirection(0, 0, 1);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, 1000.0f, layerMask.value))
        {
            Vector3 contact = hit.point;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.distance < meleeAttackRange)
            {

                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
                }

                if (hit.transform.tag == "Untagged" || hit.transform.tag == "Concrete" || hit.transform.tag == "Dirt" || hit.transform.tag == "Wood" || hit.transform.tag == "Metal")
                {
                    GameObject default1 = Instantiate(meleeHitUntagged, contact, rotation) as GameObject;
                    default1.transform.position = hit.point;
                    default1.transform.localPosition += .02f * hit.normal;
                    GetComponent<AudioSource>().clip = meleeSlash;
                    GetComponent<AudioSource>().Play();
                }

                if (hit.transform.tag == "Enemy")
                {
                    Instantiate(meleeHitEnemy, contact, rotation);
                }

                hit.collider.SendMessageUpwards("ApplyDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
        yield return new WaitForSeconds(.5f);
        inAttack = false;
    }

    IEnumerator DrawWeapon()
    {
        //draw = true;
        GetComponent<AudioSource>().clip = soundDraw;
        GetComponent<AudioSource>().Play();
        weaponAnim.GetComponent<Animation>()[DrawAnimation].speed = weaponAnim.GetComponent<Animation>()[DrawAnimation].clip.length / 0.9f;
        weaponAnim.GetComponent<Animation>().Play(DrawAnimation);
        yield return new WaitForSeconds(0.6f);

        selected = true;
    }

    void Deselect()
    {
        selected = false;
    }

}