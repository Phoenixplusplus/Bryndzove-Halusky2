using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HaveBeenKicked : MonoBehaviour {

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
        this.gameObject.SetActive(true);
    }
}
