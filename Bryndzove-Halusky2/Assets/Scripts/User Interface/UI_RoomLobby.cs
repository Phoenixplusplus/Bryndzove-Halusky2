using UnityEngine;
using UnityEngine.UI;

public class UI_RoomLobby : NetworkManager
{
    private Button m_BTN_LeaveRoom;
    private Button m_BTN_KickPlayer;
    private Image BTN_IMG_LeaveRoom;
    private Image BTN_IMG_KickPlayer;
    private string [] m_playersNames = new string[8];
    private GameManager GM;
    private int m_currentRoomID;

    private bool[] m_redPositions = new bool[4];
    private bool[] m_bluePositions = new bool[4];

    private Button m_BTN_RedTeamPosOne;
    private Button m_BTN_RedTeamPosTwo;
    private Button m_BTN_RedTeamPosThree;
    private Button m_BTN_RedTeamPosFour;

    private Button m_BTN_BlueTeamPosOne;
    private Button m_BTN_BlueTeamPosTwo;
    private Button m_BTN_BlueTeamPosThree;
    private Button m_BTN_BlueTeamPosFour;

    void Start()
    {
        m_BTN_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Button>();
        m_BTN_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Button>();
        BTN_IMG_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Image>();

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        m_redPositions[0] = true;
    }

    void Update()
    {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            m_playersNames[i] = PhotonNetwork.playerList[i].name;
            Debug.Log(m_playersNames[i] + " " + PhotonNetwork.playerList.Length);
        }
    }

	// Update is called once per frame
	void OnJoinedRoom()
    {
		if (!PhotonNetwork.isMasterClient)
        {
            m_BTN_LeaveRoom.enabled = false;
            m_BTN_KickPlayer.enabled = false;
            BTN_IMG_LeaveRoom.color = new Color32(100, 100, 100, 255);
            BTN_IMG_KickPlayer.color = new Color32(100, 100, 100, 255);
        }
        else
        {
            GM.redTeamCount++;
        }
    }


    void BecameMasterClient()
    {
        m_BTN_LeaveRoom.enabled = true;
        m_BTN_KickPlayer.enabled = true;
        BTN_IMG_LeaveRoom.color = new Color32(255, 255, 255, 255);
        BTN_IMG_KickPlayer.color = new Color32(255, 255, 255, 255);
    }

    // If the new master client is this call the function
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        // ThisPlayerBecameMasterCleint
    }

    public void SetCurrentRoomID(int newRoomID)
    {
        m_currentRoomID = newRoomID;
    }

    public void JoinRedTeam()
    {
        if (GM.blueTeamCount < roomsList[m_currentRoomID].MaxPlayers / 2)
        {
            GM.redTeamCount++;
            GM.blueTeamCount--;
        }
    }

}
