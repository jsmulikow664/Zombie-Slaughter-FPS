using UnityEngine;
using System.Collections;

public class GranadeLauncer : MonoBehaviour
{
    public GUIText grenadeGUI;
    public Rigidbody projectile;
    public float initialSpeed = 30.0f;
    public float reloadTime = 0.5f;
    public float ammoCount = 20;
    private float lastShot = -10.0f;
    public GameObject launchPosition;
    public GameObject animGL;
    public AudioClip soundFire;

    void Awake()
    {
        Gui();
    }

    void Update()
    {
        if (Input.GetButtonDown("Drop"))
        {
            BroadcastMessage("DropWeapon");
        }

        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }

    void Fire()
    {
        // Did the time exceed the reload time?
        if (Time.time > reloadTime + lastShot && ammoCount > 0)
        {
            // create a new projectile, use the same position and rotation as the Launcher.
            Rigidbody instantiatedProjectile = Instantiate(projectile, launchPosition.transform.position, launchPosition.transform.rotation) as Rigidbody;
            animGL.GetComponent<Animation>().Play("FireGL");
            GetComponent<AudioSource>().clip = soundFire;
            GetComponent<AudioSource>().Play();
            // Give it an initial forward velocity. The direction is along the z-axis of the missile launcher's transform.
            instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, initialSpeed));

            // Ignore collisions between the missile and the character controller
            Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), transform.root.GetComponent<Collider>());

            lastShot = Time.time;
            ammoCount--;
            Gui();
        }
    }
    void Gui()
    {
        grenadeGUI.text = "Grenade:  " + ammoCount.ToString();
    }
}