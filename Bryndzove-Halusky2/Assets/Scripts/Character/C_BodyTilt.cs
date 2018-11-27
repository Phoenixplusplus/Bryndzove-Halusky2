using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BodyTilt : Photon.MonoBehaviour {

    // main tilt multiplier
    public float rotateRate = 100.0f;
	
	// Update is called once per frame
	void Update ()
    {
        if (photonView.isMine)
        {
            transform.localEulerAngles = new Vector3(Input.GetAxis("Vertical") * Time.fixedDeltaTime * 8.0f * rotateRate, 0, -Input.GetAxis("Horizontal") * Time.fixedDeltaTime * 8.0f * rotateRate);
        }
    }
}