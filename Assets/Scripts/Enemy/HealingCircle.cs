using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HealingCircle : MonoBehaviour
{

    private float timer=1;
    public float attackInterval = 1f;
    public int damage = 1;
    public int healing = 1;
    private readonly List<Collider2D> targets = new List<Collider2D>();

    private Enemy owner;

    void Start()
    {
        owner = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            HealAndAttack();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       

        if (!targets.Contains(collision))
            targets.Add(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        targets.Remove(collision);
    }

    void HealAndAttack()
    {





        foreach (var target in new List<Collider2D>(targets))
        {
         
            if (target == null) continue;
            Enemy enemy = target.GetComponentInParent<Enemy>();
            Player player = target.GetComponentInParent<Player>();
            if (enemy != null && enemy != owner)
            {
               
                enemy.Heal(healing);

            }
            if (player != null)
            {

                player.TakeDamage(damage);
            }
        }

            
        }
    }
