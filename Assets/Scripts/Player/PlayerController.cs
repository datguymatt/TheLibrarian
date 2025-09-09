using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;
    public float jumpForce = 6f;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;
    public float idleDampFactor = 10f;

    [Header("Air Control")]
    public float airControlMultiplier = 0.3f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float verticalLookLimit = 80f;

    [Header("Jump Assist")]
    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.2f;

    [Header("Step Climbing")]
    public float stepHeight = 0.4f;
    public float stepCheckDistance = 0.4f;
    public LayerMask stepLayer;

    [Header("Crouch")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchCameraOffset = -0.5f;
    public float crouchSmooth = 8f;

    [Header("Head Bob")]
    public float bobSpeed = 8f;
    public float bobAmount = 0.05f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    public float slideSpeed = 5f;

    [Header("Controls")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Smoothing")]
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Jump Control")]
    public float jumpCutMultiplier = 0.5f;

    [Header("Camera Tilt")]
    public float tiltAmount = 5f;
    public float tiltSmooth = 5f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private bool isGrounded;
    private bool isCrouching = false;
    private float verticalRotation = 0f;
    private Vector2 inputDir;

    private float lastGroundedTime;
    private float lastJumpPressedTime;

    // Head bob
    private float bobTimer = 0f;
    private Vector3 defaultCamPos;

    // For slope detection
    private Vector3 groundNormal;

    // Landing detection
    private bool wasGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb.freezeRotation = true;

        defaultCamPos = playerCamera.localPosition;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleJumpBuffering();
        HandleCrouch();
        HandleCameraTilt();

        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Variable jump height
        if (Input.GetKeyUp(jumpKey) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier, rb.linearVelocity.z);
        }
    }

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;

        CheckGroundStatus();

        if (!wasGrounded && isGrounded)
        {
            OnLand();
        }

        HandleMovement();
        HandleStepClimb();
        HandleSlopeSliding();
    }

    private void HandleMovement()
    {
        float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(runKey) ? runSpeed : walkSpeed);

        Vector3 moveInput = new Vector3(inputDir.x, 0f, inputDir.y).normalized;
        Vector3 moveDir = transform.TransformDirection(moveInput);

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        if (isGrounded)
        {
            Vector3 targetVelocity = moveDir * currentSpeed;
            Vector3 velocityChange = targetVelocity - horizontalVelocity;

            float accel = (targetVelocity.magnitude > 0.1f) ? acceleration : deceleration;
            velocityChange = Vector3.ClampMagnitude(velocityChange, accel * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(horizontalVelocity.x + velocityChange.x, currentVelocity.y, horizontalVelocity.z + velocityChange.z);
        }
        else
        {
            // Air control
            Vector3 airMovement = moveDir * currentSpeed * airControlMultiplier;
            Vector3 newHorizontalVelocity = Vector3.Lerp(horizontalVelocity, airMovement, Time.fixedDeltaTime * 2f);
            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
        }

        HandleHeadBob(moveInput, currentSpeed);
    }

    private void HandleJumpBuffering()
    {
        if (Input.GetKeyDown(jumpKey))
            lastJumpPressedTime = Time.time;

        if ((Time.time - lastGroundedTime <= coyoteTime) &&
            (Time.time - lastJumpPressedTime <= jumpBufferTime))
        {
            Jump();
            lastJumpPressedTime = -999f;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckGroundStatus()
    {
        Vector3 spherePos = transform.position + Vector3.up * 0.1f;
        float checkRadius = col.radius * 0.9f;

        isGrounded = Physics.CheckSphere(spherePos + Vector3.down * (col.height / 2f), checkRadius, groundLayer);

        if (isGrounded)
        {
            lastGroundedTime = Time.time;

            if (Physics.Raycast(spherePos, Vector3.down, out RaycastHit hit, groundCheckDistance + 1f, groundLayer))
            {
                groundNormal = hit.normal;
            }
            else
            {
                groundNormal = Vector3.up;
            }
        }
        else
        {
            groundNormal = Vector3.up;
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.localEulerAngles = new Vector3(verticalRotation, 0f, playerCamera.localEulerAngles.z);
    }

    private void HandleCameraTilt()
    {
        float targetTilt = -inputDir.x * tiltAmount;
        float currentTilt = playerCamera.localEulerAngles.z;
        if (currentTilt > 180) currentTilt -= 360;

        float tilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmooth);
        playerCamera.localEulerAngles = new Vector3(verticalRotation, 0f, tilt);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;

            float targetHeight = isCrouching ? crouchHeight : standHeight;
            col.height = targetHeight;
            col.center = new Vector3(0f, targetHeight / 2f, 0f);
        }

        Vector3 targetCamPos = defaultCamPos;
        if (isCrouching)
            targetCamPos += new Vector3(0f, crouchCameraOffset, 0f);

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetCamPos, Time.deltaTime * crouchSmooth);
    }

    private void HandleHeadBob(Vector3 moveInput, float speed)
    {
        if (!isGrounded || moveInput.magnitude < 0.1f)
        {
            bobTimer = 0f;
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, defaultCamPos + (isCrouching ? new Vector3(0f, crouchCameraOffset, 0f) : Vector3.zero), Time.deltaTime * 10f);
            return;
        }

        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmount * (speed / runSpeed);

        Vector3 targetPos = defaultCamPos + new Vector3(0f, bobOffset, 0f);
        if (isCrouching) targetPos += new Vector3(0f, crouchCameraOffset, 0f);

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetPos, Time.deltaTime * 10f);
    }

    private void OnLand()
    {
        // Simple landing dip effect
        playerCamera.localPosition += Vector3.down * 0.1f;
    }

    private void HandleStepClimb()
    {
        if (!isGrounded) return;

        RaycastHit hitLower;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Vector3 forward = transform.forward;

        if (Physics.Raycast(origin, forward, out hitLower, stepCheckDistance, stepLayer))
        {
            Vector3 upperOrigin = transform.position + Vector3.up * stepHeight;
            if (!Physics.Raycast(upperOrigin, forward, stepCheckDistance, stepLayer))
            {
                rb.position += Vector3.up * stepHeight;
            }
        }
    }

    private void HandleSlopeSliding()
    {
        if (!isGrounded) return;

        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);

        if (slopeAngle > maxSlopeAngle)
        {
            Vector3 slideDirection = new Vector3(groundNormal.x, -groundNormal.y, groundNormal.z);
            rb.AddForce(slideDirection.normalized * slideSpeed, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.yellow;
        Vector3 lower = transform.position + Vector3.up * 0.1f;
        Vector3 upper = transform.position + Vector3.up * stepHeight;
        Gizmos.DrawLine(lower, lower + transform.forward * stepCheckDistance);
        Gizmos.DrawLine(upper, upper + transform.forward * stepCheckDistance);
    }
}

