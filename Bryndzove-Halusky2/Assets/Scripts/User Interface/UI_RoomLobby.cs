using UnityEngine;
using UnityEngine.UI;

public class UI_RoomLobby : NetworkManager
{
    private Button m_BTN_StartGame;
    private Button m_BTN_KickPlayer;
    private Image[] m_playersButtonsImages = new Image[8];
    private Image BTN_IMG_StartRoom;
    private Image BTN_IMG_KickPlayer;
    private bool HasPickedTeam;
    private bool HasNewPlayerJoined;
    private string lastAssignedMasterClient;
    private string m_playerRequested;
    private string m_playerMarkedToKick;
    private int MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM;
    private int m_lastMarkedPlayerToKickButtonIndex;

    void Start()
    {
        m_BTN_KickPlayer = this.gameObject.transform.GetChild(6).GetComponent<Button>();
        m_BTN_StartGame = this.gameObject.transform.GetChild(7).GetComponent<Button>();
        BTN_IMG_KickPlayer = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_StartRoom = this.gameObject.transform.GetChild(7).GetComponent<Image>();

        for (int i = 0; i < m_playersButtonsImages.Length / 2; i++) m_playersButtonsImages[i] = this.transform.GetChild(1).transform.GetChild(i).GetComponent<Image>();
        for (int i = m_playersButtonsImages.Length / 2; i < m_playersButtonsImages.Length; i++) m_playersButtonsImages[i] = this.transform.GetChild(2).transform.GetChild(i - m_playersButtonsImages.Length / 2).GetComponent<Image>();

        m_lastMarkedPlayerToKickButtonIndex = -1;
        m_playerMarkedToKick = "";

        HasPickedTeam = false;
        HasNewPlayerJoined = false;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    void Update()
    {
        // If has new player joined room Lobby, and I am master client, assign a team to new player
        if (HasNewPlayerJoined && PhotonNetwork.isMasterClient)
        {
            Debug.Log("Nudes request recieved");

            // Pick up team for new player, 
            // The first statemnt will assign player into red team if red team has less players in team than blue team
            if (GM.redTeam.GetCurPlayersTeamCount() < GM.blueTeam.GetCurPlayersTeamCount())
            {
                for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                {
                    if (PhotonNetwork.otherPlayers[i].NickName == m_playerRequested)
                    {
                        GM.redTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                    }
                }
            } // The second statemnt will assign player into blue team if blue team has less players in team than red team
            else if (GM.blueTeam.GetCurPlayersTeamCount() < GM.redTeam.GetCurPlayersTeamCount())
            {
                for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                {
                    if (PhotonNetwork.otherPlayers[i].NickName == m_playerRequested)
                    {
                        GM.blueTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                    }
                }
            }
            else // The third statement randomly assign player into one of the teams
            {
                int randNumb = Random.Range(0, 2);
                for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                {
                    if (PhotonNetwork.otherPlayers[i].NickName == m_playerRequested)
                    {
                        if (randNumb == 0) GM.redTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                        else GM.blueTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                    }
                }
            }            

            // String array with players name, will be send to another players, the array is divided onto two parts.
            // From first part to second part are names of red team players, and from second part to end are names of blue team players
            // If there is no player in the positions between, string will be assigned with text Empty Slot
            string[] tempStringArray = new string[PhotonNetwork.room.MaxPlayers];

            // Fill up arrays with red team datas
            for (int i = 0; i < MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM  ; i++)
                tempStringArray[i] = GM.redTeam.playersNameArray[i];

            // Fill up arrays with blue team datas
            for (int i = MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM; i < PhotonNetwork.room.MaxPlayers; i++)        
                tempStringArray[i] = GM.blueTeam.playersNameArray[i - (MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM )];

            // Send teams to other players
            photonView.RPC("MasterIsSendingTeamInformations", PhotonTargets.Others, (object)tempStringArray, PhotonNetwork.player.NickName);

            // Set the variable HasNewPlayerJoined to false, it is true when new player join into room
            HasNewPlayerJoined = false;
        }
    }

    // MasterClient is sending message to all players in the room except him self
    [PunRPC] void MasterIsSendingTeamInformations(string[] tempStringArray, string masterClientName)
    {
        // Get reference to gameManager
        GameManager tempGM = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Keep track of master client NickName
        lastAssignedMasterClient = masterClientName;

        // Assign red team players name into red team name array
        for (int i = 0; i < MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM; i++)
            tempGM.redTeam.playersNameArray[i] = tempStringArray[i];

        // Assign blue team players name into blue team name array
        for (int i = MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM; i < PhotonNetwork.room.MaxPlayers; i++)
            tempGM.blueTeam.playersNameArray[i - MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM] = tempStringArray[i];
        
        // Update team with new teams data, and master client name
        tempGM.redTeam.UpdateTeam(lastAssignedMasterClient);
        tempGM.blueTeam.UpdateTeam(lastAssignedMasterClient);
    }

    // New player in room is asking masterClient for team
    [PunRPC] void AssignMeTeam(string playerAsking)
    {
        HasNewPlayerJoined = true;
        m_playerRequested = playerAsking;
    }

    // Update is called once per frame
    void OnJoinedRoom()
    {
        GM.redTeam = new TeamInfo(PhotonNetwork.room.MaxPlayers, true, false);
        GM.blueTeam = new TeamInfo(PhotonNetwork.room.MaxPlayers, false, true);

        // Assign maximum count of players in one team
        MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM = PhotonNetwork.room.MaxPlayers / 2;

        // I am not master client, disable options to startGame and kickPlayer, and ask masterClient for team
        if (!PhotonNetwork.isMasterClient)
        {
            m_BTN_StartGame.enabled = false;
            m_BTN_KickPlayer.enabled = false;
            BTN_IMG_StartRoom.color = new Color32(100, 100, 100, 255);
            BTN_IMG_KickPlayer.color = new Color32(100, 100, 100, 255);

            // Ask master for team
            photonView.RPC("AssignMeTeam", PhotonTargets.MasterClient, PhotonNetwork.player.NickName);
        }
        else
        {   // I am a masterClient and have just created this room, so I dont have to ask anyone for team
            // Randomly choose team for masterClient
            int randNumb = Random.Range(0, 2);
            if (randNumb == 0) GM.redTeam.JoinTeam(PhotonNetwork.player.NickName, true);
            else GM.blueTeam.JoinTeam(PhotonNetwork.player.NickName, true);
        }
    }

    // Called inside kick player function, reset button variable to default, update teams and inform other players by RPC about the changes,.
    // This functions is always called only in masterClient
    private void UpdateAfterKickingPlayer()
    { 
        // If the button belong to red team, set the button color to red
        if (m_lastMarkedPlayerToKickButtonIndex < m_playersButtonsImages.Length / 2)
            m_playersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
        // Button belongs to blue team, set the color to blue
        else m_playersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 158, 121);
        // Unmark the variable, and set it to -1, invalid index
        m_lastMarkedPlayerToKickButtonIndex = -1;


        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
        // TODO: UPDATE TEAMS, SEND RPC ABOUT THE CHANGES
    }

