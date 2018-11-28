using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour {

    [Header("Sounds")]
    public AudioClip loggedInSound;
    public AudioClip loginFailedSound;
    private Vector3 soundPosition;

    private string createAccountURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/CreateAccount.php";
    private string loginURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/Login.php";
    private string loadPlayerDataURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/LoadPlayerData.php";
    private string savePlayerDataURL = "http://kunet.kingston.ac.uk/k1652267/Bryndzove/SavePlayerData.php";

    public string createAccountReply = "";
    public string loginReply = "";
    public string loadUserDataReply = "";
    public string saveUserDataReply = "";
    public bool loginBool = false;
    public bool createAccountBool = false;

    // events
    public delegate void LoadData();
    public static event LoadData LoadDataReady;

    // UI changes made directly from .php result
    public Text LoggedInto;

    // public functions to start neccessary Coroutines
    public void CreateAccount(string username, string userpass)                                                 { StartCoroutine(DB_CreateAccount(username, userpass)); }
    public void Login(string username, string userpass)                                                         { StartCoroutine(DB_Login(username, userpass)); }
    public void LoadPlayerData(string username, string userpass)                                                { StartCoroutine(DB_LoadPlayerData(username, userpass)); }
    public void SavePlayerData(string username, string userpass, string headtex, string bodytex, string weapon) { StartCoroutine(DB_SavePlayerData(username, userpass, headtex, bodytex, weapon)); }

    void Start()
    {
        soundPosition = GameObject.Find("LobbyCamera").transform.position;
    }

    // Coroutines
    // determine (in php) whether this username and password is valid to create and account
    // if it is, then wait for a reply that contains "created"
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

    // similarly, wait for a reply containing "Thanks" and then split the result into an array and assign the gamemanager these values
    // these will eventually be taken by the character on Start() and send it to other players in the room
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
                AudioSource.PlayClipAtPoint(loggedInSound, soundPosition);
                createAccountBool = false;
                string[] userInfo = loginReply.Split('|');
                GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.username = userInfo[1];
                gameManager.userpass = userInfo[2];
                gameManager.headtex = userInfo[3];
                gameManager.bodytex = userInfo[4];
                gameManager.weapon = userInfo[5];
                LoggedInto.text = "Logged into: " + userInfo[1];
            }
            else AudioSource.PlayClipAtPoint(loginFailedSound, soundPosition);
        }
    }

    // loading player data is similar to above, but we must send an event to the UI_Customise script to enable
    // it to apply this to a dummy model to show the player the values of textures/weapon they have in their account
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

    // update the user information in the database once the player has finished customising their dummy character
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
