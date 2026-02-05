using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Types de bâtiments
public enum BuildingCategory
{
    Structure,      // Pièces principales
    Equipment,      // Machines et outils
    Furniture,      // Meubles
    Decoration      // Décorations
}

// Classe pour représenter un élément constructible
[System.Serializable]
public class BuildingElement
{
    public string elementName;
    public string description;
    public BuildingCategory category;
    public int cost;
    public int unlockLevel;
    public bool isPurchased;
    public string iconName;  // Nom du fichier d'icône (ex: "hammer")
    
    // Bonus
    public BonusType bonusType;
    public float bonusValue;
    
    // Visuel
    public Vector2 position;
    public GameObject visualPrefab;
    
    public BuildingElement(string name, string desc, BuildingCategory cat, int price, int level, string ico, BonusType bonus, float value)
    {
        elementName = name;
        description = desc;
        category = cat;
        cost = price;
        unlockLevel = level;
        isPurchased = false;
        iconName = ico;
        bonusType = bonus;
        bonusValue = value;
        position = Vector2.zero;
    }
}

// Types de bonus
public enum BonusType
{
    None,
    SalesBonus,
    ProductionSpeed,
    MaterialDiscount,
    DailyIncome,
    XPBonus,
    StorageCapacity,
    EmployeeEfficiency,
    OrderBonus
}

public class BuildingManager : MonoBehaviour
{
    // Singleton
    private static BuildingManager instance;
    public static BuildingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BuildingManager>();
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
    
    // === ÉLÉMENTS ===
    public List<BuildingElement> allElements = new List<BuildingElement>();
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private ProgressionManager progressionManager;
    
    // === VISUEL ===
    public Transform officeSlots;       // Zone Bureau (ÉTAGE 2 - Gauche)
    public Transform workshopSlots;     // Zone Atelier (ÉTAGE 2 - Droite)
    public Transform showroomSlots;     // Zone Showroom (ÉTAGE 1 - Gauche)
    public Transform relaxSlots;        // Zone Détente (ÉTAGE 1 - Droite)
    public GameObject visualElementPrefab;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();
        
        InitializeBuildings();
        
        LoadBuildingData();
        
