using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Retour au menu principal
    public void LoadMainMenu()
    {
        Debug.Log("🏠 Retour au menu...");
        SceneManager.LoadScene("MainMenu");
    }
    
    // Recharger la scène actuelle (restart)
    public void RestartCurrentScene()
    {
        Debug.Log("🔄 Redémarrage...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}