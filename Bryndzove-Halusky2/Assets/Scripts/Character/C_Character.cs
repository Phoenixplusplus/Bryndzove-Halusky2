using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Character : Photon.MonoBehaviour, ICanPickup {

    // events
    public delegate void PlayerStart();
    public static event PlayerStart PlayerReady;

    [Header("Network Attributes")]
    public string Team;
    public string killedBy;

    [Header("Attributes")]
    public float Health;
    public float maxHealth = 10f;
    public string username;
    public string userpass;
    public string headtex;
    public string bodytex;
    public string weapon;
    public Material redMaterial;
    public Material blueMaterial;
    public AudioClip deathSound;
    public AudioClip goSound;
    public bool isDead = false;

    private GameManager gameManager;
    private C_CameraMovement localCamera;

    // do not assign these values in editor
    public W_Weapon leftWeapon, rightWeapon;
    private bool autoFire;

    // Use this for initialization
    void Start()
    {
        // Spawning of character only happen when the NetworkManager is ready to spawn one
        if (photonView.isMine)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            localCamera = transform.GetChild(1).GetComponent<C_CameraMovement>();

            // grab values needed from gamemanager
            username = gameManager.username;
            userpass = gameManager.userpass;
            headtex = gameManager.headtex;
            bodytex = gameManager.bodytex;
            weapon = gameManager.weapon;
            if (gameManager.redTeam.IsPlayerInTeam(PhotonNetwork.player.NickName)) Team = "Red";
            else Team = "Blue";

            // apply textures based on values
            transform.Find("CharacterBody").GetComponent<Renderer>().material = gameManager.characterTexDict[bodytex];
            transform.Find("CharacterBody/CharacterHead").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            transform.Find("CharacterBody/CharacterLArm").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            transform.Find("CharacterBody/CharacterRArm").GetComponent<Renderer>().material = gameManager.characterTexDict[headtex];
            // weapon values are assigned after team is chosen, as colour of weapon needs to change

            // find an available spawn point
            MoveToSpawnPoint();

            // attach weapon to slot
            AttachWeapon(gameObject, weapon);

            // send this information to other players so they can texture me and child my weapon
            photonView.RPC("SendPlayerInfo", PhotonTargets.OthersBuffered, new object[] { username, userpass, headtex, bodytex, weapon, Team });

            // set my health
            Health = maxHealth;

            // send event to listeners that the character is spawned and ready
            if (PlayerReady != null)
            {
                Debug.Log("Finished startup, Player is calling event to listeners");
                PlayerReady();
            }

            // play audio of round start
            AudioSource.PlayClipAtPoint(goSound, localCamera.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update loop is only conserned with keyboard input -- and checking conditions for this input
        if (photonView.isMine)
        {
            if (!gameManager.roundFinished)
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
                    if (Health <= 0)
                    {
                        CallDeath(3f);
                    }
                }
            }
        }
    }

    // Called on Start()
    // main function to send a users data to all other users, so they can texture them and attach their weapon, as well as register their database information
    // this is needed for things like death cam and other UI that needs to know which player was killed by which
    [PunRPC] void SendPlayerInfo(string RPCusername, string RPCuserpass, string RPCheadtex, string RPCbodytex, string RPCweapon, string RPCteam)
    {
        // set their base information
        username = RPCusername;
        userpass = RPCuserpass;
        headtex = RPCheadtex;
        bodytex = RPCbodytex;
        weapon = RPCweapon;
        Team = RPCteam;

        // loop through all characters
        GameObject[] playerRefs = GameObject.FindGameObjectsWithTag("Character");

        for (int i = 0; i < playerRefs.Length; i++)
        {
            if (playerRefs[i].GetComponent<C_Character>().username == RPCusername)
            {
                // if their username is the same as on one recieved in RPC, everyone needs to change their materials based on strings that was send in rpc too
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
                            // if the weapon name contains 3 (ie the shotgun) we must material it differently to the other weapons
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

    // move to spawn point
    void MoveToSpawnPoint()
    {
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

    // attaching a weapon to the corresponding player
    void AttachWeapon(GameObject root, string weapon)
    {
        // declare game object, and find the slot where we want to child it to
        GameObject localL_Gun;
        Transform L_gunSlot = transform.Find("CharacterBody/CharacterLArm/LGunSlot");

        // instantiate weapon and child it to the found slot
        // weapon string is ultimately taken from the database and assigned to the gamemanager, which this character recieves on start - or for other players - is recieved in the RPC
        // that was sent to them on Start()
        localL_Gun = (GameObject)PhotonNetwork.Instantiate(weapon, L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), transform.rotation, 0);
        localL_Gun.transform.parent = L_gunSlot;

        // send to everyone the owner of this weapon
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
                        StartCoroutine(OnSpeedPickup(this.GetComponent<C_CharacterMovement>().speed, 5f));
                        return;
                    }
            }
        }
    }

    public void DestroySelf() { Destroy(this); }

    // on ammo pickup, set players ammo to basically infinite for a short period of time
    IEnumerator OnAmmoPickup(int currentAmmo, float time)
    {
        GameObject AmmoUpObj = GameObject.Find("_UI/CharacterUI/AmmoUpText");
        Text AmmoUpText = AmmoUpObj.GetComponent<Text>();
        GameObject InfiniteAmmoObj = GameObject.Find("_UI/CharacterUI/InfiniteAmmoIMG");
        AmmoUpObj.SetActive(true);
        InfiniteAmmoObj.SetActive(true);

        float ammoTime = 0f;

        if (leftWeapon.ammoCount != 100) // check to see if they haven't got the effect already
        {

            while (ammoTime < time)
            {
                ammoTime += Time.deltaTime;
                leftWeapon.ammoCount = 100;
                AmmoUpText.text = (time - ammoTime).ToString("0.0");
                yield return null;
            }

            AmmoUpObj.SetActive(false);
            InfiniteAmmoObj.SetActive(false);
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
        GameObject speedUpObj2 = GameObject.Find("_UI/CharacterUI/SpeedUpIMG");
        speedUpObj2.SetActive(true);
        SpeedUpObj.SetActive(true);

        if (characterMovement.speed != 13f) // check to see if they haven't got the effect already
        {

            float speedTime = 0f;
            while (speedTime < time)
            {
                speedTime += Time.deltaTime;
                characterMovement.speed = 13f;
                SpeedUpText.text = (time - speedTime).ToString("0.0");
                yield return null;
            }

            speedUpObj2.SetActive(false);
            SpeedUpObj.SetActive(false);
            characterMovement.speed = currentSpeed;
            yield break;
        }
        else yield break;
    }

    // on health pickup, set players health to max, no coroutine needed
    IEnumerator OnHealthPickup(float time)
    {
        GameObject HealthUpObj = GameObject.Find("_UI/CharacterUI/HealthRecoveredIMG");
        HealthUpObj.SetActive(true);

        Health = maxHealth;

        float healthTime = 0f;
        while (healthTime < time)
        {
            healthTime += Time.deltaTime;
            yield return null;
        }

        HealthUpObj.SetActive(false);
        yield break;
    }

    // death coroutine
    // this is called by the paintball itself when it collides with the player and checks its health
    public IEnumerator OnDeath(float time)
    {
        GameObject RespawningObj = GameObject.Find("_UI/CharacterUI/RespawningIMG");
        RespawningObj.SetActive(true);

        // death stuff
        isDead = true;
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // also send RPC to everyone and notify them of death
        photonView.RPC("NotifyKill", PhotonTargets.All, new object[] { killedBy, username, 2f });

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
            yield return null;
        }

        Health = maxHealth;
        isDead = false;
        RespawningObj.SetActive(false);
        if (photonView.isMine) localCamera.ResetCamera();
        MoveToSpawnPoint();
        leftWeapon.ammoCount = leftWeapon.clipSize;
        yield break;
    }

    public void CallDeath(float time) { StartCoroutine(OnDeath(time)); }

    // notify players of kill
    // this coroutine is triggered by the RPC function below
    IEnumerator NotifyOfKill(string owner, string killedCharacter, float time)
    {
        GameObject KillNotifyObj = GameObject.Find("_UI/CharacterUI/KillNotificationText");
        Text KillNotifyText = KillNotifyObj.GetComponent<Text>();
        GameObject KillNotifyObj2 = GameObject.Find("_UI/CharacterUI/KillNotificationBackIMG");
        KillNotifyObj.SetActive(true);
        KillNotifyObj2.SetActive(true);

        float deathTime = 0f;
        while (deathTime < time)
        {
            deathTime += Time.deltaTime;
            KillNotifyText.text = killedCharacter + " was killed by " + owner;
            yield return null;
        }

        KillNotifyObj2.SetActive(false);
        KillNotifyObj.SetActive(false);
        yield break;
    }

    [PunRPC] public void NotifyKill(string owner, string killedCharacter, float time) { StartCoroutine(NotifyOfKill(owner, killedCharacter, time)); }
}
