using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : Photon.MonoBehaviour {

    [Header("Attributes")]
    public string Team;
    public bool Occupied = false;

    public int health = 10;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (photonView.isMine)
        {
            if (other.gameObject.tag == "Player")
            {
                photonView.RPC("TriggerEnter", PhotonTargets.All, null);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (photonView.isMine)
        {
            if (other.tag == "Player")
            {
                photonView.RPC("TriggerExit", PhotonTargets.All, null);
            }
        }
    }

    [PunRPC] void TriggerEnter()
    {
        Occupied = true;
        Debug.Log(Occupied);
        if (PhotonNetwork.isMasterClient) health = health - 1;
    }

    [PunRPC] void TriggerExit()
    {
        Occupied = false;
        Debug.Log(Occupied);
    }
}
