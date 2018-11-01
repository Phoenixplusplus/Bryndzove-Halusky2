using UnityEngine;
using UnityEngine.UI;

public class UI_PlayersOnline : MonoBehaviour
{
    private Text m_MyText;
    private UserInterfaceManager UI_manager;

    void Start()
    {
        //Text sets your text to say this message
        m_MyText = gameObject.GetComponent<Text>();
        UI_manager = GameObject.Find("UserInterfaceManager").GetComponent<UserInterfaceManager>();
    }

    void Update()
    {
        m_MyText.text = "Players Online " + UI_manager.GetPlayersInMasterServer();
    }
}