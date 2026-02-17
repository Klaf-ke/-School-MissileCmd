using UnityEngine;

public class Bunker : MonoBehaviour
{
    [Header("Bunker Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private GameObject bunkerIconUI;

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

        if (currentHealth <= 0f)
        {
            DestroyBunker();
        }
    }

    private void DestroyBunker()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        if (bunkerIconUI != null)
        {
            bunkerIconUI.SetActive(false);
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