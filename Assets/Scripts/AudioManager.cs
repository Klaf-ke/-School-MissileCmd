using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance for easy access across scripts
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip gameOverSound;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayMusic();
    }

    // --- Music Controls ---
    public void PlayMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;
        
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // --- SFX Controls ---
    public void PlayJump()
    {
        if (jumpSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayGameOver()
    {
        if (gameOverSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }
    }
}