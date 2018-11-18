using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : MonoBehaviour {

    public GameManager gameManager;
    public C_Character localCharacter;
    private W_Weapon equippedWeapon;

    // UI children
    public Slider ammoSlider, healthSlider;
    public Text AmmoUpText, SpeedUpText, DeathText, roundTime;
    public Image InfiniteAmmoIMG, SpeedUpIMG, HealthRecoveredIMG, TimeleftIMG, TimeleftBackIMG, HealthAmmoIMG;

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
        ammoSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
        roundTime.gameObject.SetActive(true);
        TimeleftIMG.gameObject.SetActive(true);
        HealthAmmoIMG.gameObject.SetActive(true);
        TimeleftBackIMG.gameObject.SetActive(true);

        // set their attributes
        ammoSlider.maxValue = equippedWeapon.clipSize;
        ammoSlider.wholeNumbers = true;
        healthSlider.maxValue = localCharacter.maxHealth;

        runUpdate = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (runUpdate)
        {
            ammoSlider.value = equippedWeapon.ammoCount;
            healthSlider.value = localCharacter.Health;
            roundTime.text = gameManager.roundTime.ToString("0.0");
        }
    }
}