    // Function called when masterClient player click on any connected player buttons
    public void MarkOrUnmarkPlayerToKick(int buttonIndex)
    {
        // If the player it not masterClient, assign new target, other clients can not call this function because they have disabled the kick button
        // If the player has been marked, and function is called again and will be trying to mark same player again, it mean that masterClient player want to unmark player
        if (buttonIndex < m_playersButtonsImages.Length / 2)
        {
            for (int i = 0; i < m_playersButtonsImages.Length / 2; i++)
            {

                if (GM.redTeam.playersNameArray[i] == m_playerMarkedToKick) m_playerMarkedToKick = "";
                else m_playerMarkedToKick = GM.redTeam.playersNameArray[buttonIndex];
            }
        }
        else
        {
            for (int i = m_playersButtonsImages.Length / 2; i < m_playersButtonsImages.Length; i++)
            {
                if (GM.blueTeam.playersNameArray[i - (m_playersButtonsImages.Length / 2)] == m_playerMarkedToKick) m_playerMarkedToKick = "";
                else m_playerMarkedToKick = GM.blueTeam.playersNameArray[buttonIndex - m_playersButtonsImages.Length / 2];
            }
        }

        // Chceck who is our new target, if it masterCleint, clean target, otherwise visualise who is targeted
        if (m_playerMarkedToKick == PhotonNetwork.player.NickName)
        {   
            m_playerMarkedToKick = "";
        }
        else
        { 
            // MasterClient clicked on different player, or first time on any player, set the button color to yellow, and visualise that player has been selected to kick
            // These changes can see only masterClient, nobody else can not see who is marked to being kick
            if (m_lastMarkedPlayerToKickButtonIndex != buttonIndex)
            {
                m_playersButtonsImages[buttonIndex].color = new Color32(255, 255, 0, 170);
                m_lastMarkedPlayerToKickButtonIndex = buttonIndex;
            }
            else
            {   // If the button belong to red team, set the button color to red
                if (buttonIndex < m_playersButtonsImages.Length / 2)
                    m_playersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                // Button belongs to blue team, set the color to blue
                else m_playersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 158, 121);
                // Unmark the variable, and set it to -1, invalid index
                m_lastMarkedPlayerToKickButtonIndex = -1;
            }
        }
    }

    // Functions called when masterClient player click on button kick player
    public void KickPlayer()
    {
        // Loop for the player in the room with the marked player name and kick him out
        for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
        {
            if (PhotonNetwork.otherPlayers[i].NickName == m_playerMarkedToKick)
            {
                PhotonNetwork.CloseConnection(PhotonNetwork.otherPlayers[i]);
                UpdateAfterKickingPlayer();
                return;
            }
        }

        Debug.Log("ERROR: Player " + m_playerMarkedToKick + " could not be kicked out from the room! Player has probably left the game before we could kick him out.");
        m_playerMarkedToKick = "";
    }

    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED

    void BecameMasterClient()
    {
        m_BTN_StartGame.enabled = true;
        m_BTN_KickPlayer.enabled = true;
        BTN_IMG_StartRoom.color = new Color32(255, 255, 255, 255);
        BTN_IMG_KickPlayer.color = new Color32(255, 255, 255, 255);
    }

    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED
    // NOT IMPLEMENTED

    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        // ThisPlayerBecameMasterCleint
    }
}
