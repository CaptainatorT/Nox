using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Weapon")]
public class Weapon : Item
{
    public float fireRate = 720f;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;
    public float reloadTime = 1.5f;
    public int maxAmmo = 10;
    public float accuracy = 100f;
    public float damage = 10f;
    public int piercing = 1;

    public enum FireMode { Semi, Burst, FullAuto }
    public FireMode fireMode = FireMode.FullAuto;
    public int projectilesPerBurst = 3;
    public float burstDelay = 1.0f;

    // Add other relevant stats as necessary
    public string GenerateTooltipInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Fire Rate: {fireRate}");
        sb.AppendLine($"Projectile Speed: {projectileSpeed}");
        sb.AppendLine($"Reload Time: {reloadTime}");
        sb.AppendLine($"Max Ammo: {maxAmmo}");
        sb.AppendLine($"Accuracy: {accuracy}");
        sb.AppendLine($"Damage: {damage}");
        // ... add other attributes as needed ...

        return sb.ToString();
    }
}

