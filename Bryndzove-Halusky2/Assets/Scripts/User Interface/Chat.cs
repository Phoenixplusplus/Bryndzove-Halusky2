using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
    [DllImport("BHChat", EntryPoint = "GetJokesCount")] public static extern int GetJokesCount();
    [DllImport("BHChat", EntryPoint = "GetJokeByID")] public static extern System.IntPtr GetJokeByID(int jokeID);
    [DllImport("BHChat", EntryPoint = "GetRandomJoke")] public static extern System.IntPtr GetRandomJoke();
    [DllImport("BHChat", EntryPoint = "GetErrorMessage")] public static extern System.IntPtr GetErrorMessage();
    [DllImport("BHChat", EntryPoint = "ErrorCheck")] public static extern bool BHChat_DLL_ErrorCheck();
    [DllImport("BHChat", EntryPoint = "GetTextColor")] public static extern Vector3Int GetTextColor(int messageType);

    const int MAXIMUM_COUNT_OF_MESSAGES = 50;
    private List<Message> messagesList = new List<Message>();
    private Color m_colorJoined;
    private Color m_colorLeft;
    private Color m_colorKicked;
    private Color m_colorText;

    public GameObject m_content;
    public InputField m_inputBox;
    public GameObject m_textObject;

    void Start()
    {
        // Assign colors from BHChat.DLL
        // Assign player input text color from BHChat.DLL
        Vector3Int colorVector = GetTextColor((int)MessageType.PLAYER_INPUT);
        m_colorText = new Color(colorVector.x, colorVector.y, colorVector.z);
        // Assign color of player joined message from BHChat.DLL
        colorVector = GetTextColor((int)MessageType.PLAYER_JOINED);
        m_colorJoined = new Color(colorVector.x, colorVector.y, colorVector.z);
        // Assign color of player left message from BHChat.DLL
        colorVector = GetTextColor((int)MessageType.PLAYER_LEFT);
        m_colorLeft = new Color(colorVector.x, colorVector.y, colorVector.z);
        // Assign color of player has been kicked message from BHChat.DLL
        colorVector = GetTextColor((int)MessageType.PLAYER_KICKED);
        m_colorKicked = new Color(colorVector.x, colorVector.y, colorVector.z);
        // Limit maximal length of message to 255 characters
        m_inputBox.characterLimit = 255;
    }

    // Check if joke string containt only digits
    bool IsStringJoke(string str)
    {
        // If the message is not joke type, return true
        if (str.Substring(0, 1) == "/") str = str.Substring(1);
        else return false;

        foreach (char c in str)
        {
            if (c < '0' || c > '9') return false;
        }

        return true;
    }

    // Check if is player sending nex message
    void Update()
    {
        // Check if has player typed anything and pressed enter
        if (m_inputBox.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            // If typed text is joke macro for example "/1", it will send message containing joke in first position of jokes array from DLL.Library
            // Check if text containt only digits and once "/"
            if (IsStringJoke(m_inputBox.text))
            {
                // Remove "/" from string
                m_inputBox.text = m_inputBox.text.Substring(1);
                // Check if the numbers is not higher than count of jokes, if is not, send a joke
                if (int.Parse(m_inputBox.text) < GetJokesCount())
                {
                    m_inputBox.text = Marshal.PtrToStringAnsi(GetJokeByID(int.Parse(m_inputBox.text)));
                    SendMessage(PhotonNetwork.player.NickName + " said joke: " + m_inputBox.text, MessageType.PLAYER_INPUT);
                    photonView.RPC("SendMessagePlayerMessage", PhotonTargets.Others, PhotonNetwork.player.NickName + " said joke: " + m_inputBox.text);
                }
                // Send standard message to other players - standard message is what has player typed
                else
                {
                    m_inputBox.text = "/" + m_inputBox.text;
                    SendMessage(PhotonNetwork.player.NickName + ": " + m_inputBox.text, MessageType.PLAYER_INPUT);
                    photonView.RPC("SendMessagePlayerMessage", PhotonTargets.Others, PhotonNetwork.player.NickName + ": " + m_inputBox.text);
                }
            }
            // If typed text is joke macro "/rj", it will send message containing random joke from DLL.Library
            else if (m_inputBox.text == "/rj")
            {
                m_inputBox.text = Marshal.PtrToStringAnsi(GetRandomJoke());
                SendMessage(PhotonNetwork.player.NickName + " said joke: " + m_inputBox.text, MessageType.PLAYER_INPUT);
                photonView.RPC("SendMessagePlayerMessage", PhotonTargets.Others, PhotonNetwork.player.NickName + " said joke: " + m_inputBox.text);
            }
            // Send standard message to other players - standard message is what has player typed
            else
            {
                SendMessage(PhotonNetwork.player.NickName + ": " + m_inputBox.text, MessageType.PLAYER_INPUT);
                photonView.RPC("SendMessagePlayerMessage", PhotonTargets.Others, PhotonNetwork.player.NickName + ": " + m_inputBox.text);
            }

            // Activate input field again
            m_inputBox.ActivateInputField();
            m_inputBox.text = "";
        }
    }

    // Called when player join room
    void OnJoinedRoom()
    {
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
    public void ResetChat()
    {
        // Check if the chat has not cross the maximum count of 
        for (int i = 0; i < messagesList.Count; i++)
        {
            Destroy(messagesList[i].textObject.gameObject);
        }

        messagesList.Clear();
    }

    // RPC messages
    // Send new message into messageList
    [PunRPC] void SendMessagePlayerLeft(string playerName)              { SendMessage("Player " + playerName + " has left.", MessageType.PLAYER_LEFT); }
    [PunRPC] void SendMessagePlayerMessage(string message)              { SendMessage(message, MessageType.PLAYER_INPUT); }
    [PunRPC] void SendMessagePlayerHasBeenKicked(string playerName)     { SendMessage("Player " + playerName + " has been kicked.", MessageType.PLAYER_KICKED); }
    [PunRPC] void SendMessagePlayerJoined(string playerName)            { SendMessage("Player " + playerName + " has joined.", MessageType.PLAYER_JOINED); }
}
