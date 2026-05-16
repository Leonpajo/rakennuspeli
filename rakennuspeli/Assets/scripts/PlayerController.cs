// PlayerController.cs

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.15f;
    public Transform cameraTransform;

    [Header("Head Bob")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.035f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    private Vector3 cameraStartPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraStartPos = cameraTransform.localPosition;
    }

    void Update()
    {
        Look();
        Move();
        HeadBob();
    }

    void Look()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        Keyboard keyboard = Keyboard.current;

        float x = 0f;
        float z = 0f;

        if (keyboard.aKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed) x = 1f;
        if (keyboard.wKey.isPressed) z = 1f;
        if (keyboard.sKey.isPressed) z = -1f;

        float currentSpeed =
            keyboard.leftShiftKey.isPressed ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (keyboard.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HeadBob()
    {
        Keyboard keyboard = Keyboard.current;

        bool isMoving =
            keyboard.wKey.isPressed ||
            keyboard.aKey.isPressed ||
            keyboard.sKey.isPressed ||
            keyboard.dKey.isPressed;

        bool isRunning = keyboard.leftShiftKey.isPressed;

        if (isMoving && controller.isGrounded)
        {
            float currentBobSpeed =
                isRunning ? bobSpeed * 1.6f : bobSpeed;

            float currentBobAmount =
                isRunning ? bobAmount * 1.8f : bobAmount;

            float bobOffset =
                Mathf.Sin(Time.time * currentBobSpeed)
                * currentBobAmount;

            float swayOffset =
                Mathf.Cos(Time.time * currentBobSpeed * 0.5f)
                * (currentBobAmount * 0.5f);

            Vector3 targetPosition = new Vector3(
                cameraStartPos.x + swayOffset,
                cameraStartPos.y + bobOffset,
                cameraStartPos.z
            );

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPosition,
                Time.deltaTime * 10f
            );
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraStartPos,
                Time.deltaTime * 8f
            );
        }
    }
}