using UnityEngine;
public class Tower_Agility : Tower_Base
{
    private float speedBoost = 0.2f;
    private float rangeDMGboost = 0.2f;

    private void Awake()
    {
        towerName = "Agility Mod";
        rewardText = "AGILITY +20%";
    }
    protected override void ApplyEffect(Stats stats)
    {

        stats.IncreaseSpeed(speedBoost);
        stats.IncreaseRangedDamage(rangeDMGboost);
        Debug.Log("Speed upgrade: " + stats.speedMultiplier);
        Debug.Log("RangeDMG upgrade: " + stats.rangedDamageMultiplier);
    }

}