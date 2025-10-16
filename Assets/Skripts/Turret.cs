using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireInterval = 1.0f;
    public float projectileSpeed = 5f;
    public float spawnOffset = 0.5f;
    public bool useDestroyableBullets = true;

    private Vector2[] directions = new Vector2[] {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Start()
    {
        tag = "Enemy";
        Health h = GetComponent<Health>();
        if (h == null)
        {
            h = gameObject.AddComponent<Health>();
            h.maxHealth = 1;
        }
        StartCoroutine(FireLoop());
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            FireAllFour();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireAllFour()
    {
        foreach (Vector2 d in directions)
        {
            Vector3 spawnPos = transform.position + (Vector3)(d * spawnOffset);
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            proj.tag = "EnemyProjectile";

            Bullet b = proj.GetComponent<Bullet>();
            if (b != null)
            {
                bool destroyable = useDestroyableBullets; // toggle type
                b.Init(d, projectileSpeed, false, destroyable);
            }
        }
    }
}
