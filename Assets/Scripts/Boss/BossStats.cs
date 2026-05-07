using System;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseSpeed = 1f;
    public int baseDamage = 1;
    public int baseHealth = 10;
    public float baseDefense = .1f;

    public float GetSpeed()
    {
        return baseSpeed;
    }

    public int GetDamage()
    {
        return Mathf.RoundToInt(baseDamage);
    }

    public int GetMaxHealth()
    {
        return Mathf.RoundToInt(baseHealth);
    }

    public float GetDefense()
    {
        return baseDefense;
    }

    public int CalculateDamageTaken(int incomingDamage, float pierce)
    {
        float effectiveDefense = Mathf.Max(0f, GetDefense() - pierce);

        float reducedDamage =
            incomingDamage * (1f / (1f + effectiveDefense));

        return Mathf.Max(1, Mathf.RoundToInt(reducedDamage));
    }
}