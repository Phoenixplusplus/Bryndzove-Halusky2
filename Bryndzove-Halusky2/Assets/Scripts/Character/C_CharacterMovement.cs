using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CharacterMovement : Photon.MonoBehaviour {

    public float mouseSensitivity = 3f, movementSpeed = 6f;
    public float WS, AD;
    public Vector3 localVelocity;
    bool isJumping = false;
    Rigidbody rb;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            Movement();
            RotateWithMouseX();
        }
    }

    // local
    void Movement()
    {
        // move on keyboard input
        AD = Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        WS = Input.GetAxis("Vertical") * Time.fixedDeltaTime;

        localVelocity = new Vector3(AD, 0, WS);
        transform.Translate(AD * movementSpeed, 0, WS * movementSpeed);

        // jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping) StartCoroutine(Jump());
        }
    }

    void RotateWithMouseX()
    {
        // rotate on mouse X
        float rawMouseRotation = Input.GetAxis("Mouse X");
        Vector3 mouseRotation = new Vector3(0, rawMouseRotation, 0);

        transform.Rotate(mouseRotation * mouseSensitivity);
    }

    IEnumerator Jump()
    {
        isJumping = true;
        float jumpHeight = 0f;
        while (jumpHeight < 0.2f)
        {
            jumpHeight += Time.deltaTime;
            transform.Translate(0, jumpHeight, 0);
            yield return null;
        }
        isJumping = false;
        yield break;
    }
}
