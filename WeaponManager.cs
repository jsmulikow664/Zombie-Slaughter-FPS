using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    /**
    *  Script written by OMA [www.armedunity.com]
    **/
    public GameObject[] weaponsInUse;					// used weapons, among which you can switch.
    public GameObject[] weaponsInGame;					// all weapons, which could be used in game 
    public Rigidbody[] worldModels; 						// just a prefab which could be instantiated when you drop weapon

    public RaycastHit hit;
    public float distance = 2.0f;
    public LayerMask layerMaskWeapon;
    public LayerMask layerMaskAmmo;

    public Transform dropPosition;

    public float switchWeaponTime = 0.5f;
    [HideInInspector]
    public bool canSwitch = true;
    [HideInInspector]
    public bool showWepGui = false;
    [HideInInspector]
    public bool showAmmoGui = false;
    private bool equipped = false;
    //[HideInInspector]
    //int i = 0;
    //[HideInInspector]
    public int weaponToSelect;
    //[HideInInspector]
    public int setElement;
    //[HideInInspector]
    public int weaponToDrop;
    public GUISkin mySkin;
    public AudioClip pickupSound;
    private string textFromPickupScript = "";
    private string notes = "";
    private string note = "Press key <E> to pick up Ammo";
    private string note2 = "Select appropriate weapon to pick up ammo";

    void Start()
    {
        for (int h = 0; h < worldModels.Length; h++)
        {
            weaponsInGame[h].gameObject.SetActive(false);
        }
        weaponToSelect = 0;
        DeselectWeapon();
    }
    void Update()
    {

        if (Input.GetKeyDown("1") && weaponsInUse.Length >= 1 && canSwitch && weaponToSelect != 0)
        {
            DeselectWeapon();
            weaponToSelect = 0;

        }
        else if (Input.GetKeyDown("2") && weaponsInUse.Length >= 2 && canSwitch && weaponToSelect != 1)
        {
            DeselectWeapon();
            weaponToSelect = 1;

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canSwitch)
        {
            weaponToSelect++;
            if (weaponToSelect > (weaponsInUse.Length - 1))
            {
                weaponToSelect = 0;
            }
            DeselectWeapon();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && canSwitch)
        {
            weaponToSelect--;
            if (weaponToSelect < 0)
            {
                weaponToSelect = weaponsInUse.Length - 1;
            }
            DeselectWeapon();
        }

        Vector3 position = transform.parent.position;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(position, direction, out hit, distance, layerMaskWeapon.value))
        {

            WeaponIndex prefab = hit.transform.GetComponent<WeaponIndex>();
            setElement = prefab.setWeapon;
            showWepGui = true;
            //if you want more than 2 weapons equip at the same time		
            if (weaponsInUse[0] != weaponsInGame[setElement] && weaponsInUse[1] != weaponsInGame[setElement])
            { //&& weaponsInUse[2] != weaponsInGame[setElement] && weaponsInUse[3] != weaponsInGame[setElement]){
                equipped = false;
            }
            else
            {
                equipped = true;
            }

            if (canSwitch)
            {
                if (!equipped && Input.GetKeyDown("e"))
                {
                    DropWeapon(weaponToDrop);
                    DeselectWeapon();
                    weaponsInUse[weaponToSelect] = weaponsInGame[setElement];
                    if (setElement == 8)
                    {
                        Pickup pickupGOW1 = hit.transform.GetComponent<Pickup>();
                        addStickGrenades(pickupGOW1.amount);
                    }
                    Destroy(hit.collider.transform.parent.gameObject);

                }
                else
                {

                    if (setElement == 8)
                    {
                        if (Input.GetKeyDown("e"))
                        {
                            Pickup pickupGOW = hit.transform.GetComponent<Pickup>();
                            addStickGrenades(pickupGOW.amount);
                            Destroy(hit.collider.transform.parent.gameObject);
                        }
                    }
                }
            }

        }
        else
        {
            showWepGui = false;
        }

        if (Physics.Raycast(position, direction, out hit, distance, layerMaskAmmo.value))
        {
            showAmmoGui = true;

            if (hit.transform.CompareTag("Ammo"))
            {
                Pickup pickupGO = hit.transform.GetComponent<Pickup>();

                //ammo for pistols, rifles 
                if (pickupGO.pickupType == PickupType.Magazines)
                {
                    WeaponScriptNEW mags = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<WeaponScriptNEW>();
                    if (mags != null && mags.firstMode != fireMode.launcher)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            if (mags.ammoMode == Ammo.Magazines)
                            {
                                mags.magazines += pickupGO.amount;
                            }
                            else
                            {
                                mags.magazines += pickupGO.amount * mags.bulletsPerMag;
                            }
                            GetComponent<AudioSource>().clip = pickupSound;
                            GetComponent<AudioSource>().Play();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = note2;
                    }
                }
                //ammo for Sniper rifle
                if (pickupGO.pickupType == PickupType.SniperMagazines)
                {
                    SniperScript magsSniper = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<SniperScript>();
                    if (magsSniper != null)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            magsSniper.magazines += pickupGO.amount;
                            GetComponent<AudioSource>().clip = pickupSound;
                            GetComponent<AudioSource>().Play();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = note2;
                    }
                }
                //ammo for weapon if second fireMode is luancher
                if (pickupGO.pickupType == PickupType.Projectiles)
                {
                    WeaponScriptNEW projectile = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<WeaponScriptNEW>();
                    if (projectile != null && projectile.secondMode == fireMode.launcher)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            projectile.projectiles += pickupGO.amount;
                            GetComponent<AudioSource>().clip = pickupSound;
                            GetComponent<AudioSource>().Play();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = note2;
                    }
                }
                //ammo for rocket launcher
                if (pickupGO.pickupType == PickupType.Rockets)
                {
                    WeaponScriptNEW rockets = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<WeaponScriptNEW>();
                    if (rockets != null && rockets.firstMode == fireMode.launcher)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            rockets.projectiles += pickupGO.amount;
                            //rockets.EnableProjectileRenderer();
                            GetComponent<AudioSource>().clip = pickupSound;
                            GetComponent<AudioSource>().Play();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = note2;
                    }
                }
                //ammo for shotgun
                if (pickupGO.pickupType == PickupType.Shells)
                {
                    ShotGunScriptNEW bullets = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<ShotGunScriptNEW>();
                    if (bullets != null)
                    {
                        notes = "";
                        textFromPickupScript = note;

                        if (Input.GetKeyDown("e"))
                        {
                            bullets.magazines += pickupGO.amount;
                            GetComponent<AudioSource>().clip = pickupSound;
                            GetComponent<AudioSource>().Play();
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = note2;
                    }
                }
                //pickup health
                if (pickupGO.pickupType == PickupType.Health)
                {
                    textFromPickupScript = pickupGO.AmmoInfo;
                    notes = "";
                    if (Input.GetKeyDown("e"))
                    {
                        GameObject playerGO = GameObject.Find("Player");
                        PlayerDamageNew hp = playerGO.gameObject.transform.GetComponent<PlayerDamageNew>();
                        hp.hitPoints += pickupGO.amount;
                        GetComponent<AudioSource>().clip = pickupSound;
                        GetComponent<AudioSource>().Play();
                        Destroy(hit.collider.gameObject);
                    }
                }
            }

        }
        else
        {
            showAmmoGui = false;
        }
    }

	public void FillAmmo()
	{
	foreach (GameObject gun in weaponsInUse)
	{
	WeaponScriptNEW mags = weaponsInUse[weaponToSelect].gameObject.transform.GetComponent<WeaponScriptNEW>();
	if (mags != null)
	{
	mags.magazines += 5;
	return;
	}
	}
	}
	

    void addStickGrenades(int amount)
    {
        GrenadeScript stickGrenade = weaponsInGame[8].gameObject.transform.GetComponent<GrenadeScript>();
        stickGrenade.grenadeCount += amount;
        stickGrenade.DrawWeapon();
    }

    void OnGUI()
    {
        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];
        if (showWepGui)
        {
            if (!equipped)
            {
                GUI.Label(new Rect(Screen.width - (Screen.width / 1.7f), Screen.height - (Screen.height / 1.4f), 800, 100), "Press key << E >> to pickup weapon", style1);
            }
            else
            {
                GUI.Label(new Rect(Screen.width - (Screen.width / 1.7f), Screen.height - (Screen.height / 1.4f), 800, 100), "Weapon is already equipped");
            }
        }

        if (showAmmoGui)
        {
            GUI.Label(new Rect(Screen.width - (Screen.width / 1.7f), Screen.height - (Screen.height / 1.4f), 800, 200), notes + "\n" + textFromPickupScript, style1);
        }
    }

    void DeselectWeapon()
    {
        //Dectivate all weapon
        for (int i = 0; i < weaponsInUse.Length; i++)
        {
            weaponsInUse[i].gameObject.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            /*Component[] deactivate = weaponsInUse[i].gameObject.GetComponentsInChildren<MonoBehaviour>();
            foreach (var d in deactivate)
            {
                MonoBehaviour d = d as MonoBehaviour;
                if (d)
                    d.enabled = false;
            }*/
            weaponsInUse[i].gameObject.SetActive(false);
        }
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        canSwitch = false;
        yield return new WaitForSeconds(switchWeaponTime);
        SelectWeapon(weaponToSelect);
        yield return new WaitForSeconds(switchWeaponTime);
        canSwitch = true;
    }

    void SelectWeapon(int i)
    {
        //Activate selected weapon
        weaponsInUse[i].gameObject.SetActive(true);
        /*Component[] activate = weaponsInUse[i].gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (var a in activate)
        {
            MonoBehaviour a = a as MonoBehaviour;
            if (a)
                a.enabled = true;
        }*/
        weaponsInUse[i].gameObject.SendMessage("DrawWeapon", SendMessageOptions.DontRequireReceiver);
        WeaponIndex temp = weaponsInUse[i].gameObject.transform.GetComponent<WeaponIndex>();
        weaponToDrop = temp.setWeapon;
    }

    void DropWeapon(int index)
    {

        for (int i = 0; i < worldModels.Length; i++)
        {
            if (i == index)
            {
                Rigidbody drop = Instantiate(worldModels[i], dropPosition.transform.position, dropPosition.transform.rotation) as Rigidbody;
                drop.AddRelativeForce(0, 50, Random.Range(100, 200));
            }
        }
    }


}