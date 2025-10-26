#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class WavePatternEditor : EditorWindow
{
    private WavePattern targetPattern;
    private Vector2 scroll;

    [MenuItem("Window/Wave Pattern Editor")]
    public static void OpenWindow() => GetWindow<WavePatternEditor>("Wave Pattern Editor");

    private void OnEnable()
    {
        // Hook nur aktiv, wenn das Fenster offen ist
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Abmelden, wenn Fenster geschlossen wird
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave Pattern Editor", EditorStyles.boldLabel);

        // WavePattern auswählen
        targetPattern = (WavePattern)EditorGUILayout.ObjectField(
            "Pattern Asset",
            targetPattern,
            typeof(WavePattern),
            false
        );

        if (targetPattern == null)
        {
            EditorGUILayout.HelpBox(
                "Ziehen Sie ein WavePattern-Asset hierhin oder erstellen Sie eins im Project-Window.",
                MessageType.Info
            );
            return;
        }

        if (GUILayout.Button("Open Pattern in Scene (Select Preview GameObject)"))
        {
            var preview = GameObject.FindObjectOfType<PatternPreview>();
            if (preview == null)
            {
                var go = new GameObject("PatternPreview");
                preview = go.AddComponent<PatternPreview>();
                Selection.activeGameObject = go;
            }
            preview.pattern = targetPattern;
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Shift + Rechtsklick in SceneView fügt Punkte hinzu (nur solange dieses Fenster offen ist).",
            MessageType.Info
        );

        if (GUILayout.Button("Rotate +45°"))
        {
            Undo.RecordObject(targetPattern, "Rotate Pattern");
            targetPattern.rotationOffset += 45f;
            EditorUtility.SetDirty(targetPattern);
        }

        if (GUILayout.Button("Mirror X"))
        {
            Undo.RecordObject(targetPattern, "Mirror Pattern X");
            foreach (var e in targetPattern.enemies)
                e.offset.x *= -1f;
            EditorUtility.SetDirty(targetPattern);
        }

        if (GUILayout.Button("Clear Points"))
        {
            if (EditorUtility.DisplayDialog("Clear", "Alle Punkte entfernen?", "Ja", "Abbrechen"))
            {
                Undo.RecordObject(targetPattern, "Clear Pattern");
                targetPattern.enemies.Clear();
                EditorUtility.SetDirty(targetPattern);
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label($"Pattern Points (Count: {targetPattern.enemies.Count})", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(150));

        for (int i = 0; i < targetPattern.enemies.Count; i++)
        {
            var e = targetPattern.enemies[i];
            EditorGUILayout.BeginHorizontal();

            e.prefab = (GameObject)EditorGUILayout.ObjectField(
                e.prefab,
                typeof(GameObject),
                false,
                GUILayout.Width(70)
            );

            e.offset = EditorGUILayout.Vector2Field("Offset", e.offset);
            e.delay = EditorGUILayout.FloatField("Delay", e.delay, GUILayout.Width(120));

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.RecordObject(targetPattern, "Remove Point");
                targetPattern.enemies.RemoveAt(i);
                EditorUtility.SetDirty(targetPattern);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed) EditorUtility.SetDirty(targetPattern);
    }

    private void OnSceneGUI(SceneView sv)
    {
        if (targetPattern == null) return;

        Event e = Event.current;
        if (e == null) return;

        // Nur Shift + Rechtsklick
        if (e.type == EventType.MouseDown && e.button == 1 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 world;

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                world = hit.point;
            else
            {
                Plane p = new Plane(Vector3.forward, Vector3.zero);
                if (!p.Raycast(ray, out float enter)) return;
                world = ray.GetPoint(enter);
            }

            AddPointToPattern(targetPattern, world);
            e.Use(); // Nur dieses Event blockieren
        }
    }

    private static void AddPointToPattern(WavePattern pattern, Vector3 worldPos)
    {
        Undo.RecordObject(pattern, "Add Pattern Point");

        var e = new WavePattern.EnemySpawn();
        Vector2 local = (Vector2)(worldPos - (Vector3)pattern.startPosition);
        e.offset = local / pattern.spacing; // als Grid-Einheiten speichern
        pattern.enemies.Add(e);

        // Optional: Startposition automatisch auf Mittelpunkt der Enemies setzen
        Vector2 center = pattern.GetCenter();
        pattern.startPosition += center * pattern.spacing;

        // Alle Offsets um den neuen Mittelpunkt verschieben
        for (int i = 0; i < pattern.enemies.Count; i++)
            pattern.enemies[i].offset -= center;

        EditorUtility.SetDirty(pattern);
    }

}
#endif
