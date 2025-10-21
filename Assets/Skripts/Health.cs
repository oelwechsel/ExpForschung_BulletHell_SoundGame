using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public System.Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Spezielles Verhalten f�r Shrink-Gegner
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
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
