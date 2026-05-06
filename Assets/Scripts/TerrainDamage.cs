using UnityEngine;

public class TerrainDamage : MonoBehaviour
{
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private Player player;
    [SerializeField] private Jump jump;
    [SerializeField] private int hazardDamage = 1;
    [SerializeField] private int deathDamage = 2;
    [SerializeField] private float damageInterval = 1f;
    
    private float damageTimer;
    
    private void Awake()
    {
        player = GetComponent<Player>();
        jump = GetComponent<Jump>();
        chunkManager = FindAnyObjectByType<ChunkManager>();
    }
    
    private void Update()
    {
        if (jump.IsJumping)
        {
            return;
        }
        
        damageTimer -= Time.deltaTime;
        Vector2 currentPosition = transform.position;
        
        if (chunkManager.IsDeathTerrain(currentPosition))
        {
            DamageTick(deathDamage);
            return;
        }
        
        if (chunkManager.IsHazardTerrain(currentPosition))
        {
            DamageTick(hazardDamage);
            return;
        }
        
        damageTimer = 0f;
    }
    
    private void DamageTick(int damage)
    {
        if (damageTimer > 0f)
        {
            return;
        }
        
        player.TakeDamage(damage);
        damageTimer = damageInterval;
    }
}
