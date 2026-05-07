using System.Collections;
using UnityEngine;


public class BulletAttack : BossAttack
{
    [Header("Charge Settings")]
    public GameObject bulletPrefab;
    private LineRenderer _lineRenderer;
    public Transform firePoint;
    public float chargeDuration = 1f;
    public float beamDistance = 30f;
    public float bulletSpeed = 25f;


    public Color beamWarningColor = new Color(1f, 0.4f, 0f, 0.5f);


    private GameObject _activeChargeVFX;


    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }


    public override IEnumerator Execute(Boss boss, Transform player, BossStats stats)
    {
        Vector2 lockedDir = (player.position - boss.transform.position).normalized;
        Vector3 endPoint = boss.transform.position + (Vector3)(lockedDir * beamDistance);


        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.startColor = beamWarningColor;
        _lineRenderer.endColor = beamWarningColor;
        _lineRenderer.SetPosition(0, boss.transform.position);
        _lineRenderer.SetPosition(1, endPoint);
        _lineRenderer.enabled = true;


        yield return new WaitForSeconds(chargeDuration);


        _lineRenderer.enabled = false;


        if (player != null)
        {
            SpawnBullet(lockedDir, stats);
        }
    }


    private void SpawnBullet(Vector2 direction, BossStats stats)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetOwner(gameObject);
            bulletScript.SetDirection(direction, bulletSpeed, stats.GetDamage(), 0);
        }
    }
}