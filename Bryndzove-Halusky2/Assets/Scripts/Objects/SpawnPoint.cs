using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : Photon.MonoBehaviour {

    [Header("Attributes")]
    public string Team;
    public bool Occupied = false;

    // if this spawnpoint is colliding with a player, send an RPC to everyone to tell them that this spawnpoint is occupied
    // this means that 2 players cannot be spawned on the same point and cause problems
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

    [PunRPC] void TriggerEnter() { Occupied = true; }
    [PunRPC] void TriggerExit() { Occupied = false; }
}
