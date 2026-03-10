using UnityEngine;

public class AudioManager : MonoBehaviour
{
public static AudioManager Instance;

[Header("Audio Sources")]
[SerializeField] private AudioSource sfxSource;

[Header("Clips")]
public AudioClip enemyDeathSound;
public AudioClip reloadSound;
public AudioClip bossSpawnSound;

private float enemyDeathCooldown = 0.1f;
private float lastEnemyDeathTime;

private void Awake()
{
    if (Instance == null)
        Instance = this;
    else
        Destroy(gameObject);
}

public void PlayEnemyDeath()
{
    if (Time.time - lastEnemyDeathTime < enemyDeathCooldown)
        return;

    lastEnemyDeathTime = Time.time;

    if (enemyDeathSound != null)
        sfxSource.PlayOneShot(enemyDeathSound);
}

public void PlayReload()
{
    if (reloadSound != null)
        sfxSource.PlayOneShot(reloadSound);
}

public void PlayBossSpawn()
{
    if (bossSpawnSound != null)
        sfxSource.PlayOneShot(bossSpawnSound);
}

}