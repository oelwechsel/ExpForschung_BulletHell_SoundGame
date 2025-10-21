using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public System.Action OnDeath;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        EnemyShrink shrink = GetComponent<EnemyShrink>();
        if (shrink != null)
            shrink.TakeHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
