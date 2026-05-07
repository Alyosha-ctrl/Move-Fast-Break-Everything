using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class Boss : MonoBehaviour
{
    public enum BossPhase { Idle, SlowStart, PhaseOne, Dead }
    public BossPhase CurrentPhase => _phase;

    [Header("SlowStart")]
    public float slowStartDuration = 3f;

    [SerializeField] private SoundDefinition hurtSound;

    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private int numXpOrbDrops = 2;
    [SerializeField] private float minXpOrbSpacing = 0.15f;
    [SerializeField] private float xpDropRadius = 1f;

    private float _maxHealth;
    private float _contactDamage;
    private float _damageCooldown = 1f;
    private float _damageTimer = 0f;
    private float _health;
    private BossPhase _phase = BossPhase.Idle;

    private CinemachineCamera _virtualCamera;
    private Transform _player;
    private EnemySpawner _spawner;
    private BossStats _stats;

    public BossHealthBar healthBar;

    [Header("Phases")]
    public BossAttack[] phaseOneAttacks;

    private void Awake()
    {
        _stats = GetComponent<BossStats>();
        _maxHealth = _stats.GetMaxHealth();
        _contactDamage = _stats.GetDamage();
        _health = _maxHealth;
    }

    public void Initialize(EnemySpawner spawner, Transform player, CinemachineCamera virtualCamera)
    {
        _spawner = spawner;
        _player = player;
        _virtualCamera = virtualCamera;
        Activate();
    }

    public void Activate()
    {
        _virtualCamera.transform.position = new Vector3(transform.position.x, transform.position.y, _virtualCamera.transform.position.z);
        _virtualCamera.Follow = null;
        _spawner.ClearAllEnemies();
        healthBar.Initialize(_maxHealth, slowStartDuration);
        SetPhase(BossPhase.SlowStart);
    }

    private void SetPhase(BossPhase newPhase)
    {
        _phase = newPhase;
        StopAllCoroutines();

        switch (_phase)
        {
            case BossPhase.SlowStart:
                StartCoroutine(SlowStart());
                break;
            case BossPhase.PhaseOne:
                StartCoroutine(PhaseOne());
                break;
            case BossPhase.Dead:
                StartCoroutine(DieCo());
                break;
        }
    }

    public void TakeDamage(int damageTaken, float pierce)
    {
        int finalDamage = damageTaken;
        if (_stats != null)
            finalDamage = _stats.CalculateDamageTaken(damageTaken, pierce);

        _health -= finalDamage;
        _health = Mathf.Max(_health, 0);

        DamagePopUp.Create(transform.position, finalDamage);
        SoundManager.Play(hurtSound);
        healthBar.Refresh(_health);

        if (_health <= 0 && _phase != BossPhase.Dead)
            SetPhase(BossPhase.Dead);
    }

    private IEnumerator SlowStart()
    {
        yield return new WaitForSeconds(slowStartDuration);
        SetPhase(BossPhase.PhaseOne);
    }

    private IEnumerator PhaseOne()
    {
        while (true)
        {
            yield return StartCoroutine(RunRandomAttack(phaseOneAttacks));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator RunRandomAttack(BossAttack[] attacks)
    {
        if (attacks.Length == 0) yield break;
        int index = Random.Range(0, attacks.Length);
        yield return StartCoroutine(attacks[index].Execute(this, _player, _stats));
    }

    private IEnumerator DieCo()
    {
        _virtualCamera.Follow = _player;
        SpawnXpOrbs();
        healthBar.gameObject.SetActive(false);
        if (_spawner != null)
            _spawner.OnBossDied();
        Destroy(gameObject, 1f);
        yield break;
    }

    private void SpawnXpOrbs()
    {
        if (xpOrbPrefab == null)
        {
            return;
        }

        Vector3 deathPosition = transform.position;
        Vector3[] placedPositions = new Vector3[numXpOrbDrops];

        for (int i = 0; i < numXpOrbDrops; i++)
        {
            Vector3 spawnPosition = FindXpOrbSpawnPosition(deathPosition, placedPositions, i);
            placedPositions[i] = spawnPosition;
            Instantiate(xpOrbPrefab, spawnPosition, xpOrbPrefab.transform.rotation);
        }
    }

    private Vector3 FindXpOrbSpawnPosition(Vector3 center, Vector3[] placedPositions, int placedCount)
    {
        const int MaxAttempts = 12;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            Vector2 offset = placedCount == 0
                ? Random.insideUnitCircle * (xpDropRadius * 0.5f)
                : Random.insideUnitCircle * xpDropRadius;
            Vector3 candidate = center + new Vector3(offset.x, offset.y, 0f);

            if (IsFarEnoughFromOtherDrops(candidate, placedPositions, placedCount))
            {
                return candidate;
            }
        }

        float angle = placedCount * Mathf.PI;
        Vector2 fallbackOffset = new(Mathf.Cos(angle), Mathf.Sin(angle));
        return center + new Vector3(fallbackOffset.x, fallbackOffset.y, 0f) * minXpOrbSpacing;
    }

    private bool IsFarEnoughFromOtherDrops(Vector3 candidate, Vector3[] placedPositions, int placedCount)
    {
        for (int i = 0; i < placedCount; i++)
        {
            if (Vector2.Distance(candidate, placedPositions[i]) < minXpOrbSpacing)
            {
                return false;
            }
        }

        return true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject != null && collision.gameObject.CompareTag("Player"))
        {
            _damageTimer -= Time.deltaTime;
            if (_damageTimer <= 0f)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage((int)_contactDamage);
                    _damageTimer = _damageCooldown;
                }
            }
        }
    }
}