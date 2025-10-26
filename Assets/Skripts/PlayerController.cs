using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;

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



    public int bulletCounter = 0;

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private float nextFireTime = 0f;
    private CircleCollider2D closeRangeCollider;

    // --- Movement Tracking ---
    public Vector2 lastPosition;
    public float totalDistanceMoved = 0f;

    // Distanz, die bereits in früheren „Leben“ gesammelt wurde
    public float accumulatedDistance = 0f;

    // --- Path Visualization ---
    [HideInInspector]
    public LineRenderer lineRenderer;
    public List<Vector3> pathPositions = new List<Vector3>();
    [SerializeField] private float minDistanceBetweenPoints = 0.2f;

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
        SetupPathRenderer();

        lastPosition = transform.position;
    }

    private void SetupCloseRangeTrigger()
    {
        GameObject triggerObj = new GameObject("CloseRangeTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.layer = gameObject.layer;

        closeRangeCollider = triggerObj.AddComponent<CircleCollider2D>();
        closeRangeCollider.isTrigger = true;
        closeRangeCollider.radius = closeRangeRadius;

        Rigidbody2D triggerRb = triggerObj.AddComponent<Rigidbody2D>();
        triggerRb.isKinematic = true;
        triggerRb.simulated = true;

        triggerObj.AddComponent<PlayerCloseRangeTrigger>().Initialize(enemyTag);
    }

    private void SetupPathRenderer()
    {
        GameObject pathObj = new GameObject("PlayerPath");
        pathObj.transform.SetParent(null);
        lineRenderer = pathObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0.8f, 1f, 0.01f);
        lineRenderer.endColor = new Color(1f, 0.8f, 1f, 0.01f);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.numCapVertices = 2;
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

        if (!PlayerLives.Instance.IsRespawning)
            TrackMovementDistance();
    }

    // ============================
    // 📏 Movement Tracking
    // ============================
    private void TrackMovementDistance()
    {
        Vector2 currentPos = transform.position;
        float dist = Vector2.Distance(currentPos, lastPosition);

        if (dist > 0.001f)
        {
            totalDistanceMoved += dist;

            if (pathPositions.Count == 0 ||
                Vector3.Distance(pathPositions[pathPositions.Count - 1], currentPos) > minDistanceBetweenPoints)
            {
                pathPositions.Add(currentPos);
                lineRenderer.positionCount = pathPositions.Count;
                lineRenderer.SetPositions(pathPositions.ToArray());
            }
        }

        lastPosition = currentPos;
    }

    // Wird aufgerufen, wenn der Spieler stirbt (um Tracking zu resetten)
    public void PauseTrackingOnDeath()
    {
        accumulatedDistance += totalDistanceMoved; // speichere Fortschritt
        totalDistanceMoved = 0f;
        pathPositions.Clear();
        lineRenderer.positionCount = 0;
    }

    // Gesamtwert fürs Data Logging:
    public float GetTotalDistanceMoved()
    {
        return accumulatedDistance + totalDistanceMoved;
    }

    // ============================
    // 📦 Movement & Shooting
    // ============================
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

        bulletCounter++;

        GameObject proj = Instantiate(projectilePrefab, shootOrigin.position, Quaternion.identity);
        Bullet bullet = proj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(direction, projectileSpeed, true, false);

        proj.tag = "PlayerProjectile";
    }

    // ============================
    // 📡 Close Range Detection
    // ============================
    public void EnemyEnteredRange()
    {
        closeRangeEnemyCounter++;
        Debug.Log($"🔸 Enemy entered close range! Count: {closeRangeEnemyCounter}");
    }

    public void EnemyLeftRange() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, closeRangeRadius);
    }
}
