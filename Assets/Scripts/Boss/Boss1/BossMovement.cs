using UnityEngine;
using Unity.Cinemachine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    public float initialSpeed = 3f;
    public float acceleration = 0.5f;
    public float maxSpeed = 12f;

    private Vector2 velocity;
    private Bounds arenaBounds;
    private Boss boss;

    private void Awake()
    {
        boss = GetComponent<Boss>();
    }

    private void Start()
    {
        BoxCollider2D arena = transform.Find("BossArena").GetComponent<BoxCollider2D>();
        arenaBounds = arena.bounds;
        arenaBounds = new Bounds(arenaBounds.center, new Vector3(arenaBounds.size.x - 5f, arenaBounds.size.y - 4f, arenaBounds.size.z));

        velocity = new Vector2(1f, 1f).normalized * initialSpeed;
    }

    private void Update()
    {
        if (boss.CurrentPhase != Boss.BossPhase.PhaseOne) return;

        transform.position += (Vector3)velocity * Time.deltaTime;

        Vector3 pos = transform.position;

        if (pos.x <= arenaBounds.min.x || pos.x >= arenaBounds.max.x)
        {
            velocity.x = -velocity.x;
            pos.x = Mathf.Clamp(pos.x, arenaBounds.min.x, arenaBounds.max.x);
            velocity = velocity.normalized * Mathf.Min(velocity.magnitude + acceleration, maxSpeed);
        }

        if (pos.y <= arenaBounds.min.y || pos.y >= arenaBounds.max.y)
        {
            velocity.y = -velocity.y;
            pos.y = Mathf.Clamp(pos.y, arenaBounds.min.y, arenaBounds.max.y);
            velocity = velocity.normalized * Mathf.Min(velocity.magnitude + acceleration, maxSpeed);
        }

        transform.position = pos;
    }
}