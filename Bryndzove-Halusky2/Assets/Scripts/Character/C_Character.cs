using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Character : Photon.MonoBehaviour, ICanPickup {

    private GameManager gameManager;

    // events
    public delegate void PlayerStart();
    public static event PlayerStart PlayerReady;

    [Header("Network Attributes")]
    public string Team;
    public string killedBy;

    [Header("Attributes")]
    public float Health;
    public float maxHealth = 10f;
    public float healthRecoverySpeed = 0.1f;
    public string username;
    public string userpass;
    public string headtex;
    public string bodytex;
    public string weapon;
    public Material redMaterial;
    public Material blueMaterial;
    public bool isDead = false;
    public AudioClip deathSound;
    bool isDying = false;

    C_CameraMovement localCamera;

    // do not assign these values in editor
    public W_Weapon leftWeapon, rightWeapon;
    private bool autoFire;

    // Use this for initialization
    void Start ()
    {
        // on spawn from network manager
        if (photonView.isMine)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            // grab values needed from gamemanager
            username = gameManager.username;
            userpass = gameManager.userpass;
            headtex = gameManager.headtex;
            bodytex = gameManager.bodytex;
            weapon = gameManager.weapon;
            // apply values
            transform.Find("CharacterBody").GetComponent<Renderer>().material = gameManager.characterTexDict[bodytex];
            transform.Find("CharacterBody/CharacterHead").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            transform.Find("CharacterBody/CharacterLArm").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            transform.Find("CharacterBody/CharacterRArm").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            // (weapon values are assigned after team is chosen, as colour of weapon needs to change)

            if (PhotonNetwork.isMasterClient == true)
            {
                Debug.Log("I am master client, setting reference to GameManager");
                Debug.Log("Press V to see how many parts a team has painted");
            }

            // pick a team based on players in server send as buffed so others that join know
            int rand = Random.Range(0, 3);
            photonView.RPC("PickTeam", PhotonTargets.AllBuffered, rand);

            MoveToSpawnPoint();
            AttachWeapon(gameObject, weapon);
            photonView.RPC("SendPlayerInfo", PhotonTargets.OthersBuffered, new object[] { username, userpass, headtex, bodytex, weapon });

            Health = maxHealth;
            localCamera = transform.GetChild(1).GetComponent<C_CameraMovement>();

            // send event to listeners
            if (PlayerReady != null)
            {
                Debug.Log("Finished startup, Player is calling event to listeners");
                PlayerReady();
            }
        }
        else
        {

        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        // keyboard input
        if (photonView.isMine)
        {
            if (!isDead)
            {
                if (autoFire == true)
                {
                    if (Input.GetMouseButton(0))
                    {
                        leftWeapon.Fire();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        leftWeapon.Fire();
                    }
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    leftWeapon.Reload();
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    // debug to check team paint count
                    if (PhotonNetwork.isMasterClient)
                    {
                        Debug.Log("Red team has painted " + gameManager.redTeamPaintCount + " parts!");
                        Debug.Log("Blue team has painted " + gameManager.blueTeamPaintCount + " parts!");
                    }
                }
                if (Health <= 0)
                {
                    CallDeath(3f);
                }
            }
        }
    }
    
    [PunRPC] void PickTeam(int rand)
    {
        int redTeamCount = 0;
        int blueTeamCount = 0;

        // find all characters in the server currently as see which team they're on
        GameObject[] playerRefs = GameObject.FindGameObjectsWithTag("Character");

        for (int i = 0; i < playerRefs.Length; i++)
        {
            if (playerRefs[i].GetComponent<C_Character>().Team == "Red") redTeamCount++;
            if (playerRefs[i].GetComponent<C_Character>().Team == "Blue") blueTeamCount++;
        }

        // pick a team based on teamcount
        if (redTeamCount > blueTeamCount)
        {
            Team = "Blue";
            blueTeamCount++;
        }
        else if (blueTeamCount > redTeamCount)
        {
            Team = "Red";
            redTeamCount++;
        }
        else
        {
            if (rand == 1)
            {
                Team = "Blue";
                blueTeamCount++;
            }
            else
            {
                Team = "Red";
                redTeamCount++;
            }
        }

        // if this client happens to be master, find and update GameManager
        if (PhotonNetwork.isMasterClient == true)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().redTeamCount = redTeamCount;
            GameObject.Find("GameManager").GetComponent<GameManager>().blueTeamCount = blueTeamCount;
        }
    }

    // main function to send a users data to all other users, so they can texture them and attach their weapon, on start
    [PunRPC] void SendPlayerInfo(string RPCusername, string RPCuserpass, string RPCheadtex, string RPCbodytex, string RPCweapon)
    {
        username = RPCusername;
        userpass = RPCuserpass;
        headtex = RPCheadtex;
        bodytex = RPCbodytex;
        weapon = RPCweapon;

        // loop through all characters
        GameObject[] playerRefs = GameObject.FindGameObjectsWithTag("Character");

        for (int i = 0; i < playerRefs.Length; i++)
        {
            if (playerRefs[i].GetComponent<C_Character>().username == RPCusername)
            {
                // if their username is the same as on one recieved in RPC, change their materials based on strings that was send in rpc too
                playerRefs[i].transform.Find("CharacterBody").GetComponent<Renderer>().material = GameObject.Find("GameManager").GetComponent<GameManager>().characterTexDict[RPCbodytex];
                playerRefs[i].transform.Find("CharacterBody/CharacterHead").GetComponent<Renderer>().material = GameObject.Find("GameManager").GetComponent<GameManager>().characterTexDict[RPCheadtex];
                playerRefs[i].transform.Find("CharacterBody/CharacterLArm").GetComponent<Renderer>().material = GameObject.Find("GameManager").GetComponent<GameManager>().characterTexDict[RPCheadtex];
                playerRefs[i].transform.Find("CharacterBody/CharacterRArm").GetComponent<Renderer>().material = GameObject.Find("GameManager").GetComponent<GameManager>().characterTexDict[RPCheadtex];

                // now find the weapon that belongs to the found player and attach/material it
                GameObject[] weaponRefs = GameObject.FindGameObjectsWithTag("Weapon");

                for (int w = 0; w < weaponRefs.Length; w++)
                {
                    if (weaponRefs[w].GetComponent<W_Weapon>().Owner == RPCusername)
                    {
                        // change colour == Team
                        if (playerRefs[i].GetComponent<C_Character>().Team == "Red")
                        {
                            if (!weaponRefs[w].name.Contains("3"))
                            {
                                weaponRefs[w].transform.GetChild(0).GetComponent<Renderer>().material = redMaterial;
                                weaponRefs[w].transform.GetChild(1).GetComponent<Renderer>().material = redMaterial;
                            }
                            else weaponRefs[w].transform.GetComponent<Renderer>().material = redMaterial;
                        }
                        else
                        {
                            if (!weaponRefs[w].name.Contains("3"))
                            {
                                weaponRefs[w].transform.GetChild(0).GetComponent<Renderer>().material = blueMaterial;
                                weaponRefs[w].transform.GetChild(1).GetComponent<Renderer>().material = blueMaterial;
                            }
                            else weaponRefs[w].transform.GetComponent<Renderer>().material = blueMaterial;
                        }
                        weaponRefs[w].transform.parent = playerRefs[i].transform.Find("CharacterBody/CharacterLArm/LGunSlot");
                        weaponRefs[w].transform.localScale = new Vector3(3.33f, 2.5f, 1.66f); // do not forget to set the scale of other peoples weapons or they will scale strangely when being childed to something. This value is uniform for all weapons anyway
                    }
                }
            }
        }
    }

    void MoveToSpawnPoint()
    {
        // move to spawn point
        GameObject[] spawnPointRefs = GameObject.FindGameObjectsWithTag("SpawnPoint");

        for (int i = 0; i < spawnPointRefs.Length; i++)
        {
            if (Team == spawnPointRefs[i].GetComponent<SpawnPoint>().Team && spawnPointRefs[i].GetComponent<SpawnPoint>().Occupied == false)
            {
                transform.position = spawnPointRefs[i].transform.position;
                transform.rotation = spawnPointRefs[i].transform.rotation;
                break;
            }
        }
    }

    void AttachWeapon(GameObject root, string weapon)
    {
        // spawn weapon
        GameObject localL_Gun;
        Transform L_gunSlot = transform.Find("CharacterBody/CharacterLArm/LGunSlot");

        localL_Gun = (GameObject)PhotonNetwork.Instantiate(weapon, L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), transform.rotation, 0); // weapon = a string taken from the database, which must match the name of the weapon in the resources folder
        localL_Gun.transform.parent = L_gunSlot;
        localL_Gun.GetPhotonView().RPC("SetOwner", PhotonTargets.AllBuffered, username);

        // change colour == Team
        if (Team == "Red")
        {
            if (!weapon.Contains("3"))
            {
                localL_Gun.transform.GetChild(0).GetComponent<Renderer>().material = redMaterial;
                localL_Gun.transform.GetChild(1).GetComponent<Renderer>().material = redMaterial;
            }
            else localL_Gun.transform.GetComponent<Renderer>().material = redMaterial;
        }
        else
        {
            if (!weapon.Contains("3"))
            {
                localL_Gun.transform.GetChild(0).GetComponent<Renderer>().material = blueMaterial;
                localL_Gun.transform.GetChild(1).GetComponent<Renderer>().material = blueMaterial;
            }
            else localL_Gun.transform.GetComponent<Renderer>().material = blueMaterial;
        }

        leftWeapon = localL_Gun.GetComponent<W_Weapon>();
        leftWeapon.enabled = true;
        if (leftWeapon.name.Contains("Weapon2")) autoFire = true;
        else autoFire = false;
    }

    // interace function on picking up pickup item
    public void OnPickUp(PickupType pickupType)
    {
        if (photonView.isMine)
        {
            switch (pickupType)
            {
                case PickupType.AmmoUp:
                    {
                        Debug.Log(username + " got ammo box");
                        StartCoroutine(OnAmmoPickup(leftWeapon.ammoCount, 5f));
                        return;
                    }
                case PickupType.HealthUp:
                    {
                        Debug.Log(username + " got health box");
                        StartCoroutine(OnHealthPickup(1f));
                        return;
                    }
                case PickupType.SpeedUp:
                    {
                        Debug.Log(username + " got speed box");
                        StartCoroutine(OnSpeedPickup(this.GetComponent<C_CharacterMovement>().movementSpeed, 5f));
                        return;
                    }
            }
        }
    }

    // on ammo pickup, set players ammo to basically infinite for a short period of time
    IEnumerator OnAmmoPickup(int currentAmmo, float time)
    {
        GameObject AmmoUpObj = GameObject.Find("_UI/CharacterUI/AmmoUpText");
        Text AmmoUpText = AmmoUpObj.GetComponent<Text>();
        AmmoUpObj.SetActive(true);

        float ammoTime = 0f;

        if (leftWeapon.ammoCount != 100) // check to see if they haven't got the effect already
        {

            while (ammoTime < time)
            {
                ammoTime += Time.deltaTime;
                leftWeapon.ammoCount = 100;
                AmmoUpText.text = "INFINITE AMMO! : " + (time - ammoTime).ToString("0.0");
                yield return null;
            }

            AmmoUpObj.SetActive(false);
            leftWeapon.ammoCount = currentAmmo;
            yield break;
        }
        else yield break;
    }

    // on speed pickup, set players speed to + 5 for a short period of time
    IEnumerator OnSpeedPickup(float currentSpeed, float time)
    {
        C_CharacterMovement characterMovement;
        characterMovement = this.GetComponent<C_CharacterMovement>();
        GameObject SpeedUpObj = GameObject.Find("_UI/CharacterUI/SpeedUpText");
        Text SpeedUpText = SpeedUpObj.GetComponent<Text>();
        SpeedUpObj.SetActive(true);

        if (characterMovement.movementSpeed != 13f) // check to see if they haven't got the effect already
        {

            float speedTime = 0f;
            while (speedTime < time)
            {
                speedTime += Time.deltaTime;
                characterMovement.movementSpeed = 8f;
                SpeedUpText.text = "SPEED UP! : " + (time - speedTime).ToString("0.0");
                yield return null;
            }

            SpeedUpObj.SetActive(false);
            characterMovement.movementSpeed = currentSpeed;
            yield break;
        }
        else yield break;
    }

    // on health pickup, set players health to max, no coroutine needed
    IEnumerator OnHealthPickup(float time)
    {
        GameObject HealthUpObj = GameObject.Find("_UI/CharacterUI/HealthUpText");
        Text HealthUpText = HealthUpObj.GetComponent<Text>();
        HealthUpObj.SetActive(true);

        Health = maxHealth;

        float healthTime = 0f;
        while (healthTime < time)
        {
            healthTime += Time.deltaTime;
            HealthUpText.text = "HEALTH RECOVERED!";
            yield return null;
        }

        HealthUpObj.SetActive(false);
        yield break;
    }

    // death coroutine
    // this is called by the paintball itself when it collides with the player and checks its health
    public IEnumerator OnDeath(float time)
    {
        GameObject DeathTextObj = GameObject.Find("_UI/CharacterUI/DeathText");
        Text DeathText = DeathTextObj.GetComponent<Text>();
        DeathTextObj.SetActive(true);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        isDead = true;
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // enabling death cam to focus on player that killed us
        GameObject[] playerRefs = GameObject.FindGameObjectsWithTag("Character");
        for (int i = 0; i < playerRefs.Length; i++)
        {
            if (playerRefs[i].GetComponent<C_Character>().username == killedBy)
            {
                localCamera.CallDeathCam(time, playerRefs[i]);
                break;
            }
        }

        float deathTime = 0f;
        while (deathTime < time)
        {
            deathTime += Time.deltaTime;
            DeathText.text = "Killed by " + killedBy + " Respawning...";
            yield return null;
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        Health = maxHealth;
        isDead = false;
        DeathTextObj.SetActive(false);
        if (photonView.isMine) localCamera.ResetCamera();
        MoveToSpawnPoint();
        yield break;
    }

    public void CallDeath(float time) { StartCoroutine(OnDeath(time)); }
}
