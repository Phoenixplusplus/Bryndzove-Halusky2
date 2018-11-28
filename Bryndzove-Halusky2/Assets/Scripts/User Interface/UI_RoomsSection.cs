using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomsSection : MonoBehaviour
{
    private UI_RoomButton[] m_roomButtonsArray;
    private NetworkManager m_networkManager;
    private UserInterfaceManager m_UI_manager;
    private int m_countOfRoomButtons;

    // Use this for initialization
    void Start()
    {
        m_networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        m_UI_manager = GameObject.Find("UIManager").GetComponent<UserInterfaceManager>();
        InitializeRoomButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_networkManager.roomsList != null)
        {
            // Update rooms button information
            UpdateRooms();
        }
    }

    public void UpdateRooms()
    {
        // Assign room information to buttons ( Room ID, Room Status if is open or not, Room Name, What map is it, but it is not implemented,
        // number of players in room and maximum possible count of players in room
        for (int i = 0; i < m_networkManager.roomsList.Length; i++)
        {
            m_roomButtonsArray[i].SetRoomDetails(i, m_networkManager.roomsList[i].IsOpen, m_networkManager.roomsList[i].Name, "Map",
                                                m_networkManager.roomsList[i].PlayerCount, m_networkManager.roomsList[i].MaxPlayers);
        }

        // We have 16 rooms, if there are for example 5 rooms created, rest 11 room button set to default, reset all button variables.
        if (m_networkManager.roomsList.Length < m_countOfRoomButtons)
        {
            for (int i = m_networkManager.roomsList.Length; i < m_countOfRoomButtons; i++)
            {
                m_roomButtonsArray[i].ResetButton();
            }
        }
    }

    // This function is called when player click on room button
    public void JoinRoom(int roomNumber)
    {
        // Check if is room open
        if (m_networkManager.roomsList[roomNumber].IsOpen)
        {
            // If there is space for me, join me
            if (m_networkManager.roomsList[roomNumber].PlayerCount < m_networkManager.roomsList[roomNumber].MaxPlayers)
            {
                PhotonNetwork.JoinRoom(m_networkManager.roomsList[roomNumber].Name);
                m_UI_manager.DisableRoomSection();
                m_UI_manager.EnableRoomLobby();
            }
            // Do not join me and show UI the room is full
            else // TODO: Implement UI room is full
            {
                Debug.Log("Rooms " + m_networkManager.roomsList[roomNumber].Name + " is full.");
            }
        }
    }

    // Initialize all room buttons
    private void InitializeRoomButtons()
    {
        // Get all button which containt UI_RoomButton script and put them into array
        m_roomButtonsArray = FindObjectsOfType(typeof(UI_RoomButton)) as UI_RoomButton[];
        m_countOfRoomButtons = m_roomButtonsArray.Length;

        // Create temporary UI_RoomButton object and swap the buttons positions in array
        UI_RoomButton tempButton;
        for (int i = 0; i < m_countOfRoomButtons; i++)
        {
            for (int k = 0; k < m_countOfRoomButtons - 1; k++)
            {
                if (m_roomButtonsArray[k].ID > m_roomButtonsArray[k + 1].ID)
                {
                    tempButton = m_roomButtonsArray[k];
                    m_roomButtonsArray[k] = m_roomButtonsArray[k + 1];
                    m_roomButtonsArray[k + 1] = tempButton;
                }
            }
        }

        // Disable all buttons
        for (int i = 0; i < m_countOfRoomButtons; i++)   
            m_roomButtonsArray[i].ResetButton();     
    }
}
