using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Optional: damage logic later
        Destroy(gameObject);
    }
}