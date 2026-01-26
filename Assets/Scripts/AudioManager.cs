using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        // Singleton : une seule instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // === AUDIO SOURCES ===
    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    // === CLIPS AUDIO ===
    [Header("Interface Sounds")]
    public AudioClip buttonClick;
    public AudioClip successSound;
    public AudioClip errorSound;
    
    [Header("Game Sounds")]
    public AudioClip moneyGain;
    public AudioClip craftSound;
    public AudioClip purchaseSound;
    public AudioClip sellSound;
    public AudioClip levelUpSound;
    public AudioClip eventSound;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
    
    void Start()
    {
        // Crée les AudioSources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    
        // Charge les volumes sauvegardés (ou utilise les valeurs par défaut)
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
    
        // Configure la musique
        musicSource.loop = true;
        musicSource.volume = musicVolume; // Utilise le volume chargé
        musicSource.playOnAwake = false;
    
        // Configure les SFX
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume; // Utilise le volume chargé
        sfxSource.playOnAwake = false;
    
        // Lance la musique
        PlayMusic();
    
        Debug.Log("🔊 AudioManager initialisé - Musique: " + musicVolume + ", SFX: " + sfxVolume);
    }
    
    // === MUSIQUE ===
    public void PlayMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    
    // === EFFETS SONORES ===
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume; 
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    // === FONCTIONS PRATIQUES ===
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlaySuccess() => PlaySFX(successSound);
    public void PlayError() => PlaySFX(errorSound);
    public void PlayMoneyGain() => PlaySFX(moneyGain);
    public void PlayCraft() => PlaySFX(craftSound);
    public void PlayPurchase() => PlaySFX(purchaseSound);
    public void PlaySell() => PlaySFX(sellSound);
    public void PlayLevelUp() => PlaySFX(levelUpSound);
    public void PlayEvent() => PlaySFX(eventSound);
}