using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Player Database Values")]
    public string username;
    public string userpass;
    public string headtex;
    public string bodytex;
    public string weapon;

    // player configuration dictionaries
    public Dictionary<string, Material> characterTexDict;
    public Material defaultHead;
    public Material defaultBody;
    public Dictionary<string, GameObject> characterWeaponDict;
    public GameObject defaultPistol;
    public GameObject defaultMachineGun;
    public GameObject defaultShotgun;

    [Header("-- Runtime Game Stats --")]
    [Header("Round")]
    public float roundTime = 120f;
    public float currentRoundTime = 0f;
    public bool roundStarted = false;
    public bool roundFinished = false;
    [Header("Teams")]
    public int redTeamCount = 0;
    public int blueTeamCount = 0;
    public TeamInfo redTeam;
    public TeamInfo blueTeam;
    [Header("Paint")]
    public int redTeamPaintCount = 0;
    public int blueTeamPaintCount = 0;

    [Header("-- Pooled GameObjects --")]
    [Header("SplatDecals")]
    public GameObject SplatDecal;
    [Tooltip("This must be assigned a value, or shader will complain :'(")]
    public Material defaultMaterial;
    private GameObject[] SplatDecals;
    public int splatDecalsSize = 5;
    public Vector3 decalsStartPosition = new Vector3(0, -10, 0);
    private int currentDecal = 0;
    [Header("Paintballs")]
    public GameObject Paintball;
    private GameObject[] Paintballs;
    public int paintballsSize = 5;
    public Vector3 paintballsStartPosition = new Vector3(0, -10, 0);
    private int currentPaintball = 0;

    // Use this for initialization
    void Start ()
    {
        // initialise dictionaries and add values from strings taken from their account in the database
        characterTexDict = new Dictionary<string, Material>();
        characterTexDict.Add("defaultHead", defaultHead);
        characterTexDict.Add("defaultBody", defaultBody);
        characterWeaponDict = new Dictionary<string, GameObject>();
        characterWeaponDict.Add("defaultPistol", defaultPistol);
        characterWeaponDict.Add("defaultMachineGun", defaultMachineGun);
        characterWeaponDict.Add("defaultShotgun", defaultShotgun);

        // initialise splat decals pool
        SplatDecals = new GameObject[splatDecalsSize];
        for (int i = 0; i < splatDecalsSize; i++)
        {
            SplatDecals[i] = (GameObject)Instantiate(SplatDecal, decalsStartPosition, Quaternion.identity);
            SplatDecals[i].GetComponent<Decal>().m_Material = defaultMaterial;
        }

        // initialise paintballs pool
        Paintballs = new GameObject[paintballsSize];
        for (int i = 0; i < paintballsSize; i++)
        {
            Paintballs[i] = (GameObject)Instantiate(Paintball, paintballsStartPosition, Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (roundStarted)
        {
            currentRoundTime += Time.deltaTime;
            roundTime -= Time.deltaTime;
            if (roundTime <= 0)
            {
                roundStarted = false;
                roundFinished = true;
            }
        }
    }

    public void LockHideCursor()
    {
        // lock and hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // called by any weapon that fires, grab one from the pool and just position it where needed
    public void SetPaintball(Vector3 position, Quaternion rotation, Vector3 colour, float speed, string team)
    {
        Paintballs[currentPaintball].transform.position = position;
        Paintballs[currentPaintball].transform.rotation = rotation;
        Paintballs[currentPaintball].GetComponent<Renderer>().material.color = new Color(colour.x, colour.y, colour.z, 1);
        P_Paintball pp = Paintballs[currentPaintball].GetComponent<P_Paintball>();
        pp.Speed = speed;
        pp.Team = team;
        pp.isInit = false;
        pp.PaintballRaycast();

        // increment through list and check
        currentPaintball++;
        if (currentPaintball >= paintballsSize) currentPaintball = 0;
    }

    // called by paintball on collision
    public void SetSplatDecal(Vector3 position, Quaternion rotation, Material material)
    {
        SplatDecals[currentDecal].transform.position = position;
        SplatDecals[currentDecal].transform.rotation = rotation;
        // the splats rotation is of the normal of the surface hit, because decals are projected down from the Y axis, rotate x by 90 so we can see it
        // then give a random local Y rotation so not all splats look the same
        SplatDecals[currentDecal].transform.Rotate(Vector3.right, 90, Space.Self);
        SplatDecals[currentDecal].transform.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);
        SplatDecals[currentDecal].GetComponent<Decal>().m_Material = material;

        // increment through list and check
        currentDecal++;
        if (currentDecal >= splatDecalsSize) currentDecal = 0;
    }
}
