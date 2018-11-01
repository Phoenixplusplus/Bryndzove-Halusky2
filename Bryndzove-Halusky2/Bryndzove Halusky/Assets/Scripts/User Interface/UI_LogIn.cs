using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LogIn : MonoBehaviour {


	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PhotonNetwork.player.NickName = "klobasa";
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            this.gameObject.transform.GetChild(4).gameObject.SetActive(true);

            enabled = false;
        }
    }
}
