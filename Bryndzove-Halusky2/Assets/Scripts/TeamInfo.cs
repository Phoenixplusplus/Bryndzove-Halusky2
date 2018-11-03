using UnityEngine;
using UnityEngine.UI;

public class TeamInfo
{
    // Private variables
    //TODO: DO I NEEED HITS VARIABLE? MAYBE I WILL DELETE IT  AT THE MOMENT I DONT REALLY USE IT OUT OF CLASS      
    //TODO: DO I NEEED HITS VARIABLE? MAYBE I WILL DELETE IT  AT THE MOMENT I DONT REALLY USE IT OUT OF CLASS      
    //TODO: DO I NEEED HITS VARIABLE? MAYBE I WILL DELETE IT  AT THE MOMENT I DONT REALLY USE IT OUT OF CLASS      
    //TODO: DO I NEEED HITS VARIABLE? MAYBE I WILL DELETE IT  AT THE MOMENT I DONT REALLY USE IT OUT OF CLASS      
    public bool[] IsSlotEmptyArray;


    private Button[] buttonsArray;
    public string[] playersNameArray;
    private int maxPlayersInTeamCount;
    private int curPlayerTeamCount;

    // Getter functions
    // Ask if the slot is empty or not
    public bool GetSlotStatus(int slotIndex)
    {
        if (slotIndex < maxPlayersInTeamCount) return IsSlotEmptyArray[slotIndex];
        else { Debug.Log("Index out of range, return false"); return false; }
    }

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
        }
        else if (blueTeam)
        {
            GameObject GO = GameObject.Find("IMG_BlueTeamBackground");
            buttonsArray[0] = GO.transform.GetChild(0).GetComponent<Button>();
            buttonsArray[1] = GO.transform.GetChild(1).GetComponent<Button>();
            buttonsArray[2] = GO.transform.GetChild(2).GetComponent<Button>();
            buttonsArray[3] = GO.transform.GetChild(3).GetComponent<Button>();
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

    public string GetPlayerByName(string requestedPlayer)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (requestedPlayer == playersNameArray[i]) return playersNameArray[i];
        }

        Debug.Log("ERROR: Function GetPlayerByName could not find the requested player! Text Empty Slot was returned instead.");
        return "Empty Slot";
    }

    public string GetPlayerNameByIndex(int index)
    {
        if (index < maxPlayersInTeamCount)
        return playersNameArray[index];

        Debug.Log("ERROR: Function GetPlayerNameByIndex could not find the player, seems like index is out of range!");
        return null;
    }

    public int GetCurPlayersTeamCount()
    {
        return curPlayerTeamCount;
    }

    public int GetMaxPlayersTeamCount()
    {
        return maxPlayersInTeamCount;
    }


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
    public void UpdateTeam(string masterClientName)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (playersNameArray[i] != "Empty Slot")
            {
                if (playersNameArray[i] == masterClientName) buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i] + " (Master)";
                else buttonsArray[i].GetComponentInChildren<Text>().text = playersNameArray[i];
            }
            else buttonsArray[i].GetComponentInChildren<Text>().text = "";
        }
    }

    public void LeaveTeam(string playerNickName)
    {
        for (int i = 0; i < maxPlayersInTeamCount; i++)
        {
            if (playersNameArray[i] != null && playersNameArray[i] == playerNickName)
            {
                IsSlotEmptyArray[i] = true;
                buttonsArray[i].enabled = false;
                playersNameArray[i] = null;
                curPlayerTeamCount--;
                return;
            }

        }
        // Can not leave team, player is not part of the team
        Debug.Log("Can not leave team, player is not part of the team");
    }
}