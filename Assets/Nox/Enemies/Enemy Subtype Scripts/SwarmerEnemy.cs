using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmerEnemy : BaseEnemy
{
    public float damageToPlayer = 20f;  // Adjust this value based on how much damage you want the swarmer to inflict
    protected override void HandleMovement()
    {
        if (!player) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = directionToPlayer * movementSpeed;

        // You can still limit the maximum speed
        if (rb.velocity.magnitude > movementSpeed)
        {
            rb.velocity = rb.velocity.normalized * movementSpeed;
        }

        // If you want to include some form of drag or mass adjustment, you can copy those lines from the original HandleMovement
        rb.drag = Mathf.Lerp(2.0f, 0.5f, rb.velocity.magnitude / movementSpeed);
        rb.mass = Mathf.Lerp(1.05f, 0.95f, rb.velocity.magnitude / movementSpeed);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
            }

            // Destroy the SwarmerEnemy upon collision
            Destroy(gameObject);
        }
    }
}
