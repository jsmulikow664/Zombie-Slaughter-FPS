using UnityEngine;
using System.Collections;

public class SniperScript : MonoBehaviour
{
    /** 
    *  Script written by OMA [www.armedunity.com]
    **/
    public enum FireMode { semi, auto }
    public FireMode Mode = FireMode.semi;

    public AudioClip soundFire;
    public AudioClip soundReload;
    public AudioClip soundEmpty;
    public AudioClip soundDraw;
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
    public float reloadTime = 2;
    public float drawTime = 1.0f;
    private float bulletsLeft = 0;
    public float fireRate = 0.1f;
    public float range = 1000;
    public float force = 500;

    //Weapon accuracy
    private float baseInaccuracy;
    public float inaccuracyIncreaseOverTime = 0.01f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    private float maximumInaccuracy;
    private float triggerTime = 0.05f;
    public float baseInaccuracyAIM = 0.005f;
    public float baseInaccuracyHIP = 1.5f;
    public float maxInaccuracyHIP = 5.0f;
    public float maxInaccuracyAIM = 1.0f;

    //Aiming
    public Vector3 hipPosition;
    public Vector3 aimPosition;
    private bool aiming;
    private Vector3 curVect;
    public float aimSpeed = 0.25f;
    public float scopeTime;
    private bool inScope = false;
    public Texture scopeTexture;

    //Field Of View
    public float zoomSpeed = 0.5f;
    public int FOV = 40;

    //KickBack
    public Transform kickGO;
    public float kickUpside = 0.5f;
    public float kickSideways = 0.5f;

    //GUI
    public GUISkin mySkin;

    private int m_LastFrameShot = -1;
    private float nextFireTime = 0.0f;
    private bool reloading;
    private bool draw;
    public GameObject weaponCamera;
    public GameObject mainCamera;
    private GameObject player;
    [HideInInspector]
    public bool selected = false;
    private bool isFiring = false;
    private bool playing = false;

    //Crosshair Textures
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;
    private float adjustMaxCroshairSize = 6.0f;

    void Start()
    {
        weaponCamera = GameObject.FindWithTag("WeaponCamera");
        mainCamera = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
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
                if (Mode == FireMode.semi)
                {
                    fireSniper();
                }

                if (bulletsLeft > 0)
                    isFiring = true;
            }

            if (Input.GetButton("Fire"))
            {
                if (Mode == FireMode.auto)
                {
                    fireSniper();
                    if (bulletsLeft > 0)
                        isFiring = true;
                }
            }

            if (Input.GetButtonDown("Reload"))
            {
                StartCoroutine(Reload());
            }

