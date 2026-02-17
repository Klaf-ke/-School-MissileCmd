using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageToBunker = 1f;
    [SerializeField] private float speed = 5f;

    private float currentHealth;

    private WaveManager waveManager;
    private Bunker targetBunker;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead || targetBunker == null || targetBunker.IsDestroyed())
            return;

        
        Vector3 dir = (targetBunker.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        
       
    }

    
    public void SetTargetBunker(Bunker bunker)
    {
        targetBunker = bunker;
    }

    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (waveManager != null)
        {
            waveManager.EnemyDied();
        }

        Destroy(gameObject);
    }

    public void AddBonusHealth(float amount)
    {
        currentHealth += amount;
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Bunker"))
    {
        Bunker bunker = other.GetComponent<Bunker>();

        if (bunker != null)
        {
            bunker.TakeDamage(damageToBunker);
        }

        
        TakeDamage(9999f);
    }
}
}