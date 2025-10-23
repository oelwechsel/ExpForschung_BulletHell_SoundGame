using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootOrigin;  // Zuweisen im Inspector (z. B. Spitze des Spielers)
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float fireRate = 0.15f; // optional: Feuerrate

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private float nextFireTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
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


    // INPUT
    private void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D oder Links/Rechts
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S oder Hoch/Runter
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
        // Nur schießen, wenn Prefab gesetzt ist
        if (projectilePrefab == null || shootOrigin == null) return;

        if (!LevelManager.Instance.isLevelActive || PlayerLives.Instance.IsRespawning) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    //  SHOOT 
    private void Shoot()
    {
        Vector2 direction = transform.up;
        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.up; // Sicherheits-Backup

        // Projektil an der Schuss-Position spawnen
        GameObject proj = Instantiate(projectilePrefab, shootOrigin.position, Quaternion.identity);

        // Wenn Bullet-Script vorhanden ist  Initialisieren
        Bullet bullet = proj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(direction, projectileSpeed, true, false); // true = vom Spieler

        proj.tag = "PlayerProjectile";
    }
}
