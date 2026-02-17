using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private float defaultDamage = 1f;
    private float damage;

    private void Start()
    {
        
        if (damage <= 0)
            damage = defaultDamage;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    
    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        
    }
}