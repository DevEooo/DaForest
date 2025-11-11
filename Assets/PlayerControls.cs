using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    [Header("Speeds")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -15f;
    public LayerMask groundMask = ~0; // detects all layers by default

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;

    [Header("References")]
    public Animator animator; // assign your Animator in Inspector

    private CharacterController controller;
    private PlayerControl controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private float rotationX;

    // Animator smoothing
    private float animSpeedSmooth = 0f;
    public float animSmoothTime = 8f;

    // Camera smoothing
    private float targetCameraY;
    public float cameraSmoothTime = 0.2f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControl();

        // Input setup
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Sprint.performed += ctx => isSprinting = true;
        controls.Player.Sprint.canceled += ctx => isSprinting = false;

        controls.Player.Crouch.performed += ctx => ToggleCrouch();
        controls.Player.Jump.performed += ctx => Jump();
    }

    private void Start()
    {
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        // Setup camera
        if (cameraTransform == null)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            if (cam != null) cameraTransform = cam.transform;
            else Debug.LogWarning("MainCamera not found. Assign cameraTransform in inspector.");
        }

        if (cameraTransform != null)
        {
            cameraTransform.SetParent(transform);
            cameraTransform.localPosition = new Vector3(-0.01f, 1.57f, 0.248f);
            cameraTransform.localRotation = Quaternion.identity;
            targetCameraY = cameraTransform.localPosition.y;
        }

        Camera camera = cameraTransform?.GetComponent<Camera>();
        if (camera != null)
        {
            camera.nearClipPlane = 1f;
            camera.farClipPlane = 400f;
            camera.fieldOfView = 72.7f;
        }
    }

    private void OnEnable()
    {
        if (controls == null) controls = new PlayerControl();
        controls.Enable();
    }

    private void OnDisable()
    {
        if (controls != null) controls.Disable();
    }

    private void Update()
    {
        GroundCheck();
        Move();
        Look();
        UpdateAnimations();
        UpdateCameraPosition();
    }

    private void GroundCheck()
    {
        // ✅ Lower the check sphere to ensure proper contact with flat surfaces
        float groundCheckOffset = 0.2f;
        Vector3 spherePosition = transform.position + Vector3.down * (controller.height / 2f - controller.radius - groundCheckOffset);

        isGrounded = Physics.CheckSphere(spherePosition, controller.radius * 0.95f, groundMask, QueryTriggerInteraction.Ignore);

        // Debug visualization
        Debug.Log($"GroundCheck: isGrounded = {isGrounded}, spherePos = {spherePosition}");

        // Reset downward velocity when grounded
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }

    private void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move = transform.TransformDirection(move.normalized);

        float speed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void Jump()
    {
        Debug.Log($"Jump attempted: grounded = {isGrounded}, crouching = {isCrouching}");

        if (isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jump executed! velocity.y = " + velocity.y);

            // Play jump animation
            if (animator != null)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            Debug.Log("Jump failed: not grounded or crouching");
        }
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? 1f : 2f;
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        // Smooth camera transition
        targetCameraY = isCrouching ? 0.785f : 1.57f;

        if (animator != null)
            animator.SetBool("IsCrouching", isCrouching);
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveInput != Vector2.zero;
        float targetSpeed = 0f;

        if (isMoving)
        {
            float moveSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
            targetSpeed = moveSpeed * moveInput.magnitude;
        }

        animSpeedSmooth = Mathf.Lerp(animSpeedSmooth, targetSpeed, Time.deltaTime * animSmoothTime);

        animator.SetFloat("Speed", animSpeedSmooth);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
    }

    private void UpdateCameraPosition()
    {
        if (cameraTransform != null)
        {
            Vector3 currentPos = cameraTransform.localPosition;
            float newY = Mathf.Lerp(currentPos.y, targetCameraY, Time.deltaTime / cameraSmoothTime);
            cameraTransform.localPosition = new Vector3(currentPos.x, newY, currentPos.z);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;

        float groundCheckOffset = 0.2f;
        Vector3 spherePosition = transform.position + Vector3.down * (controller.height / 2f - controller.radius - groundCheckOffset);
        Gizmos.DrawWireSphere(spherePosition, controller.radius * 0.95f);
    }
#endif
}