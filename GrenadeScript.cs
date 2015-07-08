using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour
{

    //ANIMATIONS
    public GameObject weaponAnim;
    public string idleAnim = "Idle";
    public string DrawAnimation = "DrawAXE";
    public string grenadeThrow = "GrenadeThrow";
    public string grenadePull = "GrenadePull";
    public AudioClip soundDraw;

    //GRENADE
    public int grenadeCount = 3;
    public float throwForce = 15.0f;
    public float powerIncreaseSpeed = 80.0f;
    private float powerBarFullWidth = 100.0f;
    private float thePower = 0.0f;
    private bool increasing = false;
    private bool throwingGrenade = false;

    public Rigidbody grenade;
    public Transform spawnPos;

    public Renderer grenadeRenderer;

    //CROSSHAIR
    public Texture2D crosshair;

    //GUI
    public GUISkin mySkin;

    public bool selected = false;

    void Start()
    {
        weaponAnim.GetComponent<Animation>()[grenadePull].wrapMode = WrapMode.ClampForever;
    }

    void Update()
    {
        if (selected)
        {
            if (grenadeCount > 0 && !throwingGrenade)
            {

                if (Input.GetButtonDown("Fire"))
                {
                    if (thePower < 5)
                    {
                        weaponAnim.GetComponent<Animation>().Stop();
                        weaponAnim.GetComponent<Animation>()[grenadePull].speed = 1.0f;
                        weaponAnim.GetComponent<Animation>().CrossFade(grenadePull);
                        increasing = true;
                    }
                }

                if (Input.GetButtonUp("Fire"))
                {
                    increasing = false;
                    if (thePower > powerBarFullWidth / 2)
                    {
                        StartCoroutine(ThrowGrenade(thePower));
                    }
                    else
                    {
                        weaponAnim.GetComponent<Animation>()[grenadePull].speed = -1.0f;
                        weaponAnim.GetComponent<Animation>().CrossFade(grenadePull);
                    }
                }

                if (thePower < 0)
                {
                    thePower = 0;
                }

                if (increasing)
                {
                    thePower += Time.deltaTime * powerIncreaseSpeed;
                    thePower = Mathf.Clamp(thePower, 0, powerBarFullWidth);
                }
                else
                {
                    if (thePower > 0)
                        thePower -= Time.deltaTime * powerIncreaseSpeed * 2;
                }
            }
        }
    }

    void OnGUI()
    {

        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];

        GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Grenades : ");
        GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + grenadeCount, style1);

        GUI.Label(new Rect(Screen.width - 200, Screen.height - 65, 200, 80), "Throwing Power : ");
        GUI.Label(new Rect(Screen.width - 70, Screen.height - 65, 200, 80), "" + thePower.ToString("F0") + "%", style1);

        if (crosshair != null)
        {
            float w = crosshair.width / 2;
            float h = crosshair.height / 2;
            Rect position = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
            GUI.DrawTexture(position, crosshair);
        }
    }

    IEnumerator ThrowGrenade(float power)
    {
        if (throwingGrenade || grenadeCount <= 0)
            yield break;

        throwingGrenade = true;
        weaponAnim.GetComponent<Animation>()[grenadeThrow].speed = weaponAnim.GetComponent<Animation>()[grenadeThrow].clip.length / 0.4f;
        weaponAnim.GetComponent<Animation>().Play(grenadeThrow);
        yield return new WaitForSeconds(0.2f);
        grenadeRenderer.enabled = false;
        Rigidbody instantGrenade = Instantiate(grenade, spawnPos.position, spawnPos.rotation) as Rigidbody;

        Vector3 fwd = spawnPos.forward;
        instantGrenade.AddForce(fwd * power * throwForce);
        //Physics.IgnoreCollision(instantGrenade.collider, transform.root.collider);
        //Physics.IgnoreCollision(instantGrenade.collider, gameObject.FindWithTag("Player").transform.root.collider);
        grenadeCount--;

        yield return new WaitForSeconds(1.0f);
        grenadeRenderer.enabled = true;
        throwingGrenade = false;
        StartCoroutine(DrawWeapon());
    }

    public IEnumerator DrawWeapon()
    {
        thePower = 0;
        grenadeRenderer.enabled = true;
        increasing = false;
        //draw = true;
        if (grenadeCount > 0)
        {
            GetComponent<AudioSource>().clip = soundDraw;
            GetComponent<AudioSource>().Play();
            weaponAnim.GetComponent<Animation>()[DrawAnimation].speed = weaponAnim.GetComponent<Animation>()[DrawAnimation].clip.length / 0.9f;
            weaponAnim.GetComponent<Animation>().Play(DrawAnimation);
            yield return new WaitForSeconds(0.6f);
        }
        selected = true;
    }

    void Deselect()
    {
        selected = false;
    }
}