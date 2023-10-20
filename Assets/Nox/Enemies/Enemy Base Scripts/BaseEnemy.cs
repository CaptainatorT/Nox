
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] hitSounds; // Array of metal clang noises.
    private AudioSource audioSource; // To play sound effects.
    public bool isDefeated = false;
    [Tooltip("Color to flash when the enemy is hit.")]
    public Color maxFlashColor = new Color(0.9f, 0.9f, 0.9f);  // Slightly off-white
    private SpriteRenderer spriteRenderer;  // This will reference the enemy's sprite renderer.
    private Color originalColor;  // To store the original color of the enemy sprite.
    private float flashIntensity = 0;
    public Transform powerCore;
    public int waveNumber { get; set; }
    public UnityEvent OnEnemyDefeated;
    [Tooltip("Determines if the enemy is a priority target for wave completion.")]
    public bool isPriorityEnemy = false;
    #region Health Handling
    [Tooltip("The maximum health of the enemy.")]
    public float maxHealth = 100f;
    public GameObject healthDropPrefab;
    public float healthDropChance = 0.1f;  // 10% chance by default
    public float dropChance = 1;  // 1 means 100% chance
    public int maxDrops = 1;     // Maximum drops an enemy can spawn in its lifetime
    private int currentDrops = 0;  // How many drops have been spawned
    public AudioClip fireSound; // Drag your sound effect here in the inspector
    // Current health is determined at runtime and typically won't be adjusted in the Inspector.
    private float currentHealth;

    [Tooltip("Reference to the enemy's health bar transform.")]
    public Transform healthBarTransform;

    // Initial size of the health bar for scaling.
    private Vector3 initialHealthBarScale;

    private SpriteRenderer healthBarSpriteRenderer;

    [Tooltip("Reference to the wave manager for enemy defeat notifications.")]
    public WaveManager waveManager;
    #endregion

    #region Player Interaction
    [Tooltip("Reference to the player's transform. This is typically set at runtime.")]
    public Transform player;

    private Vector3 directionToPlayer;
    #endregion

    #region Movement Variables

    [Tooltip("Internal timer to keep track of the enemy's next direction shift. You typically won't adjust this in the Inspector.")]
    private float nextDirectionShiftTime;
    [Tooltip("Maximum speed the enemy can move.")]
    public float maxSpeed = 10f;

    [Tooltip("The enemy's base movement speed. Increase for faster enemies, decrease for slower.")]
    public float movementSpeed = 5f;

    private float originalSpeed;

    [Tooltip("Amount the speed can fluctuate over time. The enemy will periodically adjust its speed within a range defined by this value.")]
    public float speedFluctuation = 0.5f;

    [Tooltip("Frequency (in seconds) at which the enemy slightly adjusts its direction towards the player. Lower values make it more erratic, higher values make it more consistent.")]
    public float directionShiftFrequency = 5f;

    [Tooltip("Desired distance from the player. The enemy will try to maintain this distance. Higher values make the enemy more cautious, lower values make it more aggressive.")]
    public float desiredDistance = 5f;

    [Tooltip("Maximum distance at which the enemy will start approaching the player if exceeded.")]
    public float maxDistanceFromPlayer = 7f;

    [Tooltip("Distance at which the enemy will start fleeing or braking. Lower values make it more brave, higher values make it more skittish.")]
    public float fleeDistance = 3f;

    [Tooltip("Multiplier for the enemy's speed when close to the player. Values less than 1 will slow the enemy down, greater than 1 will speed it up.")]
    public float closeRangeSpeedMultiplier = 0.5f;

    [Tooltip("Threshold to determine what is considered mid-range.")]
    public float midRangeThreshold = 20f;

    [Tooltip("Multiplier for the enemy's speed when far from the player. Values greater than 1 will speed the enemy up, making it rush towards the player aggressively.")]
    public float farRangeSpeedMultiplier = 1.5f;


    [Tooltip("Chance per second the enemy will decide to change its direction randomly.")]
    public float randomDirectionChance = 0.05f; // 5% chance every second

    [Tooltip("Duration for which the enemy moves in a random direction.")]
    public float randomDirectionDuration = 1f;
    private bool isBeingKnockedBack = false;
    private Vector2 originalVelocity;

    private bool isMovingRandomly = false;
    private Vector3 randomDirection;
    #endregion
    private float despawnDistance = 150f;

    private void Update()
    {
        // Check if the wave is inactive
        if (waveManager && !waveManager.waveActive)
        {
            // Check if the enemy should despawn
            if (!isPriorityEnemy && Vector3.Distance(transform.position, player.position) > despawnDistance)
            {
                Despawn();
            }
        }
    }

    // Simplified Flee method
    public void Flee()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        rb.velocity = fleeDirection * movementSpeed;
    }

    // Updated Despawn method
    public void Despawn()
    {
        Destroy(gameObject);
    }
    #region Enemy Avoidance
    [Tooltip("Radius within which the enemy checks for other enemies to avoid.")]
    public float avoidanceRadius = 2f;

    [Tooltip("How strongly the enemy tries to avoid others. Higher values will make it more evasive.")]
    public float avoidanceWeight = 2f;

    private Vector3 currentAvoidanceVelocity;

    [Tooltip("Time it takes for the enemy's avoidance direction to adjust.")]
    public float avoidanceSmoothTime = 0.1f;
    #endregion

    #region Visual Feedback
    [Tooltip("Reference to the enemy's internal light transform used for visual feedback.")]
    public Transform internalLightTransform;

    private SpriteRenderer internalLightSpriteRenderer;
    #endregion

    protected Rigidbody2D rb;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (!waveManager)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        powerCore = transform.Find("PowerCore");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Adding audio source component if not already attached
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        InitializeComponents();
    }

    public IEnumerator FlashColor()
    {
        isFlashing = true;

        float startIntensity = flashIntensity;
        float targetIntensity = Mathf.Clamp(flashIntensity + 0.5f, 0f, 0.75f); // adjust this added value to control the flash effect
        float progress = 0;

        // Eased ascent to target intensity
        while (progress < 1)
        {
            progress += Time.deltaTime * 15.0f;  // adjust this value for duration of the ascent
            float easedProgress = EaseOutQuad(progress);
            flashIntensity = Mathf.Lerp(startIntensity, targetIntensity, easedProgress);
            spriteRenderer.color = Color.Lerp(originalColor, maxFlashColor, flashIntensity);
            yield return null;
        }

        progress = 0;

        // Eased decay
        while (flashIntensity > 0.01f)
        {
            progress += Time.deltaTime * 5.0f;  // adjust this value for duration of the decay
            float easedProgress = EaseInQuad(progress);
            flashIntensity = Mathf.Lerp(targetIntensity, 0, easedProgress);
            spriteRenderer.color = Color.Lerp(originalColor, maxFlashColor, flashIntensity);
            yield return null;
        }

        spriteRenderer.color = originalColor;
        flashIntensity = 0;
        isFlashing = false;
    }






    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();

        // Clamp the Rigidbody velocity to ensure it doesn't exceed maxSpeed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }


    private bool isFlashing = false;

    public AudioClip[] damageSounds;  // Array of metal clanging sounds

    public void TakeDamage(float amount, Vector2 knockbackDirection, float force)
    {
        HandleDamage(amount);
        ApplyDynamicKnockback(knockbackDirection, force);

        if (!isFlashing)
        {
            StartCoroutine(FlashColor());
        }

        // Play a random damage sound
        if (damageSounds.Length > 0)
        {
            int randomSoundIndex = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[randomSoundIndex]);
        }
    }


    #region Initialization
    void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        healthBarSpriteRenderer = healthBarTransform.GetComponent<SpriteRenderer>();
        initialHealthBarScale = healthBarTransform.localScale;
        healthBarSpriteRenderer.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        // Assign the WaveManager automatically
        waveManager = GameObject.FindObjectOfType<WaveManager>();



        originalSpeed = movementSpeed;
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2.0f;
        StartCoroutine(FluctuateSpeed());

        nextDirectionShiftTime = Time.time + directionShiftFrequency; // Initialize next direction shift time

        internalLightSpriteRenderer = internalLightTransform.GetComponent<SpriteRenderer>();
        UpdateInternalLightColor();

    }

    #endregion
    public void ApplyDynamicKnockback(Vector2 direction, float force)
    {
        Vector2 knockbackForce = direction.normalized * force;

        // You can introduce a diminishing factor if needed. 
        // This ensures that continuous knockbacks don't get too extreme.
        float diminishingFactor = 0.9f;
        if (isBeingKnockedBack)
        {
            knockbackForce *= diminishingFactor;
        }

        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        // Optionally: If you want a set duration after which the knockback effect is reduced or nullified, 
        // you can still use a coroutine to set the 'isBeingKnockedBack' flag and control that.
        StartCoroutine(KnockbackCooldown());
    }


    private IEnumerator KnockbackCooldown()
    {
        isBeingKnockedBack = true;
        yield return new WaitForSeconds(0.2f); // This cooldown determines how long the enemy is affected by the knockback.
        isBeingKnockedBack = false;
    }


    #region Movement Handling
    void HandleRotation()
    {
        if (!player) return;
        directionToPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }


    protected virtual void HandleMovement()
    {
        // Decide if we should move randomly
        if (UnityEngine.Random.Range(0f, 1f) < randomDirectionChance * Time.fixedDeltaTime)
        {
            StartCoroutine(RandomDirectionMovement());
        }

        if (isMovingRandomly)
        {
            Vector2 forceApplied = randomDirection * movementSpeed;
            rb.velocity = forceApplied; // set velocity directly

        }
        else
        {
            Vector3 fleeDirection = GetFleeingDirection();
            Vector3 avoidDirection = AvoidOtherEnemies();

            if (Time.time > nextDirectionShiftTime)
            {
                directionToPlayer += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f));
                directionToPlayer.Normalize();
                nextDirectionShiftTime = Time.time + directionShiftFrequency;
            }

            Vector3 totalMoveDirection = fleeDirection + avoidDirection;
            totalMoveDirection.Normalize();

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < desiredDistance)
            {
                movementSpeed = originalSpeed * closeRangeSpeedMultiplier;
            }
            else if (distanceToPlayer < midRangeThreshold)
            {
                movementSpeed = originalSpeed;
            }
            else
            {
                movementSpeed = originalSpeed * farRangeSpeedMultiplier;
            }

            rb.AddForce(totalMoveDirection * movementSpeed, ForceMode2D.Force);
        }

        // Common adjustments for both random and non-random movements
        // Cap the speed
        if (rb.velocity.magnitude > movementSpeed)
        {
            rb.velocity = rb.velocity.normalized * movementSpeed;
        }

        rb.drag = Mathf.Lerp(2.0f, 0.5f, rb.velocity.magnitude / movementSpeed); // this will increase the drag when the velocity gets high
        rb.mass = Mathf.Lerp(1.05f, 0.95f, rb.velocity.magnitude / movementSpeed); // adjust mass slightly based on speed
    }


    Vector3 GetFleeingDirection()
    {
        if (!player) return Vector3.zero;
        directionToPlayer = (player.position - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, player.position);
        Vector3 fleeDirection = Vector3.zero;

        if (currentDistance < fleeDistance)
        {
            float moveDistance = Mathf.Min(fleeDistance - currentDistance, movementSpeed * Time.deltaTime);
            fleeDirection = -directionToPlayer * moveDistance;
        }
        else if (currentDistance < desiredDistance)
        {
            float moveDistance = Mathf.Min(desiredDistance - currentDistance, movementSpeed * Time.deltaTime);
            fleeDirection = -directionToPlayer * moveDistance;

            float brakeFactor = (desiredDistance - currentDistance) / desiredDistance;
            fleeDirection *= brakeFactor;
        }
        else if (currentDistance >= maxDistanceFromPlayer)
        {
            float moveDistance = Mathf.Min(currentDistance - maxDistanceFromPlayer, movementSpeed * Time.deltaTime);
            fleeDirection = directionToPlayer * moveDistance;
        }

        return fleeDirection;
    }

    Vector3 AvoidOtherEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        Vector3 avoidanceDirection = Vector3.zero;

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.CompareTag("Enemy"))
            {
                Vector3 directionFromEnemy = transform.position - hitCollider.transform.position;
                float distance = directionFromEnemy.magnitude;

                if (distance < 0.1f) continue;

                float weight = 1.0f / distance;
                avoidanceDirection += directionFromEnemy.normalized * weight;
            }
        }

        avoidanceDirection = Vector3.SmoothDamp(transform.position + avoidanceDirection, transform.position, ref currentAvoidanceVelocity, avoidanceSmoothTime) - transform.position;

        if (avoidanceDirection.magnitude > 1)
            avoidanceDirection = avoidanceDirection.normalized;

        return avoidanceDirection * avoidanceWeight;
    }
    #endregion
    IEnumerator RandomDirectionMovement()
    {
        isMovingRandomly = true;

        // Pick a random direction
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;

        float originalMaxSpeed = movementSpeed;
        movementSpeed = movementSpeed * 0.5f; // you can adjust this to a suitable value

        yield return new WaitForSeconds(randomDirectionDuration);

        movementSpeed = originalMaxSpeed;
        isMovingRandomly = false;
    }



    #region Damage and Health Handling
    void HandleDamage(float amount)
    {
        currentHealth -= amount;

        // Calculate the damage percentage
        float damagePercentage = amount / maxHealth;

        float baseIntensity = 0.05f;  // Halved the base value 
        flashIntensity += baseIntensity + Mathf.Sqrt(damagePercentage) * 0.3f;  // Lowered the scaling factor 

        flashIntensity = Mathf.Clamp(flashIntensity, 0f, 0.75f);



        StartCoroutine(FlashColor());
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();
        UpdateInternalLightColor();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        float targetScale = (currentHealth / maxHealth) * initialHealthBarScale.x;
        healthBarTransform.localScale = new Vector3(targetScale, initialHealthBarScale.y, initialHealthBarScale.z);
    }
    private IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 0.5f;
        float startTime = Time.time;

        SpriteRenderer powerCoreSpriteRenderer = powerCore.GetComponent<SpriteRenderer>();


        Color startEnemyColor = spriteRenderer.color;
        Color endEnemyColor = new Color(startEnemyColor.r, startEnemyColor.g, startEnemyColor.b, 0);

        Color startPowerCoreColor = powerCoreSpriteRenderer.color;
        Color endPowerCoreColor = new Color(startPowerCoreColor.r, startPowerCoreColor.g, startPowerCoreColor.b, 0);

        while (Time.time - startTime < fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            spriteRenderer.color = Color.Lerp(startEnemyColor, endEnemyColor, t);
            powerCoreSpriteRenderer.color = Color.Lerp(startPowerCoreColor, endPowerCoreColor, t);
            yield return null;
        }

        spriteRenderer.color = endEnemyColor;
        powerCoreSpriteRenderer.color = endPowerCoreColor;
        Destroy(gameObject);
    }

    public void Die()
    {
        if (currentDrops < maxDrops && Random.value < healthDropChance)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
            currentDrops++;
        }

        Debug.Log("Enemy Died");
        OnEnemyDefeated?.Invoke();
        StartCoroutine(FadeOutAndDestroy());
    }


    void TryToDropHealth()
    {
        if (currentDrops < maxDrops && Random.value < dropChance)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
            currentDrops++;
        }
    }




    #endregion

    #region Visual Feedback
    void UpdateInternalLightColor()
    {
        float healthPercent = currentHealth / maxHealth;

        Color lightGreen = new Color(0.5f, 1f, 0.5f);
        Color lightYellow = new Color(1f, 1f, 0.5f);
        Color lightRed = new Color(1f, 0.5f, 0.5f);

        if (healthPercent >= 0.5f)
        {
            float t = (healthPercent - 0.5f) * 2;
            internalLightSpriteRenderer.color = Color.Lerp(lightYellow, lightGreen, t);
        }
        else
        {
            float t = healthPercent * 2;
            internalLightSpriteRenderer.color = Color.Lerp(lightRed, lightYellow, t);
        }
    }
    #endregion

    #region Miscellaneous
    IEnumerator FluctuateSpeed()
    {
        while (true)
        {
            movementSpeed = originalSpeed + UnityEngine.Random.Range(-speedFluctuation, speedFluctuation);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        }
    }
    #endregion
    float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }

    float EaseInQuad(float x)
    {
        return x * x;
    }

}
