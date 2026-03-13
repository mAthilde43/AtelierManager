using System;
using System.Collections.Generic;

[System.Serializable]
public class StatsData
{
    // === STATISTIQUES GLOBALES ===
    public int totalMoneyEarned;           // Argent total gagné
    public int totalMoneySpent;            // Argent total dépensé
    public int totalProductsCrafted;       // Produits fabriqués
    public int totalProductsSold;          // Produits vendus
    public int totalMaterialsBought;       // Matériaux achetés
    public int totalUpgradesBought;        // Améliorations achetées
    public int totalDaysPlayed;            // Jours joués
    public int totalWeeksPlayed;           // Semaines jouées
    
    // === RECORDS ===
    public int bestDailyEarnings;          // Meilleur gain quotidien
    public int highestMoneyAmount;         // Plus grosse fortune
    public int mostExpensiveProductSold;   // Produit le plus cher vendu
    
    // === HISTORIQUE QUOTIDIEN (7 derniers jours) ===
    public List<DayStats> dailyHistory = new List<DayStats>();
    
    // Constructeur
    public StatsData()
    {
        totalMoneyEarned = 0;
        totalMoneySpent = 0;
        totalProductsCrafted = 0;
        totalProductsSold = 0;
        totalMaterialsBought = 0;
        totalUpgradesBought = 0;
        totalDaysPlayed = 0;
        totalWeeksPlayed = 0;
        bestDailyEarnings = 0;
        highestMoneyAmount = 0;
        mostExpensiveProductSold = 0;
        dailyHistory = new List<DayStats>();
    }
    
    // Ajoute un jour à l'historique
    public void AddDayToHistory(DayStats dayStats)
    {
        dailyHistory.Add(dayStats);
        
        // Garde seulement les 7 derniers jours
        if (dailyHistory.Count > 7)
        {
            dailyHistory.RemoveAt(0);
        }
    }
}

// Statistiques d'un jour spécifique
[System.Serializable]
public class DayStats
{
    public int dayNumber;              // Numéro du jour
    public int weekNumber;             // Numéro de la semaine
    public int moneyEarned;            // Argent gagné ce jour
    public int moneySpent;             // Argent dépensé ce jour
    public int productsCrafted;        // Produits fabriqués ce jour
    public int productsSold;           // Produits vendus ce jour
    public DateTime timestamp;         // Date réelle
    
    public DayStats(int day, int week)
    {
        dayNumber = day;
        weekNumber = week;
        moneyEarned = 0;
        moneySpent = 0;
        productsCrafted = 0;
        productsSold = 0;
        timestamp = DateTime.Now;
    }
}