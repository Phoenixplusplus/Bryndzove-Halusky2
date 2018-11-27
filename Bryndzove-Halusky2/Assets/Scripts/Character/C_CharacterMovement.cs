using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CharacterMovement : Photon.MonoBehaviour {

    public float mouseSensitivity = 3f, movementSpeed = 50.0f;
    Vector2 newPosition = new Vector2(0.0f, 0.0f);

    public float WS, AD;
    public Vector3 localVelocity;

    private GameManager gameManager;
    private C_Character characterRoot;
    private CharacterController controller;

    float speed = 10.0f;
    float jumpSpeed = 8.0f;
    float gravity = 20.0f;
    Vector3 moveDirection = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
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

    // the base movement for the character - values will be sent over the network in the NetworkMovement2 script
    void Movement()
    {
        // move on keyboard input
        AD = Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        WS = Input.GetAxis("Vertical") * Time.fixedDeltaTime;

        Debug.Log("VelocityX: " + AD * Time.deltaTime * movementSpeed + " VelocityY: " + WS * Time.deltaTime * movementSpeed);
        //localVelocity = new Vector3(AD, 0, WS);

        localVelocity.x = AD;
        localVelocity.y = WS;

        //transform.Translate(AD * Time.deltaTime * movementSpeed, 0, WS * Time.deltaTime * movementSpeed);



        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        //if (Input.GetKeyDown(KeyCode.Space)) { if (JumpCheck()) StartCoroutine(Jump()); }
    }

    // rotate character based on the mouse x input
    void RotateWithMouseX()
    {
        float rawMouseRotation = Input.GetAxis("Mouse X");
        Vector3 mouseRotation = new Vector3(0, rawMouseRotation, 0);

        transform.Rotate(mouseRotation * mouseSensitivity);
    }

    // cast a small ray to see if we are grounded or not
    bool JumpCheck() { return Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), -Vector3.up, 0.55f); }

    // lift character up until threshold is met
    IEnumerator Jump()
    {
        float jumpHeight = 0f;
        while (jumpHeight < 0.2f)
        {
            jumpHeight += Time.deltaTime;
            transform.Translate(0, jumpHeight, 0);
            yield return null;
        }
        yield break;
    }
}
