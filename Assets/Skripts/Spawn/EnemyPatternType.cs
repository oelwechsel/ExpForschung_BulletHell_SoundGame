using UnityEngine;

[System.Serializable]
public class WavePattern
{
    public TextAsset patternFile;         // ASCII-Pattern
    public Vector2 startPosition = Vector2.zero;
    public float spacing = 1.5f;
    public float rotationOffset = 0f;
    public int triggerRemainingEnemies = 0; // wann nächste Welle spawnt
}
