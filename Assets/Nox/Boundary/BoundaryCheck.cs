using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    public float damageRate = 1.0f; // Damage per second outside boundary
    private bool isOutside = false; // To check if player is outside boundary

    private void Update()
    {
        if (isOutside)
        {
            // Apply damage or any other consequence here
            // For example: reduce player's health by damageRate * Time.deltaTime
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boundary"))
        {
            isOutside = true;
            // Any immediate action when player exits the boundary
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boundary"))
        {
            isOutside = false;
            // Any action when player re-enters the boundary
        }
    }
}