            if (Input.GetButton("Fire2") && !Input.GetButton("Run") && !reloading)
            {
                if (!aiming)
                {
                    aiming = true;
                    curVect = aimPosition - transform.localPosition;
                    scopeTime = Time.time + aimSpeed;
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

                if (Time.time >= scopeTime && !inScope)
                {
                    inScope = true;
                    Renderer[] gos = GetComponentsInChildren<Renderer>();
                    foreach (Renderer go in gos)
                    {
                        go.GetComponent<Renderer>().enabled = false;
                    }
                }

            }
            else
            {
                if (aiming)
                {
                    aiming = false;
                    inScope = false;
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

            if (inScope)
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
                mainCamera.GetComponent<Camera>().fieldOfView += 60 * Time.deltaTime / 0.2f;
                if (mainCamera.GetComponent<Camera>().fieldOfView > 60)
                {
                    mainCamera.GetComponent<Camera>().fieldOfView = 60;
                }
                weaponCamera.GetComponent<Camera>().fieldOfView += 50 * Time.deltaTime / 0.2f;
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
        }
    }

    void LateUpdate()
    {
        if (m_LastFrameShot == Time.frameCount && !inScope)
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

            if (selected)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Bullets : ");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + bulletsLeft, style1);
                GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 200, 80), " / " + magazines);
            }

            if (scopeTexture != null && inScope)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scopeTexture, ScaleMode.StretchToFill);
            }
            else
            {
                if (crosshairFirstModeHorizontal != null)
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
        }
    }
    void fireSniper()
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

    void fireOneBullet(){
    if (nextFireTime > Time.time || draw){
		if(bulletsLeft <= 0){
			OutOfAmmo();
		}	
		return; 
	}
	
    Vector3 direction= mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.05f, 0.05f) * triggerTime/3, Random.Range(-0.05f, 0.05f) * triggerTime/3,1));
	RaycastHit hit;
	Vector3 position= transform.parent.position;

	if (Physics.Raycast (position, direction, out hit, range, layerMask.value)) {
	
	    Vector3 contact= hit.point;
		Quaternion rotation= Quaternion.FromToRotation(Vector3.up, hit.normal);
		
		if (hit.rigidbody){
			hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
		}

		if (hit.transform.tag == "Untagged") {
			GameObject default1= Instantiate (untagged, contact, rotation) as GameObject;
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
			default1.transform.parent = hit.transform;
		}
		
		if (hit.transform.tag == "Concrete") {
            GameObject bulletHole= Instantiate (Concrete, contact, rotation) as GameObject;
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
			bulletHole.transform.parent = hit.transform;
		}	
		
		if (hit.transform.tag == "Wood") {
            GameObject woodHole= Instantiate (Wood, contact, rotation) as GameObject;
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
			woodHole.transform.parent = hit.transform;
		}
		
		if (hit.transform.tag == "Metal") {
            GameObject metalHole= Instantiate (Metal, contact, rotation) as GameObject;
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
			metalHole.transform.parent = hit.transform;
		}
		
		
		if (hit.transform.tag == "Dirt") {
            GameObject dirtHole= Instantiate (Dirt, contact, rotation) as GameObject;
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
			dirtHole.transform.parent = hit.transform;
		}
		
		if (hit.transform.tag == "canBeUsed") {
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
		}	
		
		if (hit.transform.tag == "Enemy") {
			hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            Instantiate(Blood, contact, rotation);
			if(Physics.Raycast (position, direction, out hit, range, layerMask.value)){
				if(hit.rigidbody){
					hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
				}
			}	
		}
	}
	
	PlayAudioClip(soundFire, transform.position, 0.7f);
	m_LastFrameShot = Time.frameCount;

	weaponAnim.GetComponent<Animation>().Rewind("sniperFire");
	weaponAnim.GetComponent<Animation>().Play("sniperFire");
	bulletsLeft--;
	KickBack();
}

    IEnumerator OutOfAmmo()
    {
        if (reloading || playing)
            yield break;

        playing = true;
        yield return new WaitForSeconds(0.2f);
        PlayAudioClip(soundEmpty, transform.position, 0.7f);
        yield return new WaitForSeconds(0.2f);
        playing = false;
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

    IEnumerator Reload()
    {
        if (reloading)
            yield break;

        reloading = true;
        if (magazines > 0 && bulletsLeft < bulletsPerMag)
        {
            weaponAnim.GetComponent<Animation>()["Reload"].speed = 1.0f;
            weaponAnim.GetComponent<Animation>().Play("Reload", PlayMode.StopAll);
            weaponAnim.GetComponent<Animation>().CrossFade("Reload");
            GetComponent<AudioSource>().PlayOneShot(soundReload);
            yield return new WaitForSeconds(reloadTime);
            magazines--;
            bulletsLeft = bulletsPerMag;
        }
        reloading = false;
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

    void KickBack()
    {
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUpside, Random.Range(-kickSideways, kickSideways), 0));
    }

    void Deselect()
    {
        selected = false;
        mainCamera.GetComponent<Camera>().fieldOfView = 60;
        weaponCamera.GetComponent<Camera>().fieldOfView = 50;
        inScope = false;
        transform.localPosition = hipPosition;
        Renderer[] go = GetComponentsInChildren<Renderer>();
        foreach (Renderer g in go)
        {
            if (g.name != "muzzle_flash")
                g.GetComponent<Renderer>().enabled = true;
        }
    }



}