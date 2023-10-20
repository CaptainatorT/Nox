using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float damage;
    private float lifetime;
    private int piercingCount;
    private string originTag; // Who shot the projectile
    private float knockbackForce;
    private Vector2 direction;

    // This will allow setting the knockback force from another script (like the PlayerController).
    public void SetKnockbackForce(float force)
    {
        knockbackForce = force;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, float speed)
    {
        this.direction = direction;  // Set direction
        rb.velocity = direction * speed;
    }

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    public void SetPiercing(int piercing)
    {
        piercingCount = piercing;
    }

    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    public void SetOrigin(string tag)
    {
        originTag = tag;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if this is a player's bullet and it hit an enemy
        if (originTag == "Player" && other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                // Use the projectile's travel direction for knockback
                Vector2 knockbackDirection = direction.normalized;

                // The TakeDamage method of the Enemy script will now handle both damage and knockback.
                // The knockback force and duration can be passed directly.
                enemy.TakeDamage(damage, knockbackDirection, knockbackForce);
               

                HandlePiercing();
            }
        }
        // Check if this is an enemy's bullet and it hit a player
        else if (originTag == "Enemy" && other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage); // This will actually deduct the player's health.
                HandlePiercing();
            }
        }
    }

    private void HandlePiercing()
    {
        if (piercingCount <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            piercingCount--;
        }
    }
}
