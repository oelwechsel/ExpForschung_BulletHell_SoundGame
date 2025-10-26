using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WavePattern", menuName = "Waves/Wave Pattern")]
public class WavePattern : ScriptableObject
{
    [Header("General")]
    public Vector2 startPosition = Vector2.zero;
    public float spacing = 1f;
    [HideInInspector]public float rotationOffset = 0f;
    [HideInInspector]public float scale = 1f;
    public int triggerRemainingEnemies = 0;



    [Header("Randomization")]
    public bool randomSpawnArea = false;
    [Tooltip("Wenn true, wird der Spielerbereich beim Random-Spawnen vermieden")] public bool avoidPlayer = false;
    [Tooltip("-360 bis 360 by default")] public float maxRandomRotation = 360f; // volle 360° Rotation möglich



    [Header("Randomization - Optional")]
    public Vector2 randomSpawnRange = new Vector2(20f, 10f); // X=20, Y=10
    [Tooltip("Maximale zufällige Verschiebung des Patterns, wenn RandomSpawnArea false ist")] public float maxRandomOffset = 0f;
    [Tooltip("Mindestabstand zum Spieler, falls avoidPlayer true ist")] public float avoidPlayerRadius = 5f; // Standardwert




    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject prefab;
        public Vector2 offset;      // lokal in Grid/Einheiten
        public float delay;         // optionale Spawn-Delay
        public float individualScale = 1f;
    }

    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    // Berechnet Mittelpunkt aller Enemy-Offsets (lokal)
    public Vector2 GetCenter()
    {
        if (enemies.Count == 0) return Vector2.zero;
        Vector2 sum = Vector2.zero;
        foreach (var e in enemies) sum += e.offset;
        return sum / enemies.Count;
    }

    // Utility: Array aller Offsets (vor Rotation/Scale)
    public Vector2[] GetOffsetsArray()
    {
        Vector2[] arr = new Vector2[enemies.Count];
        for (int i = 0; i < enemies.Count; i++) arr[i] = enemies[i].offset;
        return arr;
    }
}
