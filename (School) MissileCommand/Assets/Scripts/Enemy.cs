using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 3f;
    [SerializeField] protected float damageToBunker = 1f;
    [SerializeField] protected float speed = 5f;

    protected float currentHealth;

    protected WaveManager waveManager;
    protected Bunker targetBunker;

    protected bool isDead = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (isDead || targetBunker == null || targetBunker.IsDestroyed())
            return;

        Move();
    }

    
    protected virtual void Move()
    {
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

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }

        Debug.Log("Boss HP: " + currentHealth);
    }

    public virtual void AddBonusHealth(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
    }



    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        if (waveManager != null)
        {
            waveManager.EnemyDied();
        }

        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
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