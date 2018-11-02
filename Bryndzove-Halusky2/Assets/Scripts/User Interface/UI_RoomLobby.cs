using UnityEngine;
using UnityEngine.UI;

public class TeamInfo
{
    // Private variables
    private bool [] IsSlotEmptyArray;
    private Button [] buttonsArray;
    private PhotonPlayer [] playersArray;
    private int playerPosInArray;
    private int maxPlayersInTeamCount;
    public int curPlayerTeamCount;
    private bool IsRedTeam;
    private bool IsBlueTeam;

    // Getter functions
    // Ask if the slot is empty or not
    public bool GetSlotStatus(int slotIndex)
    {
        if (slotIndex < maxPlayersInTeamCount) return IsSlotEmptyArray[slotIndex];
        else { Debug.Log("Index out of range, return false"); return false; }
    }

    // Constructor, takes number of players in room, if is the info red team or blue team
    public TeamInfo(int numberOfPlayers, bool redTeam, bool blueTeam, bool IsGameRunning)
    {
        maxPlayersInTeamCount = numberOfPlayers / 2;
        IsSlotEmptyArray = new bool[maxPlayersInTeamCount];
        buttonsArray = new Button[4];
        playersArray = new PhotonPlayer[maxPlayersInTeamCount];

        IsRedTeam = redTeam;
        IsBlueTeam = blueTeam;
        playerPosInArray = -1;

        if (!IsGameRunning)
        {
            // Initialize buttons
            if (redTeam)
            {
                GameObject GO = GameObject.Find("IMG_RedTeamBackground");
                buttonsArray[0] = GO.transform.GetChild(0).GetComponent<Button>();
                buttonsArray[1] = GO.transform.GetChild(1).GetComponent<Button>();
                buttonsArray[2] = GO.transform.GetChild(2).GetComponent<Button>(); 
                buttonsArray[3] = GO.transform.GetChild(3).GetComponent<Button>();

            }
            else if (blueTeam)
            {
                GameObject GO = GameObject.Find("IMG_BlueTeamBackground");
                buttonsArray[0] = GO.transform.GetChild(0).GetComponent<Button>();
                buttonsArray[1] = GO.transform.GetChild(1).GetComponent<Button>();
                buttonsArray[2] = GO.transform.GetChild(2).GetComponent<Button>();
                buttonsArray[3] = GO.transform.GetChild(3).GetComponent<Button>(); 
            }
        }

        //  maxPlayersInTeamCount
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            buttonsArray[i].enabled = false;
            buttonsArray[i].GetComponentInChildren<Text>().text = "";
            IsSlotEmptyArray[i] = true;
        }

        if (maxPlayersInTeamCount < 4)
        {
            for (int i = maxPlayersInTeamCount; i < 4; i++)
            {
                buttonsArray[i].gameObject.SetActive(false);
                buttonsArray[i] = null;
            }
        }
    }

    public int GetCurPlayersTeamCount()
    {
        return curPlayerTeamCount;
    }

    public int GetMaxPlayersTeamCount()
    {
        return maxPlayersInTeamCount;
    }

    public int GetPlayerPositionInArray()
    {
        return playerPosInArray;
    }

    public void JoinTeam(PhotonPlayer player)
    {
        if (curPlayerTeamCount < maxPlayersInTeamCount)
        {
            // Check if player can join team, if is already in the team will not join
            for (int i = 0; i < maxPlayersInTeamCount; i++)
            {
                if (playersArray[i] != null && player.NickName == playersArray[i].NickName)
                {
                    return;
                    Debug.Log("Can not join team, team is full");
                }
            }

            // Player can join the team, find empty slot for his name
            for (int i = 0; i < maxPlayersInTeamCount; i++)
            {
                if (IsSlotEmptyArray[i])
                {
                    IsSlotEmptyArray[i] = false;
                    playersArray[i] = player;
                    buttonsArray[i].enabled = true;
                    playerPosInArray = i;
                    if (!PhotonNetwork.isMasterClient) buttonsArray[i].GetComponentInChildren<Text>().text = playersArray[i].NickName;
                    else buttonsArray[i].GetComponentInChildren<Text>().text = player.NickName + " (Master)";
                    return;
                }             
            }

            // Increase the players count in the current team
            curPlayerTeamCount++;
        }
        else
        {
            // Can not join team, team is full
            Debug.Log("Can not join team, team is full");
        }
    }

    public void LeaveTeam(PhotonPlayer player)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (playersArray[i] != null && playersArray[i].NickName == player.NickName)
            {
                IsSlotEmptyArray[i] = true;
                buttonsArray[i].enabled = false;
                playersArray[i] = null;
                curPlayerTeamCount--;
                return;
            }

        }

        // Can not leave team, player is not part of the team
        Debug.Log("Can not leave team, player is not part of the team");
    }
}


