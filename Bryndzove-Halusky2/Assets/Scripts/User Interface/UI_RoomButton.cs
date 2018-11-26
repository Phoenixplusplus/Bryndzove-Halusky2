using UnityEngine;
using UnityEngine.UI;

public class UI_RoomButton : Photon.MonoBehaviour
{
    public int ID;

    private Text roomNumber;
    private Text roomStatus;
    private Text roomName;
    private Text mapName;
    private Text playersCount;
    private Button m_ThisButton;

    void Awake()
    {
        roomNumber = this.gameObject.transform.GetChild(0).GetComponent<Text>();
        roomStatus = this.gameObject.transform.GetChild(1).GetComponent<Text>();
        roomName = this.gameObject.transform.GetChild(2).GetComponent<Text>();
        mapName = this.gameObject.transform.GetChild(3).GetComponent<Text>();
        playersCount = this.gameObject.transform.GetChild(4).GetComponent<Text>();

        m_ThisButton = GetComponent<Button>();
    }


    public void SetRoomDetails(int newRoomNumber, bool newRoomStatus, string newRoomName, string newMapName, int newPlayerCount, int newMaxPlayerCount)
    {
        roomNumber.text = "" + newRoomNumber;
        if (newRoomStatus)
        {
            roomStatus.text = "Waitng";
            roomStatus.color = new Color(0, 255, 0);
        }
        else
        {
            roomStatus.text = "Playing";
            roomStatus.color = new Color(255, 0, 0);
        }
        
        roomName.text = newRoomName;
        mapName.text = newMapName;
        playersCount.text = newPlayerCount + "/" + newMaxPlayerCount;
        m_ThisButton.enabled = true;
    }

    public void ResetButton()
    {
        roomNumber.text = "";
        roomStatus.text = "";
        roomName.text = "";
        mapName.text = "";
        playersCount.text = "";
        m_ThisButton.enabled = false;
    }

}
