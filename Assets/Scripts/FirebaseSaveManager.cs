using UnityEngine;
using System;
using System.Collections.Generic;

public class FirebaseSaveManager : MonoBehaviour
{
    // === SINGLETON ===
    public static FirebaseSaveManager Instance { get; private set; }
    
    // === ID DU JOUEUR ===
    // Chaque joueur a un ID unique pour retrouver sa sauvegarde
    private string playerId;
    
    // === ÉTAT ===
    public bool IsSaving { get; private set; } = false;
    public bool IsLoading { get; private set; } = false;
    
    // === ÉVÉNEMENTS ===
    public event Action OnSaveComplete;
    public event Action OnLoadComplete;
    public event Action<string> OnError;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Génère ou récupère l'ID du joueur
        InitializePlayerId();
    }
    
    /// Crée un ID unique pour ce joueur (stocké localement)
    void InitializePlayerId()
    {
        // Vérifie si on a déjà un ID
        playerId = PlayerPrefs.GetString("FirebasePlayerId", "");
        
        if (string.IsNullOrEmpty(playerId))
        {
            // Crée un nouvel ID unique
            playerId = "joueur_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            PlayerPrefs.SetString("FirebasePlayerId", playerId);
            PlayerPrefs.Save();
            
            Debug.Log("🆔 Nouvel ID joueur créé: " + playerId);
        }
        else
        {
            Debug.Log("🆔 ID joueur existant: " + playerId);
        }
    }
    
    /// Retourne l'ID du joueur actuel
    public string GetPlayerId()
    {
        return playerId;
    }
    
    // ========================================
    //     STRUCTURE DES DONNÉES DE SAUVEGARDE
    // ========================================
    
    
    /// Toutes les données du joueur en un seul objet
    [Serializable]
    public class PlayerSaveData
    {
        // Infos générales
        public string playerId;
        public string lastSaveDate;
        
        // Progression
        public int money;
        public int level;
        public int experience;
        public int experienceToNextLevel;
        
        // Temps de jeu
        public int currentDay;
        public int currentWeek;
        
        // Inventaire (listes simples)
        public List<int> materialQuantities = new List<int>();
        public List<int> productQuantities = new List<int>();
        public List<bool> productUnlocked = new List<bool>();
        public List<bool> upgradesPurchased = new List<bool>();
        
        // Stats
        public int totalMoneyEarned;
        public int totalProductsCrafted;
        public int totalProductsSold;
    }
    
    // ========================================
    //          SAUVEGARDER DANS FIREBASE
    // ========================================
    
    /// Sauvegarde toutes les données du jeu dans Firebase
    public void SaveToFirebase()
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady())
        {
            Debug.LogWarning("Firebase pas prêt, sauvegarde locale uniquement");
            return;
        }
        
        if (IsSaving)
        {
            Debug.Log("Sauvegarde déjà en cours...");
            return;
        }
        
        IsSaving = true;
        
        // Collecte toutes les données
        PlayerSaveData saveData = CollectSaveData();
        
        // Convertit en JSON
        string json = JsonUtility.ToJson(saveData, true);
        
        // Envoie à Firebase
        string path = "sauvegardes/" + playerId;
        
        FirebaseManager.Instance.SaveData(path, json, (success) => 
        {
            IsSaving = false;
            
            if (success)
            {
                Debug.Log("Sauvegarde Firebase réussie !");
                OnSaveComplete?.Invoke();
            }
            else
            {
                Debug.LogError("Échec sauvegarde Firebase");
                OnError?.Invoke("Échec de la sauvegarde");
            }
        });
    }
    
    /// Collecte toutes les données du jeu
    PlayerSaveData CollectSaveData()
    {
        PlayerSaveData data = new PlayerSaveData();
        
        data.playerId = playerId;
        data.lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            data.money = gm.playerMoney;
            
            // Matériaux
            foreach (var mat in gm.craftingMaterials)
            {
                data.materialQuantities.Add(mat.quantity);
            }
            
            // Produits
            foreach (var prod in gm.products)
            {
                data.productQuantities.Add(prod.quantity);
                data.productUnlocked.Add(prod.isUnlocked);
            }
            
            // Améliorations
            foreach (var upg in gm.upgrades)
            {
                data.upgradesPurchased.Add(upg.isPurchased);
            }
        }
        
        // ProgressionManager
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
        if (pm != null)
        {
            data.level = pm.currentLevel;
            data.experience = pm.currentExperience;
            data.experienceToNextLevel = pm.experienceToNextLevel;
        }
        
        // TimeManager
        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null)
        {
            data.currentDay = tm.currentDay;
            data.currentWeek = tm.currentWeek;
        }
        
        // StatsManager
        if (StatsManager.Instance != null && StatsManager.Instance.stats != null)
        {
            data.totalMoneyEarned = StatsManager.Instance.stats.totalMoneyEarned;
            data.totalProductsCrafted = StatsManager.Instance.stats.totalProductsCrafted;
            data.totalProductsSold = StatsManager.Instance.stats.totalProductsSold;
        }
        
        return data;
    }
    
    // ========================================
    //          CHARGER DEPUIS FIREBASE
    // ========================================
    
    /// Charge les données depuis Firebase
    public void LoadFromFirebase(Action<bool> callback = null)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady())
        {
            Debug.LogWarning("Firebase pas prêt");
            callback?.Invoke(false);
            return;
        }
        
        if (IsLoading)
        {
            Debug.Log("Chargement déjà en cours...");
            return;
        }
        
        IsLoading = true;
        
        string path = "sauvegardes/" + playerId;
        
        FirebaseManager.Instance.LoadData(path, (json) =>
        {
            IsLoading = false;
            
            if (string.IsNullOrEmpty(json) || json == "null")
            {
                Debug.Log("Aucune sauvegarde Firebase trouvée");
                callback?.Invoke(false);
                return;
            }
            
            try
            {
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
                ApplySaveData(data);
                
                Debug.Log("Chargement Firebase réussi !");
                OnLoadComplete?.Invoke();
                callback?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogError("Erreur parsing JSON: " + e.Message);
                OnError?.Invoke("Erreur de chargement");
                callback?.Invoke(false);
            }
        });
    }
    
    /// Applique les données chargées au jeu
    void ApplySaveData(PlayerSaveData data)
    {
        // GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.playerMoney = data.money;
            gm.UpdateMoneyDisplay();
            
            // Matériaux
            for (int i = 0; i < data.materialQuantities.Count && i < gm.craftingMaterials.Count; i++)
            {
                gm.craftingMaterials[i].quantity = data.materialQuantities[i];
            }
            
            // Produits
            for (int i = 0; i < data.productQuantities.Count && i < gm.products.Count; i++)
            {
                gm.products[i].quantity = data.productQuantities[i];
                if (i < data.productUnlocked.Count)
                {
                    gm.products[i].isUnlocked = data.productUnlocked[i];
                }
            }
            
            // Améliorations
            for (int i = 0; i < data.upgradesPurchased.Count && i < gm.upgrades.Count; i++)
            {
                gm.upgrades[i].isPurchased = data.upgradesPurchased[i];
            }
        }
        
        // ProgressionManager
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
        if (pm != null)
        {
            pm.currentLevel = data.level;
            pm.currentExperience = data.experience;
            pm.experienceToNextLevel = data.experienceToNextLevel;
        }
        
        // TimeManager
        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null)
        {
            tm.currentDay = data.currentDay;
            tm.currentWeek = data.currentWeek;
        }
    }
    
    // ========================================
    //          MÉTHODES UTILITAIRES
    // ========================================
    /// sauvegarde automatique
    public void AutoSave()
    {
        // Sauvegarde locale d'abord 
        SaveManager localSave = SaveManager.Instance;
        if (localSave != null)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            TimeManager tm = FindObjectOfType<TimeManager>();
            ProgressionManager pm = FindObjectOfType<ProgressionManager>();
            
            if (gm != null && tm != null && pm != null)
            {
                localSave.SaveGame(gm, tm, pm);
            }
        }
        
        // Puis sauvegarde Firebase 
        SaveToFirebase();
    }
    
    /// Vérifie si une sauvegarde Firebase existe pour ce joueur
    public void CheckCloudSaveExists(Action<bool> callback)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady())
        {
            callback?.Invoke(false);
            return;
        }
        
        string path = "sauvegardes/" + playerId;
        
        FirebaseManager.Instance.LoadData(path, (json) =>
        {
            bool exists = !string.IsNullOrEmpty(json) && json != "null";
            callback?.Invoke(exists);
        });
    }
}