public class UI_RoomLobby : NetworkManager
{
    private Button m_BTN_LeaveRoom;
    private Button m_BTN_KickPlayer;
    private Image BTN_IMG_LeaveRoom;
    private Image BTN_IMG_KickPlayer;


    void Start()
    {
        m_BTN_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Button>();
        m_BTN_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Button>();
        BTN_IMG_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Image>();

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    void Update()
    {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
           // m_playersNames[i] = PhotonNetwork.playerList[i].name;
           // Debug.Log(PhotonNetwork.playerList[i].NickName + " " + PhotonNetwork.playerList.Length);
        }
    }

    [PunRPC] void SendMeTeamsInformations()
    {

        Debug.Log("SENDING TEAM INFOS");
        GameManager tempGM = GameObject.Find("GameManager").GetComponent<GameManager>();
        photonView.RPC("PickTeam", PhotonTargets.All, tempGM.redTeam, tempGM.blueTeam);
        Debug.Log("SENT TEAM INFOS");
    }

    [PunRPC] void PickTeam(TeamInfo redTeam, TeamInfo blueTeam)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            // find all characters in the server currently as see which team they're on
            GameManager tempGM = GameObject.Find("GameManager").GetComponent<GameManager>();

            if (tempGM.redTeam == null || tempGM.blueTeam == null)
            {
                tempGM.redTeam = redTeam;
                tempGM.blueTeam = blueTeam;

                if (tempGM.redTeam.GetCurPlayersTeamCount() < tempGM.blueTeam.GetCurPlayersTeamCount())
                {
                    tempGM.redTeam.JoinTeam(PhotonNetwork.player);
                    tempGM.redTeam.GetPlayerPositionInArray();
                }
                else if (tempGM.blueTeam.GetCurPlayersTeamCount() < tempGM.redTeam.GetCurPlayersTeamCount())
                {
                    tempGM.blueTeam.JoinTeam(PhotonNetwork.player);
                }
                else
                {
                    int randNumb = Random.Range(0, 1);
                    if (randNumb == 0) tempGM.redTeam.JoinTeam(PhotonNetwork.player);
                    else tempGM.blueTeam.JoinTeam(PhotonNetwork.player);
                }
                Debug.Log("Player has picke up team.");
            }
            else
            {
                Debug.Log("Player has got team yet.");
            }
        }
    }

    // Update is called once per frame
    void OnJoinedRoom()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            if (!IsGameRunning)
            {
                m_BTN_LeaveRoom.enabled = false;
                m_BTN_KickPlayer.enabled = false;
                BTN_IMG_LeaveRoom.color = new Color32(100, 100, 100, 255);
                BTN_IMG_KickPlayer.color = new Color32(100, 100, 100, 255);
            }

            // CALLL HERE PICK TEAM PUN RPC
            photonView.RPC("SendMeTeamsInformations", PhotonTargets.MasterClient, null);

        }
        else
        {
            GM.redTeam = new TeamInfo(PhotonNetwork.room.MaxPlayers, true, false, IsGameRunning);
            GM.blueTeam = new TeamInfo(PhotonNetwork.room.MaxPlayers, false, true, IsGameRunning);

            GM.redTeam.JoinTeam(PhotonNetwork.player);
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
}
