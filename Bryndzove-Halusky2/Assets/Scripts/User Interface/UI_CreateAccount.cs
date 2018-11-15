using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateAccount : MonoBehaviour
{
    private InputField m_NickName;
    private InputField m_PassName;
    public GameObject DBM;
    private DatabaseManager Database;
    public GameObject LoginMenu;

    // variables that need DB replies
    [Header("Database Reply Variables")]
    public Text createAccountReply;

    void Start()
    {
        m_NickName = this.gameObject.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<InputField>();
        m_PassName = this.gameObject.transform.GetChild(3).gameObject.transform.GetChild(1).GetComponent<InputField>();
        Database = DBM.GetComponent<DatabaseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Database.createAccountBool == true) { CreateAccountFinished(); }
        else { createAccountReply.text = Database.createAccountReply; }

        if (Input.GetKeyDown(KeyCode.Return) && gameObject.transform.GetChild(3).gameObject.activeInHierarchy == true)
        {
            Database.CreateAccount(m_NickName.text, m_PassName.text);
        }
    }

    // button functions


    private void CreateAccountFinished()
    {
        createAccountReply.text = Database.createAccountReply;

        this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
        LoginMenu.SetActive(true);
    }
}
