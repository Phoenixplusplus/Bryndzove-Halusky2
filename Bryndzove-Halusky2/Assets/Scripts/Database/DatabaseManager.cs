using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {

    string createAccountURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/CreateAccount.php";

    string username = "";
    string userpass = "";

    // Use this for initialization
    void Start ()
    {
        // testing if database gives right results
        CreateAccount();
	}

    public void CreateAccount() { StartCoroutine(DB_CreateAccount()); }

    // coroutines
    // if a stop delay is needed for a reply, fabricate one, it isn't done here
    IEnumerator DB_CreateAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("accountName", username);
        form.AddField("accountPass", userpass);
        WWW formRequest = new WWW(createAccountURL, form);
        yield return formRequest;

        if (!string.IsNullOrEmpty(formRequest.error)) Debug.Log("Database error " + formRequest.error);
        else Debug.Log(formRequest.text.ToString());

        if (formRequest.text.ToString() == "") Debug.Log("LOL");
    }
}
