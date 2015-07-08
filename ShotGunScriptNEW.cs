using UnityEngine;
using System.Collections;

#pragma warning disable 0219

public class ShotGunScriptNEW : MonoBehaviour
{

    /**
    *  Script written by OMA [www.armedunity.com]
    **/

    //@script ExecuteInEditMode()

    public AudioClip soundDraw;
    public AudioClip soundFire;
    public AudioClip soundReload;
    public GameObject soundGO;
    public AudioClip soundEmpty;

    public GameObject weaponAnim;
    public GameObject Concrete;
    public GameObject Wood;
    public GameObject Metal;
    public GameObject Dirt;
    public GameObject Blood;
    public GameObject untagged;

    public LayerMask layerMask;
    public Renderer muzzleFlash;
    public Light muzzleLight;
    public int pelletsPerShot = 10;

    public int damage = 50;
    public int bulletsPerMag = 50;
    public int magazines = 5;
    public float fireRate = 0.1f;
    public float reloadTime = 3.0f;
    public float drawTime = 1.5f;

    public float range = 250.0f;
    public float force = 200.0f;

    public float inacuracy = 0.2f;

    //Aiming
    public Vector3 hipPosition;
    public Vector3 aimPosition;
    private bool aiming;
    private Vector3 curVect;
    public float aimSpeed = 0.25f;

    //Field Of View
    public float zoomSpeed = 0.5f;
    public int FOV = 40;

    private int bulletsLeft = 0;
    private int m_LastFrameShot = -10;
    private float nextFireTime = 0.0f;
    private bool reloading;
    private bool draw;
    private GameObject weaponCamera;
    private GameObject mainCamera;
    private bool playing = false;
    [HideInInspector]
    public bool selected = false;
    //private GameObject player;

    //GUI
    public GUISkin mySkin;

    //KickBack
    public Transform kickGO;
    public float kickUpside = 0.5f;
    public float kickSideways = 0.5f;

    //Crosshair Textures
    public Texture2D crosshair;

    void Start()
    {
        weaponCamera = GameObject.FindWithTag("WeaponCamera");
        mainCamera = GameObject.FindWithTag("MainCamera");
        //player = GameObject.FindWithTag("Player");
        muzzleFlash.enabled = false;
        muzzleLight.enabled = false;
        bulletsLeft = bulletsPerMag;
        aiming = false;
    }

