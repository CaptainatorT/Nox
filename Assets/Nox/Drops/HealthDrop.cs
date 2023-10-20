using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    public float healPercentage = 0.2f;  // 20% by default, can adjust in the inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision Detected");
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();  // accessing the PlayerStats script
            if (playerStats != null)
            {
                playerStats.Heal(healPercentage); // Use the Heal function from PlayerStats
                Destroy(gameObject);  // destroy the health drop after use
            }
        }
    }
}
