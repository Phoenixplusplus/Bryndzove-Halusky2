﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomsSection : NetworkManager
{
    private UI_RoomButton[] m_roomButtonsArray;
    private int m_countOfRoomButtons;

    // Use this for initialization
    void Start()
    {
        InitializeRoomButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomsList != null)
        {
            // Update rooms button information
            UpdateRooms();
        }
    }

    public void UpdateRooms()
    {
        for (int i = 0; i < roomsList.Length; i++)
        {
            m_roomButtonsArray[i].SetRoomDetails(i, "Status", roomsList[i].Name, "Map", roomsList[i].PlayerCount, roomsList[i].MaxPlayers);
        }

        if (roomsList.Length < m_countOfRoomButtons)
        {
            for (int i = roomsList.Length; i < m_countOfRoomButtons; i++)
            {
                m_roomButtonsArray[i].ResetButton();
            }
        }
    }

    public void JoinRoom(int roomNumber)
    {
        if (roomsList[roomNumber].PlayerCount < roomsList[roomNumber].MaxPlayers)
        {
            PhotonNetwork.JoinRoom(roomsList[roomNumber].Name);
        }
        else // Show UI the room is full
        {
            Debug.Log("Rooms " + roomsList[roomNumber].Name + " is full.");
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