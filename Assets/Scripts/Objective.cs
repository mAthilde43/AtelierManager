using UnityEngine;

// Types d'objectifs possibles
public enum ObjectiveType
{
    EarnMoney,          // Gagner X€
    SpendMoney,         // Dépenser X€
    CraftProducts,      // Fabriquer X produits
    SellProducts,       // Vendre X produits
    BuyMaterials,       // Acheter X matériaux
    ReachLevel,         // Atteindre le niveau X
    BuyUpgrades,        // Acheter X améliorations
    SurviveDays   ,      // Survivre X jours
    CompleteOrders
}

[System.Serializable]
public class Objective
{
    public string objectiveName;           // Nom de l'objectif
    public string description;             // Description
    public ObjectiveType type;             // Type d'objectif
    public int targetAmount;               // Montant à atteindre
    public int currentProgress;            // Progression actuelle
    public int rewardMoney;                // Récompense en argent
    public int rewardXP;                   // Récompense en XP
    public bool isCompleted;               // Objectif terminé ?
    public bool isDaily;                   // Quotidien (sinon hebdomadaire)
    
    // Constructeur
    public Objective(string name, string desc, ObjectiveType objType, int target, int moneyReward, int xpReward, bool daily = true)
    {
        objectiveName = name;
        description = desc;
        type = objType;
        targetAmount = target;
        currentProgress = 0;
        rewardMoney = moneyReward;
        rewardXP = xpReward;
        isCompleted = false;
        isDaily = daily;
    }
    
    // Ajoute de la progression
    public void AddProgress(int amount)
    {
        if (isCompleted) return;
        
        currentProgress += amount;
        
        // Vérifie si l'objectif est complété
        if (currentProgress >= targetAmount)
        {
            currentProgress = targetAmount;
            isCompleted = true;
            Debug.Log("🎯 Objectif complété : " + objectiveName);
        }
    }
    
    // Obtient le pourcentage de progression (0-100)
    public float GetProgressPercentage()
    {
        if (targetAmount <= 0) return 100f;
        return (float)currentProgress / (float)targetAmount * 100f;
    }
    
    // Réinitialise l'objectif
    public void Reset()
    {
        currentProgress = 0;
        isCompleted = false;
    }
}