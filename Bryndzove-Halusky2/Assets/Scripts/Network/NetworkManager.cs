using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum RoomsListInfo
{
    STATUS,
    PASSWORD,
    BLUE_TEAM_CURRENT_COUNT,
    RED_TEAM_CURRENT_COUNT
}

public class NetworkManager : Photon.MonoBehaviour
{

    protected string roomName = "I'm hungry";
    protected TypedLobby lobbyName = new TypedLobby("NewLobby", LobbyType.Default);
    protected RoomInfo[] roomsList;
    // TODO NOT IMPLEMENTED
    // TODO NOT IMPLEMENTED
    // TODO NOT IMPLEMENTED
    protected string[] additionalRoomsListInfo = new string[4];
    protected GameManager GM;
    protected UserInterfaceManager UI_Manager;
    private GameObject lobbyCamera;
    protected GameObject localCharacter;
    protected bool IsGameRunning;

    // character prefab
    [SerializeField]
    private GameObject Character;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("v4.2");
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        lobbyCamera = GameObject.Find("LobbyCamera");
        IsGameRunning = false;
        UI_Manager = GameObject.Find("UIManager").GetComponent<UserInterfaceManager>();
    }

    // TODO NOT IMPLEMENTED
    // TODO NOT IMPLEMENTED
    // TODO NOT IMPLEMENTED
    void SetPropertiesListedInLobby(string[] propsListedInLobby)
    {
        propsListedInLobby = additionalRoomsListInfo;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.inRoom)
        {
            if (IsGameRunning)
            {
                //Debug.Log("Inside the room, and running the game ");
                OnGamePlay();
            }
            else
            {
                //Debug.Log("Inside the room lobby ");
                OnInsideRoomLobby();
            }
        }
        else if (PhotonNetwork.insideLobby)
        {
            // Debug.Log("Inside the lobby ");
            OnInsideLobby();
        }
        else if (!PhotonNetwork.connected)
        {
            //Debug.Log("Not Connected ");
            OnConnecting();
        }
    }

    public virtual void OnInsideLobby()         {}
    public virtual void OnGamePlay()            {}
    public virtual void OnInsideRoomLobby()     {}
    public virtual void OnConnecting()          {}



    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(lobbyName);
    }

    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
        Debug.Log("Room list length " + roomsList.Length);
    }

    public TypedLobby GetLobbyName()
    {
        return lobbyName;
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to " + "'" + PhotonNetwork.room.Name + "'" + " - Players(" + PhotonNetwork.playerList.Length + ")");
    }


    public void StartGame()
    {
        if (PhotonNetwork.isMasterClient) photonView.RPC("StartTheGame", PhotonTargets.All, null);
    }

    [PunRPC] void StartTheGame()
    {
        NetworkManager tempNW = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        GameManager GMR = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject LCMR = GameObject.Find("LobbyCamera");
        GMR.LockHideCursor();
        GMR.roundStarted = true;
        if (LCMR != null) LCMR.SetActive(false);
        tempNW.SetupAndSpawnCharacter();
        tempNW.IsGameRunning = true;
        Debug.Log("GOING TO DISABLE MAIN MENU");
        UI_Manager.DisableMainMenu();
    }

    public void LeaveRoomFromRoomLobby()
    {
        PhotonNetwork.LeaveRoom();
        IsGameRunning = false;
    }

    void SetupAndSpawnCharacter()
    {
        // note: we are spawning a character from a prefab, which is a 'base', the network character (the one we are controlling)
        // is the localCharacter variable, which needs to have their components enabled
        if (PhotonNetwork.playerList.Length > 1)
        {
            localCharacter = (GameObject)PhotonNetwork.Instantiate(Character.name, new Vector3(-9, 0, -7), Quaternion.identity, 0);
        }
        else
        {
            localCharacter = (GameObject)PhotonNetwork.Instantiate(Character.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
        }

        // -- activate local scripts (disabled for everyone else)
        // activate base scripts
        localCharacter.GetComponent<C_Character>().enabled = true;
        localCharacter.GetComponent<C_CharacterMovement>().enabled = true;
        // activate child components
        localCharacter.transform.Find("CharacterCamera").gameObject.SetActive(true);
        // activate child scripts
        localCharacter.GetComponentInChildren<C_LArmTilt>().enabled = true;
        localCharacter.GetComponentInChildren<C_RArmTilt>().enabled = true;
        localCharacter.GetComponentInChildren<C_BodyTilt>().enabled = true;
        localCharacter.GetComponentInChildren<C_CameraMovement>().enabled = true;

        // send reference to character UI and initialise it
        UI_Character UIC = GameObject.Find("CharacterUI").GetComponent<UI_Character>();
        UIC.localCharacter = localCharacter.GetComponent<C_Character>();
    }
}