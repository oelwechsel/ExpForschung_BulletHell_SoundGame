using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyPatternType
{
    Line,
    Circle,
    XShape
}

[System.Serializable]
public class EnemySpawnPattern
{
    public EnemyPatternType patternType;
    public GameObject enemyPrefab;
    public int count = 5;        // Anzahl Gegner
    public float radius = 3f;    // für Kreis/X
    public float spacing = 1.5f; // für Linie/X
    public float rotationOffset = 0f; // Rotation
    public int triggerRemainingEnemies = 0; // Anzahl Gegner übrig, um nächste Welle zu spawnen
}

