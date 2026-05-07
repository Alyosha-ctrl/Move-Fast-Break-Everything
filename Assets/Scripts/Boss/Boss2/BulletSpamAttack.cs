using System.Collections;
using UnityEngine;

public class BulletSpamAttack : BossAttack
{
    [Header("Barrage Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackDuration = 5f;
    public float fireRate = 3f;
    public float bulletSpeed = 6f;

    public override IEnumerator Execute(Boss boss, Transform player, BossStats stats)
    {
        float elapsed = 0f;
        float interval = 1f / fireRate;

        while (elapsed < attackDuration)
        {
            if (player != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                SpawnBullet(direction, stats);
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private void SpawnBullet(Vector2 direction, BossStats stats)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetOwner(gameObject);
            bulletScript.SetDirection(direction, bulletSpeed, stats.GetDamage(), 1);
        }
    }
}