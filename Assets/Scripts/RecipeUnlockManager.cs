using UnityEngine;
using System.Collections.Generic;

public class RecipeUnlockManager : MonoBehaviour
{
    // Singleton
    private static RecipeUnlockManager instance;
    public static RecipeUnlockManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RecipeUnlockManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private ProgressionManager progressionManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();
        
        // Vérifie les déverrouillages au démarrage
        CheckUnlocks();
        
        Debug.Log("RecipeUnlockManager initialisé");
    }
    
    // Vérifie tous les déverrouillages possibles
    public void CheckUnlocks()
    {
        if (gameManager == null || progressionManager == null) return;
        
        bool hasUnlocked = false;
        
        foreach (Product product in gameManager.products)
        {
            // Si le produit est déjà débloqué, on passe
            if (product.isUnlocked) continue;
            
            // Vérifie si on peut le débloquer
            if (CanUnlock(product))
            {
                UnlockProduct(product);
                hasUnlocked = true;
            }
        }
        
        // Si on a débloqué quelque chose, rafraîchit l'UI
        if (hasUnlocked)
        {
            gameManager.RefreshAllUI();
        }
    }
    
    // Vérifie si un produit peut être débloqué
    bool CanUnlock(Product product)
    {
        if (progressionManager == null) return false;
        
        // Condition : Niveau du joueur
        if (product.unlockLevel > 0)
        {
            return progressionManager.currentLevel >= product.unlockLevel;
        }
        
        return false;
    }
    
    // Débloque un produit
    void UnlockProduct(Product product)
    {
        product.isUnlocked = true;
        
        Debug.Log("NOUVELLE RECETTE DÉBLOQUÉE : " + product.productName + " !");
        
        // Feedback visuel
        if (FeedbackManager.Instance != null)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            FeedbackManager.Instance.ShowSuccess("NOUVELLE RECETTE : " + product.productName, screenCenter);
        }
        
        // Met à jour les notifications (badges)
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.UpdateAllNotifications();
        }
    }
    
    // Appelée quand le joueur monte de niveau
    public void OnLevelUp()
    {
        CheckUnlocks();
    }
}
