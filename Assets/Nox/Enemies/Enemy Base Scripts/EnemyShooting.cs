using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float fireRate = 1f; // Number of shots per second
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;
    public float damage = 5f;  // Default damage value
    public float maxShootingRange = 15f; // adjust as required
    public float accuracy = 80f; // Values from 0 to 100. 100 means perfect accuracy.
    public AudioClip shootingSound;
    private AudioSource audioSource;

    // New variables for magazine and reload mechanics
    public int enemyMagazineSize = 5; // default magazine size for enemy
    private int enemyCurrentAmmo;
    public float enemyReloadTime = 2f; // default reload time for enemy in seconds
    private bool enemyIsReloading = false;

    private float nextFireTime;
    private Transform player;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        FindPlayer();
        enemyCurrentAmmo = enemyMagazineSize;
    }


    private void Update()
    {
        if (!player) // in case player gets destroyed or is not available yet.
        {
            FindPlayer();
            return; // If player is still null, don't proceed to shooting logic.
        }

        if (enemyIsReloading) return;

        if (enemyCurrentAmmo <= 0)
        {
            StartCoroutine(EnemyReload());
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (Time.time > nextFireTime && distanceToPlayer <= maxShootingRange)
        {
            ShootAtPlayer();
            enemyCurrentAmmo--;
            nextFireTime = Time.time + 60f / fireRate;
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 playerPosition = player.position;
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        Vector2 playerVelocity = playerRigidbody ? playerRigidbody.velocity : Vector2.zero;
        if (shootingSound != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }

        // If player is dashing, reduce the leading effect by, say, 50%
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement && playerMovement.IsPlayerDashing())
        {
            playerVelocity *= 0.5f;
        }

        // Calculate the time it will take for the projectile to reach the player
        float distanceToPlayer = Vector2.Distance(shootingPoint.position, playerPosition);
        float timeToHit = distanceToPlayer / projectileSpeed;

        // Predicted future position of the player
        Vector2 predictedPlayerPosition = playerPosition + playerVelocity * timeToHit;

        Vector2 direction = (predictedPlayerPosition - (Vector2)shootingPoint.position).normalized;

        float maxDeviation = (100f - accuracy) / 100f * 45f; // 45 degrees is the maximum deviation
        float deviation = UnityEngine.Random.Range(-maxDeviation, maxDeviation);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + deviation;
        Vector2 deviatedDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        GameObject newProjectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.Euler(0f, 0f, angle - 90f));
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetOrigin("Enemy");
            projectile.SetDamage(damage);
            projectile.Launch(deviatedDirection, projectileSpeed);
            projectile.SetLifetime(projectileLifetime);
        }
    }



    private IEnumerator EnemyReload()
    {
        enemyIsReloading = true;
        yield return new WaitForSeconds(enemyReloadTime);
        enemyCurrentAmmo = enemyMagazineSize;
        enemyIsReloading = false;
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
