using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float fireRate = 0.15f;

    [Header("Close Range Settings")]
    [SerializeField] private float closeRangeRadius = 1.5f;
    [SerializeField] private string enemyTag = "Enemy";
    public int closeRangeEnemyCounter = 0;

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private float nextFireTime = 0f;

    // Referenz auf das Child-Objekt mit Collider
    private CircleCollider2D closeRangeCollider;

    private void Awake()
    {
         if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;

        SetupCloseRangeTrigger();
    }

    private void SetupCloseRangeTrigger()
    {
        // Neues Child für den Trigger erstellen
        GameObject triggerObj = new GameObject("CloseRangeTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.layer = gameObject.layer; // gleiche Layer für eventuelle Masken

        // CircleCollider2D hinzufügen
        closeRangeCollider = triggerObj.AddComponent<CircleCollider2D>();
        closeRangeCollider.isTrigger = true;
        closeRangeCollider.radius = closeRangeRadius;

        // Rigidbody2D hinzufügen (notwendig, damit TriggerEvents funktionieren)
        Rigidbody2D triggerRb = triggerObj.AddComponent<Rigidbody2D>();
        triggerRb.isKinematic = true;
        triggerRb.simulated = true;

        // Event-Weiterleitung (auf Child)
        triggerObj.AddComponent<PlayerCloseRangeTrigger>().Initialize(enemyTag);
    }

    private void Update()
    {
        if (!LevelManager.Instance.isLevelActive) return;

        HandleMovementInput();
        HandleAiming();
        HandleShooting();
    }

    private void FixedUpdate()
    {
        if (!LevelManager.Instance.isLevelActive) return;
        rb.velocity = moveInput * moveSpeed;
    }

    private void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
    }

    private void HandleAiming()
    {
        if (mainCam == null) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = mouseWorld - transform.position;

        if (aimDir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null || shootOrigin == null) return;
        if (!LevelManager.Instance.isLevelActive || PlayerLives.Instance.IsRespawning) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        Vector2 direction = transform.up;
        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.up;

        GameObject proj = Instantiate(projectilePrefab, shootOrigin.position, Quaternion.identity);
        Bullet bullet = proj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(direction, projectileSpeed, true, false);

        proj.tag = "PlayerProjectile";
    }

    // Wird von PlayerCloseRangeTrigger aufgerufen:
    public void EnemyEnteredRange()
    {
        closeRangeEnemyCounter++;
        Debug.Log($"🔸 Enemy entered close range! Count: {closeRangeEnemyCounter}");
    }

    public void EnemyLeftRange()
    {
        //closeRangeEnemyCounter = Mathf.Max(0, closeRangeEnemyCounter - 1);
        //Debug.Log($"🔹 Enemy left close range! Count: {closeRangeEnemyCounter}");
    }

    // Gizmos im Editor anzeigen
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, closeRangeRadius);
    }
}
