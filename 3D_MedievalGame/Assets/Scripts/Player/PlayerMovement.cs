using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    Vector3 cameraRelativeMovement;
    private bool isMovementPressed;
    private float rotationPerFrame = 15f;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        playerInput.Movement.Move.started += onMovement;
        playerInput.Movement.Move.canceled += onMovement;
        playerInput.Movement.Move.performed += onMovement;
    }

    private void Update()
    {
        PlayerRotation();
        cameraRelativeMovement = ConvertToCameraSpace(currentMovement);
        characterController.Move(cameraRelativeMovement * Time.deltaTime);
    }

    void onMovement(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float curretYValue = vectorToRotate.y;

        Vector3 cameraFoward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraFoward.y = 0;
        cameraRight.y = 0;

        cameraFoward = cameraFoward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraFowardProduct = vectorToRotate.z * cameraFoward;
        Vector3 cameraRightProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorCameraSpace = cameraFowardProduct + cameraRightProduct;
        vectorCameraSpace.y = curretYValue;
        return vectorCameraSpace;
    }

    void PlayerRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = cameraRelativeMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationPerFrame * Time.deltaTime);
        }
    }

    void Gravity()
    {
        if (characterController.isGrounded)
        {
            float groundGravity = -.05f;
            currentMovement.y = groundGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
        }
    }

    private void OnEnable()
    {
        playerInput.Movement.Enable();
    }

    private void OnDisable()
    {
        playerInput.Movement.Disable();
    }
}
