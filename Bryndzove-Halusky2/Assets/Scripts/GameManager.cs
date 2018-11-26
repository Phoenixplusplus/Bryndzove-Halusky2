using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public AudioClip buttonPressSound;
    [Header("Player Database Values")]
    public string username;
    public string userpass;
    public string headtex;
    public string bodytex;
    public string weapon;

    // player configuration dictionaries
    public Dictionary<string, Material> characterTexDict;
    public Material Head1;
    public Material Head2;
    public Material Head3;
    public Material Head4;
    public Material Head5;
    public Material Head6;
    public Material Head7;
    public Material Head8;
    public Material Head9;
    public Material Body1;
    public Material Body2;
    public Material Body3;
    public Material Body4;
    public Material Body5;
    public Material Body6;
    public Material Body7;
    public Material Body8;
    public Material Body9;
    public Dictionary<string, GameObject> characterWeaponDict;
    public GameObject Weapon1; // pistol
    public GameObject Weapon2; // machinegun
    public GameObject Weapon3; // shotgun

    [Header("-- Runtime Game Stats --")]
    [Header("Round")]
    public float roundTime = 60f;
    public float currentRoundTime = 0f;
    public bool roundStarted = false;
    public bool roundFinished = false;
    [Header("Teams")]
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
    public int splatDecalsSize = 200;
    public Vector3 decalsStartPosition = new Vector3(0, -10, 0);
    private int currentDecal = 0;
    [Header("Paintballs")]
    public GameObject Paintball;
    private GameObject[] Paintballs;
    public int paintballsSize = 100;
    public Vector3 paintballsStartPosition = new Vector3(0, -10, 0);
    private int currentPaintball = 0;

    Vector3 soundPosition;

    void OnEnable()
    {
        UI_Character.PlayerBackToLobbyStatus += ResetMatchValues;
    }

    void OnDisable()
    {
        UI_Character.PlayerBackToLobbyStatus -= ResetMatchValues;
    }

    // Use this for initialization
    void Start ()
    {
        soundPosition = GameObject.Find("LobbyCamera").transform.position;
        // initialise dictionaries and add values linked to strings that must match what could be in database
        characterTexDict = new Dictionary<string, Material>();
        characterTexDict.Add("Head1", Head1);
        characterTexDict.Add("Head2", Head2);
        characterTexDict.Add("Head3", Head3);
        characterTexDict.Add("Head4", Head4);
        characterTexDict.Add("Head5", Head5);
        characterTexDict.Add("Head6", Head6);
        characterTexDict.Add("Head7", Head7);
        characterTexDict.Add("Head8", Head8);
        characterTexDict.Add("Head9", Head9);
        characterTexDict.Add("Body1", Body1);
        characterTexDict.Add("Body2", Body2);
        characterTexDict.Add("Body3", Body3);
        characterTexDict.Add("Body4", Body4);
        characterTexDict.Add("Body5", Body5);
        characterTexDict.Add("Body6", Body6);
        characterTexDict.Add("Body7", Body7);
        characterTexDict.Add("Body8", Body8);
        characterTexDict.Add("Body9", Body9);
        characterWeaponDict = new Dictionary<string, GameObject>();
        characterWeaponDict.Add("Weapon1", Weapon1);
        characterWeaponDict.Add("Weapon2", Weapon2);
        characterWeaponDict.Add("Weapon3", Weapon3);

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
                if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
                if (!Cursor.visible) Cursor.visible = true;
            }
        }
    }

    public void PlayButtonPressSoundUI() { AudioSource.PlayClipAtPoint(buttonPressSound, soundPosition); }

    public void LockHideCursor()
    {
        // lock and hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ResetMatchValues()
    {
        roundTime = 60f;
        currentRoundTime = 0f;
        roundStarted = false;
        roundFinished = false;
        redTeamPaintCount = 0;
        blueTeamPaintCount = 0;

        // and decals/paintballs
        foreach (GameObject p in Paintballs)
        {
            p.transform.position = paintballsStartPosition;
        }

        foreach (GameObject d in SplatDecals)
        {
            d.transform.position = decalsStartPosition;
        }
    }

    // called by any weapon that fires, grab one from the pool and just position it where needed
    public void SetPaintball(Vector3 position, Quaternion rotation, Vector3 colour, float speed, string team, string owner)
    {
        Paintballs[currentPaintball].transform.position = position;
        Paintballs[currentPaintball].transform.rotation = rotation;
        Paintballs[currentPaintball].GetComponent<Renderer>().material.color = new Color(colour.x, colour.y, colour.z, 1);
        P_Paintball pp = Paintballs[currentPaintball].GetComponent<P_Paintball>();
        pp.Speed = speed;
        pp.Team = team;
        pp.Owner = owner;
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