    void Update()
    {
        if (selected)
        {

            if (Input.GetButtonDown("Fire"))
            {
                FireShotgun();
            }

            if (Input.GetButtonDown("Reload"))
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetButton("Fire2") && !Input.GetButton("Run") && !reloading)
        {
            if (!aiming)
            {
                aiming = true;
                curVect = aimPosition - transform.localPosition;
            }
            if (transform.localPosition != aimPosition && aiming)
            {
                if (Mathf.Abs(Vector3.Distance(transform.localPosition, aimPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
                {
                    transform.localPosition = aimPosition;
                }
                else
                {
                    transform.localPosition += curVect / aimSpeed * Time.deltaTime;
                }
            }
        }
        else
        {
            if (aiming)
            {
                aiming = false;
                curVect = hipPosition - transform.localPosition;
                Renderer[] go = GetComponentsInChildren<Renderer>();
                foreach (Renderer g in go)
                {
                    if (g.name != "muzzle_flash")
                        g.GetComponent<Renderer>().enabled = true;
                }
            }

            if (Mathf.Abs(Vector3.Distance(transform.localPosition, hipPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
            {
                transform.localPosition = hipPosition;
            }
            else
            {
                transform.localPosition += curVect / aimSpeed * Time.deltaTime;
            }
        }

        if (aiming)
        {
            mainCamera.GetComponent<Camera>().fieldOfView -= FOV * Time.deltaTime / zoomSpeed;
            if (mainCamera.GetComponent<Camera>().fieldOfView < FOV)
            {
                mainCamera.GetComponent<Camera>().fieldOfView = FOV;
            }
            weaponCamera.GetComponent<Camera>().fieldOfView -= 40 * Time.deltaTime / zoomSpeed;
            if (weaponCamera.GetComponent<Camera>().fieldOfView < 40)
            {
                weaponCamera.GetComponent<Camera>().fieldOfView = 40;
            }

        }
        else
        {
            mainCamera.GetComponent<Camera>().fieldOfView += 60 * Time.deltaTime / 0.5f;
            if (mainCamera.GetComponent<Camera>().fieldOfView > 60)
            {
                mainCamera.GetComponent<Camera>().fieldOfView = 60;
            }
            weaponCamera.GetComponent<Camera>().fieldOfView += 50 * Time.deltaTime / 0.5f;
            if (weaponCamera.GetComponent<Camera>().fieldOfView > 50)
            {
                weaponCamera.GetComponent<Camera>().fieldOfView = 50;
            }
        }
    }

    void LateUpdate()
    {
        if (m_LastFrameShot == Time.frameCount)
        {
            muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            muzzleFlash.enabled = true;
            muzzleLight.enabled = true;
        }
        else
        {
            muzzleFlash.enabled = false;
            muzzleLight.enabled = false;
        }
    }

    void OnGUI()
    {
        if (selected)
        {
            GUI.skin = mySkin;
            GUIStyle style1 = mySkin.customStyles[0];


            GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Ammo : ");
            GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + bulletsLeft, style1);
            GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 200, 80), " / " + magazines);

            if (crosshair != null)
            {
                float w = crosshair.width / 2;
                float h = crosshair.height / 2;
                Rect position = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
                if (!aiming)
                {
                    GUI.DrawTexture(position, crosshair);
                }
            }
        }
    }
    void FireShotgun()
    {
        if (reloading || bulletsLeft <= 0 || draw)
        {
            if (bulletsLeft == 0)
            {
                StartCoroutine(OutOfAmmo());
            }
            return;
        }

        int pellets = 0;

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;
        if (Time.time > nextFireTime)
        {
            while (pellets < pelletsPerShot)
            {
                FireOneShot();
                pellets++;
            }
            bulletsLeft--;
            nextFireTime = Time.time + fireRate;
            KickBack();
        }
    }

    void FireOneShot()
    {

        Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inacuracy, Random.Range(-0.01f, 0.01f) * inacuracy, 1));
        RaycastHit hit;
        Vector3 position = transform.parent.position;

        if (Physics.Raycast(position, direction, out hit, range, layerMask.value))
        {

            Vector3 contact = hit.point;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
            }

            if (hit.transform.tag == "Untagged")
            {
                GameObject default1 = Instantiate(untagged, contact, rotation) as GameObject;
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                default1.transform.parent = hit.transform;
            }

            if (hit.transform.tag == "Concrete")
            {
                GameObject bulletHole = Instantiate(Concrete, contact, rotation) as GameObject;
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                bulletHole.transform.parent = hit.transform;
            }

            if (hit.transform.tag == "Wood")
            {
                GameObject woodHole = Instantiate(Wood, contact, rotation) as GameObject;
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                woodHole.transform.parent = hit.transform;
            }

            if (hit.transform.tag == "Metal")
            {
                GameObject metalHole = Instantiate(Metal, contact, rotation) as GameObject;
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                metalHole.transform.parent = hit.transform;
            }


            if (hit.transform.tag == "Dirt")
            {
                GameObject dirtHole = Instantiate(Dirt, contact, rotation) as GameObject;
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                dirtHole.transform.parent = hit.transform;
            }

            if (hit.transform.tag == "canBeUsed")
            {
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            }

            if (hit.transform.tag == "Enemy")
            {
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                GameObject bloodHole = Instantiate(Blood, contact, rotation) as GameObject;
                if (Physics.Raycast(position, direction, out hit, range, layerMask.value))
                {
                    if (hit.rigidbody)
                    {
                        hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
                    }
                }
            }
        }


        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().clip = soundFire;
            GetComponent<AudioSource>().Play();
        }
        m_LastFrameShot = Time.frameCount;

        weaponAnim.GetComponent<Animation>().Rewind("ShotgunFire");
        weaponAnim.GetComponent<Animation>().Play("ShotgunFire");
    }
    IEnumerator OutOfAmmo()
    {
        if (reloading || playing)
            yield break;

        playing = true;
        soundGO.GetComponent<AudioSource>().clip = soundEmpty;
        soundGO.GetComponent<AudioSource>().volume = 0.7f;
        soundGO.GetComponent<AudioSource>().Play();

        weaponAnim.GetComponent<Animation>()["Fire"].speed = 2.0f;
        weaponAnim.GetComponent<Animation>().Play("Fire");
        yield return new WaitForSeconds(0.2f);
        playing = false;
    }
    IEnumerator Reload()
    {
        if (reloading)
            yield break;

        reloading = true;
        if (magazines > 0 && bulletsLeft < bulletsPerMag)
        {
            weaponAnim.GetComponent<Animation>()["ShotgunReload"].speed = .8f;
            weaponAnim.GetComponent<Animation>().Play("ShotgunReload", PlayMode.StopAll);
            weaponAnim.GetComponent<Animation>().CrossFade("ShotgunReload");
            GetComponent<AudioSource>().PlayOneShot(soundReload);
            yield return new WaitForSeconds(reloadTime);
            magazines--;
            magazines--;
            bulletsLeft = bulletsPerMag;
        }
        reloading = false;
    }

    void KickBack()
    {
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUpside, Random.Range(-kickSideways, kickSideways), 0));
    }

    IEnumerator DrawWeapon()
    {
        draw = true;
        GetComponent<AudioSource>().clip = soundDraw;
        GetComponent<AudioSource>().Play();
        weaponAnim.GetComponent<Animation>().Play("Draw", PlayMode.StopAll);
        weaponAnim.GetComponent<Animation>().CrossFade("Draw");
        yield return new WaitForSeconds(drawTime);
        draw = false;
        reloading = false;
        selected = true;
    }

    void Deselect()
    {
        selected = false;
        mainCamera.GetComponent<Camera>().fieldOfView = 60;
        weaponCamera.GetComponent<Camera>().fieldOfView = 50;
        transform.localPosition = hipPosition;
    }
}