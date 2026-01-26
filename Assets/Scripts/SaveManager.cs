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
        
        // === PRODUITS (stocks) ===
        for (int i = 0; i < gm.products.Count; i++)
        {
            PlayerPrefs.SetInt("Product_" + i + "_Quantity", gm.products[i].quantity);
        }
        
        // === AMÉLIORATIONS (achetées ou non) ===
        for (int i = 0; i < gm.upgrades.Count; i++)
        {
            PlayerPrefs.SetInt("Upgrade_" + i + "_Purchased", gm.upgrades[i].isPurchased ? 1 : 0);
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
        
        // === PRODUITS ===
        for (int i = 0; i < gm.products.Count; i++)
        {
            int quantity = PlayerPrefs.GetInt("Product_" + i + "_Quantity", 0);
            gm.products[i].quantity = quantity;
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
    public float autoSaveInterval = 60f; // Sauvegarde toutes les 60 secondes
    
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
        // Trouve les managers dans la scène active
        GameManager gm = FindObjectOfType<GameManager>();
        TimeManager tm = FindObjectOfType<TimeManager>();
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
    
        if (gm != null && tm != null && pm != null)
        {
            SaveGame(gm, tm, pm);
            Debug.Log("💾 Sauvegarde automatique effectuée");
        
            // Affiche l'indicateur de sauvegarde
            ShowSaveIndicator();
        }
    }

// Affiche temporairement l'indicateur de sauvegarde
    void ShowSaveIndicator()
    {
        if (saveIndicator == null)
        {
            // Cherche l'indicateur dans la scène s'il n'est pas assigné
            saveIndicator = GameObject.Find("SaveIndicator")?.GetComponent<TextMeshProUGUI>();
        }
    
        if (saveIndicator != null)
        {
            saveIndicator.gameObject.SetActive(true);
            saveIndicator.text = "Sauvegarde...";
        
            // Cache après 2 secondes
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