using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlayerLives : MonoBehaviour
{
    [Header("Lives Settings")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Respawn Settings")]
    public Vector2 respawnPosition = Vector2.zero;
    public float respawnDelay = 0.5f;

    [Header("Invulnerability Settings")]
    public float invulnDuration = 1.0f; // optional: nach Respawn unverwundbar
    private bool isRespawning = false;
    private bool isInvulnerable = false;

    private SpriteRenderer sr;
    private Collider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentLives = maxLives;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable || isRespawning) return;

        if (other.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        if (isRespawning) return;

        currentLives--;
        Debug.Log($"Player hit! Lives left: {currentLives}");

        if (currentLives > 0)
            StartCoroutine(RespawnPlayer());
        else
            GameOver();
    }

    private System.Collections.IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // Deaktiviere kurz Sichtbarkeit und Kollision
        sr.enabled = false;
        col.enabled = false;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPosition;

        // Wieder sichtbar + Kollision aktivieren
        sr.enabled = true;
        col.enabled = true;

        // Kurz unverwundbar (z. B. Blinken)
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
        Debug.Log("GAME OVER!");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }
}
