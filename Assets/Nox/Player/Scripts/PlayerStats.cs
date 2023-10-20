using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float damage = 10f;

    [Header("Audio Settings")]
    public AudioClip[] damageSounds;
    public AudioClip healthPickupSound;
    private AudioSource audioSource;

    public Image healthBarForeground;
    private float initialHealthBarWidth;
    [Header("Invincibility Settings")]
    public float invincibilityDuration = 0.15f; // Invincibility duration in seconds
    private bool isInvincible = false;

    public CameraFollowWithSmoothing cameraController;

    private void Start()
    {
        initialHealthBarWidth = healthBarForeground.rectTransform.sizeDelta.x;
        audioSource = GetComponent<AudioSource>();
    }

    public Rigidbody2D rb;
    public void Heal(float percentage)
    {
        currentHealth += maxHealth * percentage;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        audioSource.PlayOneShot(healthPickupSound); // Play health pickup sound
        UpdateHealthBar();
    }

    public void TakeDamage(float damageAmount)
    { // If the player is invincible, don't apply any damage
        if (isInvincible)
            return;
        currentHealth -= damageAmount;
        PlayRandomDamageSound();

        if (cameraController != null)
        {
            float playerSpeed = rb.velocity.magnitude;
            float shakeDuration = 0.25f;
            float shakeIntensity = 1f; // Default intensity

            if (playerSpeed > 2f) // Adjust this threshold as needed
            {
                shakeIntensity = 1.5f; // Increased intensity. Adjust this value as needed.
            }

            cameraController.TriggerShake(shakeDuration, shakeIntensity);
            // Trigger invincibility after taking damage
            StartCoroutine(InvincibilityFrames());
        }

        // Ensure health doesn't go negative
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void PlayRandomDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            int randomSoundIndex = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[randomSoundIndex]);
        }
    }
    public IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        healthBarForeground.rectTransform.sizeDelta = new Vector2(initialHealthBarWidth * healthPercentage, healthBarForeground.rectTransform.sizeDelta.y);
    }

    private void Die()
    {
        // Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
