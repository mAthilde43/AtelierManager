using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI continueText; // Texte "(Continuer la partie)"
    public Slider musicVolumeSlider; // Ajoute cette ligne
    public Slider sfxVolumeSlider;   // Ajoute cette ligne
    public GameObject settingsPanel;
    void Start()
    {
        // Vérifie si une sauvegarde existe
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null && continueText != null)
        {
            if (saveManager.HasSaveData())
            {
                continueText.text = "(Continuer la partie)";
                continueText.gameObject.SetActive(true);
            }
            else
            {
                continueText.gameObject.SetActive(false);
            }
        }
    
        // Initialise les sliders de volume avec les valeurs actuelles
        if (AudioManager.Instance != null)
        {
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.musicVolume;
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
            }
        }
    }
    // Fonction pour charger le jeu
    public void PlayGame()
    {
        Debug.Log("🎮 Chargement du jeu...");
        SceneManager.LoadScene("MainGame");
    }
    
    // Fonction pour quitter le jeu
    public void QuitGame()
    {
        Debug.Log("👋 Fermeture du jeu...");
        Application.Quit();
        
        #if UNITY_EDITOR
        // Dans l'éditeur Unity, arrête le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    // Fonction pour démarrer une nouvelle partie (efface la sauvegarde)
    public void NewGame()
    {
        Debug.Log("🆕 Nouvelle partie - Suppression de la sauvegarde...");
    
        // Trouve le SaveManager (il persiste entre les scènes)
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            saveManager.DeleteSaveData();
        }
    
        // Charge le jeu
        SceneManager.LoadScene("MainGame");
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager audioManager = AudioManager.Instance;
    
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(value);
            Debug.Log("🎵 Volume musique changé : " + value);
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager introuvable !");
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager audioManager = AudioManager.Instance;
    
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(value);
            Debug.Log("🔊 Volume SFX changé : " + value);
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager introuvable !");
        }
    }
}