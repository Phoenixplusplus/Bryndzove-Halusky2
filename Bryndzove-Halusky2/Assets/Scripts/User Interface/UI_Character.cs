using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : MonoBehaviour {

    // character
    public C_Character localCharacter;
    private W_Weapon equippedWeapon;

    // UI children
    Slider ammoSlider;

    void OnEnable()
    {
        EventManager.PlayerSpawned += Initialise;
    }

    void OnDisable()
    {
        EventManager.PlayerSpawned -= Initialise;
    }

	// Called from NetworkManager when it creates the local character
	public void Initialise()
    {
        Debug.Log("I heard from EventManager player has finished startup. Initialising..");
        equippedWeapon = localCharacter.leftWeapon;

        // enable & set children
        transform.GetChild(0).gameObject.SetActive(true);
        ammoSlider = transform.GetChild(0).GetComponent<Slider>();

        // set their attributes
        ammoSlider.maxValue = equippedWeapon.clipSize;
        ammoSlider.wholeNumbers = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // ammo slider
    public int GetAmmo() { return equippedWeapon.ammoCount; }
}
