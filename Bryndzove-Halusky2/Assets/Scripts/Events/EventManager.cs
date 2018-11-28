using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    // events
    public delegate void PlayerInstantiated();
    public static event PlayerInstantiated PlayerSpawned;

    // subscribe to other events
    void OnEnable() { C_Character.PlayerReady += ParsePlayerReadyEvent; }
    // unsubscribe to other events
    void OnDisable() { C_Character.PlayerReady -= ParsePlayerReadyEvent; }

    // when the player is ready, because we are manager, we will parse this message on to anyone listening
    void ParsePlayerReadyEvent() { PlayerSpawned(); }
}
