using UnityEngine;

public class EnemyOrbitCenter : MonoBehaviour
{
    public GameObject orbitingEnemyPrefab;
    public float orbitRadius = 1.2f;
    public float orbitSpeed = 90f;
    public float moveSpeed = 1.5f;

    private Transform player;
    private GameObject enemyA, enemyB;
    private float currentAngle = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Zwei Gegner im Kreis spawnen
        enemyA = Instantiate(orbitingEnemyPrefab, transform.position + Vector3.right * orbitRadius, Quaternion.identity);
        enemyB = Instantiate(orbitingEnemyPrefab, transform.position - Vector3.right * orbitRadius, Quaternion.identity);

        PatternSpawner.Instance.activeEnemies.Add(enemyA);
        PatternSpawner.Instance.activeEnemies.Add(enemyB);
    }

    private void Update()
    {
        if (!player)
        {
            Destroy(gameObject);
            return;
        }

        // Mittelpunkt bewegt sich leicht in Richtung Spieler
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        // Orbit-Bewegung aktualisieren
        currentAngle += orbitSpeed * Time.deltaTime;

        if (enemyA && enemyB)
        {
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector2 posA = (Vector2)transform.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
            Vector2 posB = (Vector2)transform.position + new Vector2(Mathf.Cos(rad + Mathf.PI), Mathf.Sin(rad + Mathf.PI)) * orbitRadius;

            enemyA.transform.position = posA;
            enemyB.transform.position = posB;

            // Optional: Gegner sollen den Spieler anschauen
            Vector2 dirToPlayer = (player.position - enemyA.transform.position).normalized;
            float angleA = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;
            enemyA.transform.rotation = Quaternion.Euler(0, 0, angleA);

            Vector2 dirToPlayerB = (player.position - enemyB.transform.position).normalized;
            float angleB = Mathf.Atan2(dirToPlayerB.y, dirToPlayerB.x) * Mathf.Rad2Deg - 90f;
            enemyB.transform.rotation = Quaternion.Euler(0, 0, angleB);
        }

        // Optional: Orbit aufl�sen, wenn beide Gegner zerst�rt sind
        if (enemyA == null && enemyB == null)
            Destroy(gameObject);
    }
}
