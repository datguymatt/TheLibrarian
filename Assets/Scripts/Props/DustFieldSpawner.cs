using UnityEngine;

public class DustFieldSpawner : MonoBehaviour
{
    [Header("Dust Settings")]
    public GameObject dustPrefab;
    public int dustCount = 50;
    public Vector3 fieldSize = new Vector3(10, 5, 10);

    [Header("Floating Settings Override")]
    public float minSpeed = 0.2f;
    public float maxSpeed = 1.0f;
    public float moveRadius = 0.5f;
    public float smoothness = 2.0f;

    [Header("Scale Variation")]
    public float minScale = 0.05f;
    public float maxScale = 0.2f;

    void Start()
    {
        for (int i = 0; i < dustCount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-fieldSize.x / 2, fieldSize.x / 2),
                Random.Range(-fieldSize.y / 2, fieldSize.y / 2),
                Random.Range(-fieldSize.z / 2, fieldSize.z / 2)
            );

            GameObject dust = Instantiate(dustPrefab, randomPos, Quaternion.identity, transform);

            // Random scale
            float scale = Random.Range(minScale, maxScale);
            dust.transform.localScale = new Vector3(scale, scale, scale);

            // Apply overrides if FloatingDust is attached
            FloatingDust floating = dust.GetComponent<FloatingDust>();
            if (floating != null)
            {
                floating.minSpeed = minSpeed;
                floating.maxSpeed = maxSpeed;
                floating.moveRadius = moveRadius;
                floating.smoothness = smoothness;
            }
        }
    }
}
