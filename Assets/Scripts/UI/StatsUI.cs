using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StatsUI : MonoBehaviour
{
    [Header("Containers")]
    public Transform globalStatsContainer;
    public Transform recordsContainer;
    
    [Header("Prefabs")]
    public GameObject statLineItemPrefab;
    
    [Header("References")]
    private StatsManager statsManager;
    
    // Listes des items UI créés
    private List<GameObject> globalStatsItems = new List<GameObject>();
    private List<GameObject> recordsItems = new List<GameObject>();
    
    void Start()
    {
        statsManager = StatsManager.Instance;
        
        // Génère l'UI des stats au démarrage
        RefreshStatsDisplay();
    }
    
    // Rafraîchit l'affichage complet des stats
    public void RefreshStatsDisplay()
    {
        if (statsManager == null) return;
        
        // Nettoie les anciennes stats
        ClearStatsDisplay();
        
        // Affiche les stats globales
        DisplayGlobalStats();
        
        // Affiche les records
        DisplayRecords();

        DisplayHistory();
    }
    
    // Nettoie l'affichage
    void ClearStatsDisplay()
    {
        foreach (GameObject item in globalStatsItems)
        {
            if (item != null) Destroy(item);
        }
        globalStatsItems.Clear();
        
        foreach (GameObject item in recordsItems)
        {
            if (item != null) Destroy(item);
        }
        recordsItems.Clear();
    }
    
    // Affiche les statistiques globales
    void DisplayGlobalStats()
    {
        if (globalStatsContainer == null || statLineItemPrefab == null) return;
        
        StatsData stats = statsManager.stats;
        
        // Argent total gagné
        CreateStatLine(globalStatsContainer, "Argent total gagné :", stats.totalMoneyEarned + "€", 
           new Color(0.1f, 0.6f, 0.1f), globalStatsItems);
        
        // Argent total dépensé
        CreateStatLine(globalStatsContainer, "Argent total dépensé :", stats.totalMoneySpent + "€", 
            new Color(0.8f, 0.3f, 0.3f), globalStatsItems);
        
        // Bénéfice net
        int netProfit = stats.totalMoneyEarned - stats.totalMoneySpent;
        CreateStatLine(globalStatsContainer, "Bénéfice net :", netProfit + "€", 
            netProfit >= 0 ? new Color(0.1f, 0.6f, 0.1f) : new Color(0.8f, 0.3f, 0.3f), globalStatsItems);
        
        // Produits fabriqués
        CreateStatLine(globalStatsContainer, "Produits fabriqués :", stats.totalProductsCrafted.ToString(), 
            new Color(0.3f, 0.5f, 0.9f), globalStatsItems);
        
        // Produits vendus
        CreateStatLine(globalStatsContainer, "Produits vendus :", stats.totalProductsSold.ToString(), 
            new Color(0.3f, 0.5f, 0.9f), globalStatsItems);
        
        // Matériaux achetés
        CreateStatLine(globalStatsContainer, "Matériaux achetés :", stats.totalMaterialsBought.ToString(), 
            new Color(0.6f, 0.4f, 0.2f), globalStatsItems);
        
        // Améliorations achetées
        CreateStatLine(globalStatsContainer, "Améliorations achetées :", stats.totalUpgradesBought.ToString(), 
            new Color(0.7f, 0.4f, 0.9f), globalStatsItems);
        
        // Jours joués
        CreateStatLine(globalStatsContainer, "Jours joués :", stats.totalDaysPlayed.ToString(), 
            new Color(0.5f, 0.5f, 0.5f), globalStatsItems);
        
        // Semaines jouées
        CreateStatLine(globalStatsContainer, "Semaines jouées :", stats.totalWeeksPlayed.ToString(), 
            new Color(0.5f, 0.5f, 0.5f), globalStatsItems);
    }
    
    // Affiche les records
    void DisplayRecords()
    {
        if (recordsContainer == null || statLineItemPrefab == null) return;
        
        StatsData stats = statsManager.stats;
        
        // Meilleur gain quotidien
        CreateStatLine(recordsContainer, "Meilleur gain quotidien :", stats.bestDailyEarnings + "€", 
            new Color(0.85f, 0.65f, 0.05f), recordsItems);
        
        // Plus grosse fortune
        CreateStatLine(recordsContainer, "Plus grosse fortune :", stats.highestMoneyAmount + "€", 
            new Color(0.85f, 0.65f, 0.05f), recordsItems);
        
        // Produit le plus cher vendu
        CreateStatLine(recordsContainer, "Produit le plus cher vendu :", stats.mostExpensiveProductSold + "€", 
            new Color(0.85f, 0.65f, 0.05f), recordsItems);
    }
    
    // Crée une ligne de stat
    void CreateStatLine(Transform container, string label, string value, Color valueColor, List<GameObject> list)
    {
        GameObject item = Instantiate(statLineItemPrefab, container);
        
        // Trouve les textes
        TextMeshProUGUI labelText = item.transform.Find("LabelText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI valueText = item.transform.Find("ValueText")?.GetComponent<TextMeshProUGUI>();
        
        // Configure les textes
        if (labelText != null)
        {
            labelText.text = label;
        }
        
        if (valueText != null)
        {
            valueText.text = value;
            valueText.color = valueColor;
        }
        
        // Ajoute à la liste
        list.Add(item);
    }
    
    // Dans StatsUI.cs, ajoute cette fonction :

    public void DisplayHistory()
    {
        if (statsManager == null) return;
    
        Debug.Log("=== HISTORIQUE DES 7 DERNIERS JOURS ===");
    
        foreach (DayStats day in statsManager.stats.dailyHistory)
        {
            Debug.Log($"Jour {day.dayNumber} (Semaine {day.weekNumber}) : " +
                      $"+{day.moneyEarned}€, -{day.moneySpent}€, " +
                      $"{day.productsCrafted} fabriqués, {day.productsSold} vendus");
        }
    }
}