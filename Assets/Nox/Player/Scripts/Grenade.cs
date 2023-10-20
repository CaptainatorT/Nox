using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float damage = 50f;
    public float grenadeSpeed = 10f; // This is the constant speed for the grenade.
    public float knockbackForce = 10f;
    public float rollingSpeed = 2f; // Speed at which grenade will roll upon landing

    private float countdown;
    private bool hasExploded = false;
    private bool isRolling = false;
    private Rigidbody2D rb;
    private Vector2 rollDirection; // Stores the direction the grenade was thrown in

    [SerializeField]
    private GameObject explosionVisual;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;

        if (isRolling)
        {
            // Update the rolling logic here if needed
            // For example, you can slow down the grenade gradually
        }

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRolling)
        {
            isRolling = true;
            rb.velocity = rollDirection * rollingSpeed;  // Grenade rolls in the direction it was thrown
        }
    }
    public void Initialize(Vector3 startPosition, Vector3 direction)
    {
        Debug.Log("Rigidbody: " + rb);
        Debug.Log("Direction: " + direction);
        rollDirection = direction.normalized;
        rb.velocity = rollDirection * grenadeSpeed;
    }


    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D nearbyObject in colliders)
        {
            Vector2 direction = nearbyObject.transform.position - transform.position;
            direction.Normalize();

            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            // Handle damage to enemy
            if (nearbyObject.CompareTag("Enemy"))
            {
                BaseEnemy enemy = nearbyObject.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, direction, knockbackForce);
                }
            }

            if (explosionVisual != null)
            {
                Instantiate(explosionVisual, transform.position, Quaternion.identity);
            }

            // Handle damage to player
            if (nearbyObject.CompareTag("Player"))
            {
                PlayerStats playerStats = nearbyObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject); // Destroy the grenade
    }
}
