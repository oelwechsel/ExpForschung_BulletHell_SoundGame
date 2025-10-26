using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyShrink : EnemyBase
{
    private Vector3 originalScale;
    private int hitsTaken = 0;
    public int maxHits = 3;

    private void Start()
    {
     
        originalScale = transform.localScale;

        // Health so einstellen, dass er 3 Treffer überlebt
        health.maxHealth = maxHits;
    }

    public override void Move()
    {
        base.Move();
    }

    public void Shrink()
    {
        hitsTaken++;
        float scaleFactor = Mathf.Clamp01(1f - (float)hitsTaken / maxHits);
        transform.localScale = originalScale * scaleFactor;
    }

    public void TakeHit()
    {
        Shrink();
    }

    protected override void OnDeath()
    {
        // Hier könnten Partikeleffekte etc. hinzugefügt werden
    }
}
