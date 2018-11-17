using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CameraMovement : Photon.MonoBehaviour {

    public float cameraSensitivity, cameraSensitivityDamp = 3f;
    C_CharacterMovement characterMovement;
    Vector3 mouseRotation;
    Vector3 initialPos;
    Quaternion initialRot;
    Transform parent;
    C_Character character;

    void Awake()
    {

    }

    // Use this for initialization
    void Start ()
    {
        characterMovement = transform.parent.GetComponent<C_CharacterMovement>();
        cameraSensitivity = characterMovement.mouseSensitivity / cameraSensitivityDamp;
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        parent = transform.parent;
        character = parent.GetComponent<C_Character>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        RotatePitch(355f, 40f); // default =  RotatePitch(355f, 20f);

        if (character.isDead == true)
        {
            transform.parent = null;
            transform.LookAt(parent.transform.position + new Vector3(0f, 1f, 0f));
        }
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

    public void ResetCamera()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.parent = parent;
        transform.localPosition = initialPos;
        transform.localRotation = initialRot;
    }
}
