using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

class Message
{
    public string text;
    public Text textObject;
}

public class Chat : Photon.MonoBehaviour
{
    const int MAXIMUM_COUNT_OF_MESSAGES = 50;
    private List<Message> messagesList = new List<Message>();
    private GameObject m_content;
    private InputField m_inputBox;
    private GameObject m_textObject;




    // Constructor
    public Chat(InputField newInputBox, GameObject newContent)
    {
        // Assign chat input box
        m_inputBox = newInputBox;
        // Assign chat content
        m_content = newContent;
        // Load text prefab from memory
        m_textObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Chat_Text.prefab", typeof(GameObject));

        // Print out chat has been created - Maybe Delete this
        Debug.Log("Chat has been created.");
    }

    public void UpdateChat()
    {
        if (m_inputBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage(PhotonNetwork.player.NickName + ": " + m_inputBox.text);
                m_inputBox.text = "";
                m_inputBox.ActivateInputField();
            }

        }
    }

    private void SendMessage(string newText)
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

        messagesList.Add(newMessage);
    }
}
