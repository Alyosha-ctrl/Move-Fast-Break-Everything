using System.Collections;
using UnityEngine;

public class CircleBullet : MonoBehaviour
{
    public WeaponSO weaponSO;
    
    public float damageDelay = 0.5f; 
    private bool canTakeDamage = true;
    private float pierce=1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canTakeDamage)
        {
            return;
        }

        DestructibleObstacle obstacle = collision.GetComponentInParent<DestructibleObstacle>();
        Enemy enemy = collision.GetComponent<Enemy>();
        Boss boss = collision.GetComponentInParent<Boss>();

        if (enemy != null)
        {
            enemy.TakeDamage(weaponSO.baseDamage,pierce);
            StartCoroutineCooldown();
        }
        if (obstacle != null)
        {
            obstacle.TakeDamage(weaponSO.baseDamage);
            StartCoroutineCooldown();
        }
        if(boss != null)
        {
            boss.TakeDamage(weaponSO.baseDamage,pierce);
            StartCoroutineCooldown();
        }
        
    }
    private IEnumerator StartCoroutineCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageDelay);
        canTakeDamage = true;
    }
}
