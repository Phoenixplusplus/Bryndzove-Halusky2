using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_LArmTilt : Photon.MonoBehaviour
{
    private GameObject characterCamera;

    // Use this for initialization
    void Start()
    {
        characterCamera = GameObject.Find("CharacterCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            SetArmRotation();
        }
    }

    // tilt arm based on camera rotation
    void SetArmRotation()
    {
        Vector3 finalRotation = new Vector3(characterCamera.transform.eulerAngles.x + 70f, characterCamera.transform.eulerAngles.y, characterCamera.transform.eulerAngles.z);
        transform.eulerAngles = finalRotation;
    }
}
