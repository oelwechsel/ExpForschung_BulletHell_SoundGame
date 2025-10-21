using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyBase : MonoBehaviour
{
    protected Transform player;
    protected Health health;
    protected Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    protected virtual void OnEnable()
    {
        health.OnDeath += OnDeath;
    }

    protected virtual void OnDisable()
    {
        health.OnDeath -= OnDeath;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        if (!player) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }

    protected virtual void OnDeath()
    {
        // Kann von abgeleiteten Klassen überschrieben werden
    }

    /*

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Zugriff auf das PlayerLives-Skript
            PlayerLives playerLives = other.GetComponent<PlayerLives>();
            if (playerLives != null)
            {
                playerLives.TakeHit(); // neue Methode im PlayerLives-Script
            }
        }
    }
    */


}
