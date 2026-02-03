using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        // Singleton : une seule instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // Sauvegarde toutes les données
    public void SaveGame(GameManager gm, TimeManager tm, ProgressionManager pm)
    {
        if (gm == null || tm == null || pm == null)
        {
            Debug.LogError("❌ Impossible de sauvegarder : références manquantes !");
            return;
        }
        
        // === ARGENT ===
        PlayerPrefs.SetInt("PlayerMoney", gm.playerMoney);
        
        // === TEMPS ===
        PlayerPrefs.SetInt("CurrentDay", tm.currentDay);
        PlayerPrefs.SetInt("CurrentWeek", tm.currentWeek);
        
        // === PROGRESSION ===
        PlayerPrefs.SetInt("CurrentLevel", pm.currentLevel);
        PlayerPrefs.SetInt("CurrentExperience", pm.currentExperience);
        PlayerPrefs.SetInt("ExperienceToNextLevel", pm.experienceToNextLevel);
        
        // === MATÉRIAUX (stocks) ===
        for (int i = 0; i < gm.craftingMaterials.Count; i++)
        {
            PlayerPrefs.SetInt("Material_" + i + "_Quantity", gm.craftingMaterials[i].quantity);
        }
        
        // === PRODUITS (stocks + déverrouillages) ===
        for (int i = 0; i < gm.products.Count; i++)
        {
            PlayerPrefs.SetInt("Product_" + i + "_Quantity", gm.products[i].quantity);
            PlayerPrefs.SetInt("Product_" + i + "_Unlocked", gm.products[i].isUnlocked ? 1 : 0);
            
            Debug.Log("💾 Sauvegarde produit " + i + " (" + gm.products[i].productName + ") : Débloqué = " + gm.products[i].isUnlocked);
        }
        
        // === AMÉLIORATIONS (achetées ou non) ===
        for (int i = 0; i < gm.upgrades.Count; i++)
        {
            PlayerPrefs.SetInt("Upgrade_" + i + "_Purchased", gm.upgrades[i].isPurchased ? 1 : 0);
        }
        
        // ===== STATISTIQUES =====
        StatsManager sm = StatsManager.Instance;
        if (sm != null && sm.stats != null)
        {
            // Stats globales
            PlayerPrefs.SetInt("Stats_TotalMoneyEarned", sm.stats.totalMoneyEarned);
            PlayerPrefs.SetInt("Stats_TotalMoneySpent", sm.stats.totalMoneySpent);
            PlayerPrefs.SetInt("Stats_TotalProductsCrafted", sm.stats.totalProductsCrafted);
            PlayerPrefs.SetInt("Stats_TotalProductsSold", sm.stats.totalProductsSold);
            PlayerPrefs.SetInt("Stats_TotalMaterialsBought", sm.stats.totalMaterialsBought);
            PlayerPrefs.SetInt("Stats_TotalUpgradesBought", sm.stats.totalUpgradesBought);
            PlayerPrefs.SetInt("Stats_TotalDaysPlayed", sm.stats.totalDaysPlayed);
            PlayerPrefs.SetInt("Stats_TotalWeeksPlayed", sm.stats.totalWeeksPlayed);
            
            // Records
            PlayerPrefs.SetInt("Stats_BestDailyEarnings", sm.stats.bestDailyEarnings);
            PlayerPrefs.SetInt("Stats_HighestMoneyAmount", sm.stats.highestMoneyAmount);
            PlayerPrefs.SetInt("Stats_MostExpensiveProductSold", sm.stats.mostExpensiveProductSold);
            
            Debug.Log("💾 Statistiques sauvegardées");
        }
        
        // ===== EMPLOYÉS =====
        EmployeeManager em = EmployeeManager.Instance;
        if (em != null && em.employees != null)
        {
            for (int i = 0; i < em.employees.Count; i++)
            {
                Employee emp = em.employees[i];
                PlayerPrefs.SetInt("Employee_" + i + "_IsHired", emp.isHired ? 1 : 0);
                PlayerPrefs.SetInt("Employee_" + i + "_IsActive", emp.isActive ? 1 : 0);
                PlayerPrefs.SetInt("Employee_" + i + "_Level", emp.level);
            }
    
            Debug.Log("💾 Employés sauvegardés");
        }
        
        // ===== COMMANDES =====
        OrderManager om = OrderManager.Instance;
        if (om != null && om.activeOrders != null)
        {
            // Sauvegarde le nombre de commandes
            PlayerPrefs.SetInt("ActiveOrders_Count", om.activeOrders.Count);
            
            // Sauvegarde chaque commande
            for (int i = 0; i < om.activeOrders.Count; i++)
            {
                Order order = om.activeOrders[i];
                
                PlayerPrefs.SetString("Order_" + i + "_ID", order.orderID);
                PlayerPrefs.SetString("Order_" + i + "_Client", order.clientName);
                PlayerPrefs.SetInt("Order_" + i + "_Reward", order.reward);
                PlayerPrefs.SetFloat("Order_" + i + "_TimeLimit", order.timeLimit);
                PlayerPrefs.SetFloat("Order_" + i + "_TimeRemaining", order.timeRemaining);
                PlayerPrefs.SetInt("Order_" + i + "_Completed", order.isCompleted ? 1 : 0);
                PlayerPrefs.SetInt("Order_" + i + "_Failed", order.isFailed ? 1 : 0);
                
                // Sauvegarde les produits demandés
                PlayerPrefs.SetInt("Order_" + i + "_Requirements_Count", order.requirements.Count);
                for (int j = 0; j < order.requirements.Count; j++)
                {
                    OrderRequirement req = order.requirements[j];
                    PlayerPrefs.SetInt("Order_" + i + "_Req_" + j + "_ProductIndex", req.productIndex);
                    PlayerPrefs.SetInt("Order_" + i + "_Req_" + j + "_Quantity", req.quantity);
                }
            }
            
            // Sauvegarde le compteur de commandes
            var counterField = om.GetType().GetField("orderCounter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (counterField != null)
            {
                int orderCounter = (int)counterField.GetValue(om);
                PlayerPrefs.SetInt("OrderCounter", orderCounter);
            }
            
            Debug.Log("💾 Commandes sauvegardées");
        }
        
        // === DATE DE SAUVEGARDE ===
        PlayerPrefs.SetString("LastSaveDate", System.DateTime.Now.ToString());
        
        PlayerPrefs.Save();
        Debug.Log("💾 Jeu sauvegardé avec succès !");
    }
    
    // Charge toutes les données
    public void LoadGame(GameManager gm, TimeManager tm, ProgressionManager pm)
    {
        if (!HasSaveData())
        {
            Debug.Log("ℹ️ Aucune sauvegarde trouvée - Nouvelle partie");
            return;
        }
        
        if (gm == null || tm == null || pm == null)
        {
            Debug.LogError("❌ Impossible de charger : références manquantes !");
            return;
        }
        
        Debug.Log("📂 Chargement de la sauvegarde...");
        
        // === ARGENT ===
        gm.playerMoney = PlayerPrefs.GetInt("PlayerMoney", 1000);
        gm.UpdateMoneyDisplay();
        
        // === TEMPS ===
        tm.currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        tm.currentWeek = PlayerPrefs.GetInt("CurrentWeek", 1);
        
        // === PROGRESSION ===
        pm.currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        pm.currentExperience = PlayerPrefs.GetInt("CurrentExperience", 0);
        pm.experienceToNextLevel = PlayerPrefs.GetInt("ExperienceToNextLevel", 100);
        
        // === MATÉRIAUX ===
        for (int i = 0; i < gm.craftingMaterials.Count; i++)
        {
            int quantity = PlayerPrefs.GetInt("Material_" + i + "_Quantity", 0);
            gm.craftingMaterials[i].quantity = quantity;
        }
        
        // === PRODUITS (stocks + déverrouillages) ===
        for (int i = 0; i < gm.products.Count; i++)
        {
            int quantity = PlayerPrefs.GetInt("Product_" + i + "_Quantity", 0);
            gm.products[i].quantity = quantity;
            
            // Charge le statut de déverrouillage
            bool isUnlocked = PlayerPrefs.GetInt("Product_" + i + "_Unlocked", 0) == 1;
            
            Debug.Log("📂 Chargement produit " + i + " (" + gm.products[i].productName + ") : Débloqué en save = " + isUnlocked);
            
            // Si le produit était débloqué en sauvegarde, on le débloque
            if (isUnlocked)
            {
                gm.products[i].isUnlocked = true;
                Debug.Log("✅ Produit " + gm.products[i].productName + " débloqué depuis la sauvegarde");
            }
        }
        
        // === AMÉLIORATIONS ===
        for (int i = 0; i < gm.upgrades.Count; i++)
        {
            bool isPurchased = PlayerPrefs.GetInt("Upgrade_" + i + "_Purchased", 0) == 1;
            gm.upgrades[i].isPurchased = isPurchased;
    
            // Si l'amélioration était achetée, réapplique ses effets
            if (isPurchased)
            {
                gm.ReapplyUpgradeEffect(gm.upgrades[i]);
                Debug.Log("🔧 Réapplication de l'amélioration : " + gm.upgrades[i].upgradeName);
            }
        }
        
        // ===== STATISTIQUES =====
        StatsManager sm = StatsManager.Instance;
        if (sm != null && sm.stats != null)
        {
            // Stats globales
            sm.stats.totalMoneyEarned = PlayerPrefs.GetInt("Stats_TotalMoneyEarned", 0);
            sm.stats.totalMoneySpent = PlayerPrefs.GetInt("Stats_TotalMoneySpent", 0);
            sm.stats.totalProductsCrafted = PlayerPrefs.GetInt("Stats_TotalProductsCrafted", 0);
            sm.stats.totalProductsSold = PlayerPrefs.GetInt("Stats_TotalProductsSold", 0);
            sm.stats.totalMaterialsBought = PlayerPrefs.GetInt("Stats_TotalMaterialsBought", 0);
            sm.stats.totalUpgradesBought = PlayerPrefs.GetInt("Stats_TotalUpgradesBought", 0);
            sm.stats.totalDaysPlayed = PlayerPrefs.GetInt("Stats_TotalDaysPlayed", 0);
            sm.stats.totalWeeksPlayed = PlayerPrefs.GetInt("Stats_TotalWeeksPlayed", 0);
            
            // Records
            sm.stats.bestDailyEarnings = PlayerPrefs.GetInt("Stats_BestDailyEarnings", 0);
            sm.stats.highestMoneyAmount = PlayerPrefs.GetInt("Stats_HighestMoneyAmount", 0);
            sm.stats.mostExpensiveProductSold = PlayerPrefs.GetInt("Stats_MostExpensiveProductSold", 0);
            
            Debug.Log("📊 Statistiques chargées");
        }

        // ===== EMPLOYÉS =====
        EmployeeManager em = EmployeeManager.Instance;
        if (em != null && em.employees != null)
        {
            for (int i = 0; i < em.employees.Count; i++)
            {
                bool isHired = PlayerPrefs.GetInt("Employee_" + i + "_IsHired", 0) == 1;
                bool isActive = PlayerPrefs.GetInt("Employee_" + i + "_IsActive", 0) == 1;
                int level = PlayerPrefs.GetInt("Employee_" + i + "_Level", 1);
        
                if (isHired)
                {
                    em.employees[i].isHired = true;
                    em.employees[i].isActive = isActive;
                    em.employees[i].level = level;
                }
            }
    
            Debug.Log("📊 Employés chargés");
        }
        
        // ===== COMMANDES =====
        OrderManager om = OrderManager.Instance;
        if (om != null)
        {
            // Charge le compteur de commandes
            int orderCounter = PlayerPrefs.GetInt("OrderCounter", 0);
            var counterField = om.GetType().GetField("orderCounter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (counterField != null)
            {
                counterField.SetValue(om, orderCounter);
            }
            
            // Charge le nombre de commandes
            int ordersCount = PlayerPrefs.GetInt("ActiveOrders_Count", 0);
            
            if (ordersCount > 0)
            {
                om.activeOrders.Clear();
                
                // Charge chaque commande
                for (int i = 0; i < ordersCount; i++)
                {
                    string orderID = PlayerPrefs.GetString("Order_" + i + "_ID", "");
                    string clientName = PlayerPrefs.GetString("Order_" + i + "_Client", "");
                    int reward = PlayerPrefs.GetInt("Order_" + i + "_Reward", 0);
                    float timeLimit = PlayerPrefs.GetFloat("Order_" + i + "_TimeLimit", 0f);
                    float timeRemaining = PlayerPrefs.GetFloat("Order_" + i + "_TimeRemaining", 0f);
                    bool isCompleted = PlayerPrefs.GetInt("Order_" + i + "_Completed", 0) == 1;
                    bool isFailed = PlayerPrefs.GetInt("Order_" + i + "_Failed", 0) == 1;
                    
                    // Crée la commande
                    Order order = new Order(orderID, clientName, reward, timeLimit);
                    order.timeRemaining = timeRemaining;
                    order.isCompleted = isCompleted;
                    order.isFailed = isFailed;
                    
                    // Charge les produits demandés
                    int reqCount = PlayerPrefs.GetInt("Order_" + i + "_Requirements_Count", 0);
                    for (int j = 0; j < reqCount; j++)
                    {
                        int productIndex = PlayerPrefs.GetInt("Order_" + i + "_Req_" + j + "_ProductIndex", 0);
                        int quantity = PlayerPrefs.GetInt("Order_" + i + "_Req_" + j + "_Quantity", 0);
                        order.AddRequirement(productIndex, quantity);
                    }
                    
                    // Ajoute la commande uniquement si elle n'est pas terminée ou échouée
                    if (!isCompleted && !isFailed)
                    {
                        om.activeOrders.Add(order);
                    }
                }
                
                Debug.Log("📊 Commandes chargées : " + om.activeOrders.Count + " actives");
            }
        }

        // Met à jour toute l'interface
        gm.RefreshAllUI();
        
        string lastSave = PlayerPrefs.GetString("LastSaveDate", "Inconnue");
        Debug.Log("✅ Sauvegarde chargée ! Dernière sauvegarde : " + lastSave);
    }
    
    // Vérifie si une sauvegarde existe
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey("PlayerMoney");
    }
    
    // Supprime toute la sauvegarde (Nouvelle Partie)
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🗑️ Sauvegarde supprimée - Nouvelle partie");
    }
    
    // Sauvegarde automatique toutes les X secondes
    private float autoSaveTimer = 0f;
    public float autoSaveInterval = 60f;
    
    void Update()
    {
        autoSaveTimer += Time.deltaTime;
        
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;
            AutoSave();
        }
    }
    
    void AutoSave()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        TimeManager tm = FindObjectOfType<TimeManager>();
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
    
        if (gm != null && tm != null && pm != null)
        {
            SaveGame(gm, tm, pm);
            Debug.Log("💾 Sauvegarde automatique effectuée");
        
            ShowSaveIndicator();
        }
    }

    void ShowSaveIndicator()
    {
        if (saveIndicator == null)
        {
            saveIndicator = GameObject.Find("SaveIndicator")?.GetComponent<TextMeshProUGUI>();
        }
    
        if (saveIndicator != null)
        {
            saveIndicator.gameObject.SetActive(true);
            saveIndicator.text = "Sauvegarde...";
        
            CancelInvoke("HideSaveIndicator");
            Invoke("HideSaveIndicator", 2f);
        }
    }

    void HideSaveIndicator()
    {
        if (saveIndicator != null)
        {
            saveIndicator.gameObject.SetActive(false);
        }
    }
    
    public TextMeshProUGUI saveIndicator;
}
