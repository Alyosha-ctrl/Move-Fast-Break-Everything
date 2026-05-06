using UnityEngine;

public class Tower_Defense : Tower_Base
{
    private float defenseBoost = 0.2f;
    
        private void Awake()
    {
        towerName = "Agility Mod";
        rewardText = "AGILITY +20%";
    }

    protected override void ApplyEffect(Stats stats)
    {
        
        stats.IncreaseDefense(defenseBoost);
        Debug.Log("Defense upgraded: " + stats.defense);
        
    }
}

