using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TMP_Text waveText;
    public TMP_Text countdownText;
    public WaveManager waveManager;

    private float countdown;
    private bool countdownActive = false;

    public Slider waveProgressSlider;  // Reference to the new slider

    private void Update()
    {
    
        if (!countdownActive)
        {   // Debug logs to check for potential null objects:
            if (waveProgressSlider == null)
            {
               
            }
            if (waveManager == null)
            {
                Debug.LogError("waveManager is not set in UIManager.");
            }
            if (waveManager.CurrentWaveConfig == null)
            {
                Debug.LogError("CurrentWaveConfig is returning null. CurrentWaveIndex is: " + waveManager.CurrentWaveIndex);
            }
            waveText.text = "Wave: " + (waveManager.CurrentWaveIndex + 1);

            // Update the slider's value based on the wave's progress
            float waveDuration = waveManager.CurrentWaveConfig.waveDuration;
            float remainingTime = waveDuration - waveManager.CurrentWaveTime;
            waveProgressSlider.value = 1 - (remainingTime / waveDuration);
        }
    }

    public void UpdateWaveCountdown(float remainingTime)
    {
        countdownText.text = "Next wave in: " + Mathf.CeilToInt(remainingTime) + "s";
    }

    public void StartCountdown(float duration)
    {
        countdownText.gameObject.SetActive(true); // Show the countdown text
        countdown = duration;
        countdownActive = true;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            countdownText.text = "Next wave in: " + Mathf.CeilToInt(countdown) + "s";
            yield return null;
        }
        countdownText.gameObject.SetActive(false); // Hide the countdown text
        countdownActive = false;
        waveText.text = "Wave: " + (waveManager.CurrentWaveIndex + 1);  // Display the current wave
    }


    public void OnWaveCompleted()
    {
        waveText.text = "Wave: " + (waveManager.CurrentWaveIndex + 1) + " Completed!";
    }
}
