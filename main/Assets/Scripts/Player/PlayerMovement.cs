using UnityEngine;
using UnityEngine.InputSystem; // новий інпут

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float crouchSpeed = 3f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float crouchHeight = 0.8f; // downing
    private float normalCameraY;

    private bool isCrouching = false;

    private CharacterController controller;
    private Vector3 velocity;

    private PlayerControls controls; // class form new input system .inputactions
    private Vector2 moveInput;

    void Awake()
    {
        controls = new PlayerControls();

        // subscribing on input
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Crouch.performed += ctx => StartCrouch();
        controls.Player.Crouch.canceled += ctx => StopCrouch();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        normalCameraY = cameraTransform.localPosition.y;
    }

    void Update()
    {
        // movement
        float currentSpeed = isCrouching ? crouchSpeed : speed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // gravity (no ragdolls for new input sys)
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void StartCrouch()
    {
        isCrouching = true;
        Vector3 pos = cameraTransform.localPosition;
        pos.y = normalCameraY - crouchHeight;
        cameraTransform.localPosition = pos;
    }

    void StopCrouch()
    {
        isCrouching = false;
        Vector3 pos = cameraTransform.localPosition;
        pos.y = normalCameraY;
        cameraTransform.localPosition = pos;
    }
}
