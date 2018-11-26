using UnityEngine;
using UnityEngine.UI;

enum enumTeams
{
    NO_TEAM = -1,
    RED_TEAM,
    BLUE_TEAM
}

public class UI_RoomLobby : NetworkManager
{
    private UserInterfaceManager UI_Manager;

    private Button m_BTN_StartGame;
    private Button m_BTN_KickPlayer;
    private Image[] m_redTeamPlayersButtonsImages = new Image[4];
    private Image[] m_blueTeamPlayersButtonsImages = new Image[4];
    private Image BTN_IMG_StartRoom;
    private Image BTN_IMG_KickPlayer;
    private bool HasPickedTeam;
    private bool HasNewPlayerJoined;
    private string lastAssignedMasterClient;
    private string m_playerRequested;
    private string m_playerMarkedToKick;
    private int MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM;
    private int m_lastMarkedPlayerToKickButtonIndex;
    private enumTeams m_lastMarkedPlayerToKickIsFromTeam;

    void Start()
    {
        m_BTN_KickPlayer = this.gameObject.transform.GetChild(6).GetComponent<Button>();
        m_BTN_StartGame = this.gameObject.transform.GetChild(7).GetComponent<Button>();
        BTN_IMG_KickPlayer = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_StartRoom = this.gameObject.transform.GetChild(7).GetComponent<Image>();

        for (int i = 0; i < m_redTeamPlayersButtonsImages.Length; i++) m_redTeamPlayersButtonsImages[i] = this.transform.GetChild(1).transform.GetChild(i).GetComponent<Image>();
        for (int i = 0; i < m_blueTeamPlayersButtonsImages.Length; i++) m_blueTeamPlayersButtonsImages[i] = this.transform.GetChild(2).transform.GetChild(i).GetComponent<Image>();

        m_lastMarkedPlayerToKickButtonIndex = -1;
        m_playerMarkedToKick = "";
        m_lastMarkedPlayerToKickIsFromTeam = enumTeams.NO_TEAM;

        HasPickedTeam = false;
        HasNewPlayerJoined = false;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        UI_Manager = GameObject.Find("UIManager").GetComponent<UserInterfaceManager>();
    }

