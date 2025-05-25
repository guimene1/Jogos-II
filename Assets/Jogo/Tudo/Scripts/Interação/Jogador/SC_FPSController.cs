using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Look Settings")]
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [Header("Crouch Settings")]
    public float crouchHeightMultiplier = 0.5f;
    public float crouchRadiusMultiplier = 0.8f;
    public float crouchSpeed = 3.5f;
    public float crouchTransitionSpeed = 10f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool isCrouching = false;
    private float originalHeight;
    private float originalRadius;
    private Vector3 originalCameraPosition;
    private Vector3 originalCenter;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        originalRadius = characterController.radius;
        originalCenter = characterController.center;
        originalCameraPosition = playerCamera.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        
        // Handle crouch input (toggle)
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        HandleCrouch();
        HandleMovement();
        HandleRotation();

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCrouch()
    {
        float targetHeight = isCrouching ? originalHeight * crouchHeightMultiplier : originalHeight;
        float targetRadius = isCrouching ? originalRadius * crouchRadiusMultiplier : originalRadius;
        Vector3 targetCameraPos = isCrouching ?
            new Vector3(originalCameraPosition.x,
                       originalCameraPosition.y * crouchHeightMultiplier,
                       originalCameraPosition.z) :
            originalCameraPosition;

        Vector3 targetCenter = new Vector3(originalCenter.x, targetHeight / 2, originalCenter.z);

        // Smooth transition
        characterController.height = Mathf.Lerp(
            characterController.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime);

        characterController.radius = Mathf.Lerp(
            characterController.radius,
            targetRadius,
            crouchTransitionSpeed * Time.deltaTime);

        characterController.center = Vector3.Lerp(
            characterController.center,
            targetCenter,
            crouchTransitionSpeed * Time.deltaTime);

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetCameraPos,
            crouchTransitionSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runningSpeed : walkingSpeed);

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Normalize diagonal movement
        float inputMagnitude = new Vector2(horizontalInput, verticalInput).normalized.magnitude;

        float curSpeedX = canMove ? currentSpeed * verticalInput * inputMagnitude : 0;
        float curSpeedY = canMove ? currentSpeed * horizontalInput * inputMagnitude : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isCrouching)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
    }

    void HandleRotation()
    {
        if (!canMove) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}