using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMovement2 : Photon.MonoBehaviour {

    // determine how much data will be sent and recieved every second
    // movement is synched based upon data recieved on these 'frames'
    public int sendRate = 20, serializedSendRate = 20;

    private GameObject pCharacter;
    private C_CharacterMovement characterMovement;
    private float lastSyncTime = 0f;
    private float syncTime = 0f;
    private float syncDelay = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Quaternion syncStartRotation = Quaternion.identity;
    private Quaternion syncEndRotation = Quaternion.identity;

    void Awake()
    {
        // grab a reference to the root class (returns self if class == root)
        pCharacter = transform.root.gameObject;
        characterMovement = pCharacter.GetComponent<C_CharacterMovement>();

        lastSyncTime = Time.time;
    }

	// Use this for initialization
	void Start ()
    {
        // set network send rates
        PhotonNetwork.sendRate = sendRate;
        PhotonNetwork.sendRateOnSerialize = serializedSendRate;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (photonView.isMine)
        {

        }
        else
        {
            SyncMovement();
        }
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (pCharacter != null)
        {
            // give the data other players need to synchronise our movement
            if (stream.isWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(characterMovement.localVelocity);
                stream.SendNext(transform.rotation);
            }
            // account for delays inbetween data recieved
            else
            {
                Vector3 syncPosition = (Vector3)stream.ReceiveNext();
                Vector3 syncVelocity = (Vector3)stream.ReceiveNext();
                Quaternion syncRotation = (Quaternion)stream.ReceiveNext();
                syncTime = 0f;
                syncDelay = Time.time - lastSyncTime;
                lastSyncTime = Time.time;
                syncEndPosition = syncPosition + syncVelocity * syncDelay;
                syncStartPosition = transform.position;
                syncEndRotation = syncRotation;
                syncStartRotation = transform.rotation;
            }
        }
    }

    // for smoother movement, lerp between the previously recieved data and the currently recieved data
    // note: Vector3.MoveTowards() creates an unrealistic movement, lerping of course never reaches the end values, but it enough for smoother movement
    // for positional updates, we must lerp over time that is when we recieved the last update and when we recieved the current one
    // rotation is not as important and a static value seems to work well enough
    void SyncMovement()
    {
        if (pCharacter != null)
        {
            syncTime += Time.deltaTime;
            transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
            transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, 180f * sendRate * Time.deltaTime);
        }
    }
}
