using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour {

    string createAccountURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/CreateAccount.php";
    string loginURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/Login.php";
    string loadPlayerDataURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/LoadPlayerData.php";
    string savePlayerDataURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/SavePlayerData.php";

    public string createAccountReply = "";
    public bool createAccountBool = false;
    public string loginReply = "";
    public bool loginBool = false;
    public string loadUserDataReply = "";
    public string saveUserDataReply = "";

    // events
    public delegate void LoadData();
    public static event LoadData LoadDataReady;

    // UI changes made directly from databasemanager
    public Text LoggedInto;

    // public functions
    public void CreateAccount(string username, string userpass) { StartCoroutine(DB_CreateAccount(username, userpass)); }
    public void Login(string username, string userpass) { StartCoroutine(DB_Login(username, userpass)); }
    public void LoadPlayerData(string username, string userpass) { StartCoroutine(DB_LoadPlayerData(username, userpass)); }
    public void SavePlayerData(string username, string userpass, string headtex, string bodytex, string weapon) { StartCoroutine(DB_SavePlayerData(username, userpass, headtex, bodytex, weapon)); }

    // coroutines
    // (if a stop delay is needed for a reply, fabricate one, it isn't done here)
    IEnumerator DB_CreateAccount(string username, string userpass)
    {
        WWWForm form = new WWWForm();
        form.AddField("accountName", username);
        form.AddField("accountPass", userpass);
        WWW formRequest = new WWW(createAccountURL, form);
        yield return formRequest;

        if (!string.IsNullOrEmpty(formRequest.error)) Debug.Log("Database error " + formRequest.error);
        else
        {
            Debug.Log(formRequest.text.ToString());
            createAccountReply = formRequest.text.ToString();
            if (createAccountReply.Contains("created")) createAccountBool = true; // used to process to next UI (returned by database)
        }
    }

    IEnumerator DB_Login(string username, string userpass)
    {
        WWWForm form = new WWWForm();
        form.AddField("accountName", username);
        form.AddField("accountPass", userpass);
        WWW formRequest = new WWW(loginURL, form);
        yield return formRequest;

        if (!string.IsNullOrEmpty(formRequest.error)) Debug.Log("Database error " + formRequest.error);
        else
        {
            Debug.Log(formRequest.text.ToString());
            loginReply = formRequest.text.ToString();
            if (loginReply.Contains("Thanks"))
            {
                loginBool = true;
                string[] userInfo = loginReply.Split('|');
                GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.username = userInfo[1];
                gameManager.userpass = userInfo[2];
                gameManager.headtex = userInfo[3];
                gameManager.bodytex = userInfo[4];
                gameManager.weapon = userInfo[5];
                LoggedInto.text = "Logged into: " + userInfo[1];
            }
        }
    }

    IEnumerator DB_LoadPlayerData(string username, string userpass)
    {
        WWWForm form = new WWWForm();
        form.AddField("accountName", username);
        form.AddField("accountPass", userpass);
        WWW formRequest = new WWW(loadPlayerDataURL, form);
        yield return formRequest;

        if (!string.IsNullOrEmpty(formRequest.error)) Debug.Log("Database error " + formRequest.error);
        else
        {
            loadUserDataReply = formRequest.text.ToString();
            string[] userInfo = loadUserDataReply.Split('|');
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.username = userInfo[0];
            gameManager.userpass = userInfo[1];
            gameManager.headtex = userInfo[2];
            gameManager.bodytex = userInfo[3];
            gameManager.weapon = userInfo[4];
        }

        if (formRequest.isDone)
        {
            if (LoadDataReady != null) // send event to listeners now that data is ready
            {
                LoadDataReady();
            }
            Debug.Log("Loading player data done");
            yield break;
        }
    }

    IEnumerator DB_SavePlayerData(string username, string userpass, string headtex, string bodytex, string weapon)
    {
        WWWForm form = new WWWForm();
        form.AddField("accountName", username);
        form.AddField("accountPass", userpass);
        form.AddField("accountHeadTex", headtex);
        form.AddField("accountBodyTex", bodytex);
        form.AddField("accountWeapon", weapon);
        WWW formRequest = new WWW(savePlayerDataURL, form);
        yield return formRequest;
    }
}
