using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Wave Patterns")]
    public List<WavePattern> wavePatterns = new List<WavePattern>();

    [Header("Enemy Prefabs")]
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;

    private int currentPatternIndex = 0;
    public List<GameObject> activeEnemies = new List<GameObject>();
    private Dictionary<char, GameObject> symbolMap;

    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton initialisieren
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        symbolMap = new Dictionary<char, GameObject>()
    {
        {'A', enemyA },
        {'B', enemyB },
        {'C', enemyC }
    };
    }

    private void Update()
    {
        if (!LevelManager.Instance.isLevelActive) return;

        if (currentPatternIndex < wavePatterns.Count)
        {
            if (CountAliveEnemies() <= wavePatterns[currentPatternIndex].triggerRemainingEnemies)
            {
                SpawnNextPattern();
            }
        }
    }

    private void SpawnNextPattern()
    {
        if (currentPatternIndex >= wavePatterns.Count) return;

        WavePattern wave = wavePatterns[currentPatternIndex];
        List<GameObject> newEnemies = SpawnPatternFromText(wave);

        activeEnemies.AddRange(newEnemies);
        currentPatternIndex++;
    }

    private int CountAliveEnemies()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count;
    }

    private List<GameObject> SpawnPatternFromText(WavePattern wave)
    {
        List<GameObject> spawnedEnemies = new List<GameObject>();
        if (wave.patternFile == null) return spawnedEnemies;

        string[] lines = wave.patternFile.text.Split('\n');

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y].Trim();
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                if (symbolMap.ContainsKey(c) && symbolMap[c] != null)
                {
                    Vector2 pos = wave.startPosition + new Vector2(x * wave.spacing, -y * wave.spacing);
                    pos = RotatePoint(pos - wave.startPosition, wave.rotationOffset) + wave.startPosition;

                    spawnedEnemies.Add(Instantiate(symbolMap[c], pos, Quaternion.identity));
                }
            }
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

    public void StartCurrentPattern()
    {
        // Zerstöre alte Gegner
        foreach (var enemy in activeEnemies)
            if (enemy != null) Destroy(enemy);

        activeEnemies.Clear();
        currentPatternIndex = 0;

        if (wavePatterns.Count > 0)
            SpawnNextPattern();
    }

}
