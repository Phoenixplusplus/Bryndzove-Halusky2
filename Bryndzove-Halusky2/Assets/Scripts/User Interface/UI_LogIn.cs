using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogIn : MonoBehaviour
{

    private InputField m_NickName;
    private InputField m_PassName;
    private DatabaseManager Database;
    private UserInterfaceManager m_UI_manager;

    // variables that need DB replies
    [Header("Database Reply Variables")]
    public Text loginReply;

    void Start()
    {
        // Find nickname and password input field
        m_NickName = this.gameObject.transform.GetChild(0).GetComponent<InputField>();
        m_PassName = this.gameObject.transform.GetChild(1).GetComponent<InputField>();
        // Find Database reference
        Database = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        // Find UI manager reference
        m_UI_manager = GameObject.Find("UIManager").GetComponent<UserInterfaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Database.loginBool == true) { LoginFinished(); }
        else { loginReply.text = Database.loginReply; }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && gameObject.transform.GetChild(1).gameObject.activeInHierarchy == true)
        {
            Database.Login(m_NickName.text, m_PassName.text);
        }

        // If player is typing down his login or password and press tab, change focus to other input field
        if (m_NickName.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            m_NickName.DeactivateInputField();
            m_PassName.ActivateInputField();
        }
        else if (m_PassName.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            m_NickName.ActivateInputField();
            m_PassName.DeactivateInputField();
        }
    }

    // Clear input fields
    public void ClearInputFields()
    {
        m_NickName.text = "";
        m_PassName.text = "";
    }

    private void LoginFinished()
    {
        loginReply.text = Database.createAccountReply;
        // Assign player name from database data
        PhotonNetwork.player.NickName = m_NickName.text;

        // Enable/Disable UI
        m_UI_manager.DisableLoginMenu();
        m_UI_manager.EnableRoomSection();
        m_UI_manager.EnableLobbyButtons();
    }
}
