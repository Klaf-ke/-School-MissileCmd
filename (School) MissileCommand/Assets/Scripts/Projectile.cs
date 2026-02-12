using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}