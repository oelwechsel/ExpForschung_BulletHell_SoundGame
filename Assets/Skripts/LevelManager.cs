
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameState
{
    StartScreen,
    Playing,
    LevelComplete,
    GameOver,
    EndScreen
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("UI Screens")]
    public GameObject startScreen;
    public GameObject levelCompleteScreen;
    public GameObject endScreen;

    public Button startButton;
    public Button nextLevelButton;

    [Header("Audio")]
    public AudioSource musicSource;
    public List<AudioClip> tracks; // 3 Tracks

    [Header("Level Settings")]
    public float levelDuration = 120f; // 2 Minuten pro Runde

    public bool shuffleSpawnPatternsInLevel = false;

    private List<int> remainingTracks;
    private int currentLevel = 0;

    public bool isLevelActive = false;


    public float currentTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        // Alle Tracks verf�gbar
        remainingTracks = new List<int> { 0, 1, 2 };

        startButton.onClick.AddListener(StartLevel);
        nextLevelButton.onClick.AddListener(StartLevel);

        ShowStartScreen();

    }

    private void ShowStartScreen()
    {
        isLevelActive = false;
        startScreen.SetActive(true);
        levelCompleteScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    private void StartLevel()
    {
        startScreen.SetActive(false);
        levelCompleteScreen.SetActive(false);
        endScreen.SetActive(false);

        // Wenn keine Tracks mehr übrig sind → Endscreen
        if (remainingTracks.Count == 0)
        {
            ShowEndScreen();
            return;
        }

        int trackIndex;

        // Beim ersten Level IMMER Track 0 spielen
        if (currentLevel == 0)
        {
            trackIndex = 0;
            remainingTracks.Remove(trackIndex);
        }
        else
        {
            // Danach zufällig aus den restlichen Tracks wählen
            trackIndex = remainingTracks[Random.Range(0, remainingTracks.Count)];
            remainingTracks.Remove(trackIndex);
        }

        // Musik starten
        musicSource.clip = tracks[trackIndex];
        musicSource.Play();

        DataCollector.Instance.Set("Track Name", musicSource.clip.name);

        // Level starten
        currentLevel++;
        isLevelActive = true;

        PlayerLives.Instance.ResetLives();

        if (shuffleSpawnPatternsInLevel)
            PatternSpawner.Instance.ShufflePatterns();

        PatternSpawner.Instance.StartFirstWave();

        StartCoroutine(RunLevel());
    }

    private System.Collections.IEnumerator RunLevel()
    {
        float timer = 0f;
        currentTimer = timer;
        bool levelEnded = false;

        while (!levelEnded)
        {
            timer += Time.deltaTime;
            currentTimer = timer;

            // Level endet, wenn Spieler alle Leben verloren hat
            if (PlayerLives.Instance.CurrentLives <= 0)
            {
                levelEnded = true;
                DataCollector.Instance.Set("HasPlayerSurvived", false);
                DataCollector.Instance.Set("Lifes left", PlayerLives.Instance.CurrentLives);
                DataCollector.Instance.Set("Time Survived", (int)timer);
                DataCollector.Instance.Set("Enemy in Close Range Detected", PlayerController.Instance.closeRangeEnemyCounter);
                PlayerController.Instance.closeRangeEnemyCounter = 0;
                DataCollector.Instance.Set("Total Bullets Fired", PlayerController.Instance.bulletCounter);
                PlayerController.Instance.bulletCounter = 0;
                DataCollector.Instance.Set($"Distance traveled", PlayerController.Instance.GetTotalDistanceMoved());
            }
            else if (PlayerLives.Instance.CurrentLives == 3)
            {
                DataCollector.Instance.Set("Player Lost his first life at time", "--------");
                DataCollector.Instance.Set("Player Lost his second life at time", "--------");
                DataCollector.Instance.Set("Player Lost his last life at time", "--------");
            }


            // Level endet, wenn Zeit abgelaufen ist
            if (timer >= levelDuration)
            {
                levelEnded = true;
                DataCollector.Instance.Set("HasPlayerSurvived", true);
                DataCollector.Instance.Set("Lifes left", PlayerLives.Instance.CurrentLives);
                DataCollector.Instance.Set("Time Survived", (int)timer);
                DataCollector.Instance.Set("Enemy in Close Range Detected", PlayerController.Instance.closeRangeEnemyCounter);
                PlayerController.Instance.closeRangeEnemyCounter = 0;
                DataCollector.Instance.Set("Total Bullets Fired", PlayerController.Instance.bulletCounter);
                PlayerController.Instance.bulletCounter = 0;
                DataCollector.Instance.Set($"Distance traveled", PlayerController.Instance.GetTotalDistanceMoved());

            }
            
            yield return null;
        }

        
        DataCollector.Instance.SaveLevelData(currentLevel);

        

        // Musik stoppen
        musicSource.Stop();

        // Spieler wieder volle Leben f�r n�chsten Level
        PlayerLives.Instance.ResetLives();

        // Pr�fen, ob noch Tracks �brig sind
        if (remainingTracks.Count > 0)
        {
            ShowLevelCompleteScreen();
        }
        else
        {
            ShowEndScreen();
        }
    }


    private void ShowLevelCompleteScreen()
    {
        isLevelActive = false;
        levelCompleteScreen.SetActive(true);
    }

    private void ShowEndScreen()
    {
        isLevelActive = false;
        endScreen.SetActive(true);
    }
}
