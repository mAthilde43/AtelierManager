using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI continueText; // Texte "(Continuer la partie)"
    
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
}