using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlayerLives : MonoBehaviour
{
    [Header("Lives Settings")]
    public int maxLives = 3;
    private int currentLives;
    public int CurrentLives => currentLives;

    [Header("Respawn Settings")]
    public Vector2 respawnPosition = Vector2.zero;
    public float respawnDelay = 0.5f;

    [Header("Invulnerability Settings")]
    public float invulnDuration = 1.0f;
    private bool isRespawning = false;
    public bool isInvulnerable = false;

    public bool IsRespawning => isRespawning;

    [Header("Visual Effects")]
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;

    private SpriteRenderer sr;
    private Collider2D col;
    private Rigidbody2D rb;

    public static PlayerLives Instance { get; private set; }

    private void Awake()
    {
        // Singleton initialisieren
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentLives = maxLives;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        DataCollector.Instance.Set("Player Lost his first life at time", "--------");
        DataCollector.Instance.Set("Player Lost his second life at time", "--------");
        DataCollector.Instance.Set("Player Lost his last life at time", "--------");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable || isRespawning) return;

        if (other.CompareTag("Enemy"))
        {
            TakeHit();
        }
    }

    public void TakeHit()
    {
        if (!LevelManager.Instance.isLevelActive || isInvulnerable || isRespawning) return;

        if (hitEffectPrefab != null)
        {
            GameObject hitFX = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitFX, 2f);
        }

        LoseLife();
    }

    private void LoseLife()
    {
        if (isRespawning) return;

        currentLives--;



        if (currentLives == 2)
            DataCollector.Instance.Set("Player Lost his first life at time", (int)LevelManager.Instance.currentTimer);
        else if (currentLives == 1)
            DataCollector.Instance.Set("Player Lost his second life at time", (int)LevelManager.Instance.currentTimer);
        else if (currentLives == 0)
            DataCollector.Instance.Set("Player Lost his last life at time", (int)LevelManager.Instance.currentTimer);
            
        Debug.Log($"Player hit! Lives left: {currentLives}");

        if (currentLives > 0)
            StartCoroutine(RespawnPlayer());
        else
            GameOver();
    }

    private System.Collections.IEnumerator RespawnPlayer()
    {
        if (!LevelManager.Instance.isLevelActive)
        {
            isRespawning = false;
            yield break;
        }

        isRespawning = true;

        // 👇 Distanztracking pausieren & sichern
        PlayerController.Instance.PauseTrackingOnDeath();

        // Deaktiviere Sichtbarkeit & Kollision
        sr.enabled = false;
        col.enabled = false;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        transform.position = respawnPosition;
        PlayerController.Instance.lastPosition = transform.position;

        // Wieder sichtbar & aktiv
        sr.enabled = true;
        col.enabled = true;

        // Kurze Unverwundbarkeit
        StartCoroutine(InvulnerabilityBlink());

        isRespawning = false;
    }


    private System.Collections.IEnumerator InvulnerabilityBlink()
    {
        isInvulnerable = true;
        float timer = 0f;

        while (timer < invulnDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        sr.enabled = true;
        isInvulnerable = false;
    }

    private void GameOver()
    {
        Debug.Log("Level GAME OVER!");

        currentLives = 0;

        if (deathEffectPrefab != null)
        {
            GameObject deathFX = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathFX, 3f);
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;


        Debug.Log("Player lives reset------------------------------------------------.");
        // Spieler sichtbar machen und Collider aktivieren
        sr.enabled = true;
        col.enabled = true;
        rb.velocity = Vector2.zero;
        transform.position = respawnPosition;
        PlayerController.Instance.lastPosition = transform.position;
        PlayerController.Instance.totalDistanceMoved = 0f;
        PlayerController.Instance.accumulatedDistance = 0f;
        PlayerController.Instance.pathPositions.Clear();
        PlayerController.Instance.lineRenderer.positionCount = 0;
    }

}
