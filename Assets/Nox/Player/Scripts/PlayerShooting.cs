using System.Collections;
using UnityEngine;


public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float fireRate = 720f;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;
    public float reloadTime = 1.5f;
    public float knockbackForce = 10f; // Adjust in Inspector for desired knockback effect
    public int maxAmmo = 10;
    public float accuracy = 100f;
    public float damage = 10f;
    public int piercing = 1; // Default to piercing 1 enemy
    [Header("Firing Intent Grace Period")]
    [SerializeField] private float intentGracePeriod = 0.2f;  // Default to 0.2 seconds, adjust in inspector.
    private bool fireIntentQueued = false; // This will store the player's intent to fire when on cooldown.
    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip shootingSound;
    [SerializeField]
    private AudioClip startReloadSound;
    [SerializeField]
    private AudioClip endReloadSound;
    [SerializeField]
    private AudioSource audioSource;
    public float grenadeCooldownTime = 5.0f; // 5 seconds cooldown
    private float nextGrenadeTime = 0.0f;
    public float grenadeThrowForce = 10f; // Adjust this value as needed

    public enum FireMode { Semi, Burst, FullAuto }
    public FireMode fireMode = FireMode.FullAuto;
    public int projectilesPerBurst = 3; // Number of projectiles per burst
    public float burstDelay = 1.0f; // Delay between bursts

    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public float reloadStartTime;

    private float timeBetweenShots;
    private float nextFireTime;
    private bool isReloading = false;
    private float nextBurstTime = 0f;
    private PlayerMovement playerMovement;
    [Header("Grenade Throwing Parameters")]
    [SerializeField] private float minArcHeight = 0.5f; // Minimum height of the arc
    [SerializeField] private float maxArcHeight = 5.0f; // Maximum height of the arc
    [SerializeField] private GameObject grenadePrefab;
    public bool CanThrowGrenade()
    {
        return Time.time > nextGrenadeTime;
    }
    void Start() 
{
    playerMovement = GetComponent<PlayerMovement>();
}

    public bool IsReloading { get { return isReloading; } }
    [Header("Shooting Direction Settings")]
    public float allowableFireAngle = 90f;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        timeBetweenShots = 60f / fireRate;
        currentAmmo = maxAmmo;
  }

    private bool firstFrame = true;

