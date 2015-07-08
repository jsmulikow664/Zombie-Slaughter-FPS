using UnityEngine;
using System.Collections;

#pragma warning disable 0219

public enum fireMode { none, semi, auto, burst, launcher }
public enum Ammo { Magazines, Bullets }

[ExecuteInEditMode]
public class WeaponScriptNEW : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/


    public fireMode currentMode = fireMode.semi;
    public fireMode firstMode = fireMode.semi;
    public fireMode secondMode = fireMode.launcher;

    public Ammo ammoMode = Ammo.Magazines;

    public bool canReloadLauncher = false;

    public AudioClip soundDraw;
    public AudioClip soundFire;
    public AudioClip soundReload;
    public AudioClip soundEmpty;
    public AudioClip switchModeSound;

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

    public int damage = 50;
    public int bulletsPerMag = 50;
    public int magazines = 5;
    private float fireRate = 0.1f;
    public float fireRateFirstMode = 0.1f;
    public float fireRateSecondMode = 0.1f;
    public float reloadTime = 3.0f;
    public float reloadAnimSpeed = 1.2f;
    public float drawTime = 1.5f;

    public float range = 250.0f;
    public float force = 200.0f;

    //Weapon accuracy
    private float baseInaccuracy;
    public float baseInaccuracyAIM = 0.005f;
    public float baseInaccuracyHIP = 1.5f;
    public float inaccuracyIncreaseOverTime = 0.2f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    private float maximumInaccuracy;
    public float maxInaccuracyHIP = 5.0f;
    public float maxInaccuracyAIM = 1.0f;

    private float triggerTime = 0.05f;

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
    [HideInInspector]
    public bool reloading;
    private bool draw;
    private GameObject weaponCamera;
    private GameObject mainCamera;
    private bool playing = false;
    [HideInInspector]
    public bool selected = false;
    private GameObject player;
    private bool isFiring = false;
    private bool bursting = false;

    //Burst
    public int shotsPerBurst = 3;
    public float burstTime = 0.07f;

    //GUI
    public GUISkin mySkin;

    //Launcher
    public Rigidbody projectilePrefab;
    public float projectileSpeed = 30.0f;
    public float projectiles = 20;
    public GameObject launchPosition;
    public GameObject fireLauncherAnimGO;
    public AudioClip soundReloadLauncher;
    private bool launcjerReload = false;
    public float launcherReloadTime = 2.0f;
    public string reloadlauncher = "reloadlauncher";
    private GameObject rocket;

    //KickBack
    public Transform kickGO;
    public float kickUpside = 0.5f;
    public float kickSideways = 0.5f;

    //Crosshair Textures
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;
    public Texture2D crosshairSecondMode;
    private float adjustMaxCroshairSize = 6.0f;

    //Switch Weapon Fire Modes
    private bool canSwicthMode = true;

    void Start()
    {
        weaponCamera = GameObject.FindWithTag("WeaponCamera");
        mainCamera = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
        muzzleFlash.enabled = false;
        muzzleLight.enabled = false;
        bulletsLeft = bulletsPerMag;
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        aiming = false;
        if (firstMode == fireMode.launcher)
        {
            rocket = GameObject.Find("RPGrocket");
        }
        if (ammoMode == Ammo.Bullets)
        {
            magazines = magazines * bulletsPerMag;
        }
    }

    void Update()
    {
        if (selected)
        {

            if (Input.GetButtonDown("Fire"))
            {
                if (currentMode == fireMode.semi)
                {
                    fireSemi();
                }

                if (currentMode == fireMode.launcher)
                {
                    fireLauncher();
                }

                if (currentMode == fireMode.burst)
                {
                    fireBurst();
                }

                if (bulletsLeft > 0)
                    isFiring = true;
            }

            if (Input.GetButton("Fire"))
            {
                if (currentMode == fireMode.auto)
                {
                    fireSemi();
                    if (bulletsLeft > 0)
                        isFiring = true;
                }
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
                /*
                FIXME_VAR_TYPE go= GetComponentsInChildren<Renderer>();
                foreach( Renderer g in go){
                if (g.name != "muzzle_flash")
                    g.renderer.enabled = true;
                }
                */
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
            maximumInaccuracy = maxInaccuracyAIM;
            baseInaccuracy = baseInaccuracyAIM;
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
            maximumInaccuracy = maxInaccuracyHIP;
            baseInaccuracy = baseInaccuracyHIP;
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

        if (player.GetComponent<Rigidbody>().velocity.magnitude > 3.0f)
        {
            triggerTime += inaccuracyDecreaseOverTime;
        }

        if (isFiring)
        {
            triggerTime += inaccuracyIncreaseOverTime;
        }
        else
        {
            if (player.GetComponent<Rigidbody>().velocity.magnitude < 3.0f)
                triggerTime -= inaccuracyDecreaseOverTime;
        }

        if (triggerTime >= maximumInaccuracy)
        {
            triggerTime = maximumInaccuracy;
        }

        if (triggerTime <= baseInaccuracy)
        {
            triggerTime = baseInaccuracy;
        }

        if (nextFireTime > Time.time)
        {
            isFiring = false;
        }

        if (Input.GetButtonDown("switchFireMode") && secondMode != fireMode.none && canSwicthMode)
        {
            if (currentMode != firstMode)
            {
                StartCoroutine(FirstFireMode());
            }
            else
            {
                StartCoroutine(SecondFireMode());
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
            if (ammoMode == Ammo.Magazines)
            {
                if (firstMode != fireMode.none && firstMode != fireMode.launcher || secondMode != fireMode.none && secondMode != fireMode.launcher)
                {
                    GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Ammo : ");
                    GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + bulletsLeft, style1);
                    GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 200, 80), " / " + magazines);
                }
            }

            if (ammoMode == Ammo.Bullets)
            {
                if (firstMode != fireMode.none && firstMode != fireMode.launcher || secondMode != fireMode.none && secondMode != fireMode.launcher)
                {
                    GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Bullets : ");
                    GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + bulletsLeft, style1);
                    GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 200, 80), " |  " + magazines);
                }
            }

            if (firstMode != fireMode.none || secondMode != fireMode.none)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 65, 200, 80), "Firing Mode :");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 65, 200, 80), "" + currentMode, style1);
            }

            if (secondMode == fireMode.launcher || firstMode == fireMode.launcher)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 95, 200, 80), "Projectiles : ");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 95, 200, 80), "" + projectiles, style1);
            }

            if (crosshairFirstModeHorizontal != null)
            {
                if (currentMode != fireMode.launcher)
                {
                    float w = crosshairFirstModeHorizontal.width;
                    float h = crosshairFirstModeHorizontal.height;
                    Rect position1 = new Rect((Screen.width + w) / 2 + (triggerTime * adjustMaxCroshairSize), (Screen.height - h) / 2, w, h);
                    Rect position2 = new Rect((Screen.width - w) / 2, (Screen.height + h) / 2 + (triggerTime * adjustMaxCroshairSize), w, h);
                    Rect position3 = new Rect((Screen.width - w) / 2 - (triggerTime * adjustMaxCroshairSize) - w, (Screen.height - h) / 2, w, h);
                    Rect position4 = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2 - (triggerTime * adjustMaxCroshairSize) - h, w, h);
                    if (!aiming)
                    {
                        GUI.DrawTexture(position1, crosshairFirstModeHorizontal); 	//Right
                        GUI.DrawTexture(position2, crosshairFirstModeVertical); 	//Up
                        GUI.DrawTexture(position3, crosshairFirstModeHorizontal); 	//Left
                        GUI.DrawTexture(position4, crosshairFirstModeVertical);		//Down
                    }
                }
            }

            if (crosshairSecondMode != null)
            {
                if (currentMode == fireMode.launcher)
                {
                    float width = crosshairSecondMode.width / 2;
                    float height = crosshairSecondMode.height / 2;
                    Rect position = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
                    if (!aiming)
                    {
                        GUI.DrawTexture(position, crosshairSecondMode);
                    }
                }
            }
        }
    }

    IEnumerator FirstFireMode()
    {

        canSwicthMode = false;
        selected = false;
        weaponAnim.GetComponent<Animation>().Rewind("SwitchAnim");
        weaponAnim.GetComponent<Animation>().Play("SwitchAnim");
        GetComponent<AudioSource>().clip = switchModeSound;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.6f);
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        selected = true;
        canSwicthMode = true;
    }

    IEnumerator SecondFireMode()
    {

        canSwicthMode = false;
        selected = false;
        GetComponent<AudioSource>().clip = switchModeSound;
        GetComponent<AudioSource>().Play();
        weaponAnim.GetComponent<Animation>().Play("SwitchAnim");
        yield return new WaitForSeconds(0.6f);
        currentMode = secondMode;
        fireRate = fireRateSecondMode;
        selected = true;
        canSwicthMode = true;

    }

    void fireSemi()
    {
        if (reloading || bulletsLeft <= 0)
        {
            if (bulletsLeft == 0)
            {
                OutOfAmmo();
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            fireOneBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    void fireLauncher()
    {
        if (reloading || projectiles <= 0)
        {
            if (projectiles == 0)
            {
                OutOfAmmo();
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            fireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator fireBurst()
    {
        int shotCounter = 0;

        if (reloading || bursting || bulletsLeft <= 0)
        {
            if (bulletsLeft <= 0)
            {
                StartCoroutine(OutOfAmmo());
            }
            yield break;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (shotCounter < shotsPerBurst)
            {
                bursting = true;
                shotCounter++;
                if (bulletsLeft > 0)
                {
                    fireOneBullet();
                }
                yield return new WaitForSeconds(burstTime);
            }
            nextFireTime = Time.time + fireRate;
        }
        bursting = false;
    }
    public void fireOneBullet()
    {
        if (nextFireTime > Time.time || draw)
        {
            if (bulletsLeft <= 0)
            {
                OutOfAmmo();
            }
            return;
        }

        Vector3 direction = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
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

        PlayAudioClip(soundFire, transform.position, 0.7f);
        m_LastFrameShot = Time.frameCount;

        weaponAnim.GetComponent<Animation>().Rewind("Fire");
        weaponAnim.GetComponent<Animation>().Play("Fire");
        KickBack();
        bulletsLeft--;
    }

    void PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(0.95f, 1.05f);
        source.Play();
        Destroy(go, clip.length);
    }

    void fireProjectile()
    {
        if (projectiles < 1 || draw || launcjerReload)
        {
            return;
        }

        Rigidbody instantiatedProjectile = Instantiate(projectilePrefab, launchPosition.transform.position, launchPosition.transform.rotation) as Rigidbody;
        fireLauncherAnimGO.GetComponent<Animation>().Play("FireGL");
        instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, projectileSpeed));
        Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), GameObject.FindWithTag("Player").transform.root.GetComponent<Collider>());
        projectiles--;

        if (canReloadLauncher)
            StartCoroutine(ReloadLauncher());
    }

   
     IEnumerator OutOfAmmo()
    {
        if (reloading || playing)
            yield break;

        playing = true;
        PlayAudioClip(soundEmpty, transform.position, 0.7f);

        weaponAnim.GetComponent<Animation>()["Fire"].speed = 2.0f;
        weaponAnim.GetComponent<Animation>().Rewind("Fire");
        weaponAnim.GetComponent<Animation>().Play("Fire");
        yield return new WaitForSeconds(0.2f);
        playing = false;
    }
    //For RPG
    IEnumerator ReloadLauncher()
    {
        if (projectiles > 0)
        {
            launcjerReload = true;
            canSwicthMode = false;
            StartCoroutine(DisableProjectileRenderer());
            yield return new WaitForSeconds(0.5f);
            GetComponent<AudioSource>().PlayOneShot(soundReloadLauncher);
            weaponAnim.GetComponent<Animation>()[reloadlauncher].speed = 1.2f;
            weaponAnim.GetComponent<Animation>().Play(reloadlauncher);
            yield return new WaitForSeconds(launcherReloadTime);
            canSwicthMode = true;
            launcjerReload = false;
        }
        else
        {
            if (rocket != null && projectiles == 0)
            {
                rocket.GetComponent<Renderer>().enabled = false;
            }
        }

    }

    IEnumerator DisableProjectileRenderer()
    {
        if (rocket != null)
        {
            rocket.GetComponent<Renderer>().enabled = false;
        }
        yield return new WaitForSeconds(launcherReloadTime / 1.5f);
        if (rocket != null)
        {
            rocket.GetComponent<Renderer>().enabled = true;
        }
    }

    void EnableProjectileRenderer()
    {
        if (rocket != null)
        {
            rocket.GetComponent<Renderer>().enabled = true;
        }
    }

    IEnumerator Reload()
    {
        if (reloading)
            yield break;

        if (ammoMode == Ammo.Magazines)
        {
            reloading = true;
            canSwicthMode = false;
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                weaponAnim.GetComponent<Animation>()["Reload"].speed = reloadAnimSpeed;
                weaponAnim.GetComponent<Animation>().Play("Reload", PlayMode.StopAll);
                weaponAnim.GetComponent<Animation>().CrossFade("Reload");
                GetComponent<AudioSource>().PlayOneShot(soundReload);
                yield return new WaitForSeconds(reloadTime);
                magazines--;
                bulletsLeft = bulletsPerMag;
            }
            reloading = false;
            canSwicthMode = true;
            isFiring = false;
        }

        if (ammoMode == Ammo.Bullets)
        {
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                if (magazines > bulletsPerMag)
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim.GetComponent<Animation>()["Reload"].speed = reloadAnimSpeed;
                    weaponAnim.GetComponent<Animation>().Play("Reload", PlayMode.StopAll);
                    weaponAnim.GetComponent<Animation>().CrossFade("Reload");
                    GetComponent<AudioSource>().PlayOneShot(soundReload);
                    yield return new WaitForSeconds(reloadTime);
                    magazines -= bulletsPerMag - bulletsLeft;
                    bulletsLeft = bulletsPerMag;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
                else
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim.GetComponent<Animation>()["Reload"].speed = reloadAnimSpeed;
                    weaponAnim.GetComponent<Animation>().Play("Reload", PlayMode.StopAll);
                    weaponAnim.GetComponent<Animation>().CrossFade("Reload");
                    GetComponent<AudioSource>().PlayOneShot(soundReload);
                    yield return new WaitForSeconds(reloadTime);
                    int bullet = Mathf.Clamp(bulletsPerMag, magazines, bulletsLeft + magazines);
                    magazines -= (bullet - bulletsLeft);
                    bulletsLeft = bullet;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
            }
        }
    }

    void KickBack()
    {
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUpside, Random.Range(-kickSideways, kickSideways), 0));
    }

    IEnumerator DrawWeapon()
    {
        draw = true;
        canSwicthMode = false;
        GetComponent<AudioSource>().clip = soundDraw;
        GetComponent<AudioSource>().Play();
        weaponAnim.GetComponent<Animation>().Play("Draw", PlayMode.StopAll);
        weaponAnim.GetComponent<Animation>().CrossFade("Draw");
        yield return new WaitForSeconds(drawTime);
        draw = false;
        reloading = false;
        canSwicthMode = true;
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