using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Character : Photon.MonoBehaviour {

    private GameManager gameManager;

    [Header("Network Attributes")]
    public string loginName;
    public string Team;

    [Header("Attributes")]
    public float Health = 10f;
    public float healthRecoverySpeed = 0.1f;
    public float movementSpeed;
    public Texture headTex, bodyTex;

    // do not assign these values in editor
    public W_Weapon leftWeapon, rightWeapon;

    private bool autoFire;

    // Use this for initialization
    void Start ()
    {
        // on spawn from network manager
        if (photonView.isMine)
        {
            if (PhotonNetwork.isMasterClient == true)
            {
                Debug.Log("I am master client, setting reference to GameManager");
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                Debug.Log("Press V to see how many parts a team has painted");
            }
            else gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // pick a team based on players in server send as buffed so others that join know
            int rand = Random.Range(0, 3);
            photonView.RPC("PickTeam", PhotonTargets.AllBuffered, rand);

            MoveToSpawnPoint();
            AttachWeapon();
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

        if (Team == "Red")
        {
            localL_Gun = (GameObject)PhotonNetwork.Instantiate("RedMachineGun", L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), transform.rotation, 0);
            localL_Gun.transform.parent = L_gunSlot;
        }
        else
        {
            localL_Gun = (GameObject)PhotonNetwork.Instantiate("BlueMachineGun", L_gunSlot.transform.position + new Vector3(0, 0.1f, 0), transform.rotation, 0);
            localL_Gun.transform.parent = L_gunSlot;
        }

        leftWeapon = localL_Gun.GetComponent<W_Weapon>();
        leftWeapon.enabled = true;
        if (leftWeapon.name.Contains("Machine")) autoFire = true;
        else autoFire = false;
    }
}
