using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CharacterMovement : Photon.MonoBehaviour {

    GameManager gameManager;
    public float mouseSensitivity = 3f, movementSpeed = 5f;
    public float WS, AD;
    public Vector3 localVelocity;
    bool isJumping = false;
    C_Character characterRoot;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        characterRoot = transform.root.GetComponent<C_Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            if (!gameManager.roundFinished)
            {
                if (characterRoot.isDead == false)
                {
                    Movement();
                    RotateWithMouseX();
                }
            }
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
            if (JumpCheck()) StartCoroutine(Jump());
        }
    }

    bool JumpCheck() { return Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), -Vector3.up, 0.55f); }

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
