using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public List<EnemySpawnPattern> patterns = new List<EnemySpawnPattern>();

    private int currentPatternIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        SpawnNextPattern();
    }

    private void Update()
    {
        // Prüfe, ob nächste Welle spawnen soll
        if (currentPatternIndex < patterns.Count)
        {
            EnemySpawnPattern pattern = patterns[currentPatternIndex];
            int aliveCount = CountAliveEnemies();

            if (aliveCount <= pattern.triggerRemainingEnemies)
            {
                SpawnNextPattern();
            }
        }
    }

    private void SpawnNextPattern()
    {
        if (currentPatternIndex >= patterns.Count) return;

        EnemySpawnPattern pattern = patterns[currentPatternIndex];
        List<GameObject> newEnemies = SpawnPattern(pattern);

        // Aktive Gegner speichern
        activeEnemies.AddRange(newEnemies);

        currentPatternIndex++;
    }

    private int CountAliveEnemies()
    {
        activeEnemies.RemoveAll(e => e == null); // entferne zerstörte Gegner
        return activeEnemies.Count;
    }

    private List<GameObject> SpawnPattern(EnemySpawnPattern pattern)
    {
        List<GameObject> spawnedEnemies = new List<GameObject>();
        Vector2 center = transform.position;

        switch (pattern.patternType)
        {
            case EnemyPatternType.Line:
                float totalWidth = (pattern.count - 1) * pattern.spacing;
                for (int i = 0; i < pattern.count; i++)
                {
                    float x = i * pattern.spacing - totalWidth / 2f;
                    Vector2 pos = center + RotatePoint(new Vector2(x, 0), pattern.rotationOffset);
                    spawnedEnemies.Add(Instantiate(pattern.enemyPrefab, pos, Quaternion.identity));
                }
                break;

            case EnemyPatternType.Circle:
                float angleStep = 360f / pattern.count;
                for (int i = 0; i < pattern.count; i++)
                {
                    float angle = i * angleStep + pattern.rotationOffset;
                    Vector2 pos = center + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * pattern.radius;
                    spawnedEnemies.Add(Instantiate(pattern.enemyPrefab, pos, Quaternion.identity));
                }
                break;

            case EnemyPatternType.XShape:
                int half = pattern.count / 2;
                for (int i = -half; i <= half; i++)
                {
                    float x = i * pattern.spacing;
                    float y = i * pattern.spacing;

                    Vector2 pos1 = center + RotatePoint(new Vector2(x, y), pattern.rotationOffset);
                    Vector2 pos2 = center + RotatePoint(new Vector2(x, -y), pattern.rotationOffset);

                    spawnedEnemies.Add(Instantiate(pattern.enemyPrefab, pos1, Quaternion.identity));
                    spawnedEnemies.Add(Instantiate(pattern.enemyPrefab, pos2, Quaternion.identity));
                }
                break;
        }

        return spawnedEnemies;
    }

    private Vector2 RotatePoint(Vector2 point, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
    }
}
