using UnityEngine;
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
    public string icon;  // Emoji pour l'icône
    
    // Bonus
    public BonusType bonusType;
    public float bonusValue;
    
    // Visuel
    public Vector2 position;  // Position dans l'atelier (pour le sprite)
    public GameObject visualPrefab;  // Prefab visuel (optionnel)
    
    public BuildingElement(string name, string desc, BuildingCategory cat, int price, int level, string ico, BonusType bonus, float value)
    {
        elementName = name;
        description = desc;
        category = cat;
        cost = price;
        unlockLevel = level;
        isPurchased = false;
        icon = ico;
        bonusType = bonus;
        bonusValue = value;
        position = Vector2.zero;
    }
}

// Types de bonus
public enum BonusType
{
    None,
    SalesBonus,           // Bonus sur les ventes
    ProductionSpeed,      // Vitesse de production
    MaterialDiscount,     // Réduction coût matériaux
    DailyIncome,          // Revenus quotidiens
    XPBonus,              // Bonus XP
    StorageCapacity,      // Capacité de stockage
    EmployeeEfficiency,   // Efficacité employés
    OrderBonus            // Bonus commandes
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
    public Transform buildingContainer;  // Container pour les sprites
    public GameObject visualElementPrefab;  // Prefab des éléments visuels
    
    
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
            "🏠",
            BonusType.None,
            0f
        ));
        allElements[0].isPurchased = true;  // Déjà acheté
        
        allElements.Add(new BuildingElement(
            "Showroom",
            "Une vitrine pour exposer vos créations. +10% prix de vente.",
            BuildingCategory.Structure,
            1000,
            3,
            "🪟",
            BonusType.SalesBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Zone de stockage",
            "Espace supplémentaire pour vos matériaux. +50 capacité.",
            BuildingCategory.Structure,
            1500,
            5,
            "📦",
            BonusType.StorageCapacity,
            50f
        ));
        
        allElements.Add(new BuildingElement(
            "Bureau administratif",
            "Pour gérer votre entreprise. +15% revenus quotidiens.",
            BuildingCategory.Structure,
            2000,
            7,
            "💼",
            BonusType.DailyIncome,
            15f
        ));
        
        allElements.Add(new BuildingElement(
            "Annexe de production",
            "Doublez votre espace de travail ! +20% vitesse production.",
            BuildingCategory.Structure,
            3500,
            10,
            "🏢",
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
            "🔨",
            BonusType.ProductionSpeed,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Scie électrique",
            "Coupe précise et rapide. -8% coût matériaux.",
            BuildingCategory.Equipment,
            900,
            4,
            "🪚",
            BonusType.MaterialDiscount,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Ponceuse industrielle",
            "Finitions parfaites. +8% prix de vente.",
            BuildingCategory.Equipment,
            1100,
            5,
            "⚙️",
            BonusType.SalesBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Compresseur d'air",
            "Pour tous vos outils pneumatiques. +12% vitesse.",
            BuildingCategory.Equipment,
            1300,
            6,
            "💨",
            BonusType.ProductionSpeed,
            12f
        ));
        
        allElements.Add(new BuildingElement(
            "Perceuse à colonne",
            "Précision maximale. +10% qualité produits.",
            BuildingCategory.Equipment,
            1500,
            7,
            "🔩",
            BonusType.SalesBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Ordinateur de gestion",
            "Optimise votre production. +5% tous bonus.",
            BuildingCategory.Equipment,
            1800,
            8,
            "🖥️",
            BonusType.XPBonus,
            15f
        ));
        
        allElements.Add(new BuildingElement(
            "Robot d'assemblage",
            "Automatisation partielle ! +25% vitesse production.",
            BuildingCategory.Equipment,
            3000,
            10,
            "🤖",
            BonusType.ProductionSpeed,
            25f
        ));
        
        allElements.Add(new BuildingElement(
            "Imprimante 3D",
            "Prototypage rapide. -12% coût matériaux.",
            BuildingCategory.Equipment,
            2500,
            9,
            "🖨️",
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
            "🪑",
            BonusType.EmployeeEfficiency,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Étagères murales",
            "Organisation optimale. +10 capacité stockage.",
            BuildingCategory.Furniture,
            400,
            3,
            "📚",
            BonusType.StorageCapacity,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Armoire sécurisée",
            "Protégez vos outils. -5% coût maintenance.",
            BuildingCategory.Furniture,
            600,
            4,
            "🗄️",
            BonusType.MaterialDiscount,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Bureau design",
            "Impressionnez vos clients ! +8% prix de vente.",
            BuildingCategory.Furniture,
            800,
            5,
            "🪑",
            BonusType.SalesBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Canapé d'accueil",
            "Zone d'attente confortable. +5% commandes.",
            BuildingCategory.Furniture,
            900,
            6,
            "🛋️",
            BonusType.OrderBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Table de réunion",
            "Organisez vos équipes. +8% efficacité employés.",
            BuildingCategory.Furniture,
            1000,
            7,
            "🪑",
            BonusType.EmployeeEfficiency,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Casiers vestiaires",
            "Espace pour vos employés. +10% efficacité.",
            BuildingCategory.Furniture,
            700,
            6,
            "🚪",
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
            "🪴",
            BonusType.XPBonus,
            3f
        ));
        
        allElements.Add(new BuildingElement(
            "Éclairage LED",
            "Lumière parfaite. +5% vitesse production.",
            BuildingCategory.Decoration,
            400,
            3,
            "💡",
            BonusType.ProductionSpeed,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Tableau motivant",
            "Inspirez vos équipes ! +5% XP.",
            BuildingCategory.Decoration,
            300,
            3,
            "🖼️",
            BonusType.XPBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Horloge murale",
            "Gestion du temps optimale. +3% vitesse.",
            BuildingCategory.Decoration,
            200,
            4,
            "🕐",
            BonusType.ProductionSpeed,
            3f
        ));
        
        allElements.Add(new BuildingElement(
            "Tapis oriental",
            "Style et confort. +5% prix de vente.",
            BuildingCategory.Decoration,
            500,
            5,
            "🧶",
            BonusType.SalesBonus,
            5f
        ));
        
        allElements.Add(new BuildingElement(
            "Fontaine décorative",
            "Ambiance relaxante. +8% XP.",
            BuildingCategory.Decoration,
            800,
            6,
            "⛲",
            BonusType.XPBonus,
            8f
        ));
        
        allElements.Add(new BuildingElement(
            "Trophées de récompenses",
            "Célébrez vos succès ! +10% XP.",
            BuildingCategory.Decoration,
            1000,
            8,
            "🏆",
            BonusType.XPBonus,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Aquarium",
            "Détente et concentration. +7% tous bonus.",
            BuildingCategory.Decoration,
            1200,
            9,
            "🐠",
            BonusType.XPBonus,
            7f
        ));
        
        allElements.Add(new BuildingElement(
            "Climatisation",
            "Confort optimal. +10% efficacité employés.",
            BuildingCategory.Decoration,
            1500,
            10,
            "❄️",
            BonusType.EmployeeEfficiency,
            10f
        ));
        
        allElements.Add(new BuildingElement(
            "Système audio",
            "Musique motivante. +8% productivité générale.",
            BuildingCategory.Decoration,
            900,
            7,
            "🔊",
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
        
        // Vérifie si déjà acheté
        if (element.isPurchased)
        {
            Debug.LogWarning("⚠️ Déjà acheté : " + element.elementName);
            return;
        }
        
        // Vérifie le niveau
        if (progressionManager != null && progressionManager.currentLevel < element.unlockLevel)
        {
            Debug.LogWarning("⚠️ Niveau " + element.unlockLevel + " requis !");
            return;
        }
        
        // Vérifie l'argent
        if (gameManager != null && gameManager.HasEnoughMoney(element.cost))
        {
            // Retire l'argent
            gameManager.RemoveMoney(element.cost);
            
            // Marque comme acheté
            element.isPurchased = true;
            
            // Applique le bonus
            ApplyBonus(element);
            
            // Affiche visuellement (on fera ça après)
            ShowElementVisual(element);
            
            SaveBuildingData();
            
            Debug.Log("🏗️ Construit : " + element.elementName);
            
            // Son
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccess();
            }
            
            // Feedback
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
    
    // Applique le bonus d'un élément (ne fait que logger, les calculs sont dans les Get)
    void ApplyBonus(BuildingElement element)
    {
        Debug.Log("💪 Bonus appliqué : " + element.bonusType + " +" + element.bonusValue + "%");
    }

    
    // Affiche visuellement l'élément dans l'atelier
    void ShowElementVisual(BuildingElement element)
    {
        if (buildingContainer == null || visualElementPrefab == null)
        {
            Debug.LogWarning("⚠️ Container ou prefab manquant !");
            return;
        }
    
        // Crée l'élément visuel
        GameObject visualElement = Instantiate(visualElementPrefab, buildingContainer);
    
        // Trouve l'icône
        TextMeshProUGUI iconText = visualElement.transform.Find("IconText")?.GetComponent<TextMeshProUGUI>();
        if (iconText != null)
        {
            iconText.text = element.icon;
        }
    
        // Position aléatoire dans le container (pour un effet naturel)
        RectTransform rect = visualElement.GetComponent<RectTransform>();
        if (rect != null)
        {
            // Position aléatoire
            float randomX = Random.Range(-200f, 200f);
            float randomY = Random.Range(-150f, 150f);
            rect.anchoredPosition = new Vector2(randomX, randomY);
        
            // Rotation légère aléatoire
            rect.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
        }
    
        // Animation d'apparition (scale)
        visualElement.transform.localScale = Vector3.zero;
        StartCoroutine(AnimateElementAppearance(visualElement));
    
        Debug.Log("🎨 Élément visuel affiché : " + element.elementName);
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
        
            // Courbe d'animation (bounce)
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

    
    // Compte les éléments achetés par catégorie
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
    
    // Compte le total acheté
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
    
    // Sauvegarde les éléments achetés
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
    // Charge les éléments achetés
    public void LoadBuildingData()
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            string key = "BuildingElement_" + i + "_Purchased";

            if (PlayerPrefs.HasKey(key))
            {
                bool isPurchased = PlayerPrefs.GetInt(key) == 1;
                allElements[i].isPurchased = isPurchased;

                // Si acheté, applique le bonus et affiche visuellement
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

    // Retourne le multiplicateur total pour les ventes
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

    // Retourne le multiplicateur pour la vitesse de production
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

    // Retourne le multiplicateur pour la réduction des matériaux
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

    // Retourne le multiplicateur pour les revenus quotidiens
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

    // Retourne le multiplicateur d'XP
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

    // Retourne la capacité de stockage supplémentaire
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

    // Retourne le multiplicateur d'efficacité des employés
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

    // Retourne le multiplicateur pour les commandes
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

    // ===== AFFICHAGE DES BONUS =====

    // Retourne le texte des bonus actifs (pour l'UI)
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
