using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    protected Canvas CNVS_MainMenu;
    protected Canvas CNVS_Character;

    private Text m_TXT_ConnectingToServer;

    private Transform UI_RoomLobby;
    private Transform UI_RoomSection;
    private Transform UI_PlayerBeenKicked;

    // Use this for initialization
    void Start ()
    {
        CNVS_MainMenu = GameObject.Find("MainMenuUI").GetComponent<Canvas>();

        // Assign UI references
        m_TXT_ConnectingToServer = CNVS_MainMenu.GetComponentInChildren<Text>();
        UI_RoomLobby = CNVS_MainMenu.transform.GetChild(2).transform.GetChild(2);
        UI_RoomSection = CNVS_MainMenu.transform.GetChild(2).transform.GetChild(4);
        UI_PlayerBeenKicked = CNVS_MainMenu.transform.GetChild(2).transform.GetChild(6);


        // Disable UI
        UI_PlayerBeenKicked.gameObject.SetActive(false);
        UI_RoomSection.gameObject.SetActive(false);
        CNVS_MainMenu.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
        CNVS_MainMenu.transform.GetChild(2).transform.GetChild(5).gameObject.SetActive(false);
        CNVS_MainMenu.transform.GetChild(2).gameObject.SetActive(false);




    }

    public void EnableMainMenu()                { CNVS_MainMenu.gameObject.SetActive(false); }
    public void DisableMainMenu()               { CNVS_MainMenu.gameObject.SetActive(false); }

    public void EnablePlayerKickedWidget()      { UI_PlayerBeenKicked.gameObject.SetActive(true); }
    public void DisablePlayerKickedWidget()     { UI_PlayerBeenKicked.gameObject.SetActive(false); }

    public void EnableRoomSection()             { UI_RoomSection.gameObject.SetActive(true); }
    public void DisableRoomSection()            { UI_RoomSection.gameObject.SetActive(false); }

    public void EnableRoomLobby()               { UI_RoomLobby.gameObject.SetActive(true); }
    public void DisableRoomLobby()              { UI_RoomLobby.gameObject.SetActive(false); }

    public void EnableConnectingToServer()      { m_TXT_ConnectingToServer.gameObject.SetActive(true); }
    public void DisableConnectingToServer()     { m_TXT_ConnectingToServer.gameObject.SetActive(false); }

    public void EnableLobbyUI()                 { CNVS_MainMenu.transform.GetChild(2).gameObject.SetActive(true); }
    public void DisableLobbyUI()                { CNVS_MainMenu.transform.GetChild(2).gameObject.SetActive(false); }

    public bool IsConnectingToServerActive()    { return m_TXT_ConnectingToServer.gameObject.activeSelf; }

    // Return the count of players connected in master server and rooms together
    public int GetPlayersInMasterServer()       { return PhotonNetwork.countOfPlayersOnMaster + PhotonNetwork.countOfPlayersInRooms; }

}
