using UnityEngine;

public class Tower_Magnet : Tower_Base
{
    private bool hasBeenUSed = false;

    private void Awake()
    {
        towerName = "MAGNET";
        rewardText = "ATTRACT ALL PICKUPS";
    }

    protected override void ApplyEffect(Stats stats)
    {
        if (hasBeenUSed) return;
        hasBeenUSed = true;
        var magnet = FindFirstObjectByType<PlayerPickupMagnet>();
        if (magnet == null) return;

        var pickups = FindObjectsByType<MagneticPickup>(FindObjectsSortMode.None);
        foreach (var pickup in pickups)
            pickup.TryStartMagnetSequence(magnet.MagnetTarget);
    }
}

