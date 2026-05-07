using System.Collections;
using UnityEngine;

public class BulletWallAttack : BossAttack
{
    [Header("Wall Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;

    public int topBulletCount = 12;
    public int sideBulletCount = 8;

    public float spawnOffset = 0.5f;

    public float cooldown = 3f;

    public BoxCollider2D arenaBounds;

    private enum WallSide { Top, Bottom, Left, Right }

    public override IEnumerator Execute(Boss boss, Transform player, BossStats stats)
    {
        if (arenaBounds == null)
        {
            Debug.LogWarning("BulletWallAttack: arenaBounds not assigned.");
            yield break;
        }

        WallSide side = (WallSide)Random.Range(0, 4);
        SpawnWall(side, stats);
        yield return new WaitForSeconds(cooldown);
    }

    private void SpawnWall(WallSide side, BossStats stats)
    {
        Bounds b = arenaBounds.bounds;

        bool isHorizontal = ((side == WallSide.Top) || (side == WallSide.Bottom));
        int count = isHorizontal ? topBulletCount : sideBulletCount;

        // Randomly pick a spot for the gap
        int gapIndex = Random.Range(0, count);

        // Travel direction
        Vector2 travelDir = side switch
        {
            WallSide.Top    => Vector2.down,
            WallSide.Bottom => Vector2.up,
            WallSide.Left   => Vector2.right,
            WallSide.Right  => Vector2.left,
            _               => Vector2.down
        };

        for (int i = 0; i < count; i++)
        {
            if (i == gapIndex) continue;

            float t = (count > 1) ? (float)i / (count - 1) : 0.5f;
            Vector2 spawnPos = GetSpawnPosition(side, t, b);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetOwner(gameObject);
                bulletScript.SetDirection(travelDir, bulletSpeed, stats.GetDamage(), 0);
            }
        }
    }

    private Vector2 GetSpawnPosition(WallSide side, float t, Bounds b)
    {
        return side switch
        {
            WallSide.Top    => new Vector2(Mathf.Lerp(b.min.x, b.max.x, t), b.max.y + spawnOffset),
            WallSide.Bottom => new Vector2(Mathf.Lerp(b.min.x, b.max.x, t), b.min.y - spawnOffset),
            WallSide.Left   => new Vector2(b.min.x - spawnOffset, Mathf.Lerp(b.min.y, b.max.y, t)),
            WallSide.Right  => new Vector2(b.max.x + spawnOffset, Mathf.Lerp(b.min.y, b.max.y, t)),
            _               => b.center
        };
    }
}