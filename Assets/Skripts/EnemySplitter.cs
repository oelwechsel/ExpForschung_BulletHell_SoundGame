using UnityEngine;

public class EnemySplitter : EnemyBase
{
    [Header("Split Settings")]
    public GameObject orbitCenterPrefab;
    public GameObject orbitingEnemyPrefab;

    private void Start()
    {
        moveSpeed = 3.5f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Gegner dreht sich leicht während der Bewegung
        transform.Rotate(Vector3.forward, 100f * Time.fixedDeltaTime);
    }

    protected override void OnDeath()
    {
        if (orbitCenterPrefab == null || orbitingEnemyPrefab == null)
            return;

        // Spawn des Orbit-Systems
        Instantiate(orbitCenterPrefab, transform.position, Quaternion.identity);
    }
}
