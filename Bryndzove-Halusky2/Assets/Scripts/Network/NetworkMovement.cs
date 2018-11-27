using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//// LEGACY CODE ////
public class NetworkMovement : NetworkManager
{
    private GameObject pCharacter;
    private C_CharacterMovement characterMovement;
    private Vector3 networkPosition, networkVelocity, predictedPosition;
    private Quaternion networkRotation;
    public int sendRate = 30, serializedSendRate = 30;
    private double lastTimestamp;
    private float movementSpeed;

    // Use this for initialization
    void Start ()
    {
        // set network send rates
        PhotonNetwork.sendRate = sendRate;
        PhotonNetwork.sendRateOnSerialize = serializedSendRate;

        // grab a reference to the root class (returns self if class == root)
        pCharacter = transform.root.gameObject;
        characterMovement = pCharacter.GetComponent<C_CharacterMovement>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (photonView.isMine)
        {
            
        }
        // update network player behaviour
        else
        {
            // calculate roundtrip time for packet
            float ping = (float)PhotonNetwork.GetPing() * 0.001f;
            float lastUpdate = (float)(PhotonNetwork.time - lastTimestamp);
            float totalUpdateTime = ping + lastUpdate;

            //update position
            predictedPosition = networkPosition + networkVelocity * movementSpeed * totalUpdateTime;
            transform.position = Vector3.MoveTowards(transform.position, predictedPosition, Vector3.Distance(transform.position, predictedPosition) * sendRate * Time.deltaTime);

            // update rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 180f * sendRate * Time.deltaTime);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // local component sending transform to the network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (characterMovement)
            {
                stream.SendNext(characterMovement.localVelocity);
                stream.SendNext(characterMovement.speed);
            }
        }
        else
        {
            // receiving network players transforms
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            if (characterMovement)
            {
                networkVelocity = (Vector3)stream.ReceiveNext();
                movementSpeed = (float)stream.ReceiveNext();
            }

            lastTimestamp = info.timestamp;
        }
    }
}
