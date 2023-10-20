using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerStats playerStats;

    public float moveSpeed = 13f;
    public float smoothTime = 0.4f;
    public Vector3 LastMove { get; private set; }

    public float dashSpeed = 18f;
    public float dashDuration = 0.4f;
    public float dashCooldown = 2f;
    public float dashDecelerationDuration = 0.2f;
    public float dashPivotSensitivity = 0.5f;
    public Vector2 CurrentDirection { get; private set; }

    private Vector3 currentInput = Vector3.zero;
    private Vector3 inputVelocity = Vector3.zero;  // Added this back
    private Rigidbody2D rb;

    private bool isDashing = false;
    private bool canDash = true;
    [Header("Audio Settings")]
    [SerializeField] private AudioClip dashSound;   // Dash sound effect
    [SerializeField] private AudioSource audioSource; // Audio source to play the sound

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        // Ensure the rb is properly initialized if it wasn't set in the inspector
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 targetInput = new Vector3(horizontalInput, verticalInput, 0f);
        if (targetInput.magnitude > 1)
        {
            targetInput.Normalize();
        }

        currentInput = Vector3.SmoothDamp(currentInput, targetInput, ref inputVelocity, smoothTime);

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && canDash)
        {
            GetComponent<DashAfterImage>().CreateAfterImage();
            StartCoroutine(DashRoutine());
        }

        FaceTowardsCursor();
    }

    private void FixedUpdate()
    {
        Vector3 movement = currentInput * moveSpeed;
        CurrentDirection = movement.normalized;

        if (isDashing)
        {
            Vector3 dynamicDashDirection = Vector3.Lerp(currentInput, currentInput, dashPivotSensitivity).normalized;
            movement = dynamicDashDirection * dashSpeed;
        }

        rb.velocity = movement;
    }


    public Vector2 GetFacingDirection()
    {
        return transform.up;
    }


    private IEnumerator DashRoutine()
    {
        // Play the dash sound effect
        if (dashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
        StartCoroutine(playerStats.InvincibilityFrames());

        isDashing = true;
        canDash = false;

        // Initial Burst
        Vector2 dashDirection = currentInput.normalized;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // Deceleration Phase
        float elapsedDecelerationTime = 0f;
        while (elapsedDecelerationTime < dashDecelerationDuration)
        {
            float progress = elapsedDecelerationTime / dashDecelerationDuration;
            float decelerationFactor = Mathf.Lerp(1, 0, progress); // Linearly decrease from 1 to 0

            rb.velocity = dashDirection * dashSpeed * decelerationFactor;
            elapsedDecelerationTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown - (dashDuration + dashDecelerationDuration));
        canDash = true;
    }





    public float rotationSpeed = 360f;
    public bool IsPlayerDashing()
    {
        return isDashing;
    }
    void FaceTowardsCursor()
    {
        Vector3 cursorScreenPosition = Input.mousePosition;
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition);
        Vector3 directionToCursor = cursorWorldPosition - transform.position;

        float targetAngle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        const float maxAllowedAngle = 45f; // Set this to whatever angle limit you desire

        if (Mathf.Abs(angleDifference) > maxAllowedAngle)
        {
            if (angleDifference < 0)
                targetAngle = currentAngle - maxAllowedAngle;
            else
                targetAngle = currentAngle + maxAllowedAngle;
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

}
