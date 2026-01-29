using UnityEngine;

public class TurretShooter : MonoBehaviour
{
    [Header("Firing")]
    public GameObject projectilePrefab;
    public Transform[] firePoints;
    public float fireRate = 0.25f;
    public float projectileForce = 40f;

    [HideInInspector] public bool isActive = false;

    private float nextFireTime;

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        foreach (Transform firePoint in firePoints)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                firePoint.rotation
            );

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * projectileForce;
            }
        }
    }
}