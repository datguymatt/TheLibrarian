using UnityEngine;
using UnityEngine.UI;

public class AlphaFluctuator : MonoBehaviour
{
    [Range(0f, 1f)] public float minAlpha = 0.3f;
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public float duration = 2f; // time for one full cycle

    [Tooltip("Controls how the alpha fluctuates between min and max")]
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Material _material;
    private Color _originalColor;
    private Graphic _uiGraphic; // For UI (Image, Text, etc.)

    void Start()
    {
        // Try UI first
        _uiGraphic = GetComponent<Graphic>();

        if (_uiGraphic != null)
        {
            _originalColor = _uiGraphic.color;
        }
        else
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                _material = renderer.material; // unique instance
                _originalColor = _material.color;
            }
            else
            {
                Debug.LogWarning("AlphaFluctuator: No Renderer or UI Graphic found on " + gameObject.name);
            }
        }
    }

    void Update()
    {
        if (_uiGraphic == null && _material == null) return;

        // Normalized time (0 → 1 → 0) loop
        float t = Mathf.PingPong(Time.time / duration, 1f);

        // Evaluate curve for smoothing, shaping, or pulsing
        float curvedT = alphaCurve.Evaluate(t);

        // Lerp alpha between min and max
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, curvedT);

        if (_uiGraphic != null)
        {
            Color newColor = _uiGraphic.color;
            newColor.a = alpha;
            _uiGraphic.color = newColor;
        }
        else
        {
            Color newColor = _material.color;
            newColor.a = alpha;
            _material.color = newColor;
        }
    }
}
