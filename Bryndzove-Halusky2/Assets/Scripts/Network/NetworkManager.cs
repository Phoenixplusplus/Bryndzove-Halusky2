using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.MonoBehaviour
{
    protected TypedLobby lobbyName = new TypedLobby("NewLobby", LobbyType.Default);
    public RoomInfo[] roomsList;
    protected GameManager GM;
    protected GameObject localCharacter;
    private UserInterfaceManager m_UI_manager;


    // character prefab
    [SerializeField]
    private GameObject Character;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("v4.2");
        m_UI_manager = GameObject.Find("UIManager").GetComponent<UserInterfaceManager>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player is inside the lobby
        if (PhotonNetwork.insideLobby)
        {
            if (m_UI_manager.IsConnectingToServerActive())
            {
                m_UI_manager.DisableConnectingToServer();
                m_UI_manager.EnableLobbyUI();
            }
        }
        // Player lost connection or is connecting
        else if (!PhotonNetwork.connected)
        {
            if (!m_UI_manager.IsConnectingToServerActive())
            {
                m_UI_manager.EnableConnectingToServer();
                m_UI_manager.DisableLobbyUI();
            }
        }
    }

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
        roomsList = PhotonNetwork.GetRoomList();
        Debug.Log("Joined lobby");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to " + "'" + PhotonNetwork.room.Name + "'" + " - Players(" + PhotonNetwork.playerList.Length + ")");
    }

    public void LeaveRoomFromRoomLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SetupAndSpawnCharacter()
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