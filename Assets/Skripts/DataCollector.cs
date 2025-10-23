using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance { get; private set; }

    private Dictionary<string, object> data = new Dictionary<string, object>();
    private string filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "GameData.txt");

        // Überschreibt die Datei beim Start der Session
        File.WriteAllText(filePath, "=== GAME SESSION DATA ===\n\n");

        Debug.Log($"📁 DataCollector: Speichert Daten unter {filePath}");
    }

    public void Set(string key, object value)
    {
        data[key] = value;
    }

    public object Get(string key)
    {
        if (data.TryGetValue(key, out var val))
            return val;
        return null;
    }

    public void ClearData()
    {
        data.Clear();
    }

    public void SaveLevelData(int levelNumber)
    {
        if (data.Count == 0)
        {
            Debug.LogWarning("DataCollector: Keine Daten zum Speichern!");
            return;
        }

        var sortedKeys = new List<string>(data.Keys);
        sortedKeys.Sort(System.StringComparer.OrdinalIgnoreCase);

        StringBuilder txt = new StringBuilder();
        txt.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        txt.AppendLine($"      LEVEL {levelNumber} REPORT");
        txt.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        foreach (var key in sortedKeys)
        {
            txt.AppendLine($"{key.PadRight(25)} : {data[key]}");
        }

        // Zwei Leerzeilen zwischen den Levels
        txt.AppendLine();
        txt.AppendLine();

        File.AppendAllText(filePath, txt.ToString());

        Debug.Log($"📁 DataCollector: Level {levelNumber} Daten gespeichert!");
    }
}
