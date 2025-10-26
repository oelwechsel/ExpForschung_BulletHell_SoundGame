using UnityEngine;

// Einfaches MonoBehaviour, das ein WavePattern referenziert und im SceneView Punkte zeichnet.
[ExecuteAlways]
public class PatternPreview : MonoBehaviour
{
    public WavePattern pattern;
    public Color gizmoColor = Color.cyan;
    public float gizmoSize = 0.12f;

    private void OnDrawGizmos()
    {
        if (pattern == null) return;

        Gizmos.color = gizmoColor;
        for (int i = 0; i < pattern.enemies.Count; i++)
        {
            var e = pattern.enemies[i];
            Vector2 localPos = e.offset * pattern.spacing * pattern.scale;
            Vector2 rotated = RotatePoint(localPos, pattern.rotationOffset);
            Vector3 world = (Vector3)(pattern.startPosition + rotated);
            Gizmos.DrawSphere(world + transform.position, gizmoSize);

            // Draw label
#if UNITY_EDITOR
            UnityEditor.Handles.Label(world + transform.position + Vector3.up * 0.2f, i.ToString());
#endif
        }
    }

    private Vector2 RotatePoint(Vector2 point, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(point.x * cos - point.y * sin, point.x * sin + point.y * cos);
    }
}