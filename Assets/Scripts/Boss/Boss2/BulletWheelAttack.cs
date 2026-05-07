using System.Collections;
using UnityEngine;

public class BulletWheelAttack : BossAttack
{
    [Header("Volley Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackDuration = 4f;
    public float fireCooldown = 0.15f;
    public float bulletSpeed = 5f;

    public float rotationSpeed = 45f;

    private float startAngle = 0f;

    public override IEnumerator Execute(Boss boss, Transform player, BossStats stats)
    {
        float elapsed = 0f;
        float currentAngle = startAngle;

        while (elapsed < attackDuration)
        {
            Fire(currentAngle, stats);

            yield return new WaitForSeconds(fireCooldown);
            elapsed += fireCooldown;
            currentAngle += rotationSpeed * fireCooldown;
        }
    }

    private void Fire(float baseAngle, BossStats stats)
    {
        for (int i = 0; i < 4; i++)
        {
            float angleDeg = baseAngle + i * 90f;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetOwner(gameObject);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction, bulletSpeed, stats.GetDamage(), 0);
            }
        }
    }
}