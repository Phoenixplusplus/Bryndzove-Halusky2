using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogIn : MonoBehaviour {

    private InputField m_NickName;

    void Start()
    {
        m_NickName = this.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PhotonNetwork.player.NickName = m_NickName.text;
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            this.gameObject.transform.GetChild(4).gameObject.SetActive(true);

            enabled = false;
        }
    }
}
