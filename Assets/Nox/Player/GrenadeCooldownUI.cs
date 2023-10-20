using UnityEngine;
using UnityEngine.UI;

public class GrenadeCooldownUI : MonoBehaviour
{
    public PlayerShooting playerShooting; // Reference to your PlayerShooting script
    public Image cooldownImage; // Reference to the cooldown UI Image

    // In GrenadeCooldownUI.cs
    void Update()
    {
        float remainingCooldown = playerShooting.RemainingCooldown();
        if (remainingCooldown > 0)
        {
            cooldownImage.fillAmount = remainingCooldown / playerShooting.grenadeCooldownTime;
        }
        else
        {
            cooldownImage.fillAmount = 0;
        }
    }

}
