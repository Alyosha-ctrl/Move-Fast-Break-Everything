using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public WeaponSO weaponSO;

    private GameObject owner;
    private Stats stats;

    private float timer;
    private float attackInterval;

    private Collider2D hitbox;

    private float pierce=1;

    private readonly List<Collider2D> targets = new List<Collider2D>();
    int damage;

    public SpriteRenderer visual1;
    public SpriteRenderer visual2;

    void Start()
    {
        owner = transform.root.gameObject;
        stats = owner.GetComponent<Stats>();

        attackInterval = weaponSO.coolDown;

        hitbox = GetComponent<Collider2D>();

        Debug.Log("Melee running");
        damage = weaponSO.baseDamage;
        Debug.Log("Damage: " + weaponSO.baseDamage);
    }
    public void IncreaseDamage(int amount)
    {
        
        damage+=amount;
        Debug.Log("Damage increased by " + damage);
    }
    public void hitSpeedIncrease(float amount)
    {
        attackInterval -= amount;
        Debug.Log("Attack speed increased by " + amount);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        visual1.color = Color.darkRed;
        visual2.color = Color.darkRed;

        StartCoroutine(VisualTimer());

        if (stats != null)
            damage = Mathf.RoundToInt(weaponSO.baseDamage * stats.damageMultiplier);

        foreach (var target in new List<Collider2D>(targets))
        {
            if (target == null) continue;

            Enemy enemy = target.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage,pierce);
                continue;
            }

            BossController boss = target.GetComponentInParent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("Boss Damage: " + damage);
                continue;
            }

            DestructibleObstacle obstacle = target.GetComponentInParent<DestructibleObstacle>();
            if (obstacle != null)
            {
                obstacle.TakeDamage(damage);

            }
        }
    }

    IEnumerator VisualTimer()
    {
        yield return new WaitForSeconds(.1f);
        visual1.color = Color.white;
        visual2.color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner) return;

        if (!targets.Contains(collision))
            targets.Add(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        targets.Remove(collision);
    }
}