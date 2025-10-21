using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public float spawnDuration = 0.5f;
    public GameObject spawnEffectPrefab;

    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (col) col.enabled = false;

        if (sr != null)
        {
            originalColor = sr.color;               // originale Farbe merken
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // nur Alpha 0
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        if (spawnEffectPrefab != null)
        {
            GameObject fx = Instantiate(spawnEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, spawnDuration + 0.1f);
        }

        float timer = 0f;
        while (timer < spawnDuration)
        {
            timer += Time.deltaTime;

            if (sr != null)
            {
                float alpha = Mathf.Clamp01(timer / spawnDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            yield return null;
        }

        if (col != null) col.enabled = true;
    }
}