private void Update()
{
    // Ignore controller input on the first frame
    if (firstFrame)
    {
        firstFrame = false;
        return;
    }
        if (isReloading) return;

        if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        switch (fireMode)
        {
            case FireMode.FullAuto:
                if (Input.GetButton("Fire1") && Time.time > nextFireTime)
                {
                    FireProjectile();
                }
                break;

            case FireMode.Semi:
                if (Input.GetButtonDown("Fire1"))
                {
                    if (Time.time > nextFireTime - intentGracePeriod)
                    {
                        if (Time.time > nextFireTime)
                        {
                            FireProjectile();
                        }
                        else
                        {
                            fireIntentQueued = true; // Queue the intent if within the grace period
                        }
                    }
                }
                else if (fireIntentQueued && Time.time > nextFireTime)
                {
                    FireProjectile();
                    fireIntentQueued = false; // Reset the intent after firing
                }
                break;

            case FireMode.Burst:
                if (Input.GetButtonDown("Fire1"))
                {
                    if (Time.time > nextBurstTime - intentGracePeriod)
                    {
                        if (Time.time > nextBurstTime)
                        {
                            StartCoroutine(BurstFire());
                        }
                        else
                        {
                            fireIntentQueued = true; // Queue the intent if within the grace period
                        }
                    }
                }
                else if (fireIntentQueued && Time.time > nextBurstTime)
                {
                    StartCoroutine(BurstFire());
                    fireIntentQueued = false; // Reset the intent after firing
                }
                break;
        }
        if (CanThrowGrenade() && Input.GetKeyDown(KeyCode.G)) // Replace 'G' with your grenade throw key
        {
            // Call the code to actually throw the grenade
            ThrowGrenadeTowardsCursor();
            nextGrenadeTime = Time.time + grenadeCooldownTime;
        }


    }

    private IEnumerator BurstFire()
    {
        nextBurstTime = Time.time + burstDelay + timeBetweenShots * projectilesPerBurst;
        for (int i = 0; i < projectilesPerBurst; i++)
        {
            FireProjectile();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }


    private void FireProjectile()
    {
        if (currentAmmo > 0)
        {
            ShootTowardsCursor(); // We don't pass any parameters here anymore
            currentAmmo--;
            nextFireTime = Time.time + timeBetweenShots;
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
    // In PlayerShooting.cs
    public float RemainingCooldown()
    {
        return Mathf.Max(0, nextGrenadeTime - Time.time);
    }

    // This method should be on the class-level, not nested
    private void ShootTowardsCursor()
    {

        // Fetch aimDirection
        Vector2 aimDirection = GetAimDirection();

        // Get the player's facing direction
        Vector2 playerFacingDirection = playerMovement.GetFacingDirection();

        // Check if the aiming direction is outside the allowable angle
        float angleDifference = Vector2.Angle(playerFacingDirection, aimDirection);
        if (angleDifference > allowableFireAngle / 2)
        {
            // If outside the allowable angle, adjust the aiming direction to the nearest edge
            float rotationDirection = (Vector3.Cross(playerFacingDirection, aimDirection).z > 0) ? 1 : -1;
            aimDirection = Quaternion.Euler(0, 0, rotationDirection * allowableFireAngle / 2) * playerFacingDirection;
        }


        // Adjust for accuracy
        float maxDeviation = (100f - accuracy) / 100f * 45f;
        float deviation = UnityEngine.Random.Range(-maxDeviation, maxDeviation);
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + deviation;
        Vector2 deviatedDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // Instantiate and configure the projectile
        GameObject newProjectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.Euler(0f, 0f, angle - 90f));
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetKnockbackForce(knockbackForce);
            projectile.SetOrigin("Player");
            projectile.SetDamage(damage);
            projectile.Launch(deviatedDirection, projectileSpeed);
            projectile.SetLifetime(projectileLifetime);
            projectile.SetPiercing(piercing);
        }
        // Play the shooting sound
        if (shootingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
    }



    private Vector2 GetAimDirection()
    {
        if (IsUsingController())
        {
            return new Vector2(Input.GetAxis("HorizontalRightStick"), Input.GetAxis("VerticalRightStick")).normalized;
        }
        else
        {
            Vector3 cursorWorldPosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 cursorWorldPosition = new Vector2(cursorWorldPosition3D.x, cursorWorldPosition3D.y);
            Vector2 direction = cursorWorldPosition - (Vector2)shootingPoint.position;
            return direction.normalized;
        }
    }

    private bool IsUsingController()
    {
        // Increased dead zone threshold
        return Mathf.Abs(Input.GetAxis("HorizontalRightStick")) > 0.2f || Mathf.Abs(Input.GetAxis("VerticalRightStick")) > 0.2f;
    }
    private void ThrowGrenadeTowardsCursor()
    {
        Vector2 aimDirection = GetAimDirection();

        // Instantiate and configure the grenade
        GameObject newGrenade = Instantiate(grenadePrefab, shootingPoint.position, Quaternion.identity);
        Grenade grenade = newGrenade.GetComponent<Grenade>();
        if (grenade != null)
        {
            grenade.Initialize(shootingPoint.position, aimDirection * grenadeThrowForce);
        }
    }




    Vector2 CalculateThrowForce(float distance, Vector3 direction)
    {
        // This multiplier can be adjusted to get the desired throw strength
        float forceMultiplier = grenadeThrowForce;

        // Calculate the vertical and horizontal components of the force
        float xForce = direction.x * forceMultiplier;
        float yForce = (direction.y + 1.0f) * forceMultiplier;  // Added an upward force for a parabolic arc

        return new Vector2(xForce, yForce);
    }








    private Vector3 GetCursorPosition()
    {
        Vector3 cursorWorldPosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPosition3D.z = 0;  // Reset the z-coordinate to match your 2D plane
        return cursorWorldPosition3D;
    }





    private IEnumerator Reload()
    {
        // Check if current ammo is already max
        if (currentAmmo == maxAmmo)
            yield break;  // Exit the coroutine if mag is full

        isReloading = true;
        reloadStartTime = Time.time;

        // Play the start reload sound
        if (startReloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(startReloadSound);
        }

        float soundOffset = 0.5f; // Adjust this value to your needs

        yield return new WaitForSeconds(reloadTime - soundOffset);

        // Play the end reload sound slightly before reloading finishes
        if (endReloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(endReloadSound);
        }

        yield return new WaitForSeconds(soundOffset); // Wait for the remaining reload time

        currentAmmo = maxAmmo;
        isReloading = false;
    }

}