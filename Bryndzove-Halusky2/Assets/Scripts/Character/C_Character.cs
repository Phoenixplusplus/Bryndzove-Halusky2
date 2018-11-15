using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Character : Photon.MonoBehaviour {

    private GameManager gameManager;

    // events
    public delegate void PlayerStart();
    public static event PlayerStart PlayerReady;

    [Header("Network Attributes")]
    public string Team;

    [Header("Attributes")]
    public float Health;
    public float maxHealth = 10f;
    public float healthRecoverySpeed = 0.1f;
    public float movementSpeed;
    public string username;
    public string userpass;
    public string headtex;
    public string bodytex;
    public string weapon;
    public Material redMaterial;
    public Material blueMaterial;

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
            // weapon values are assigned after team is chosen, as colour of weapon needs to change

            if (PhotonNetwork.isMasterClient == true)
            {
                Debug.Log("I am master client, setting reference to GameManager");
                Debug.Log("Press V to see how many parts a team has painted");
            }

            // pick a team based on players in server send as buffed so others that join know
            int rand = Random.Range(0, 3);
            photonView.RPC("PickTeam", PhotonTargets.AllBuffered, rand);

            MoveToSpawnPoint();
            AttachWeapon();

            Health = maxHealth;

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

    void AttachWeapon()
    {
        // spawn weapon
        GameObject localL_Gun;
        Transform L_gunSlot = transform.Find("CharacterBody/CharacterLArm/LGunSlot");

        localL_Gun = (GameObject)PhotonNetwork.Instantiate(weapon, L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), transform.rotation, 0); // weapon = a string taken from the database, which must match the name of the weapon in the resources folder
        localL_Gun.transform.parent = L_gunSlot;

        // change colour == Team
        if (Team == "Red")
        {
            localL_Gun.transform.GetChild(0).GetComponent<Renderer>().material = redMaterial;
            localL_Gun.transform.GetChild(1).GetComponent<Renderer>().material = redMaterial;
        }
        else
        {
            localL_Gun.transform.GetChild(0).GetComponent<Renderer>().material = blueMaterial;
            localL_Gun.transform.GetChild(1).GetComponent<Renderer>().material = blueMaterial;
        }

        leftWeapon = localL_Gun.GetComponent<W_Weapon>();
        leftWeapon.enabled = true;
        if (leftWeapon.name.Contains("Weapon2")) autoFire = true;
        else autoFire = false;
    }
}
