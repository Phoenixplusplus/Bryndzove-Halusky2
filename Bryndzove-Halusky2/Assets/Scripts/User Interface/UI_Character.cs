using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : MonoBehaviour {

    public GameManager gameManager;
    public C_Character localCharacter;
    private W_Weapon equippedWeapon;

    // UI children
    Slider ammoSlider;
    Text roundTime;

    bool runUpdate = false;

    void OnEnable()
    {
        EventManager.PlayerSpawned += Initialise;
    }

    void OnDisable()
    {
        EventManager.PlayerSpawned -= Initialise;
    }

	public void Initialise()
    {
        Debug.Log("I heard from EventManager player has finished startup. Initialising..");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        equippedWeapon = localCharacter.leftWeapon;

        // enable & set children
        transform.GetChild(0).gameObject.SetActive(true);
        ammoSlider = transform.GetChild(0).GetComponent<Slider>();
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        roundTime = transform.GetChild(2).GetComponent<Text>();

        // set their attributes
        ammoSlider.maxValue = equippedWeapon.clipSize;
        ammoSlider.wholeNumbers = true;

        runUpdate = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (runUpdate)
        {
            ammoSlider.value = equippedWeapon.ammoCount;
            roundTime.text = "zostávajúci čas: " + gameManager.roundTime.ToString("0.0");
        }
    }
}