    void Update()
    {
        // If has new player joined room Lobby, and I am master client, assign a team to new player
        if (HasNewPlayerJoined && PhotonNetwork.isMasterClient)
        {
            //Debug.Log("Nudes request recieved");

            // Pick up team for new player, 
            // The first statemnt will assign player into red team if red team has less players in team than blue team
            if (GM.redTeam.GetCurPlayersTeamCount() < GM.blueTeam.GetCurPlayersTeamCount())
            {
                for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                {
                    if (PhotonNetwork.otherPlayers[i].NickName == m_playerRequested)
                    {
                        photonView.RPC("SendTeamToGameManager", PhotonTargets.All, new object[] { m_playerRequested, "Red" });
                        GM.redTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                        //GM.isRedTeam = true;
                    }
                }
            } // The second statemnt will assign player into blue team if blue team has less players in team than red team
            else if (GM.blueTeam.GetCurPlayersTeamCount() < GM.redTeam.GetCurPlayersTeamCount())
            {
                for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                {
                    if (PhotonNetwork.otherPlayers[i].NickName == m_playerRequested)
                    {
                        photonView.RPC("SendTeamToGameManager", PhotonTargets.All, new object[] { m_playerRequested, "Blue" });
                        GM.blueTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                        //GM.isBlueTeam = true;
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
                        if (randNumb == 0)
                        {
                            photonView.RPC("SendTeamToGameManager", PhotonTargets.All, new object[] { m_playerRequested, "Red" });
                            GM.redTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                            //GM.isRedTeam = true;
                        }
                        else
                        {
                            photonView.RPC("SendTeamToGameManager", PhotonTargets.All, new object[] { m_playerRequested, "Blue" });
                            GM.blueTeam.JoinTeam(PhotonNetwork.otherPlayers[i].NickName);
                            //GM.isBlueTeam = true;
                        }
                    }
                }
            }

            // Strings arrays with team players name
            string[] tempRedTeamStringArray = new string[MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM];
            string[] tempBlueTeamStringArray = new string[MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM];

            // Fill up arrays with red team datas
            for (int i = 0; i < MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM; i++)
                tempRedTeamStringArray[i] = GM.redTeam.playersNameArray[i];

            // Fill up arrays with blue team datas
            for (int i = 0; i < MAXIMUM_COUNT_OF_PLAYERS_IN_TEAM; i++)
                tempBlueTeamStringArray[i] = GM.blueTeam.playersNameArray[i];

            // Send team information and master nick name to other players in room
            photonView.RPC("MasterIsSendingTeamInformations", PhotonTargets.Others, (object)tempRedTeamStringArray, (object)tempBlueTeamStringArray, PhotonNetwork.player.NickName);

            // Set the variable HasNewPlayerJoined to false, it is true when new player join into room
            HasNewPlayerJoined = false;
        }
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

            // this must be reset because players that were not master in 1 room, could be master when they finish game and make a room
            m_BTN_StartGame.enabled = true;
            m_BTN_KickPlayer.enabled = true;
            BTN_IMG_StartRoom.color = new Color32(255, 255, 255, 255);
            BTN_IMG_KickPlayer.color = new Color32(255, 255, 255, 255);

            // Randomly choose team for masterClient
            int randNumb = Random.Range(0, 2);
            if (randNumb == 0)
            {
                GM.redTeam.JoinTeam(PhotonNetwork.player.NickName, true);
                GM.isRedTeam = true;
                GM.redTeamCount++;
            }
            else
            {
                GM.blueTeam.JoinTeam(PhotonNetwork.player.NickName, true);
                GM.isBlueTeam = true;
                GM.blueTeamCount++;
            }
        }
        Debug.Log("Master client of this room: " + PhotonNetwork.masterClient);
    }

    // Called when player click on leave room or game button
    public void LeaveRoom()
    {
        int tempTeamColor;
        if (GM.redTeam.IsPlayerInTeam(PhotonNetwork.player.NickName)) tempTeamColor = 0;
        else tempTeamColor = 1;

        // Player which is leaving sends RPC to other players in room, message contain his name and team color
        photonView.RPC("PlayerLeft", PhotonTargets.Others, PhotonNetwork.player.NickName, tempTeamColor);
        photonView.RPC("SendMessagePlayerLeft", PhotonTargets.All, PhotonNetwork.player.NickName);
        PhotonNetwork.LeaveRoom();
    }

    // Called when masterClient player click on button start game, other client can not call this functions until they are not masterClient
    // Reason why they can not call this function is that their start game button is disabled
    public void StartGame()
    {
        photonView.RPC("StartTheGame", PhotonTargets.Others, null);
        PhotonNetwork.room.IsOpen = false;
        StartTheGame();
    }

    // These functions are called only when player click on one of the players buttons inside gameplay lobby/idle lobby when player join room and is waiting until masterClient wont start the game
    // Mark the team which containt player which is going to be kick, because player has always same index as the button which display his name, we can easily find him under buttonIndex.
    public void MarkRedTeamPlayerToKick() { m_lastMarkedPlayerToKickIsFromTeam = enumTeams.RED_TEAM; }
    public void MarkBlueTeamPlayerToKick() { m_lastMarkedPlayerToKickIsFromTeam = enumTeams.BLUE_TEAM; }

    // Function called when masterClient player click on any connected players's button
    public void MarkOrUnmarkPlayerToKick(int buttonIndex)
    {
        // If the player it not masterClient, assign new target, other clients can not call this function because they have disabled the kick button
        switch (m_lastMarkedPlayerToKickIsFromTeam)
        {   // If the player belong to red team, get him and set his name to variable m_playerMarkedToKick, or if he was marked before, unmark him
            case enumTeams.RED_TEAM:
                // If selected target is same as last selected targed or its mastterClient, unselect this target
                if (GM.redTeam.playersNameArray[buttonIndex] == m_playerMarkedToKick || GM.redTeam.playersNameArray[buttonIndex] == PhotonNetwork.player.NickName)
                {
                    // Unmark the variable, and set it to -1, invalid index plus update button visualisation
                    if (m_lastMarkedPlayerToKickButtonIndex > -1)
                    {
                        m_playerMarkedToKick = "No Player Selected";
                        m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                        m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                        m_lastMarkedPlayerToKickButtonIndex = -1;
                    }
                    m_lastMarkedPlayerToKickIsFromTeam = enumTeams.NO_TEAM;
                }
                else // Select new target and update button visualisation
                {   // Assign the target
                    m_playerMarkedToKick = GM.redTeam.playersNameArray[buttonIndex];
                    // We check in both cases if m_lastMarkedPlayerToKickButtonIndex is higher than 0, it might be - 1 if noone was targeted yet.
                    // If the button belong to red team, set the button color to red
                    if (m_lastMarkedPlayerToKickButtonIndex > -1)
                    {
                        m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                        m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                    }
                    m_lastMarkedPlayerToKickButtonIndex = buttonIndex;
                    m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 255, 0, 170);
                }
                break;

            // If the player belong to blue team, get him and set his name to variable m_playerMarkedToKick, or if he was marked before, unmark him
            case enumTeams.BLUE_TEAM:
                // If selected target is same as last selected targed or its mastterClient, unselect this target
                if (GM.blueTeam.playersNameArray[buttonIndex] == m_playerMarkedToKick || GM.blueTeam.playersNameArray[buttonIndex] == PhotonNetwork.player.NickName)
                {
                    // Unmark the variable, and set it to -1, invalid index plus update button visualisation
                    if (m_lastMarkedPlayerToKickButtonIndex > -1)
                    {
                        m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                        m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                        m_playerMarkedToKick = "No Player Selected";
                        m_lastMarkedPlayerToKickIsFromTeam = enumTeams.NO_TEAM;
                    }
                    m_lastMarkedPlayerToKickButtonIndex = -1;
                }
                else // Select new target and update button visualisation
                {   // Assign the target
                    m_playerMarkedToKick = GM.blueTeam.playersNameArray[buttonIndex];
                    // We check in both cases if m_lastMarkedPlayerToKickButtonIndex is higher than 0, it might be - 1 if noone was targeted yet.
                    // If the button belong to blue team, set the button color to blue
                    if (m_lastMarkedPlayerToKickButtonIndex > -1)
                    {
                        m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                        m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                    }
                    m_lastMarkedPlayerToKickButtonIndex = buttonIndex;
                    m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 255, 0, 170);
                }
                break;
        }
    }

    // Functions called when masterClient player click on button kick player
    public void KickPlayer()
    {
        photonView.RPC("IsMasterKickingMe", PhotonTargets.Others, m_playerMarkedToKick, m_lastMarkedPlayerToKickButtonIndex, (int)m_lastMarkedPlayerToKickIsFromTeam);
        UpdateAfterKickingPlayer();
    }

    // MasterClient kicked player, check who has been kicked
    // playerToBeKicked store player name
    // buttonIndex store the index of button which belong to player which is going to be kicked
    // teamColor help determine in which team player was, 0 is red, 1 is blue
    [PunRPC] void IsMasterKickingMe(string playerToBeKicked, int buttonIndex, int teamColor)
    {
        // Check if masterClient wants to kick me, if not update teams and get out from the function, if yes close connection with this room
        if (PhotonNetwork.player.NickName != playerToBeKicked)
        {
            UpdateAfterKickingPlayer(playerToBeKicked, teamColor);
            return;
        }

        Debug.Log("Got kicked !" + playerToBeKicked);
        photonView.RPC("SendMessagePlayerHasBeenKicked", PhotonTargets.All, PhotonNetwork.player.NickName);
        PhotonNetwork.LeaveRoom();
        UI_Manager.EnablePlayerKickedWidget();
        UI_Manager.EnableRoomSection();
        this.gameObject.SetActive(false);
    }


    // Called inside kick player function, reset button variable to default and update teams. This functions is always called only in masterClient
    private void UpdateAfterKickingPlayer()
    {
        switch (m_lastMarkedPlayerToKickIsFromTeam)
        {
            case enumTeams.RED_TEAM: // If the button belong to red team, set the button color to red
                GM.redTeam.LeaveTeam(m_playerMarkedToKick);
                m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                break;
            case enumTeams.BLUE_TEAM: // Button belongs to blue team, set the color to blue
                GM.blueTeam.LeaveTeam(m_playerMarkedToKick);
                m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                break;
        }

        // Unmark the variable, and set it to -1, invalid index
        m_playerMarkedToKick = "No Player Selected";
        m_lastMarkedPlayerToKickButtonIndex = -1;
        m_lastMarkedPlayerToKickIsFromTeam = enumTeams.NO_TEAM;
    }

    // Message recieved from leaving player or called by master, containt player name and team color ID.
    // If he was part of red team, ID = 0, if blue team ID = 1
    [PunRPC] void PlayerLeft(string tempPlayerLeftName, int tempTeamColor)
    {
        switch (tempTeamColor)
        {
            case (int)enumTeams.RED_TEAM: // If the button belong to red team, set the button color to red, and ged rid of player from team
                m_redTeamPlayersButtonsImages[GM.redTeam.LeaveTeam(tempPlayerLeftName)].color = new Color32(255, 158, 158, 121);
                break;
            case (int)enumTeams.BLUE_TEAM: // If the button belong to blue team, set the button color to blue, and ged rid of player from team
                m_blueTeamPlayersButtonsImages[GM.blueTeam.LeaveTeam(tempPlayerLeftName)].color = new Color32(176, 160, 255, 121);
                break;
        }
    }

    // Start the game
    [PunRPC] void StartTheGame()
    {
        NetworkManager tempNW = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        GameManager GMR = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject LCMR = GameObject.Find("LobbyCamera");
        GMR.LockHideCursor();
        GMR.roundStarted = true;
        if (LCMR != null) LCMR.SetActive(false);
        tempNW.SetupAndSpawnCharacter();
        Debug.Log("GOING TO DISABLE MAIN MENU");
        UI_Manager.DisableMainMenu();
    }

    // MasterClient is sending message to all players in the room except him self
    [PunRPC] void MasterIsSendingTeamInformations(string[] tempRedTeamStringArray, string[] tempBlueTeamStringArray, string masterClientName)
    {
        // Keep track of master client NickName
        lastAssignedMasterClient = masterClientName;

        // Update teams with new data, and master client name
        GM.redTeam.UpdateTeam(tempRedTeamStringArray, masterClientName);
        GM.blueTeam.UpdateTeam(tempBlueTeamStringArray, masterClientName);
    }

    // New player in room is asking masterClient for team
    [PunRPC] void AssignMeTeam(string playerAsking)
    {
        HasNewPlayerJoined = true;
        m_playerRequested = playerAsking;
    }

    // Message recieved from new masterClient, containt his name and team color ID, depends on observed information, rename his name in lobby
    // If he was part of red team, ID = 0, if blue team ID = 1
    [PunRPC] void OnNewMaster(string newMasterName, int tempTeamColor)
    {
        switch (tempTeamColor)
        {
            case (int)enumTeams.RED_TEAM: // If the button belong to red team, set the button color to red
                GM.redTeam.UpdateMaster(newMasterName);
                break;
            case (int)enumTeams.BLUE_TEAM: // Button belongs to blue team, set the color to blue
                GM.blueTeam.UpdateMaster(newMasterName);
                break;
        }
    }

    // Called on other clients when master send RPC
    private void UpdateAfterKickingPlayer(string tempPlayerMarkedToKick, int tempTeamColor)
    {
        switch (tempTeamColor)
        {
            case (int)enumTeams.RED_TEAM: // If the button belong to red team, set the button color to red
                GM.redTeam.LeaveTeam(tempPlayerMarkedToKick);
                m_redTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(255, 158, 158, 121);
                break;
            case (int)enumTeams.BLUE_TEAM: // Button belongs to blue team, set the color to blue
                GM.blueTeam.LeaveTeam(tempPlayerMarkedToKick);
                m_blueTeamPlayersButtonsImages[m_lastMarkedPlayerToKickButtonIndex].color = new Color32(176, 160, 255, 121);
                break;
        }

        // Unmark the variable, and set it to -1, invalid index
        m_playerMarkedToKick = "No Player Selected";
        m_lastMarkedPlayerToKickButtonIndex = -1;
        m_lastMarkedPlayerToKickIsFromTeam = enumTeams.NO_TEAM;
    }

    // Find new masterClient, and assign him his rights
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        // If I am master client, enable me masterClient functionalities
        if (PhotonNetwork.isMasterClient)
        {
            // Activate buttons "Start Game" and "Kick Player"
            m_BTN_StartGame.enabled = true;
            m_BTN_KickPlayer.enabled = true;
            BTN_IMG_StartRoom.color = new Color32(255, 255, 255, 255);
            BTN_IMG_KickPlayer.color = new Color32(255, 255, 255, 255);

            // If the new master is part of red team, update his name in lobby, send RPC to other players to update his name
            if (GM.redTeam.IsPlayerInTeam(PhotonNetwork.player.NickName))
            {
                GM.redTeam.UpdateMaster(PhotonNetwork.player.NickName);
                photonView.RPC("OnNewMaster", PhotonTargets.Others, PhotonNetwork.player.NickName, (int)enumTeams.RED_TEAM);

            } // else he is part of blue team, update his name in lobby, send zz to other players to update his name
            else
            {
                GM.blueTeam.UpdateMaster(PhotonNetwork.player.NickName);
                photonView.RPC("OnNewMaster", PhotonTargets.All, PhotonNetwork.player.NickName, (int)enumTeams.BLUE_TEAM);
            }
        }
    }

    [PunRPC] void SendTeamToGameManager(string name, string team)
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (GM.username == name)
            if (team == "Red")
                GM.isRedTeam = true;
            else GM.isBlueTeam = true;
    }
}