using UnityEngine;
using System.Collections;

public class CameraFollowWithSmoothing : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public bool followSmoothing = true;
    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentDesiredPosition;
    private Vector3 shakeOffset = Vector3.zero;

    // Shake properties
    public float shakeMagnitude = 0.05f;

    public bool IsShaking { get; private set; } = false;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        if (followSmoothing)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition + shakeOffset, ref velocity, smoothSpeed);
        }
        else
        {
            transform.position = desiredPosition + shakeOffset;
        }
    }

    public void TriggerShake(float duration, float intensity = 1f) // Added the intensity parameter with a default value
    {
        if (!IsShaking)
        {
            StartCoroutine(Shake(duration, intensity)); // Pass the intensity to the coroutine
        }
    }

    private IEnumerator Shake(float duration, float intensity) // Added the intensity parameter here as well
    {
        IsShaking = true;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Multiply the shakeMagnitude with the intensity to adjust the shake
            shakeOffset = new Vector3(Random.Range(-1f, 1f) * shakeMagnitude * intensity, Random.Range(-1f, 1f) * shakeMagnitude * intensity, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        IsShaking = false;
    }
}
