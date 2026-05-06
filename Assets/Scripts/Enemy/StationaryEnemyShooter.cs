using UnityEngine;

public class StationaryEnemyShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float fireCooldown = 3f;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private int baseDamage = 5;
    
    private Transform player;
    private float fireTimer;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    private void Update()
    {
        fireTimer -= Time.deltaTime;
        
        if (fireTimer > 0f || Vector2.Distance(transform.position, player.position) > detectionRange)
        {
            return;
        }
        
        ShootAtPlayer();
        fireTimer = fireCooldown;
    }
    
    private void ShootAtPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Bullet bullet = projectile.GetComponent<Bullet>();
        
        bullet.SetOwner(gameObject);
        bullet.SetDirection(direction, projectileSpeed, baseDamage, 0f);
    }
}
