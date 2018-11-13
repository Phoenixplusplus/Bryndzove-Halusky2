using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomsSection : MonoBehaviour
{
    private UI_RoomButton[] m_roomButtonsArray;
    private int m_countOfRoomButtons;
    private NetworkManager m_networkManager;
    private UserInterfaceManager m_UI_manager;

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
        for (int i = 0; i < m_networkManager.roomsList.Length; i++)
        {
            m_roomButtonsArray[i].SetRoomDetails(i, "Status", m_networkManager.roomsList[i].Name, "Map", m_networkManager.roomsList[i].PlayerCount, m_networkManager.roomsList[i].MaxPlayers);
        }

        if (m_networkManager.roomsList.Length < m_countOfRoomButtons)
        {
            for (int i = m_networkManager.roomsList.Length; i < m_countOfRoomButtons; i++)
            {
                m_roomButtonsArray[i].ResetButton();
            }
        }
    }

    public void JoinRoom(int roomNumber)
    {
        if (m_networkManager.roomsList[roomNumber].PlayerCount < m_networkManager.roomsList[roomNumber].MaxPlayers)
        {
            PhotonNetwork.JoinRoom(m_networkManager.roomsList[roomNumber].Name);
            m_UI_manager.DisableRoomSection();
            m_UI_manager.EnableRoomLobby();
        }
        else // Show UI the room is full
        {
            Debug.Log("Rooms " + m_networkManager.roomsList[roomNumber].Name + " is full.");
        }
    }

    // Initialize all room buttons
    private void InitializeRoomButtons()
    {
        // Get all button which containt UI_RoomButton script and put them into array
        m_roomButtonsArray = FindObjectsOfType(typeof(UI_RoomButton)) as UI_RoomButton[];
        m_countOfRoomButtons = m_roomButtonsArray.Length;

        // Swap the buttons positions in array
        // Create temporary UI_RoomButton object
        UI_RoomButton a;
        for (int i = 0; i < m_countOfRoomButtons; i++)
        {
            for (int k = 0; k < m_countOfRoomButtons - 1; k++)
            {
                if (m_roomButtonsArray[k].ID > m_roomButtonsArray[k + 1].ID)
                {
                    a = m_roomButtonsArray[k];
                    m_roomButtonsArray[k] = m_roomButtonsArray[k + 1];
                    m_roomButtonsArray[k + 1] = a;
                }
            }
        }

        // Disable all buttons
        for (int i = 0; i < m_countOfRoomButtons; i++)
        {
            m_roomButtonsArray[i].ResetButton();
        }
    }
}
