using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour
{
    // === PROGRESSION ===
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 1500;
    
    // === UI ===
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Image xpBarFill;
    public GameObject levelUpNotification;
    public TextMeshProUGUI levelUpText;
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        UpdateProgressionDisplay();
        
        Debug.Log("ProgressionManager initialisé - Niveau " + currentLevel);
    }
    
    // Ajoute de l'expérience
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        
        // ===== APPLIQUE LES MULTIPLICATEURS D'XP =====
        float finalXP = amount;
        
        // Bonus Building
        if (BuildingManager.Instance != null)
        {
            finalXP *= BuildingManager.Instance.GetXPBonusMultiplier();
        }
        
        // Bonus Booster
        if (BoosterManager.Instance != null)
        {
            finalXP *= BoosterManager.Instance.GetXPMultiplier();
        }
        
        // Arrondit et ajoute
        int xpToAdd = Mathf.RoundToInt(finalXP);
        currentExperience += xpToAdd;
        
        Debug.Log("XP gagné : " + amount + " → " + xpToAdd + " (avec bonus)");
        // ==============================================
    
        // Feedback visuel XP
        if (FeedbackManager.Instance != null && xpText != null)
        {
            Vector3 position = xpText.transform.position;
            FeedbackManager.Instance.ShowXPGain(xpToAdd, position);
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
    
        // Augmente l'XP nécessaire pour le prochain niveau
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.8f);
    
        Debug.Log("NIVEAU SUPÉRIEUR ! Niveau " + currentLevel + " atteint !");
        
        // Son de level up
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUp();
        }
        
        // Affiche la notification
        ShowLevelUpNotification();
    
        // Récompenses du niveau
        GiveLevelRewards();
        
        // Vérifie si des recettes peuvent être débloquées
        if (RecipeUnlockManager.Instance != null)
        {
            RecipeUnlockManager.Instance.OnLevelUp();
        }
        
        // Met à jour l'accès à l'onglet Atelier
        TabManager tabManager = FindObjectOfType<TabManager>();
        if (tabManager != null)
        {
            tabManager.UpdateBuildingTabAccess();
        }
    }
    
    // Donne les récompenses du niveau
    void GiveLevelRewards()
    {
        // Récompense en argent
        int moneyReward = currentLevel * 100;
        gameManager.AddMoney(moneyReward);
        Debug.Log("Récompense de niveau : +" + moneyReward + "€");
        
        // Déblocages selon le niveau
        UnlockContentAtLevel(currentLevel);
        
        // ===== NOUVEAU CODE =====
        // Notification de déblocage de l'atelier
        if (currentLevel == 10)
        {
            if (FeedbackManager.Instance != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                FeedbackManager.Instance.ShowSuccess("ATELIER DÉBLOQUÉ !\nVous pouvez maintenant améliorer votre espace !", screenCenter);
            }
            
            Debug.Log("ATELIER DÉBLOQUÉ ! Vous pouvez maintenant construire et améliorer votre espace de travail !");
        }
        // ========================
    }
    
    // Débloque du contenu selon le niveau
    void UnlockContentAtLevel(int level)
    {
        switch (level)
        {
            case 2:
                Debug.Log("Niveau 2 : Vous pouvez maintenant accéder à de meilleurs fournisseurs !");
                break;
                
            case 3:
                Debug.Log("Niveau 3 : Nouveau matériau débloqué !");
                break;
                
            case 5:
                Debug.Log("Niveau 5 : Nouveau produit débloqué !");
                break;
                
            case 10:
                Debug.Log("Niveau 10 : Maître artisan ! Tous les contenus débloqués !");
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
    
        // Met à jour la barre d'XP avec animation fluide
        if (xpBarFill != null)
        {
            float targetFillAmount = (float)currentExperience / (float)experienceToNextLevel;
    
            // Arrête l'animation précédente si elle existe
            StopCoroutine("AnimateXPBar");
    
            // Lance l'animation
            StartCoroutine(AnimateXPBar(xpBarFill.fillAmount, targetFillAmount));
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
            levelUpText.text = "NIVEAU SUPÉRIEUR !\nNiveau " + currentLevel + " atteint !";
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
    
    // === ANIMATION DE LA BARRE XP ===
    private System.Collections.IEnumerator AnimateXPBar(float startValue, float targetValue)
    {
        float duration = 0.5f;
        float elapsed = 0f;
    
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
        
            // Courbe d'animation (ease out)
            t = 1f - Mathf.Pow(1f - t, 3f);
        
            xpBarFill.fillAmount = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }
    
        // Assure que la valeur finale est exacte
        xpBarFill.fillAmount = targetValue;
    }
}
