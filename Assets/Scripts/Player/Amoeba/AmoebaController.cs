using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AmoebaController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 5f;
    public float deceleration = 2f;

    [Header("Burst Settings")]
    public float burstForce = 10f;
    public float burstDuration = 0.3f;
    public float burstCooldown = 2f;
    [Range(0f, 1f)] public float rareBurstChance = 0.2f;
    public float rareBurstMultiplier = 1.5f;

    [Header("Progression Settings")]
    public int amoebasToUnlockFreeSwim = 10;
    private int amoebasEaten = 0;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;

    [Header("Camera Effects")]
    public Transform cameraTransform;       // Assign your camera here in Inspector
    public float stretchAmount = 0.1f;      // How much to stretch
    public float stretchSpeed = 5f;         // How fast it stretches and returns

    private Rigidbody rb;
    private Vector3 targetVelocity;

    private bool canFreeSwim = false;
    private bool isBursting = false;
    private float burstTimer = 0f;
    private float cooldownTimer = 0f;

    private Vector3 defaultCamScale;
    private Vector3 stretchedCamScale;
    private bool isRareBurst = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Physics.gravity = new Vector3(0, -2, 0);

        if (cameraTransform != null)
        {
            defaultCamScale = cameraTransform.localScale;
            stretchedCamScale = defaultCamScale + new Vector3(0, 0, stretchAmount);
        }
    }

    void Update()
    {
        HandleRotation();

        if (canFreeSwim)
        {
            HandleMovementInput();
        }
        else
        {
            HandleBurstInput();
        }

        // Timers
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (isBursting)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f)
            {
                isBursting = false;
                isRareBurst = false; // Reset effect flag
            }
        }

        HandleCameraStretch();
    }

    void FixedUpdate()
    {
        if (canFreeSwim)
        {
            ApplyMovement();
        }
        else
        {
            ApplyFloatyDrag();
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX, Space.World);
        transform.Rotate(Vector3.left * mouseY, Space.Self);
    }

    void HandleMovementInput()
    {
        Vector3 input = GetInputDirection();
        targetVelocity = input * moveSpeed;
    }

    void HandleBurstInput()
    {
        Vector3 inputDir = GetInputDirection();

        if (inputDir.magnitude > 0.1f && cooldownTimer <= 0f && !isBursting)
        {
            float duration = burstDuration;
            isRareBurst = false;

            if (Random.value < rareBurstChance)
            {
                duration *= rareBurstMultiplier;
                isRareBurst = true; // mark this as a rare burst
            }

            rb.linearVelocity = inputDir * burstForce;
            isBursting = true;
            burstTimer = duration;
            cooldownTimer = burstCooldown;
        }
    }

    Vector3 GetInputDirection()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) input += transform.forward;
        if (Input.GetKey(KeyCode.S)) input -= transform.forward;
        if (Input.GetKey(KeyCode.A)) input -= transform.right;
        if (Input.GetKey(KeyCode.D)) input += transform.right;
        if (Input.GetKey(KeyCode.Space)) input += transform.up;
        if (Input.GetKey(KeyCode.LeftControl)) input -= transform.up;

        return input.normalized;
    }

    void ApplyMovement()
    {
        Vector3 velocity = rb.linearVelocity;

        if (targetVelocity.magnitude > 0.1f)
        {
            velocity = Vector3.Lerp(velocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        rb.linearVelocity = velocity;
    }

    void ApplyFloatyDrag()
    {
        if (!isBursting)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }
    }

    void HandleCameraStretch()
    {
        if (cameraTransform == null) return;

        if (isRareBurst && isBursting)
        {
            // Stretch smoothly during burst
            cameraTransform.localScale = Vector3.Lerp(cameraTransform.localScale, stretchedCamScale, Time.deltaTime * stretchSpeed);
        }
        else
        {
            // Return to normal
            cameraTransform.localScale = Vector3.Lerp(cameraTransform.localScale, defaultCamScale, Time.deltaTime * stretchSpeed);
        }
    }

    public void EatAmoeba()
    {
        amoebasEaten++;
        if (amoebasEaten >= amoebasToUnlockFreeSwim)
        {
            canFreeSwim = true;
        }
    }
}
