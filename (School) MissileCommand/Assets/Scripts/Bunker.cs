using UnityEngine;

public class Bunker : MonoBehaviour
{
    [Header("Bunker Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Audio")]
    [SerializeField] private AudioClip destructionSound;
    [SerializeField] private AudioClip lowHealthWarning;

    private bool warningPlayed = false;
    private float currentHealth;
    private bool isDestroyed = false;

    public System.Action<Bunker> OnBunkerDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        if (!warningPlayed && currentHealth <= maxHealth * 0.25f)
        {
            warningPlayed = true;

            if (lowHealthWarning != null)
            {
                AudioSource.PlayClipAtPoint(lowHealthWarning, transform.position);
            }
        }

        if (currentHealth <= 0f)
        {
            DestroyBunker();
        }
    }

    private void DestroyBunker()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        
        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        OnBunkerDestroyed?.Invoke(this);

        Destroy(gameObject);
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}