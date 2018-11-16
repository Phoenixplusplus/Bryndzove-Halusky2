using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Customise : MonoBehaviour {

    GameManager gameManager;
    DatabaseManager Database;
    public GameObject sampleDude;

    public GameObject Background;
    public GameObject RoomsSelection;
    public GameObject MainButtonsSelection;
    public GameObject CustomiseMenu;

    public int currentHeadTex;
    public int currentBodyTex;
    public int currentWeapon;

	// Use this for initialization
	void Start ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Database = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    // event subscription
    void OnEnable()
    {
        DatabaseManager.LoadDataReady += SetupDude;
    }

    void OnDisable()
    {
        DatabaseManager.LoadDataReady -= SetupDude;
    }

    public void EnableCustomiseScreen()
    {
        Database.LoadPlayerData(gameManager.username, gameManager.userpass);

        Background.SetActive(false);
        RoomsSelection.SetActive(false);
        MainButtonsSelection.SetActive(false);
        CustomiseMenu.SetActive(true);

        currentHeadTex = int.Parse(gameManager.headtex.Substring(gameManager.headtex.Length - 1, 1));
        currentBodyTex = int.Parse(gameManager.bodytex.Substring(gameManager.bodytex.Length - 1, 1));
        currentWeapon = int.Parse(gameManager.weapon.Substring(gameManager.weapon.Length - 1, 1));
    }

    public void DisableCustomiseScreen()
    {
        // save values to gameManager and use them for database query
        gameManager.headtex = "Head" + currentHeadTex;
        gameManager.bodytex = "Body" + currentBodyTex;
        gameManager.weapon = "Weapon" + currentWeapon;
        Database.SavePlayerData(gameManager.username, gameManager.userpass, gameManager.headtex, gameManager.bodytex, gameManager.weapon);

        Background.SetActive(true);
        RoomsSelection.SetActive(true);
        MainButtonsSelection.SetActive(true);
        CustomiseMenu.SetActive(false);

        Destroy(sampleDude.transform.Find("CharacterBody/CharacterLArm/LGunSlot").transform.GetChild(0).gameObject);
    }

    public void SetupDude()
    {
        sampleDude.transform.Find("CharacterBody").GetComponent<Renderer>().material = gameManager.characterTexDict[gameManager.bodytex];
        sampleDude.transform.Find("CharacterBody/CharacterHead").GetComponent<Renderer>().material = gameManager.characterTexDict[gameManager.headtex];
        sampleDude.transform.Find("CharacterBody/CharacterLArm").GetComponent<Renderer>().material = gameManager.characterTexDict[gameManager.headtex];
        sampleDude.transform.Find("CharacterBody/CharacterRArm").GetComponent<Renderer>().material = gameManager.characterTexDict[gameManager.headtex];

        // spawn weapon
        GameObject localL_Gun;
        Transform L_gunSlot = sampleDude.transform.Find("CharacterBody/CharacterLArm/LGunSlot");

        localL_Gun = Instantiate(gameManager.characterWeaponDict[gameManager.weapon], L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), L_gunSlot.transform.rotation);
        localL_Gun.GetComponent<PhotonView>().enabled = false;
        localL_Gun.GetComponent<NetworkMovement2>().enabled = false;
        localL_Gun.transform.parent = L_gunSlot;
    }

    // button functions
    public void HeadUp()
    {
        if (currentHeadTex < 9) currentHeadTex++;
        else currentHeadTex = 1;
        sampleDude.transform.Find("CharacterBody/CharacterHead").GetComponent<Renderer>().material = gameManager.characterTexDict["Head" + currentHeadTex];
        sampleDude.transform.Find("CharacterBody/CharacterLArm").GetComponent<Renderer>().material = gameManager.characterTexDict["Head" + currentHeadTex];
        sampleDude.transform.Find("CharacterBody/CharacterRArm").GetComponent<Renderer>().material = gameManager.characterTexDict["Head" + currentHeadTex];
    }

    public void BodyUp()
    {
        if (currentBodyTex < 9) currentBodyTex++;
        else currentBodyTex = 1;
        sampleDude.transform.Find("CharacterBody").GetComponent<Renderer>().material = gameManager.characterTexDict["Body" + currentBodyTex];
    }

    public void WeaponUp()
    {
        Destroy(sampleDude.transform.Find("CharacterBody/CharacterLArm/LGunSlot").transform.GetChild(0).gameObject);

        if (currentWeapon < 3) currentWeapon++;
        else currentWeapon = 1;

        GameObject localL_Gun;
        Transform L_gunSlot = sampleDude.transform.Find("CharacterBody/CharacterLArm/LGunSlot");

        localL_Gun = Instantiate(gameManager.characterWeaponDict["Weapon" + currentWeapon], L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), L_gunSlot.transform.rotation);
        localL_Gun.GetComponent<PhotonView>().enabled = false;
        localL_Gun.GetComponent<NetworkMovement2>().enabled = false;
        localL_Gun.transform.parent = L_gunSlot;
    }
}
