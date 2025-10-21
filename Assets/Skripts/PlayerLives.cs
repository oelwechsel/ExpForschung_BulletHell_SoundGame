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
    public float invulnDuration = 1.0f;
    private bool isRespawning = false;
    private bool isInvulnerable = false;

    [Header("Visual Effects")]
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;

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
            TakeHit();
        }
    }

    public void TakeHit()
    {
        if (isInvulnerable || isRespawning) return;

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
        Debug.Log($"Player hit! Lives left: {currentLives}");

        if (currentLives > 0)
            StartCoroutine(RespawnPlayer());
        else
            GameOver();
    }

    private System.Collections.IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // Deaktiviere Sichtbarkeit & Kollision
        sr.enabled = false;
        col.enabled = false;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn in Mitte
        transform.position = respawnPosition;

        // Wieder sichtbar & aktiv
        sr.enabled = true;
        col.enabled = true;

        // Kurze Unverwundbarkeit mit Blinken
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

        if (deathEffectPrefab != null)
        {
            GameObject deathFX = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathFX, 3f);
        }

        // Szene neu laden oder Spielobjekt zerstören
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }
}
