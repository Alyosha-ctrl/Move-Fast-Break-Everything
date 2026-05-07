using System.Collections;
using UnityEngine;

public class SprayAttack : BossAttack
{
    [Header("References")]
    public SpriteRenderer coneRenderer;
    public Transform cone;

    [Header("Settings")]
    public float warningDuration = 1f;
    public float displayDuration = 0.3f;
    public float cooldown = 3f;
    public Color warningColor = new Color(1f, 0.4f, 0f, 0.5f);
    public Color fireColor = new Color(1f, 0.1f, 0f, 1f);

    public override IEnumerator Execute(Boss boss, Transform player, BossStats stats)
    {
        Vector2 lockedDir = (player.position - boss.transform.position).normalized;
        float angle = Mathf.Atan2(lockedDir.y, lockedDir.x) * Mathf.Rad2Deg;

        // Point at the player
        cone.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        coneRenderer.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        coneRenderer.color = warningColor;
        coneRenderer.enabled = true;

        yield return new WaitForSeconds(warningDuration);

        coneRenderer.color = fireColor;

        // Check if player is in cone upon firing
        Collider2D coneCollider = coneRenderer.GetComponent<Collider2D>();
        Collider2D[] hits = new Collider2D[10];
        int count = coneCollider.Overlap(new ContactFilter2D().NoFilter(), hits);
        for (int i = 0; i < count; i++)
        {
            if (hits[i].CompareTag("Player"))
            {
                Player p = hits[i].GetComponent<Player>();
                if (p != null) p.TakeDamage(stats.GetDamage());
                break;
            }
}

        yield return new WaitForSeconds(displayDuration);

        coneRenderer.enabled = false;

        yield return new WaitForSeconds(cooldown);
    }
}