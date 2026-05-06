using UnityEngine;

public class Tower_Health : Tower_Base
{
    public float healthBoost = 0.2f;
   
        private void Awake()
    {
        towerName = "HEALTH Mod";
        rewardText = "HEALTH +20%";
    }

    protected override void ApplyEffect(Stats stats)
    {
      
        stats.IncreaseHealthPercent(healthBoost);
        Debug.Log("Health upgraded");
        
    }
}
