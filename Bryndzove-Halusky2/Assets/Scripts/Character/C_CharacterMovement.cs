using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CharacterMovement : Photon.MonoBehaviour {

    public float mouseSensitivity = 3f;

    private GameManager gameManager;
    private C_Character characterRoot;
    private CharacterController controller;

    // movement attributes
    public float speed = 10.0f;
    public Vector3 moveDirection = Vector3.zero;
    private float jumpSpeed = 8.0f;
    private float gravity = 20.0f;

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
        // We are grounded, so recalculate
        // move direction directly from axes
        if (controller.isGrounded)
        {
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
    }

    // rotate character based on the mouse x input
    void RotateWithMouseX()
    {
        float rawMouseRotation = Input.GetAxis("Mouse X");
        Vector3 mouseRotation = new Vector3(0, rawMouseRotation, 0);

        transform.Rotate(mouseRotation * mouseSensitivity);
    }
}
