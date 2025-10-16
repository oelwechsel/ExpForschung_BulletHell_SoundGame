using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 6f;
    public float lifeTime = 4f;

    [Header("Type")]
    public bool isPlayerBullet = false;
    public bool canBeDestroyed = false; // only relevant for enemy bullets
    public string ownerTag; // "Player" or "Enemy"

    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public void Init(Vector2 dir, float speedOverride = -1f, bool fromPlayer = false, bool destroyable = false)
    {
        direction = dir.normalized;
        isPlayerBullet = fromPlayer;
        canBeDestroyed = destroyable;
        ownerTag = fromPlayer ? "Player" : "Enemy";

        if (speedOverride > 0f) speed = speedOverride;

        if (rb) rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent self-collisions (projectile hits its owner)
        if (other.CompareTag(ownerTag)) return;

        // Player projectile hits Enemy or EnemyProjectile
        if (isPlayerBullet)
        {
            if (other.CompareTag("Enemy"))
            {
                Health h = other.GetComponent<Health>();
                if (h != null) h.TakeDamage(1);
                Destroy(gameObject);
            }
            else if (other.CompareTag("EnemyProjectile"))
            {
                Bullet enemyB = other.GetComponent<Bullet>();
                if (enemyB != null && enemyB.canBeDestroyed)
                {
                    Destroy(enemyB.gameObject);
                    Destroy(gameObject);
                }
            }
        }
        else // Enemy bullet hits Player or PlayerProjectile
        {
            if (other.CompareTag("Player"))
            {
                Health h = other.GetComponent<Health>();
                if (h != null) h.TakeDamage(1);
                Destroy(gameObject);
            }
            else if (other.CompareTag("PlayerProjectile"))
            {
                Bullet playerB = other.GetComponent<Bullet>();
                if (playerB != null && playerB.isPlayerBullet)
                {
                    if (canBeDestroyed)
                    {
                        Destroy(gameObject);
                        Destroy(playerB.gameObject);
                    }
                }
            }
        }
    }
}
