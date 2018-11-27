using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BodyTilt : Photon.MonoBehaviour {

    // main tilt multiplier
    public float rotateRate = 100f;

    private C_CharacterMovement characterMovement;
    private Vector3 WSADTilt;

    void Awake()
    {

    }

	// Use this for initialization
	void Start ()
    {
        characterMovement = transform.parent.GetComponent<C_CharacterMovement>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (photonView.isMine)
        {
            Tilt();
        }
    }

    // set x,z rotation based on movement from C_CharacterMovement script
    void Tilt()
    {
        WSADTilt = new Vector3(characterMovement.WS * 8.0f * rotateRate, 0, -characterMovement.AD * 8.0f * rotateRate);

        transform.localEulerAngles = WSADTilt;
    }
}