using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public GameObject projectilePrefab;
    public Transform shootOrigin; // assign to the Arrow transform or center
    public float projectileSpeed = 8f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Camera mainCam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Movement input
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S, Up/Down
        moveInput = moveInput.normalized;

        // Aim: rotate the Arrow child to point at mouse
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = (mouseWorld - transform.position);
        if (aimDir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // player points up by default
        }

        // Shooting on left click
        if (Input.GetMouseButtonDown(0) && projectilePrefab != null)
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    private void Shoot()
    {
        Vector2 dir = transform.up;
        if (dir.sqrMagnitude < 0.001f) dir = Vector2.up; // fallback

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Bullet b = proj.GetComponent<Bullet>();
        if (b != null) b.Init(transform.up, projectileSpeed, true, false); // fromPlayer=true
        proj.tag = "PlayerProjectile";

    }
}
