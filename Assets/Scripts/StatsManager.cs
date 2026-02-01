using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // Singleton
    private static StatsManager instance;
    public static StatsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StatsManager>();
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
        
        // ===== AJOUTE CETTE LIGNE =====
        DontDestroyOnLoad(gameObject);
        // ==============================
    }
    
    // === DONNÉES ===
    public StatsData stats = new StatsData();
    private DayStats currentDayStats;
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private TimeManager timeManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        timeManager = FindObjectOfType<TimeManager>();
        
        // Commence un nouveau jour
        StartNewDay();
        
        Debug.Log("📊 StatsManager initialisé");
        Debug.Log($"📊 Stats actuelles: Argent gagné = {stats.totalMoneyEarned}€, Produits fabriqués = {stats.totalProductsCrafted}");
    }
    
    // === TRACKING DES ACTIONS ===
    
    public void OnMoneyEarned(int amount)
    {
        stats.totalMoneyEarned += amount;
        
        if (currentDayStats != null)
        {
            currentDayStats.moneyEarned += amount;
        }
        
        // Record du jour
        if (currentDayStats != null && currentDayStats.moneyEarned > stats.bestDailyEarnings)
        {
            stats.bestDailyEarnings = currentDayStats.moneyEarned;
            Debug.Log("🏆 Nouveau record de gains quotidiens : " + stats.bestDailyEarnings + "€");
        }
        
        // Record de fortune
        if (gameManager != null && gameManager.playerMoney > stats.highestMoneyAmount)
        {
            stats.highestMoneyAmount = gameManager.playerMoney;
            Debug.Log("🏆 Nouvelle plus grosse fortune : " + stats.highestMoneyAmount + "€");
        }
    }
    
    public void OnMoneySpent(int amount)
    {
        stats.totalMoneySpent += amount;
        
        if (currentDayStats != null)
        {
            currentDayStats.moneySpent += amount;
        }
    }
    
    public void OnProductCrafted()
    {
        stats.totalProductsCrafted++;
        
        if (currentDayStats != null)
        {
            currentDayStats.productsCrafted++;
        }
        
        Debug.Log($"📊 Produit fabriqué ! Total: {stats.totalProductsCrafted}");
    }
    
    public void OnProductSold(int price)
    {
        stats.totalProductsSold++;
        
        if (currentDayStats != null)
        {
            currentDayStats.productsSold++;
        }
        
        // Record du produit le plus cher
        if (price > stats.mostExpensiveProductSold)
        {
            stats.mostExpensiveProductSold = price;
            Debug.Log("🏆 Nouveau produit le plus cher vendu : " + price + "€");
        }
        
        Debug.Log($"📊 Produit vendu ! Total: {stats.totalProductsSold}");
    }
    
    public void OnMaterialBought()
    {
        stats.totalMaterialsBought++;
        Debug.Log($"📊 Matériau acheté ! Total: {stats.totalMaterialsBought}");
    }
    
    public void OnUpgradeBought()
    {
        stats.totalUpgradesBought++;
        Debug.Log($"📊 Amélioration achetée ! Total: {stats.totalUpgradesBought}");
    }
    
    // === GESTION DES JOURS ===
    
    public void StartNewDay()
    {
        if (timeManager == null)
        {
            timeManager = FindObjectOfType<TimeManager>();
        }
        
        if (timeManager == null) return;
        
        // Sauvegarde le jour précédent dans l'historique
        if (currentDayStats != null)
        {
            stats.AddDayToHistory(currentDayStats);
        }
        
        // Commence un nouveau jour
        currentDayStats = new DayStats(timeManager.currentDay, timeManager.currentWeek);
        stats.totalDaysPlayed++;
        
        Debug.Log("📅 Nouveau jour commencé - Stats réinitialisées");
    }
    
    public void OnNewWeek()
    {
        stats.totalWeeksPlayed++;
        Debug.Log("📆 Nouvelle semaine - Total : " + stats.totalWeeksPlayed + " semaines");
    }
    
    // === AFFICHAGE DES STATS ===
    
    public void DisplayStats()
    {
        Debug.Log("=== STATISTIQUES GLOBALES ===");
        Debug.Log("💰 Argent gagné : " + stats.totalMoneyEarned + "€");
        Debug.Log("💸 Argent dépensé : " + stats.totalMoneySpent + "€");
        Debug.Log("🛠️ Produits fabriqués : " + stats.totalProductsCrafted);
        Debug.Log("💼 Produits vendus : " + stats.totalProductsSold);
        Debug.Log("📦 Matériaux achetés : " + stats.totalMaterialsBought);
        Debug.Log("⬆️ Améliorations achetées : " + stats.totalUpgradesBought);
        Debug.Log("📅 Jours joués : " + stats.totalDaysPlayed);
        Debug.Log("📆 Semaines jouées : " + stats.totalWeeksPlayed);
        Debug.Log("");
        Debug.Log("=== RECORDS ===");
        Debug.Log("🏆 Meilleur gain quotidien : " + stats.bestDailyEarnings + "€");
        Debug.Log("🏆 Plus grosse fortune : " + stats.highestMoneyAmount + "€");
        Debug.Log("🏆 Produit le plus cher vendu : " + stats.mostExpensiveProductSold + "€");
    }
}