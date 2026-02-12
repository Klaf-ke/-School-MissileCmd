using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    private WaveManager waveManager;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    
    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
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
}