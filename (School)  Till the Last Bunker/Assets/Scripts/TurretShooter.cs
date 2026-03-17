using UnityEngine;
using System.Collections;

public class TurretShooter : MonoBehaviour
{
    [Header("Firing")]
    public GameObject projectilePrefab;
    public Transform[] firePoints;
    public float fireRate = 0.25f;
    public float projectileForce = 40f;
    [SerializeField] private AudioClip fireSound;

    [Header("Damage Settings")]
    public float damage = 10f;


    [Header("Ammo Settings")]
    public int magazineSize = 12;
    public int maxReserveAmmo = 120;
    public int startingReserveAmmo = 60;
    public float reloadTime = 2f;

    [HideInInspector] public bool isActive = false;

    private float nextFireTime;

    private int currentAmmo;
    private int reserveAmmo;

    private bool isReloading = false;
    private int currentBarrelIndex = 0;
    
    [SerializeField] private TurretUIManager uiManager;
    
    void Start()
    {
        currentAmmo = magazineSize;
        reserveAmmo = startingReserveAmmo;
        UpdateAmmoUI();
        
    }

    void Update()
    {
        if (!isActive) return;
        if (isReloading) return;

        bool isFiring =
            Input.GetButton("Fire1") ||
            Input.GetKey(KeyCode.Space);

        if (isFiring && Time.time >= nextFireTime)
    {
        if (currentAmmo > 0)
        {
            Shoot();
            currentAmmo--;
            nextFireTime = Time.time + fireRate;
            UpdateAmmoUI();
        }
            else
        {   
            if (!isReloading && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
}

    
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentAmmo < magazineSize && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

void Shoot()
{
    Transform firePoint = firePoints[currentBarrelIndex];

    Quaternion rotationOffset = Quaternion.Euler(-90f, 0f, 0f);

    GameObject projectile = Instantiate(
        projectilePrefab,
        firePoint.position,
        firePoint.rotation * rotationOffset
    );

    Rigidbody rb = projectile.GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.linearVelocity = firePoint.forward * projectileForce;
    }

    if (fireSound != null)
    {
        AudioSource.PlayClipAtPoint(fireSound, firePoint.position);
    }

    Projectile projectileScript = projectile.GetComponent<Projectile>();

    if (projectileScript != null)
    {
        projectileScript.SetDamage(damage);
    }

    currentBarrelIndex++;
    if (currentBarrelIndex >= firePoints.Length)
        currentBarrelIndex = 0;
}
    IEnumerator Reload()
    {
        if (reserveAmmo <= 0)
            yield break;

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayReload();
        }

        int ammoNeeded = magazineSize - currentAmmo;

        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        UpdateAmmoUI();
    }

    
    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        reserveAmmo = Mathf.Clamp(reserveAmmo, 0, maxReserveAmmo);

        UpdateAmmoUI(); 
    }
    private void UpdateAmmoUI()
    {
        if (isActive && uiManager != null)
        uiManager.UpdateAmmo(currentAmmo, reserveAmmo);
    }

    public void UpgradeTurret(int wave)
    {
        maxReserveAmmo += 10;

        if (wave % 5 == 0)
        {
            magazineSize += 5;
            currentAmmo += 5;
        }

    
        reloadTime = Mathf.Max(0.8f, reloadTime - 0.05f);

    
        fireRate = Mathf.Max(0.08f, fireRate - 0.01f);

        UpdateAmmoUI();
    }






}