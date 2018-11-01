using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Character : MonoBehaviour {

    public C_Character localCharacter;
    private W_Weapon equippedWeapon;

	// Called from NetworkManager when it creates the local character
	public void Initialise()
    {
        equippedWeapon = localCharacter.leftWeapon;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
