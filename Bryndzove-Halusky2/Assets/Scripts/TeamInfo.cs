using UnityEngine;
using UnityEngine.UI;

public class TeamInfo
{
    // Private variables 
    private bool[] IsSlotEmptyArray;
    private Button[] buttonsArray;
    private int maxPlayersInTeamCount;
    private int curPlayerTeamCount;
    private int teamColor;

    // Public variables
    public string[] playersNameArray;

    // Constructor, takes number of players in room, if is the info red team or blue team
    public TeamInfo(int numberOfPlayers, bool redTeam, bool blueTeam)
    {
        maxPlayersInTeamCount = numberOfPlayers / 2;
        IsSlotEmptyArray = new bool[maxPlayersInTeamCount];
        buttonsArray = new Button[4];
        playersNameArray = new string[maxPlayersInTeamCount];


        for (int i = 0; i < playersNameArray.Length; i++) playersNameArray[i] = "Empty Slot";


        // Initialize buttons
        if (redTeam)
        {
            GameObject GO = GameObject.Find("IMG_RedTeamBackground");
            buttonsArray[0] = GO.transform.GetChild(0).GetComponent<Button>();
            buttonsArray[1] = GO.transform.GetChild(1).GetComponent<Button>();
            buttonsArray[2] = GO.transform.GetChild(2).GetComponent<Button>();
            buttonsArray[3] = GO.transform.GetChild(3).GetComponent<Button>();
            teamColor = 0;
        }
        else if (blueTeam)
        {
            GameObject GO = GameObject.Find("IMG_BlueTeamBackground");
            buttonsArray[0] = GO.transform.GetChild(0).GetComponent<Button>();
            buttonsArray[1] = GO.transform.GetChild(1).GetComponent<Button>();
            buttonsArray[2] = GO.transform.GetChild(2).GetComponent<Button>();
            buttonsArray[3] = GO.transform.GetChild(3).GetComponent<Button>();
            teamColor = 1;
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




    // Getter functions
    // Ask if the slot is empty or not
    public bool GetSlotStatus(int slotIndex)
    {
        if (slotIndex < maxPlayersInTeamCount) return IsSlotEmptyArray[slotIndex];
        else { Debug.Log("Index is out of range, return false"); return false; }
    }

    // Find player by name and return him
    public string GetPlayerByName(string requestedPlayer)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++) if (requestedPlayer == playersNameArray[i]) return playersNameArray[i];
        Debug.Log("ERROR: Function GetPlayerByName could not find the requested player! Text Empty Slot was returned instead.");
        return "Slot is empty or player is not in this team";
    }

    // Return team color
    public int GetTeamColor()               { return teamColor; }
    // Return players count in team
    public int GetCurPlayersTeamCount()     { return curPlayerTeamCount; }
    // Get maximus size of team
    public int GetMaxPlayersTeamCount()     { return maxPlayersInTeamCount; }




    // Utility functions
    // Check if is player in team
    public bool IsPlayerInTeam(string playerName)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++) { if (playersNameArray[i] == playerName) return true; }
        return false;
    }

    // Join team, request player nickName
    public void JoinTeam(string playerNickName, bool IsMasterClient = false)
    {
        if (curPlayerTeamCount < maxPlayersInTeamCount)
        {
            // Check if player can join team, if is already in the team will not join
            for (int i = 0; i < maxPlayersInTeamCount; i++)
            {
                if (playersNameArray[i] != "" && playerNickName == playersNameArray[i])
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
                    playersNameArray[i] = playerNickName;
                    buttonsArray[i].enabled = true;
                    if (!IsMasterClient) buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i];
                    else buttonsArray[i].GetComponentInChildren<Text>().text = playerNickName + " (Master)";
                    // Increase the players count in the current team
                    curPlayerTeamCount++;
                    return;
                }
            }
        }
        else
        {
            // Can not join team, team is full
            Debug.Log("Can not join team, team is full");
        }
    }

    // Find the masterClient in team, and addjust his namy by tag (Master)
    public void UpdateMaster(string masterClientName)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (playersNameArray[i] == masterClientName)
            {
                buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i] + " (Master)";
            }
        }
    }

    // Update team information, request players name in team, and masteClient name
    public void UpdateTeam(string [] playersNames, string masterClientName)
    {
        // Loop through team slots and assignt the data
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            // Assign the name from input, can be player name or "Empty Slot"
            playersNameArray[i] = playersNames[i];

            // If the player in this slot is master client, assign his name into text component and adjust his name with text (Master), also enable the buttons,
            // in purpose to make the button clickable, and add option to players select other players and kick him out (Only for Master Client), or add him to friend list
            if (playersNameArray[i] == masterClientName)
            {
                buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i] + " (Master)";
                IsSlotEmptyArray[i] = false;
                buttonsArray[i].enabled = true;
            }
            // If the slot is empty, disable buttons, and dont assignt any text into text component
            else if (playersNameArray[i] == "Empty Slot")
            {
                buttonsArray[i].GetComponentInChildren<Text>().text = "";
                IsSlotEmptyArray[i] = true;
                buttonsArray[i].enabled = false;
            }
            // Slot is not empty, assign other players names into text component and unlock buttons
            else
            {
                buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i];
                IsSlotEmptyArray[i] = false;
                buttonsArray[i].enabled = true;
            }
        }
    }

    public int LeaveTeam(string playerNickName)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (playersNameArray[i] != null && playersNameArray[i] == playerNickName)
            {
                IsSlotEmptyArray[i] = true;
                playersNameArray[i] = "";
                buttonsArray[i].enabled = false;
                buttonsArray[i].GetComponentInChildren<Text>().text = "";
                curPlayerTeamCount--;
                return i;
            }
        }
        // Can not leave team, player is not part of the team
        Debug.Log("ERROR: Can not leave team, player is not part of the team");
        return -1;
    }
}