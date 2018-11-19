using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateAccount : MonoBehaviour
{
    private InputField m_NickName;
    private InputField m_PassName;
    private DatabaseManager Database;
    private UserInterfaceManager m_UI_manager;
    // variables that need DB replies
    [Header("Database Reply Variables")]
    public Text createAccountReply;

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
        if (Database.createAccountBool == true) { CreateAccountFinished(); }
        else { createAccountReply.text = Database.createAccountReply; }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && gameObject.transform.GetChild(3).gameObject.activeInHierarchy == true)
        {
            Database.CreateAccount(m_NickName.text, m_PassName.text);
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

    private void CreateAccountFinished()
    {
        // Create new account
        createAccountReply.text = Database.createAccountReply;

        // Disable create new account widget and show login widget
        m_UI_manager.DisableCreateNewAccountMenu();
        m_UI_manager.EnableLoginMenu();

        // Clear input fields
        m_NickName.text = "";
        m_PassName.text = "";
    }
}
