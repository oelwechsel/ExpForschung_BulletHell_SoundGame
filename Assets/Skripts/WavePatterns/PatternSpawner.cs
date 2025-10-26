using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternSpawner : MonoBehaviour
{
    public static PatternSpawner Instance { get; private set; }

    [Header("Waves")]
    public List<WavePattern> wavePatterns = new List<WavePattern>();

    [Header("Runtime")]
    public int currentPatternIndex = 0;
    public List<GameObject> activeEnemies = new List<GameObject>();

    private bool hasStarted = false;
    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!hasStarted || !LevelManager.Instance.isLevelActive) return;
        if (currentPatternIndex >= wavePatterns.Count) return;

        activeEnemies.RemoveAll(e => e == null);

        WavePattern currentWave = wavePatterns[currentPatternIndex];

        if (!isSpawning && activeEnemies.Count <= currentWave.triggerRemainingEnemies)
        {
            StartCoroutine(SpawnNextPatternCoroutine(currentWave));
            currentPatternIndex++;
        }
    }

    public void ShufflePatterns()
    {
        int n = wavePatterns.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int j = Random.Range(i, n); // zufälliger Index ab i
            WavePattern temp = wavePatterns[i];
            wavePatterns[i] = wavePatterns[j];
            wavePatterns[j] = temp;
        }
        Debug.Log("WavePatterns have been shuffled!");
    }

    public void StartFirstWave()
    {
        StopAllCoroutines();
        foreach (var e in activeEnemies) if (e != null) Destroy(e);
        activeEnemies.Clear();
        currentPatternIndex = 0;
        hasStarted = true;

        if (wavePatterns.Count > 0)
        {
            StartCoroutine(SpawnNextPatternCoroutine(wavePatterns[0]));
            currentPatternIndex = 1;
        }
    }

    public void ResetSpawner()
    {
        StopAllCoroutines();
        foreach (var e in activeEnemies) if (e != null) Destroy(e);
        activeEnemies.Clear();
        currentPatternIndex = 0;
        hasStarted = false;
        isSpawning = false;
    }

    private IEnumerator SpawnNextPatternCoroutine(WavePattern wave)
    {
        if (wave == null) yield break;
        isSpawning = true;

        // Mittelpunkt der Enemy-Offsets (lokal)
        Vector2 center = wave.GetCenter() * wave.spacing;

        Vector2 spawnBasePosition;
        Vector2 randomOffset = Vector2.zero;

        if (wave.randomSpawnArea)
        {
            Vector2 candidatePos = Vector2.zero;
            Vector2 playerPos = PlayerController.Instance.transform.position;
            int tries = 0;
            const int maxTries = 50;

            do
            {
                float rx = Random.Range(-wave.randomSpawnRange.x / 2f, wave.randomSpawnRange.x / 2f);
                float ry = Random.Range(-wave.randomSpawnRange.y / 2f, wave.randomSpawnRange.y / 2f);
                candidatePos = new Vector2(rx, ry);
                tries++;
            }
            while (wave.avoidPlayer && Vector2.Distance(candidatePos, playerPos) < wave.avoidPlayerRadius && tries < maxTries);

            spawnBasePosition = candidatePos;
        }
        else
        {
            // Pattern an der startPosition
            spawnBasePosition = wave.startPosition;

            // Optional: RandomOffset um Pattern-Mittelpunkt
            if (wave.maxRandomOffset > 0f)
                randomOffset = Random.insideUnitCircle * wave.maxRandomOffset;
        }

        // Zufällige Rotation um Pattern-Mittelpunkt
        float randomRotation = Random.Range(-wave.maxRandomRotation, wave.maxRandomRotation);

        for (int i = 0; i < wave.enemies.Count; i++)
        {
            var e = wave.enemies[i];
            if (e.prefab == null) continue;

            // Lokale Position relativ zum Pattern-Mittelpunkt
            Vector2 localPos = (e.offset * wave.spacing - center) * wave.scale;

            // Rotation um Pattern-Mittelpunkt
            localPos = RotatePoint(localPos, wave.rotationOffset + randomRotation);

            // Weltposition: BasePosition + LocalPos + RandomOffset (falls aktiv)
            Vector2 worldPos = spawnBasePosition + localPos + randomOffset;

            if (e.delay > 0f)
                yield return new WaitForSeconds(e.delay);

            GameObject go = Instantiate(e.prefab, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity);
            go.transform.localScale *= e.individualScale;

            activeEnemies.Add(go);
        }

        isSpawning = false;
    }



    private Vector2 RotatePoint(Vector2 point, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
    }

}
