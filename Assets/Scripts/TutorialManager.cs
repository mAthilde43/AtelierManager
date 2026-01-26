using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    private bool tutorialShown = false;
    
    void Start()
    {
        // Vérifie si c'est la première fois (via PlayerPrefs)
        int hasSeenTutorial = PlayerPrefs.GetInt("HasSeenTutorial", 0);
        
        if (hasSeenTutorial == 0)
        {
            // Première fois : montre le tutoriel
            ShowTutorial();
        }
        else
        {
            // Déjà vu : cache le tutoriel
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }
    }
    
    public void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            tutorialShown = true;
            // Met en pause le temps pendant le tutoriel
            Time.timeScale = 0f;
        }
    }
    
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            // Sauvegarde qu'on a vu le tutoriel
            PlayerPrefs.SetInt("HasSeenTutorial", 1);
            PlayerPrefs.Save();
            // Reprend le temps
            Time.timeScale = 1f;
        }
    }
    
    // Pour réinitialiser (debug)
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("HasSeenTutorial");
        PlayerPrefs.Save();
        Debug.Log("🔄 Tutoriel réinitialisé");
    }
}