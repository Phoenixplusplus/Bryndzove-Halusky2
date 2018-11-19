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
    PLAYER_LEFT,
    PLAYER_KICKED
}

public class Chat : Photon.MonoBehaviour
{
    const int MAXIMUM_COUNT_OF_MESSAGES = 50;
    private List<Message> messagesList = new List<Message>();
    private Color m_colorJoined = new Color(0, 255, 0);
    private Color m_colorLeft = new Color(255, 0, 0);
    private Color m_colorKicked = new Color(255, 128, 0);
    private Color m_colorText = new Color(255, 255, 255);

    public GameObject m_content;
    public InputField m_inputBox;
    public GameObject m_textObject;

    // Check if is player sending nex message
    public void Update()
    {
        if (m_inputBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage(PhotonNetwork.player.NickName + ": " + m_inputBox.text, MessageType.PLAYER_INPUT);
                photonView.RPC("SendMessagePlayerMessage", PhotonTargets.Others, PhotonNetwork.player.NickName + ": " + m_inputBox.text);
                
                m_inputBox.text = "";
                m_inputBox.ActivateInputField();
            }
        }
    }

    // Called when player join room
    void OnJoinedRoom()
    {
        // Clear chat if it cointain any messages
        if (messagesList.Count > 0) ResetChat();

        // Send message to the chat
        if (PhotonNetwork.isMasterClient) SendMessage("Room has been created", MessageType.PLAYER_JOINED);
        else photonView.RPC("SendMessagePlayerJoined", PhotonTargets.All, PhotonNetwork.player.NickName);
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

        // Instantiate new text object and push him into array
        GameObject newTextObject = Instantiate(m_textObject, m_content.transform);
        newMessage.textObject = newTextObject.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;

        // Assign color to text, depends on message type
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

            case MessageType.PLAYER_KICKED:
                newMessage.textObject.color = m_colorKicked;
                break;
        }

        messagesList.Add(newMessage);
    }

    // Reset the chat
    private void ResetChat()
    {
        // Check if the chat has not cross the maximum count of messages
        for (int i = 0; i < messagesList.Count; i++)
        {
            Destroy(messagesList[i].textObject.gameObject);
            messagesList.Remove(messagesList[i]);
        }
    }

    // RPC messages
    // Send new message into messageList
    [PunRPC] void SendMessagePlayerLeft(string playerName)              { SendMessage("Player " + playerName + " has left.", MessageType.PLAYER_LEFT); }
    [PunRPC] void SendMessagePlayerMessage(string message)              { SendMessage(message, MessageType.PLAYER_INPUT); }
    [PunRPC] void SendMessagePlayerHasBeenKicked(string playerName)     { SendMessage("Player " + playerName + " has been kicked.", MessageType.PLAYER_KICKED); }
    [PunRPC] void SendMessagePlayerJoined(string playerName)            { SendMessage("Player " + playerName + " has joined.", MessageType.PLAYER_JOINED); }
}
