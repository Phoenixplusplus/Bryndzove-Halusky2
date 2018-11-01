using UnityEngine;
using UnityEngine.UI;

public class UI_RoomLobby : Photon.MonoBehaviour
{
    private Button m_BTN_LeaveRoom;
    private Button m_BTN_KickPlayer;
    private Image BTN_IMG_LeaveRoom;
    private Image BTN_IMG_KickPlayer;

    void Start()
    {
        m_BTN_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Button>();
        m_BTN_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Button>();
        BTN_IMG_LeaveRoom = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        BTN_IMG_KickPlayer = this.gameObject.transform.GetChild(7).GetComponent<Image>();
    }

	// Update is called once per frame
	void OnJoinedRoom()
    {
		if (!PhotonNetwork.isMasterClient)
        {
            m_BTN_LeaveRoom.enabled = false;
            m_BTN_KickPlayer.enabled = false;
            BTN_IMG_LeaveRoom.color = new Color32(100, 100, 100, 255);
            BTN_IMG_KickPlayer.color = new Color32(100, 100, 100, 255);
        }
    }

    void BecameMasterClient()
    {
        m_BTN_LeaveRoom.enabled = true;
        m_BTN_KickPlayer.enabled = true;
        BTN_IMG_LeaveRoom.color = new Color32(255, 255, 255, 255);
        BTN_IMG_KickPlayer.color = new Color32(255, 255, 255, 255);
    }

    // If the new master client is this call the function
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        // ThisPlayerBecameMasterCleint
    }

}
