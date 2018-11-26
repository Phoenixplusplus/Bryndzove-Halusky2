using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMovement2 : Photon.MonoBehaviour {

    private GameObject pCharacter;
    private C_CharacterMovement characterMovement;
    float lastSyncTime = 0f;
    float syncTime = 0f;
    float syncDelay = 0f;
    Vector3 syncStartPosition = Vector3.zero;
    Vector3 syncEndPosition = Vector3.zero;
    Quaternion syncStartRotation = Quaternion.identity;
    Quaternion syncEndRotation = Quaternion.identity;
    public int sendRate = 20, serializedSendRate = 20;

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
            if (stream.isWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(characterMovement.localVelocity);
                stream.SendNext(transform.rotation);
            }
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
