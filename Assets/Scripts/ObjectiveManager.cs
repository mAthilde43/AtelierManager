using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    // Singleton
    private static ObjectiveManager instance;
    public static ObjectiveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectiveManager>();
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
    }
    
    // === LISTES D'OBJECTIFS ===
    public List<Objective> dailyObjectives = new List<Objective>();
    public List<Objective> weeklyObjectives = new List<Objective>();
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private TimeManager timeManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        timeManager = FindObjectOfType<TimeManager>();
    
        // Génère les objectifs initiaux
        GenerateDailyObjectives();
        GenerateWeeklyObjectives();
    
        // Crée l'UI
        CreateObjectivesUI();
    
        Debug.Log("🎯 ObjectiveManager initialisé");
    }

// Crée l'UI des objectifs
    void CreateObjectivesUI()
    {
        if (objectiveItemPrefab == null) return;
    
        // Nettoie les anciennes UIs
        foreach (ObjectiveItemUI ui in dailyObjectiveUIs)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        dailyObjectiveUIs.Clear();
    
        foreach (ObjectiveItemUI ui in weeklyObjectiveUIs)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        weeklyObjectiveUIs.Clear();
    
        // Crée les UIs quotidiennes
        if (dailyObjectivesContainer != null)
        {
            foreach (Objective obj in dailyObjectives)
            {
                GameObject itemObj = Instantiate(objectiveItemPrefab, dailyObjectivesContainer);
                ObjectiveItemUI itemUI = itemObj.GetComponent<ObjectiveItemUI>();
            
                if (itemUI != null)
                {
                    itemUI.DisplayObjective(obj);
                    dailyObjectiveUIs.Add(itemUI);
                }
            }
        }
    
        // Crée les UIs hebdomadaires
        if (weeklyObjectivesContainer != null)
        {
            foreach (Objective obj in weeklyObjectives)
            {
                GameObject itemObj = Instantiate(objectiveItemPrefab, weeklyObjectivesContainer);
                ObjectiveItemUI itemUI = itemObj.GetComponent<ObjectiveItemUI>();
            
                if (itemUI != null)
                {
                    itemUI.DisplayObjective(obj);
                    weeklyObjectiveUIs.Add(itemUI);
                }
            }
        }
    }
    
    // === GÉNÉRATION DES OBJECTIFS QUOTIDIENS ===
    public void GenerateDailyObjectives()
    {
        dailyObjectives.Clear();
        
        // 3 objectifs quotidiens aléatoires
        dailyObjectives.Add(new Objective(
            "Gain quotidien",
            "Gagne 500€ aujourd'hui",
            ObjectiveType.EarnMoney,
            500,
            100,  // Récompense : 100€
            50,   // Récompense : 50 XP
            true
        ));
        
        dailyObjectives.Add(new Objective(
            "Production du jour",
            "Fabrique 3 produits",
            ObjectiveType.CraftProducts,
            3,
            150,
            75,
            true
        ));
        
        dailyObjectives.Add(new Objective(
            "Commerçant actif",
            "Vends 2 produits",
            ObjectiveType.SellProducts,
            2,
            200,
            100,
            true
        ));
        
        Debug.Log("📋 3 objectifs quotidiens générés");
    }
    
    // === GÉNÉRATION DES OBJECTIFS HEBDOMADAIRES ===
    public void GenerateWeeklyObjectives()
    {
        weeklyObjectives.Clear();
        
        // 2 objectifs hebdomadaires plus ambitieux
        weeklyObjectives.Add(new Objective(
            "Fortune hebdomadaire",
            "Gagne 3000€ cette semaine",
            ObjectiveType.EarnMoney,
            3000,
            500,  // Récompense : 500€
            250,  // Récompense : 250 XP
            false
        ));
        
        weeklyObjectives.Add(new Objective(
            "Artisan prolifique",
            "Fabrique 15 produits cette semaine",
            ObjectiveType.CraftProducts,
            15,
            400,
            200,
            false
        ));
        
        Debug.Log("📋 2 objectifs hebdomadaires générés");
    }
    
    // === PROGRESSION DES OBJECTIFS ===
    
    public void OnMoneyEarned(int amount)
    {
        UpdateObjectives(ObjectiveType.EarnMoney, amount);
    }
    
    public void OnMoneySpent(int amount)
    {
        UpdateObjectives(ObjectiveType.SpendMoney, amount);
    }
    
    public void OnProductCrafted(int count = 1)
    {
        UpdateObjectives(ObjectiveType.CraftProducts, count);
    }
    
    public void OnProductSold(int count = 1)
    {
        UpdateObjectives(ObjectiveType.SellProducts, count);
    }
    
    public void OnMaterialBought(int count = 1)
    {
        UpdateObjectives(ObjectiveType.BuyMaterials, count);
    }
    
    public void OnUpgradeBought(int count = 1)
    {
        UpdateObjectives(ObjectiveType.BuyUpgrades, count);
    }
    
    // Met à jour tous les objectifs d'un certain type
    void UpdateObjectives(ObjectiveType type, int amount)
    {
        // Objectifs quotidiens
        foreach (Objective obj in dailyObjectives)
        {
            if (obj.type == type && !obj.isCompleted)
            {
                obj.AddProgress(amount);
                
                // Si complété, donne la récompense
                if (obj.isCompleted)
                {
                    GiveReward(obj);
                }
            }
        }
        
        // Objectifs hebdomadaires
        foreach (Objective obj in weeklyObjectives)
        {
            if (obj.type == type && !obj.isCompleted)
            {
                obj.AddProgress(amount);
                
                // Si complété, donne la récompense
                if (obj.isCompleted)
                {
                    GiveReward(obj);
                }
            }
        }
        
        // Met à jour l'UI (on le fera dans la prochaine étape)
        UpdateObjectivesUI();
    }
    
    // Donne la récompense d'un objectif
    void GiveReward(Objective obj)
    {
        if (gameManager == null) return;
        
        Debug.Log("🎁 Récompense : +" + obj.rewardMoney + "€ et +" + obj.rewardXP + " XP");
        
        // Donne l'argent
        if (obj.rewardMoney > 0)
        {
            gameManager.AddMoney(obj.rewardMoney);
        }
        
        // Donne l'XP
        if (obj.rewardXP > 0)
        {
            ProgressionManager pm = FindObjectOfType<ProgressionManager>();
            if (pm != null)
            {
                pm.AddExperience(obj.rewardXP);
            }
        }
        
        // Feedback visuel
        if (FeedbackManager.Instance != null)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            FeedbackManager.Instance.ShowSuccess("OBJECTIF COMPLÉTÉ !", screenCenter);
        }
    }
    
    // Réinitialise les objectifs quotidiens (appelé chaque nouveau jour)
    public void ResetDailyObjectives()
    {
        GenerateDailyObjectives();
        CreateObjectivesUI();
        Debug.Log("🔄 Objectifs quotidiens réinitialisés");
    }
    
    // Réinitialise les objectifs hebdomadaires (appelé chaque nouvelle semaine)
    public void ResetWeeklyObjectives()
    {
        GenerateWeeklyObjectives();
        CreateObjectivesUI();
        Debug.Log("🔄 Objectifs hebdomadaires réinitialisés");
    }
    
    // Met à jour l'UI des objectifs
    void UpdateObjectivesUI()
    {
        // Met à jour les objectifs quotidiens
        for (int i = 0; i < dailyObjectiveUIs.Count && i < dailyObjectives.Count; i++)
        {
            dailyObjectiveUIs[i].UpdateProgress();
        }
    
        // Met à jour les objectifs hebdomadaires
        for (int i = 0; i < weeklyObjectiveUIs.Count && i < weeklyObjectives.Count; i++)
        {
            weeklyObjectiveUIs[i].UpdateProgress();
        }
    }
    
    public Transform dailyObjectivesContainer;
    public Transform weeklyObjectivesContainer;
    public GameObject objectiveItemPrefab;
    
    private List<ObjectiveItemUI> dailyObjectiveUIs = new List<ObjectiveItemUI>();
    private List<ObjectiveItemUI> weeklyObjectiveUIs = new List<ObjectiveItemUI>();
    
}