        Debug.Log("🏗️ BuildingManager initialisé avec " + allElements.Count + " éléments");
    }
    
    // Initialise tous les éléments constructibles
    void InitializeBuildings()
    {
        allElements.Clear();
        
        // ===== STRUCTURES (5 éléments) =====
        
        allElements.Add(new BuildingElement(
            "Atelier de base",
            "Votre premier atelier ! Gratuit et déjà construit.",
            BuildingCategory.Structure,
            0,
            1,
            "home",
            BonusType.None,
            0f
        ));
        allElements[0].isPurchased = true;
        
        allElements.Add(new BuildingElement(
            "Showroom",
            "Une vitrine pour exposer vos créations. +10% prix de vente.",
            BuildingCategory.Structure,
            1000,
            3,
            "shop",
            BonusType.SalesBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Zone de stockage",
            "Espace supplémentaire pour vos matériaux. +50 capacité.",
            BuildingCategory.Structure,
            1500,
            5,
            "storage",
            BonusType.StorageCapacity,
            50f
        ));
        
        allElements.Add(new BuildingElement(
            "Bureau administratif",
            "Pour gérer votre entreprise. +15% revenus quotidiens.",
            BuildingCategory.Structure,
            2000,
            7,
            "office",
            BonusType.DailyIncome,
            15f
        ));
        
        allElements.Add(new BuildingElement(
            "Annexe de production",
            "Doublez votre espace de travail ! +20% vitesse production.",
            BuildingCategory.Structure,
            3500,
            10,
            "factory",
            BonusType.ProductionSpeed,
            20f
        ));
        
        // ===== ÉQUIPEMENTS (8 éléments) =====
        
        allElements.Add(new BuildingElement(
            "Établi professionnel",
            "Un établi de qualité. +10% vitesse de production.",
            BuildingCategory.Equipment,
            600,
            2,
            "workbench",
            BonusType.ProductionSpeed,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Scie électrique",
            "Coupe précise et rapide. -8% coût matériaux.",
            BuildingCategory.Equipment,
            900,
            4,
            "saw",
            BonusType.MaterialDiscount,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Ponceuse industrielle",
            "Finitions parfaites. +8% prix de vente.",
            BuildingCategory.Equipment,
            1100,
            5,
            "sander",
            BonusType.SalesBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Compresseur d'air",
            "Pour tous vos outils pneumatiques. +12% vitesse.",
            BuildingCategory.Equipment,
            1300,
            6,
            "compressor",
            BonusType.ProductionSpeed,
            12f
        ));
        
        allElements.Add(new BuildingElement(
            "Perceuse à colonne",
            "Précision maximale. +10% qualité produits.",
            BuildingCategory.Equipment,
            1500,
            7,
            "drill",
            BonusType.SalesBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Ordinateur de gestion",
            "Optimise votre production. +15% XP.",
            BuildingCategory.Equipment,
            1800,
            8,
            "computer",
            BonusType.XPBonus,
            15f
        ));
        
        allElements.Add(new BuildingElement(
            "Robot d'assemblage",
            "Automatisation partielle ! +25% vitesse production.",
            BuildingCategory.Equipment,
            3000,
            10,
            "robot",
            BonusType.ProductionSpeed,
            25f
        ));
        
        allElements.Add(new BuildingElement(
            "Imprimante 3D",
            "Prototypage rapide. -12% coût matériaux.",
            BuildingCategory.Equipment,
            2500,
            9,
            "printer3d",
            BonusType.MaterialDiscount,
            12f
        ));
        
        // ===== MEUBLES (7 éléments) =====
        
        allElements.Add(new BuildingElement(
            "Chaise ergonomique",
            "Confort au travail. +5% efficacité employés.",
            BuildingCategory.Furniture,
            300,
            2,
            "chair",
            BonusType.EmployeeEfficiency,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Étagères murales",
            "Organisation optimale. +10 capacité stockage.",
            BuildingCategory.Furniture,
            400,
            3,
            "shelf",
            BonusType.StorageCapacity,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Armoire sécurisée",
            "Protégez vos outils. -5% coût maintenance.",
            BuildingCategory.Furniture,
            600,
            4,
            "locker",
            BonusType.MaterialDiscount,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Bureau design",
            "Impressionnez vos clients ! +8% prix de vente.",
            BuildingCategory.Furniture,
            800,
            5,
            "desk",
            BonusType.SalesBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Canapé d'accueil",
            "Zone d'attente confortable. +5% commandes.",
            BuildingCategory.Furniture,
            900,
            6,
            "couch",
            BonusType.OrderBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Table de réunion",
            "Organisez vos équipes. +8% efficacité employés.",
            BuildingCategory.Furniture,
            1000,
            7,
            "table",
            BonusType.EmployeeEfficiency,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Casiers vestiaires",
            "Espace pour vos employés. +10% efficacité.",
            BuildingCategory.Furniture,
            700,
            6,
            "lockers",
            BonusType.EmployeeEfficiency,
            10f
        ));
        
        // ===== DÉCORATIONS (10 éléments) =====
        
        allElements.Add(new BuildingElement(
            "Plante verte",
            "Air frais et zen. +3% XP.",
            BuildingCategory.Decoration,
            150,
            2,
            "plant",
            BonusType.XPBonus,
            3f
        ));
        
        allElements.Add(new BuildingElement(
            "Éclairage LED",
            "Lumière parfaite. +5% vitesse production.",
            BuildingCategory.Decoration,
            400,
            3,
            "light",
            BonusType.ProductionSpeed,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Tableau motivant",
            "Inspirez vos équipes ! +5% XP.",
            BuildingCategory.Decoration,
            300,
            3,
            "painting",
            BonusType.XPBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Horloge murale",
            "Gestion du temps optimale. +3% vitesse.",
            BuildingCategory.Decoration,
            200,
            4,
            "clock",
            BonusType.ProductionSpeed,
            3f
        ));
        
        allElements.Add(new BuildingElement(
            "Tapis oriental",
            "Style et confort. +5% prix de vente.",
            BuildingCategory.Decoration,
            500,
            5,
            "carpet",
            BonusType.SalesBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Fontaine décorative",
            "Ambiance relaxante. +8% XP.",
            BuildingCategory.Decoration,
            800,
            6,
            "fountain",
            BonusType.XPBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Trophées de récompenses",
            "Célébrez vos succès ! +10% XP.",
            BuildingCategory.Decoration,
            1000,
            8,
            "trophy",
            BonusType.XPBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Aquarium",
            "Détente et concentration. +7% XP.",
            BuildingCategory.Decoration,
            1200,
            9,
            "aquarium",
            BonusType.XPBonus,
            7f
        ));
        
        allElements.Add(new BuildingElement(
            "Climatisation",
            "Confort optimal. +10% efficacité employés.",
            BuildingCategory.Decoration,
            1500,
            10,
            "ac",
            BonusType.EmployeeEfficiency,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Système audio",
            "Musique motivante. +8% productivité générale.",
            BuildingCategory.Decoration,
            900,
            7,
            "speaker",
            BonusType.ProductionSpeed,
            8f
        ));
        
        Debug.Log("✅ " + allElements.Count + " éléments de construction créés");
    }
    
    // Achète un élément
    public void PurchaseElement(int index)
    {
        if (index < 0 || index >= allElements.Count)
        {
            Debug.LogError("❌ Index invalide !");
            return;
        }
        
        BuildingElement element = allElements[index];
        
        if (element.isPurchased)
        {
            Debug.LogWarning("⚠️ Déjà acheté : " + element.elementName);
            return;
        }
        
        if (progressionManager != null && progressionManager.currentLevel < element.unlockLevel)
        {
            Debug.LogWarning("⚠️ Niveau " + element.unlockLevel + " requis !");
            return;
        }
        
        if (gameManager != null && gameManager.HasEnoughMoney(element.cost))
        {
            gameManager.RemoveMoney(element.cost);
            element.isPurchased = true;
            
            ApplyBonus(element);
            ShowElementVisual(element);
            SaveBuildingData();
            
            Debug.Log("🏗️ Construit : " + element.elementName);
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccess();
            }
            
            if (FeedbackManager.Instance != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                FeedbackManager.Instance.ShowSuccess("🏗️ " + element.elementName.ToUpper() + " CONSTRUIT !", screenCenter);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Pas assez d'argent !");
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    }
    
    void ApplyBonus(BuildingElement element)
    {
        Debug.Log("💪 Bonus appliqué : " + element.bonusType + " +" + element.bonusValue + "%");
    }
    
    // Affiche visuellement l'élément dans l'atelier
    void ShowElementVisual(BuildingElement element)
    {
        // Détermine dans quelle zone placer l'élément
        Transform targetZone = GetZoneForCategory(element.category);
        
        if (targetZone == null || visualElementPrefab == null)
        {
            Debug.LogWarning("⚠️ Zone ou prefab manquant !");
            return;
        }
        
        // Crée l'élément visuel dans la bonne zone
        GameObject visualElement = Instantiate(visualElementPrefab, targetZone);
        
        // Trouve le composant Image pour l'icône
        Image iconImage = visualElement.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null)
        {
            // Charge le sprite depuis Resources/Icons/
            Sprite sprite = Resources.Load<Sprite>("Icons/" + element.iconName);
            if (sprite != null)
            {
                iconImage.sprite = sprite;
                Debug.Log("✅ Icône chargée : " + element.iconName);
            }
            else
            {
                Debug.LogWarning("⚠️ Icône introuvable : Icons/" + element.iconName);
            }
        }
        
        // Animation d'apparition
        visualElement.transform.localScale = Vector3.zero;
        StartCoroutine(AnimateElementAppearance(visualElement));
        
        Debug.Log("🎨 Élément visuel affiché : " + element.elementName + " dans " + targetZone.name);
    }
    
    // Retourne la zone correspondant à la catégorie
    Transform GetZoneForCategory(BuildingCategory category)
    {
        switch (category)
        {
            case BuildingCategory.Structure:
                return officeSlots;
            case BuildingCategory.Equipment:
                return workshopSlots;
            case BuildingCategory.Furniture:
                return showroomSlots;
            case BuildingCategory.Decoration:
                return relaxSlots;
            default:
                return officeSlots;
        }
    }
    
    // Animation d'apparition d'un élément
    System.Collections.IEnumerator AnimateElementAppearance(GameObject element)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            float scale = Mathf.Sin(progress * Mathf.PI) * 1.2f;
            if (progress > 0.5f)
            {
                scale = Mathf.Lerp(1.2f, 1f, (progress - 0.5f) * 2f);
            }
            
            element.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        element.transform.localScale = Vector3.one;
    }
    
    public int GetPurchasedCount(BuildingCategory category)
    {
        int count = 0;
        foreach (BuildingElement element in allElements)
        {
            if (element.category == category && element.isPurchased)
            {
                count++;
            }
        }
        return count;
    }
    
    public int GetTotalPurchased()
    {
        int count = 0;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased)
            {
                count++;
            }
        }
        return count;
    }
    
    public void SaveBuildingData()
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            string key = "BuildingElement_" + i + "_Purchased";
            PlayerPrefs.SetInt(key, allElements[i].isPurchased ? 1 : 0);
        }
        
        PlayerPrefs.Save();
        Debug.Log("💾 Données de construction sauvegardées");
    }
    
    public void LoadBuildingData()
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            string key = "BuildingElement_" + i + "_Purchased";
            
            if (PlayerPrefs.HasKey(key))
            {
                bool isPurchased = PlayerPrefs.GetInt(key) == 1;
                allElements[i].isPurchased = isPurchased;
                
                if (isPurchased)
                {
                    ApplyBonus(allElements[i]);
                    ShowElementVisual(allElements[i]);
                }
            }
        }
        
        Debug.Log("📂 Données de construction chargées");
    }
    
    // ===== CALCUL DES BONUS TOTAUX =====
    
    public float GetSalesBonusMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.SalesBonus)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public float GetProductionSpeedMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.ProductionSpeed)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public float GetMaterialDiscountMultiplier()
    {
        float discount = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.MaterialDiscount)
            {
                discount += element.bonusValue;
            }
        }
        return 1f - (discount / 100f);
    }
    
    public float GetDailyIncomeMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.DailyIncome)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public float GetXPBonusMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.XPBonus)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public int GetStorageCapacityBonus()
    {
        int bonus = 0;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.StorageCapacity)
            {
                bonus += (int)element.bonusValue;
            }
        }
        return bonus;
    }
    
    public float GetEmployeeEfficiencyMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.EmployeeEfficiency)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public float GetOrderBonusMultiplier()
    {
        float bonus = 0f;
        foreach (BuildingElement element in allElements)
        {
            if (element.isPurchased && element.bonusType == BonusType.OrderBonus)
            {
                bonus += element.bonusValue;
            }
        }
        return 1f + (bonus / 100f);
    }
    
    public string GetActiveBonusText()
    {
        string text = "";
        
        float salesBonus = (GetSalesBonusMultiplier() - 1f) * 100f;
        if (salesBonus > 0)
            text += "💰 Ventes : +" + salesBonus.ToString("F0") + "%\n";
        
        float prodSpeed = (GetProductionSpeedMultiplier() - 1f) * 100f;
        if (prodSpeed > 0)
            text += "⚡ Production : +" + prodSpeed.ToString("F0") + "%\n";
        
        float matDiscount = (1f - GetMaterialDiscountMultiplier()) * 100f;
        if (matDiscount > 0)
            text += "💸 Matériaux : -" + matDiscount.ToString("F0") + "%\n";
        
        float xpBonus = (GetXPBonusMultiplier() - 1f) * 100f;
        if (xpBonus > 0)
            text += "⭐ XP : +" + xpBonus.ToString("F0") + "%\n";
        
        int storageBonus = GetStorageCapacityBonus();
        if (storageBonus > 0)
            text += "📦 Stockage : +" + storageBonus + "\n";
        
        float employeeBonus = (GetEmployeeEfficiencyMultiplier() - 1f) * 100f;
        if (employeeBonus > 0)
            text += "👷 Employés : +" + employeeBonus.ToString("F0") + "%\n";
        
        if (text == "")
            text = "Aucun bonus actif";
        
        return text.TrimEnd('\n');
    }
}
