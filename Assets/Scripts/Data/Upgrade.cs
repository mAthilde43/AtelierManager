using UnityEngine;

// Classe pour représenter une amélioration achetable
[System.Serializable]
public class Upgrade
{
    public string upgradeName;           // Nom de l'amélioration
    public string description;           // Description
    public int cost;                     // Prix d'achat
    public bool isPurchased;             // Déjà acheté ?
    public UpgradeType type;             // Type d'amélioration
    public int value;                    // Valeur de l'effet (ex: -10% coût, +20% vente)
    
    // Constructeur
    public Upgrade(string name, string desc, int price, UpgradeType upgradeType, int effectValue)
    {
        upgradeName = name;
        description = desc;
        cost = price;
        isPurchased = false;
        type = upgradeType;
        value = effectValue;
    }
    
    // Fonction pour acheter l'amélioration
    public void Purchase()
    {
        if (!isPurchased)
        {
            isPurchased = true;
            Debug.Log("Amélioration achetée : " + upgradeName);
        }
    }
}

// Types d'améliorations possibles
public enum UpgradeType
{
    MaterialDiscount,        // Réduction sur les matériaux
    ProductionSpeed,         // Vitesse de production augmentée
    SalesBonus,             // Bonus sur les ventes
    DailyIncomeBoost,       // Augmentation revenus quotidiens
    WeeklyCostReduction     // Réduction charges hebdomadaires
}