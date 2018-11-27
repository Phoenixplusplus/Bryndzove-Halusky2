using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : Photon.MonoBehaviour {

    // events
    public delegate void PlayerBackToLobby();
    public static event PlayerBackToLobby PlayerBackToLobbyStatus;

    public GameManager gameManager;
    public UserInterfaceManager UI_Manager;
    public C_Character localCharacter;
    private W_Weapon equippedWeapon;
    public Camera lobbyCamera;
    bool endTrigger = false;
    float totalPaint = 0f;
    float redRatio = 0f;
    float blueRatio = 0f;
    float redLerp = 0f;
    float blueLerp = 0f;

    // UI children
    public Slider ammoSlider, healthSlider;
    public Text AmmoUpText, SpeedUpText, roundTime, redWinPercent, blueWinPercent;
    public Image InfiniteAmmoIMG, SpeedUpIMG, HealthRecoveredIMG, TimeleftIMG, TimeleftBackIMG, HealthAmmoIMG, TimeUpIMG, WinBackgroundIMG, RedWinIMG, BlueWinIMG, WeWinIMG, WeLoseIMG;
    public Button BackToLobbyBTN;
    public GameObject chat;

    [Header("Sounds")]
    public AudioClip Yeah;
    public AudioClip No;
    public AudioClip TimeUp;

    bool runUpdate = false;

    void OnEnable()
    {
        EventManager.PlayerSpawned += Initialise;
    }

    void OnDisable()
    {
        EventManager.PlayerSpawned -= Initialise;
    }

	public void Initialise()
    {
        Debug.Log("I heard from EventManager player has finished startup. Initialising..");
        equippedWeapon = localCharacter.leftWeapon;

        // enable & set children
        ammoSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
        roundTime.gameObject.SetActive(true);
        TimeleftIMG.gameObject.SetActive(true);
        HealthAmmoIMG.gameObject.SetActive(true);
        TimeleftBackIMG.gameObject.SetActive(true);
        chat.SetActive(true);
        chat.GetComponent<Chat>().ResetChat();

        // set their attributes
        ammoSlider.maxValue = equippedWeapon.clipSize;
        ammoSlider.wholeNumbers = true;
        healthSlider.maxValue = localCharacter.maxHealth;

        runUpdate = true;
        endTrigger = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (runUpdate)
        {
            ammoSlider.value = equippedWeapon.ammoCount;
            healthSlider.value = localCharacter.Health;
            roundTime.text = gameManager.roundTime.ToString("0.0");
            if (gameManager.roundFinished && endTrigger == false) OnRoundEnd();
        }
    }

    void OnRoundEnd()
    {
        endTrigger = true;
        AudioSource.PlayClipAtPoint(TimeUp, localCharacter.transform.position);
        if (ammoSlider.isActiveAndEnabled) ammoSlider.gameObject.SetActive(false);
        if (healthSlider.isActiveAndEnabled) healthSlider.gameObject.SetActive(false);
        if (roundTime.isActiveAndEnabled) roundTime.gameObject.SetActive(false);
        if (TimeleftIMG.isActiveAndEnabled) TimeleftIMG.gameObject.SetActive(false);
        if (HealthAmmoIMG.isActiveAndEnabled) HealthAmmoIMG.gameObject.SetActive(false);
        if (TimeleftBackIMG.isActiveAndEnabled) TimeleftBackIMG.gameObject.SetActive(false);
        if (!TimeUpIMG.isActiveAndEnabled) TimeUpIMG.gameObject.SetActive(true);

        StartCoroutine(RoundEnd(2f, 5f));
    }

    public void BackToLobby()
    {
        localCharacter.DestroySelf();
        lobbyCamera.gameObject.SetActive(true);
        PhotonNetwork.LeaveRoom();
        for (int i = 0; i < transform.childCount; i++) { transform.GetChild(i).gameObject.SetActive(false); }
        // send event to any components that need to know about reseting values/enabling UI when player goes back to lobby
        if (PlayerBackToLobbyStatus != null) { PlayerBackToLobbyStatus(); }
    }

    // round end coroutine for UI and calculation
    public IEnumerator RoundEnd(float time, float timeMultiplier)
    {
        yield return new WaitForSeconds(2f);

        float firstTime = 0f;
        float secondTime = 0f;

        TimeUpIMG.gameObject.SetActive(false);
        WinBackgroundIMG.gameObject.SetActive(true);
        RedWinIMG.gameObject.SetActive(true);
        BlueWinIMG.gameObject.SetActive(true);

        // only master calculates the result of the round and sends this result to all other players
        if (PhotonNetwork.isMasterClient)
        {
            totalPaint = gameManager.redTeamPaintCount + gameManager.blueTeamPaintCount;
            redRatio = gameManager.redTeamPaintCount / totalPaint;
            blueRatio = gameManager.blueTeamPaintCount / totalPaint;

            // check for minimum or maximum values
            if (gameManager.redTeamPaintCount == totalPaint) redRatio = 1;
            if (gameManager.blueTeamPaintCount == totalPaint) blueRatio = 1;
            if (gameManager.redTeamPaintCount == 0) redRatio = 0;
            if (gameManager.blueTeamPaintCount == 0) blueRatio = 0;

            photonView.RPC("GetWinLoseRatio", PhotonTargets.All, redRatio, blueRatio);
        }

        while (firstTime < time + 0.1f)
        {
            redLerp = Mathf.Lerp(0f, 0.5f, firstTime / time);
            blueLerp = Mathf.Lerp(0f, 0.5f, firstTime / time);
            RedWinIMG.fillAmount = redLerp;
            BlueWinIMG.fillAmount = blueLerp;
            firstTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (secondTime <= (time / timeMultiplier) + 0.1f)
        {
            float finalRedLerp = Mathf.Lerp(0.5f, redRatio, secondTime / (time / timeMultiplier));
            float finalBlueLerp = Mathf.Lerp(0.5f, blueRatio, secondTime / (time / timeMultiplier));
            RedWinIMG.fillAmount = finalRedLerp;
            BlueWinIMG.fillAmount = finalBlueLerp;
            secondTime += Time.deltaTime;
            yield return null;
        }

        if (redRatio > blueRatio)
        {
            if (localCharacter.Team == "Red")
            {
                WeWinIMG.gameObject.SetActive(true);
                AudioSource.PlayClipAtPoint(Yeah, localCharacter.transform.position);
            }
            else
            {
                WeLoseIMG.gameObject.SetActive(true);
                AudioSource.PlayClipAtPoint(No, localCharacter.transform.position);
            }
        }
        else
        {
            if (localCharacter.Team == "Blue")
            {
                WeWinIMG.gameObject.SetActive(true);
                AudioSource.PlayClipAtPoint(Yeah, localCharacter.transform.position);
            }
            else
            {
                WeLoseIMG.gameObject.SetActive(true);
                AudioSource.PlayClipAtPoint(No, localCharacter.transform.position);
            }
        }

        redWinPercent.gameObject.SetActive(true);
        blueWinPercent.gameObject.SetActive(true);
        redWinPercent.text = (redRatio * 100).ToString("0.00") + "%";
        blueWinPercent.text = (blueRatio * 100).ToString("0.00") + "%";

        yield return new WaitForSeconds(1f);

        BackToLobbyBTN.gameObject.SetActive(true);

        Debug.Log("Breaking");
        yield break;
    }

    [PunRPC] void GetWinLoseRatio(float RPCredRatio, float RPCblueRatio)
    {
        redRatio = RPCredRatio;
        blueRatio = RPCblueRatio;
    }
}
