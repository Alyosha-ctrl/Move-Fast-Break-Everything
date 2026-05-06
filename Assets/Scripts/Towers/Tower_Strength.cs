using UnityEngine;

public class Tower_Strength : Tower_Base
{
    public float pierceBoost = 0.2f;
    public float thornsBoost = 0.2f;

        private void Awake()
    {
        towerName = "STRENGTH Mod";
        rewardText = "STRENGTH +20%";
    }

    protected override void ApplyEffect(Stats stats)
    {
    
        stats.IncreasePierce(pierceBoost);
        stats.IncreaseThorns(thornsBoost);
        Debug.Log("Pierce and Throns upgraded");
        
    }
}
