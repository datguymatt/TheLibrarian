using UnityEngine;

public class FloatingDust : MonoBehaviour
{
    [Header("Floating Settings")]
    public float minSpeed = 0.2f;
    public float maxSpeed = 1.0f;
    public float moveRadius = 0.5f;
    public float smoothness = 2.0f;

    [Header("Rotation Settings")]
    public float rotationSpeedMin = 5f;
    public float rotationSpeedMax = 20f;

    [Header("Fading Settings")]
    public float fadeDuration = 0;       // How long it takes to fade in/out
    public float visibleDuration = 5.0f;   // How long dust stays visible before fading out
    public float invisibleDuration = 3.0f; // How long dust stays invisible before fading back in

    private Vector3 startPos;
    private Vector3 targetPos;
    private float speed;
    private Vector3 rotationAxis;
    private float rotationSpeed;

    private Material matInstance;
    private float alpha = 0f;
    private float fadeTimer = 0f;
    private enum FadeState { FadingIn, Visible, FadingOut, Invisible }
    private FadeState state = FadeState.FadingIn;

    void Start()
    {
        //set fade
        fadeDuration = Random.Range(0.7f, 15f);

        startPos = transform.position;
        PickNewTarget();

        // Random rotation
        rotationAxis = Random.onUnitSphere;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        // Get material instance (avoid editing shared material)
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            matInstance = rend.material;
            SetAlpha(0f); // start invisible
        }
    }

    void Update()
    {
        // Floating drift
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            PickNewTarget();
        }

        // Gentle rotation
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        // Handle fading
        HandleFading();
    }

    void PickNewTarget()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius)
        );

        targetPos = startPos + randomOffset;
        speed = Random.Range(minSpeed, maxSpeed) / smoothness;
    }

    void HandleFading()
    {
        if (matInstance == null) return;

        fadeTimer += Time.deltaTime;

        switch (state)
        {
            case FadeState.FadingIn:
                alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                SetAlpha(alpha);
                if (alpha >= 1f)
                {
                    state = FadeState.Visible;
                    fadeTimer = 0f;
                }
                break;

            case FadeState.Visible:
                if (fadeTimer >= visibleDuration)
                {
                    state = FadeState.FadingOut;
                    fadeTimer = 0f;
                }
                break;

            case FadeState.FadingOut:
                alpha = 1f - Mathf.Clamp01(fadeTimer / fadeDuration);
                SetAlpha(alpha);
                if (alpha <= 0f)
                {
                    state = FadeState.Invisible;
                    fadeTimer = 0f;
                }
                break;

            case FadeState.Invisible:
                if (fadeTimer >= invisibleDuration)
                {
                    state = FadeState.FadingIn;
                    fadeTimer = 0f;
                }
                break;
        }
    }

    void SetAlpha(float a)
    {
        if (matInstance != null && matInstance.HasProperty("_Color"))
        {
            Color c = matInstance.color;
            c.a = a;
            matInstance.color = c;
        }
    }
}
