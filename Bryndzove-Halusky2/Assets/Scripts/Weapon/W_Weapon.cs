using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Weapon : Photon.MonoBehaviour {

    public C_Character Character;
    public GameObject Paintball;

    [Header("Attributes")]
    public string Owner;
    public int clipSize;
    public int ammoCount;
    public float reloadDelay;
    public float shotDelay;
    public AudioClip shotSound;
    public float shotSpeed;
    public Transform Muzzle;
    public Vector3 paintballColour;

    private float shotTime = 0f;
    private float reloadTime = 0f;
    private bool isFiring = false;
    private bool isReloading = false;

    // main fire function called by the parent of the weapon
    // virtual to be overriden for custom functionality in subclesses if needed
    public bool Fire()
    {
        if (ammoCount <= 0 || isFiring == true || isReloading == true) return false;

        ammoCount = ammoCount - 1;
        AudioSource.PlayClipAtPoint(shotSound, transform.position);
        StartCoroutine(RunShotDelay());
        Debug.Log("Shooting.. Delay of " + shotDelay + " seconds - Paintball colour is " + Paintball.GetComponent<Renderer>().sharedMaterial);
        CreatePaintball();
        return true;
    }

    // reload function called by the parent of the weapon
    // virtual to be overriden for custom functionality in subclesses if needed
    public bool Reload()
    {
        if (ammoCount == clipSize) return false;

        Debug.Log("Reloading.. Delay of " + reloadDelay + " seconds");
        StartCoroutine(RunReloadDelay());
        return true;
    }

    // overridden in subclasses
    public virtual void CreatePaintball() { }

    // weapon delay coroutines
    IEnumerator RunShotDelay()
    {
        while (shotTime < shotDelay)
        {
            isFiring = true;
            shotTime += Time.deltaTime;
            yield return null;
        }

        shotTime = 0f;
        isFiring = false;
        yield break;      
    }

    IEnumerator RunReloadDelay()
    {
        while (reloadTime < reloadDelay)
        {
            isReloading = true;
            reloadTime += Time.deltaTime;
            yield return null;
        }

        reloadTime = 0f;
        isReloading = false;
        ammoCount = clipSize;
        Debug.Log("Ammo now " + ammoCount);
        yield break;
    }

    // RPC send on character Start() to determine the owner of this weapon so it can be childed properly over the network
    [PunRPC] public void SetOwner(string name) { Owner = name; }

}
