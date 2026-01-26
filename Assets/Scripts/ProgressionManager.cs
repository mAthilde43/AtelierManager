using UnityEngine;
using TMPro;
using UnityEngine.UI; // ← Ajoute cette ligne tout en haut avec les autres using

public class ProgressionManager : MonoBehaviour
{
    // === PROGRESSION ===
    public int currentLevel = 1;              // Niveau actuel
    public int currentExperience = 0;         // Expérience actuelle
    public int experienceToNextLevel = 100;   // XP nécessaire pour niveau suivant
    
    // === UI ===
    public TextMeshProUGUI levelText;         // Affichage du niveau
    public TextMeshProUGUI xpText;            // Affichage de l'XP
    public Image xpBarFill;  
    public GameObject levelUpNotification;  
    public TextMeshProUGUI levelUpText; 
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        UpdateProgressionDisplay();
        
        Debug.Log("📊 ProgressionManager initialisé - Niveau " + currentLevel);
    }
    
    // Ajoute de l'expérience
    // Ajoute de l'expérience
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("⭐ +" + amount + " XP | Total: " + currentExperience + "/" + experienceToNextLevel);
    
        // Feedback visuel XP
        if (FeedbackManager.Instance != null && xpText != null)
        {
            Vector3 position = xpText.transform.position;
            FeedbackManager.Instance.ShowXPGain(amount, position);
        }
    
        // Vérifie si on passe au niveau suivant
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    
        UpdateProgressionDisplay();
    }
    
    // Passe au niveau suivant
    void LevelUp()
    {
        currentExperience -= experienceToNextLevel;
        currentLevel++;
    
        // Augmente l'XP nécessaire pour le prochain niveau (difficulté croissante)
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);
    
        Debug.Log("🎉 NIVEAU SUPÉRIEUR ! Niveau " + currentLevel + " atteint !");
        // Son de level up
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUp();
        }
        
        // Affiche la notification
        ShowLevelUpNotification();
    
        // Récompenses du niveau
        GiveLevelRewards();
    }
    
    // Donne les récompenses du niveau
    void GiveLevelRewards()
    {
        // Récompense en argent
        int moneyReward = currentLevel * 100;
        gameManager.AddMoney(moneyReward);
        Debug.Log("💰 Récompense de niveau : +" + moneyReward + "€");
        
        // Déblocages selon le niveau
        UnlockContentAtLevel(currentLevel);
    }
    
    // Débloque du contenu selon le niveau
    void UnlockContentAtLevel(int level)
    {
        switch (level)
        {
            case 2:
                Debug.Log("🔓 Niveau 2 : Vous pouvez maintenant accéder à de meilleurs fournisseurs !");
                break;
                
            case 3:
                Debug.Log("🔓 Niveau 3 : Nouveau matériau débloqué !");
                // On ajoutera de vrais déblocages dans les prochaines étapes
                break;
                
            case 5:
                Debug.Log("🔓 Niveau 5 : Nouveau produit débloqué !");
                break;
                
            case 10:
                Debug.Log("🔓 Niveau 10 : Maître artisan ! Tous les contenus débloqués !");
                break;
        }
    }
    
    // Met à jour l'affichage de la progression
    void UpdateProgressionDisplay()
    {
        if (levelText != null)
        {
            levelText.text = "Niveau " + currentLevel;
        }
    
        if (xpText != null)
        {
            xpText.text = currentExperience + " / " + experienceToNextLevel + " XP";
        }
    
        // Met à jour la barre d'XP
        if (xpBarFill != null)
        {
            float fillAmount = (float)currentExperience / (float)experienceToNextLevel;
            xpBarFill.fillAmount = fillAmount;
        }
    }
    
    // Fonction pour donner de l'XP selon l'action
    public void OnProductCrafted(int productValue)
    {
        // Donne de l'XP en fonction de la valeur du produit
        int xp = Mathf.RoundToInt(productValue / 10f);
        AddExperience(xp);
    }
    
    public void OnProductSold(int saleValue)
    {
        // Donne de l'XP en fonction de la vente
        int xp = Mathf.RoundToInt(saleValue / 5f);
        AddExperience(xp);
    }
    
    public void OnUpgradeBought()
    {
        // Bonus d'XP pour avoir acheté une amélioration
        AddExperience(50);
    }
    
    // Affiche la notification de level up
    void ShowLevelUpNotification()
    {
        if (levelUpNotification != null && levelUpText != null)
        {
            levelUpText.text = "🎉 NIVEAU SUPÉRIEUR !\nNiveau " + currentLevel + " atteint !";
            levelUpNotification.SetActive(true);
        
            // Cache automatiquement après 3 secondes
            Invoke("HideLevelUpNotification", 3f);
        }
    }

// Cache la notification
    void HideLevelUpNotification()
    {
        if (levelUpNotification != null)
        {
            levelUpNotification.SetActive(false);
        }
    }
}