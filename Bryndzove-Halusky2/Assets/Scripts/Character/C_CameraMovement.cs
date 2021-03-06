﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CameraMovement : Photon.MonoBehaviour {

    // control variables
    public float cameraSensitivity, cameraSensitivityDamp = 3f;

    // references
    private GameManager gameManager;
    private C_CharacterMovement characterMovement;
    // initial variables
    private Vector3 mouseRotation;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Transform parent;

    // Use this for initialization
    void Start ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        characterMovement = transform.parent.GetComponent<C_CharacterMovement>();
        cameraSensitivity = characterMovement.mouseSensitivity / cameraSensitivityDamp;
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        parent = transform.parent;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!gameManager.roundFinished) RotatePitch(355f, 40f); // default =  RotatePitch(355f, 20f);
    }

    // rotate camera only on x axis
    void RotatePitch(float upperClamp, float lowerClamp)
    {
        // clamp rotation to parameter local eulers
        if (transform.localEulerAngles.x >= upperClamp || transform.localEulerAngles.x <= lowerClamp)
        {
            float rawMouseRotation = Input.GetAxis("Mouse Y");
            mouseRotation = new Vector3(-rawMouseRotation, 0, 0);

            transform.Rotate(mouseRotation * cameraSensitivity, Space.Self);
        }

        // reset if goes outside parameter values (with 20 degree offset in case of frame delay)
        if (transform.localEulerAngles.x < upperClamp && transform.localEulerAngles.x > upperClamp - 20f)
        {
            Vector3 fixAngle = new Vector3(upperClamp + 0.01f, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.localEulerAngles = fixAngle;
        }

        if (transform.localEulerAngles.x > lowerClamp && transform.localEulerAngles.x < lowerClamp + 20f)
        {
            Vector3 fixAngle = new Vector3(lowerClamp - 0.01f, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.localEulerAngles = fixAngle;
        }
    }

    // -- Functions/Coroutine only called by C_Character.OnDeath() when it detects death
    // reset camera back to zero and then reparent to it's original parent
    public void ResetCamera()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.parent = parent;
        transform.localPosition = initialPos;
        transform.localRotation = initialRot;
    }

    // parent the camera to the GameObject that killed the player and start the Coroutine EnableDeathCam
    public void CallDeathCam(float time, GameObject killedByPlayer)
    {
        transform.parent = null;
        StartCoroutine(EnableDeathCam(time, killedByPlayer));
    }

    // Coroutine is enabled and the position/rotation of the camera is adjusted slightly - break out after a given amount of time
    IEnumerator EnableDeathCam(float time, GameObject killedByPlayer)
    {
        float deathTime = 0f;
        while (deathTime < time)
        {
            deathTime += Time.deltaTime;
            transform.LookAt(killedByPlayer.transform.position + new Vector3(0f, 1f, 0f));
            transform.position = Vector3.Lerp(transform.position, killedByPlayer.transform.position + new Vector3(0, 15f, 0), Time.fixedDeltaTime);
            yield return null;
        }
        yield break;
    }
}
