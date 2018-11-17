using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

class Message
{
    public string text;
    public Text textObject;
}

enum MessageType
{
    PLAYER_INPUT,
    PLAYER_JOINED,
    PLAYER_LEFT
}

enum ChatType
{
    LOBBY_ROOM, // Lobby or RoomLobby
    GAMEPLAY    // Inside room but game is running
}

public class Chat : Photon.MonoBehaviour
{
    const int MAXIMUM_COUNT_OF_MESSAGES = 50;
    private List<Message> messagesList = new List<Message>();
    private GameObject m_content;
    private InputField m_inputBox;
    private GameObject m_textObject;
    private Color m_colorJoined = new Color(0, 255, 0);
    private Color m_colorLeft = new Color(255, 0, 0);
    private Color m_colorText = new Color(255, 255, 255);
    private ChatType m_chatType;
    private PhotonView photonView;


    // Constructor
    public Chat(InputField newInputBox, GameObject newContent, PhotonView newPhotonView)
    {
        // Assign chat input box
        m_inputBox = newInputBox;
        // Assign chat content
        m_content = newContent;
        // Load text prefab from memory
        //m_textObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Chat_Text.prefab", typeof(GameObject));
        // Decide what chat type is it
        m_chatType = 0;

        photonView = newPhotonView;
        // Print out chat has been created - Maybe Delete this
        Debug.Log("Chat has been created.");
    }

    public void UpdateChat()
    {
        if (m_inputBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage(PhotonNetwork.player.NickName + ": " + m_inputBox.text, MessageType.PLAYER_INPUT);
                m_inputBox.text = "";
                m_inputBox.ActivateInputField();
            }

        }
    }

    // Called when new player joined room bh joined player
    public void PlayerJoinedRoomSendMessage(string playerName)
    {
        Debug.Log("I have joined roooooom");
        photonView.RPC("PlayerJoinedRoom", PhotonTargets.All, playerName, 5);
        Debug.Log("I have joined roooooom roooooom roooooom roooooom ");
    }

    // Send new message into messageList
    [PunRPC] void PlayerJoinedRoom(string playerName, int lol)
    {
        SendMessage("Player " + playerName + " has joined room.", MessageType.PLAYER_JOINED);

    }

    private void SendMessage(string newText, MessageType messageType)
    {
        // Check if the chat has not cross the maximum count of messages
        if (messagesList.Count >= MAXIMUM_COUNT_OF_MESSAGES)
        {
            Destroy(messagesList[0].textObject.gameObject);
            messagesList.Remove(messagesList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = newText;

        GameObject newTextObject = Instantiate(m_textObject, m_content.transform);
        newMessage.textObject = newTextObject.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;


        // This chat is only inside the room lobby, or lobby
        if (m_chatType == ChatType.LOBBY_ROOM)
        {
            switch (messageType)
            {
                case MessageType.PLAYER_INPUT:
                    newMessage.textObject.color = m_colorText;
                    break;

                case MessageType.PLAYER_JOINED:
                    newMessage.textObject.color = m_colorJoined;
                    break;

                case MessageType.PLAYER_LEFT:
                    newMessage.textObject.color = m_colorLeft;
                    break;
            }
        }
        // This is gameplay chat, inside the room
        else
        {
            switch (messageType)
            {
                case MessageType.PLAYER_INPUT:
                    newMessage.textObject.color = m_colorText;
                    break;

                case MessageType.PLAYER_LEFT:
                    newMessage.textObject.color = m_colorLeft;
                    break;
            }
        }


        messagesList.Add(newMessage);
    }
}
