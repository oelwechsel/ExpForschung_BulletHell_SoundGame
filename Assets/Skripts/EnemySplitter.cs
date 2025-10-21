using UnityEngine;

public class EnemySplitter : EnemyBase
{
    [Header("Split Settings")]
    public GameObject smallEnemyPrefab;
    public int splitCount = 2;
    public float orbitSpeed = 100f;

    private void Start()
    {
        moveSpeed = 3.5f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Gegner dreht sich leicht während der Bewegung
        transform.Rotate(Vector3.forward, orbitSpeed * Time.fixedDeltaTime);
    }

    protected override void OnDeath()
    {
        if (smallEnemyPrefab == null) return;

        for (int i = 0; i < splitCount; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            GameObject small = Instantiate(smallEnemyPrefab, spawnPos, Quaternion.identity);
            EnemyBase e = small.GetComponent<EnemyBase>();
            if (e != null)
                e.moveSpeed = 1.5f; // kleinere, langsamere Version
        }
    }
}
