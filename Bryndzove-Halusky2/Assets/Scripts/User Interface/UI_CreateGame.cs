﻿using UnityEngine;
using UnityEngine.UI;

public class UI_CreateGame : MonoBehaviour
{
    private NetworkManager m_networkManager;
    private byte m_playersCount;
    private int m_mapID;
    private InputField m_roomName;
   // private InputField m_roomPasword;
    private Image BTN_IMG_Public;
    private Image BTN_IMG_Private;
    private Image BTN_IMG_1vs1;
    private Image BTN_IMG_2vs2;
    private Image BTN_IMG_3vs3;
    private Image BTN_IMG_4vs4;
    private Color32 m_colorSelected = new Color32(155, 232, 255, 255);
    private Color32 m_colorNotSelected = new Color32(255, 255, 255, 255);

    void Start()
    {
        //Text sets your text to say this message
        m_networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        // Initialize a text component
        m_roomName = this.gameObject.transform.GetChild(2).GetComponent<InputField>();
        //m_roomPasword = this.gameObject.transform.GetChild(3).GetComponent<InputField>();

        // Get buttons images
        BTN_IMG_1vs1 = this.gameObject.transform.GetChild(4).GetComponent<Image>();
        BTN_IMG_2vs2 = this.gameObject.transform.GetChild(5).GetComponent<Image>();
        BTN_IMG_3vs3 = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_4vs4 = this.gameObject.transform.GetChild(7).GetComponent<Image>();
        BTN_IMG_Public = this.gameObject.transform.GetChild(8).GetComponent<Image>();
        BTN_IMG_Private = this.gameObject.transform.GetChild(9).GetComponent<Image>();

        // Set room details to default
        SetToDefault();
    }

    public void CreateNewRoom()
    {
        if (m_roomName.text != "")
        {
            PhotonNetwork.CreateRoom(m_roomName.text, new RoomOptions() { MaxPlayers = m_playersCount, IsOpen = true, IsVisible = true }, m_networkManager.GetLobbyName());
        }
        else
        {
            PhotonNetwork.CreateRoom("Phoenix's Room", new RoomOptions() { MaxPlayers = m_playersCount, IsOpen = true, IsVisible = true }, m_networkManager.GetLobbyName());
        }
    }

    public void SetMaximumPlayerCountTwo()
    {
        m_playersCount = 2;
        BTN_IMG_1vs1.color = m_colorSelected;
        BTN_IMG_2vs2.color = m_colorNotSelected;
        BTN_IMG_3vs3.color = m_colorNotSelected;
        BTN_IMG_4vs4.color = m_colorNotSelected;
    }
    public void SetMaximumPlayerCountFour()
    {
        m_playersCount = 4;
        BTN_IMG_1vs1.color = m_colorNotSelected;
        BTN_IMG_2vs2.color = m_colorSelected;
        BTN_IMG_3vs3.color = m_colorNotSelected;
        BTN_IMG_4vs4.color = m_colorNotSelected;
    }

    public void SetMaximumPlayerCountSix()
    {
        m_playersCount = 6;
        BTN_IMG_1vs1.color = m_colorNotSelected;
        BTN_IMG_2vs2.color = m_colorNotSelected;
        BTN_IMG_3vs3.color = m_colorSelected;
        BTN_IMG_4vs4.color = m_colorNotSelected;
    }

    public void SetMaximumPlayerCountEight()
    {
        m_playersCount = 8;
        BTN_IMG_1vs1.color = m_colorNotSelected;
        BTN_IMG_2vs2.color = m_colorNotSelected;
        BTN_IMG_3vs3.color = m_colorNotSelected;
        BTN_IMG_4vs4.color = m_colorSelected;
    }

    public void SetToDefault()
    {
        // Set player count
        m_playersCount = 4;

        // Reset the room name
        if (m_roomName) m_roomName.text = PhotonNetwork.player.NickName + "'s room";

        // Set the buttons colors
        if (BTN_IMG_1vs1) BTN_IMG_1vs1.color = m_colorNotSelected;
        if (BTN_IMG_2vs2) BTN_IMG_2vs2.color = m_colorSelected;
        if (BTN_IMG_3vs3) BTN_IMG_3vs3.color = m_colorNotSelected;
        if (BTN_IMG_4vs4) BTN_IMG_4vs4.color = m_colorNotSelected;
    }

    public void SetRoomToPublic()
    {
        BTN_IMG_Public.color = m_colorSelected;
        BTN_IMG_Private.color = m_colorNotSelected;
    }

    public void SetRoomToPrivate()
    {
        BTN_IMG_Public.color = m_colorNotSelected;
        BTN_IMG_Private.color = m_colorSelected;
    }
}

