using UnityEngine;
using UnityEngine.UI;

public class AmmoIndicator : MonoBehaviour
{
    public PlayerShooting playerShooting;
    public Image ammoBarFill;
    public Image reloadFill;

    private float originalAmmoBarWidth;
    private float originalReloadBarWidth;

    private void Start()
    {
        originalAmmoBarWidth = ammoBarFill.rectTransform.sizeDelta.x;
        originalReloadBarWidth = reloadFill.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (playerShooting.IsReloading)
        {
            float reloadProgress = (Time.time - playerShooting.reloadStartTime) / playerShooting.reloadTime;
            ammoBarFill.rectTransform.sizeDelta = new Vector2(0, ammoBarFill.rectTransform.sizeDelta.y);
            reloadFill.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(reloadProgress, 0f, 1f) * originalReloadBarWidth, reloadFill.rectTransform.sizeDelta.y);
        }
        else
        {
            float ammoPercentage = (float)playerShooting.currentAmmo / playerShooting.maxAmmo;
            ammoBarFill.rectTransform.sizeDelta = new Vector2(ammoPercentage * originalAmmoBarWidth, ammoBarFill.rectTransform.sizeDelta.y);
            reloadFill.rectTransform.sizeDelta = new Vector2(0, reloadFill.rectTransform.sizeDelta.y);
        }
    }
}
