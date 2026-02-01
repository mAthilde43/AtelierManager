using UnityEngine;
using TMPro;
using System.Collections.Generic; // Nécessaire pour utiliser les listes

public class GameManager : MonoBehaviour
{
    // === ARGENT ===
    public int playerMoney = 1000;
    public TextMeshProUGUI moneyText;
    
    // === MATÉRIAUX ===
    public List<CraftingMaterial> craftingMaterials = new List<CraftingMaterial>(); // Liste de tous les matériaux
    // === PRODUITS ===
    public List<Product> products = new List<Product>(); // Liste de tous les produits
    // === UI BOUTIQUE ===
    // === UI BOUTIQUE ===
    public MaterialItemUI woodItemUI;    // Élément UI pour le bois de chêne
    public MaterialItemUI pineItemUI;    // Élément UI pour le bois de pin
    public MaterialItemUI varnishItemUI; // Élément UI pour le vernis
    public MaterialItemUI metalItemUI;   // Élément UI pour le métal
    public MaterialItemUI fabricItemUI;  // Élément UI pour le tissu
    public MaterialItemUI leatherItemUI; // Élément UI pour le cuire
    
// === UI ATELIER ===
    public ProductItemUI tableItemUI; // Élément UI pour la table en chêne
    public ProductItemUI chairItemUI;
    public ProductItemUI shelfItemUI;
    public ProductItemUI lampItemUI;      
    public ProductItemUI armchairItemUI;  
    public ProductItemUI deskItemUI;      
    public ProductItemUI sofaItemUI;      
    public ProductItemUI wardrobeItemUI;  
    
    
    // === UI VENTE ===
    public SaleItemUI saleTableItemUI;  // Élément UI pour vendre la table
    public SaleItemUI saleChairItemUI;  // Élément UI pour vendre la chaise
    public SaleItemUI saleShelfItemUI;  // Élément UI pour vendre l'étagère
    public SaleItemUI saleLampItemUI;  
    public SaleItemUI saleArmchairItemUI; 
    public SaleItemUI saleDeskItemUI; 
    public SaleItemUI saleSofaItemUI; 
    public SaleItemUI saleWardrobefItemUI;
    
    // === UI AMÉLIORATIONS ===
    public UpgradeItemUI upgrade1ItemUI;
    public UpgradeItemUI upgrade2ItemUI;
    public UpgradeItemUI upgrade3ItemUI;
    public UpgradeItemUI upgrade4ItemUI;
    public UpgradeItemUI upgrade5ItemUI;
    
    // === AMÉLIORATIONS ===
    public List<Upgrade> upgrades = new List<Upgrade>();
    // === RÉFÉRENCES ===
    private ProgressionManager progressionManager;
    
    void Start()
    {
        Debug.Log("🎮 Atelier Manager démarré avec succès !");

        // Initialise les matériaux de base
        InitializeMaterials();

        // Initialise les produits
        InitializeProducts();
    
        // Initialise les améliorations
        InitializeUpgrades();

        // Affiche l'argent au démarrage
        UpdateMoneyDisplay();

        // Initialise l'UI de la boutique
        InitializeUI();
    
        // Récupère le ProgressionManager
        progressionManager = FindObjectOfType<ProgressionManager>();
    
        // === CHARGE LA SAUVEGARDE (si elle existe) ===
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            TimeManager timeManager = FindObjectOfType<TimeManager>();
        
            if (saveManager.HasSaveData())
            {
                saveManager.LoadGame(this, timeManager, progressionManager);
                Debug.Log("📂 Partie chargée !");
            }
            else
            {
                Debug.Log("🆕 Nouvelle partie");
            }
        }
    }
    
    // Fonction pour créer les matériaux de départ
    void InitializeMaterials()
    {
        // Crée 3 types de matériaux
        craftingMaterials.Add(new CraftingMaterial("Bois de chêne", 50));
        craftingMaterials.Add(new CraftingMaterial("Bois de pin", 30));
        craftingMaterials.Add(new CraftingMaterial("Vernis", 20));
        craftingMaterials.Add(new CraftingMaterial("Métal", 80));
        craftingMaterials.Add(new CraftingMaterial("Tissu", 40));
        craftingMaterials.Add(new CraftingMaterial("Cuir", 60));
        
        Debug.Log("📦 " + craftingMaterials.Count + " types de matériaux initialisés");
    }
    
    // Fonction pour créer les produits de départ
    void InitializeProducts()
    {
        // Produit 1 : Table en chêne
        Product tableChene = new Product("Table en chêne", 200, 5);
        tableChene.AddMaterialRequirement(0, 3); // 3x Bois de chêne (index 0)
        tableChene.AddMaterialRequirement(2, 1); // 1x Vernis (index 2)
        products.Add(tableChene);
    
        // Produit 2 : Chaise en pin
        Product chaisePine = new Product("Chaise en pin", 80, 3);
        chaisePine.AddMaterialRequirement(1, 2); // 2x Bois de pin (index 1)
        chaisePine.AddMaterialRequirement(2, 1); // 1x Vernis (index 2)
        products.Add(chaisePine);
    
        // Produit 3 : Étagère mixte
        Product etagere = new Product("Étagère mixte", 150, 4);
        etagere.AddMaterialRequirement(0, 2); // 2x Bois de chêne
        etagere.AddMaterialRequirement(1, 2); // 2x Bois de pin
        etagere.AddMaterialRequirement(2, 2); // 2x Vernis
        products.Add(etagere);
        
        // Lampe : 1x Métal + 1x Tissu = 180€
        Product lamp = new Product("Lampe", 180, 25);
        lamp.AddMaterialRequirement(3, 1); // 1x Métal
        lamp.AddMaterialRequirement(4, 1); // 1x Tissu
        products.Add(lamp);
    
        // Fauteuil : 2x Pin + 2x Tissu + 1x Cuir = 280€
        Product armchair = new Product("Fauteuil", 280, 35);
        armchair.AddMaterialRequirement(1, 2); // 2x Bois de pin
        armchair.AddMaterialRequirement(4, 2); // 2x Tissu
        armchair.AddMaterialRequirement(5, 1); // 1x Cuir
        products.Add(armchair);
    
        // Bureau : 4x Chêne + 2x Métal + 1x Vernis = 450€
        Product desk = new Product("Bureau", 450, 40);
        desk.AddMaterialRequirement(0, 4); // 4x Bois de chêne
        desk.AddMaterialRequirement(3, 2); // 2x Métal
        desk.AddMaterialRequirement(2, 1); // 1x Vernis
        products.Add(desk);
    
        // Canapé : 3x Pin + 4x Tissu + 2x Cuir = 520€
        Product sofa = new Product("Canapé", 520, 50);
        sofa.AddMaterialRequirement(1, 3); // 3x Bois de pin
        sofa.AddMaterialRequirement(4, 4); // 4x Tissu
        sofa.AddMaterialRequirement(5, 2); // 2x Cuir
        products.Add(sofa);
        
        // Armoire : 5x Chêne + 3x Métal + 2x Vernis = 650€
        Product wardrobe = new Product("Armoire", 650, 60);
        wardrobe.AddMaterialRequirement(0, 5); // 5x Bois de chêne
        wardrobe.AddMaterialRequirement(3, 3); // 3x Métal
        wardrobe.AddMaterialRequirement(2, 2); // 2x Vernis
        products.Add(wardrobe);
    
        Debug.Log("🔨 " + products.Count + " types de produits initialisés");
    }
    
    // Fonction pour créer les améliorations de départ
    void InitializeUpgrades()
    {
        // Amélioration 1 : Réduction coût matériaux
        upgrades.Add(new Upgrade(
            "Fournisseur de confiance",
            "Réduit le coût des matériaux de 10%",
            300,
            UpgradeType.MaterialDiscount,
            10
        ));
    
        // Amélioration 2 : Bonus sur les ventes
        upgrades.Add(new Upgrade(
            "Meilleure réputation",
            "Augmente le prix de vente de 15%",
            500,
            UpgradeType.SalesBonus,
            15
        ));
    
        // Amélioration 3 : Outils perfectionnés
        upgrades.Add(new Upgrade(
            "Outils perfectionnés",
            "Production plus rapide (non implémenté)",
            400,
            UpgradeType.ProductionSpeed,
            20
        ));
    
        // Amélioration 4 : Augmentation revenus quotidiens
        upgrades.Add(new Upgrade(
            "Contrat régulier",
            "Augmente les revenus quotidiens de 30€",
            600,
            UpgradeType.DailyIncomeBoost,
            30
        ));
    
        // Amélioration 5 : Réduction charges
        upgrades.Add(new Upgrade(
            "Local optimisé",
            "Réduit les charges hebdomadaires de 50€",
            800,
            UpgradeType.WeeklyCostReduction,
            50
        ));
    
        Debug.Log("🔧 " + upgrades.Count + " améliorations disponibles");
    }
    
    // === FONCTIONS ARGENT (inchangées) ===
    public void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = playerMoney + " €";
        }
        else
        {
            Debug.LogWarning("⚠️ MoneyText n'est pas assigné dans l'Inspector !");
        }
    }
    
    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyDisplay();
        Debug.Log("💰 +" + amount + "€ | Total: " + playerMoney + "€");
    
        // Son de gain d'argent
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMoneyGain();
        }
    
        // Feedback visuel (texte flottant)
        if (FeedbackManager.Instance != null && moneyText != null)
        {
            Vector3 position = moneyText.transform.position;
            FeedbackManager.Instance.ShowMoneyGain(amount, position);
        }

        //track pour les objectifs
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnMoneyEarned(amount);
        }
    }
    
    public void RemoveMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyDisplay();
        Debug.Log("💸 -" + amount + "€ | Total: " + playerMoney + "€");
    
        // Feedback visuel (texte flottant rouge)
        if (FeedbackManager.Instance != null && moneyText != null)
        {
            Vector3 position = moneyText.transform.position;
            FeedbackManager.Instance.ShowMoneyLoss(amount, position);
        }
        
        //track pour les objectifs
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnMoneySpent(amount);
        }
    }
    
    public bool HasEnoughMoney(int amount)
    {
        return playerMoney >= amount;
    }
    
    // === FONCTIONS D'ACHAT DE MATÉRIAUX ===
    
    // Fonction pour acheter un matériau
    public void BuyMaterial(int materialIndex, int quantity)
    {
        // Vérifie que l'index existe
        if (materialIndex < 0 || materialIndex >= craftingMaterials.Count)
        {
            Debug.LogError("❌ Index de matériau invalide !");
            return;
        }
        
        CraftingMaterial mat = craftingMaterials[materialIndex];
        int totalCost = mat.price * quantity;
        
        // Vérifie si on a assez d'argent
        if (HasEnoughMoney(totalCost))
        {
            // Retire l'argent
            RemoveMoney(totalCost);
            
            // Ajoute les matériaux au stock
            mat.AddQuantity(quantity);
            
            Debug.Log("✅ Achat réussi : " + quantity + "x " + mat.materialName + " pour " + totalCost + "€");
            
            // Track pour les objectifs
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnMaterialBought(quantity);
            }
            // Son d'achat
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPurchase();
            }
            // Met à jour les notifications
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.UpdateAllNotifications();
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Pas assez d'argent ! Il manque " + (totalCost - playerMoney) + "€");
        }
    }
    
    // Fonction pour obtenir un matériau par son index
    public CraftingMaterial GetMaterial(int index)
    {
        if (index >= 0 && index < craftingMaterials.Count)
        {
            return craftingMaterials[index];
        }
        return null;
    }
    
    // Fonction pour initialiser l'UI de la boutique
    // Fonction pour initialiser l'UI de la boutique et de l'atelier
    void InitializeUI()
    {
        // Configure les éléments UI des matériaux
        if (woodItemUI != null)
        {
            woodItemUI.Setup(0, craftingMaterials[0], this);
        }
        if (pineItemUI != null)
        {
            pineItemUI.Setup(1, craftingMaterials[1], this);
        }
        if (varnishItemUI != null)
        {
            varnishItemUI.Setup(2, craftingMaterials[2], this);
        }
        if (metalItemUI != null)
        {
            metalItemUI.Setup(3, craftingMaterials[3], this);
        } 
        if (fabricItemUI != null)
        {
            fabricItemUI.Setup(4, craftingMaterials[4], this);
        } 
        if (leatherItemUI != null)
        {
            leatherItemUI.Setup(5, craftingMaterials[5], this);
        }
    
        // Configure les éléments UI des produits (atelier)
        if (tableItemUI != null)
            tableItemUI.Setup(0, products[0], this);
        if (chairItemUI != null)
            chairItemUI.Setup(1, products[1], this);
        if (shelfItemUI != null)
            shelfItemUI.Setup(2, products[2], this);
        if (lampItemUI != null)
            lampItemUI.Setup(3, products[3], this);     
        if (armchairItemUI != null)
            armchairItemUI.Setup(4, products[4],this);  
        if (deskItemUI != null)
            deskItemUI.Setup(5, products[5],this);     
        if (sofaItemUI != null)
            sofaItemUI.Setup(6, products[6],this);     
        if (wardrobeItemUI != null)
            wardrobeItemUI.Setup(7, products[7],this);  
    
        // Configure les éléments UI de vente
        if (saleTableItemUI != null)
            saleTableItemUI.Setup(0, products[0], this);
        if (saleChairItemUI != null)
            saleChairItemUI.Setup(1, products[1], this);
        if (saleShelfItemUI != null)
            saleShelfItemUI.Setup(2, products[2], this);
        if (saleLampItemUI != null)
            saleLampItemUI.Setup(3, products[3], this);
        if (saleArmchairItemUI != null)
            saleArmchairItemUI.Setup(4, products[4], this);
        if (saleDeskItemUI != null)
            saleDeskItemUI.Setup(5, products[5], this);
        if (saleSofaItemUI != null)
            saleSofaItemUI.Setup(6, products[6], this);
        if (saleWardrobefItemUI != null)
            saleWardrobefItemUI.Setup(7, products[7], this);
        
        
        
        // Configure les éléments UI des améliorations
        if (upgrade1ItemUI != null)
            upgrade1ItemUI.Setup(0, upgrades[0], this);
        if (upgrade2ItemUI != null)
            upgrade2ItemUI.Setup(1, upgrades[1], this);
        if (upgrade3ItemUI != null)
            upgrade3ItemUI.Setup(2, upgrades[2], this);
        if (upgrade4ItemUI != null)
            upgrade4ItemUI.Setup(3, upgrades[3], this);
        if (upgrade5ItemUI != null)
            upgrade5ItemUI.Setup(4, upgrades[4], this);
    }
    
    // === FONCTIONS DE PRODUCTION ===

// Fonction pour fabriquer un produit
    public void CraftProduct(int productIndex)
    {
        // Vérifie que l'index existe
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Debug.LogError("❌ Index de produit invalide !");
            return;
        }
    
        Product prod = products[productIndex];
    
        // Vérifie si on a tous les matériaux nécessaires
        bool hasAllMaterials = true;
        foreach (MaterialRequirement req in prod.recipe)
        {
            CraftingMaterial mat = GetMaterial(req.materialIndex);
            if (mat == null || !mat.HasEnoughQuantity(req.amount))
            {
                hasAllMaterials = false;
                if (mat != null)
                {
                    Debug.LogWarning("⚠️ Pas assez de " + mat.materialName + " (besoin: " + req.amount + ", stock: " + mat.quantity + ")");
                }
                break;
            }
        }
    
        // Si on a tout, on fabrique
        if (hasAllMaterials)
        {
            // Retire les matériaux du stock
            foreach (MaterialRequirement req in prod.recipe)
            {
                CraftingMaterial mat = GetMaterial(req.materialIndex);
                mat.RemoveQuantity(req.amount);
            }
        
            // Ajoute le produit fini au stock
            prod.AddQuantity(1);
        
            Debug.Log("✅ Production réussie : 1x " + prod.productName);
            // Track pour les objectifs
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnProductCrafted(1);
            }
            
            // Son de craft
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCraft();
            }
            
            // Donne de l'XP pour la fabrication
            if (progressionManager != null)
            {
                progressionManager.OnProductCrafted(prod.sellPrice);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Matériaux insuffisants pour fabriquer " + prod.productName);
        }
    
        // Met à jour toute l'interface
        RefreshAllUI();
    }
    
    // === FONCTIONS DE VENTE ===

// Fonction pour vendre un produit
    public void SellProduct(int productIndex, int quantity)
    {
        // Vérifie que l'index existe
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Debug.LogError("❌ Index de produit invalide !");
            return;
        }
    
        Product prod = products[productIndex];
    
        // Vérifie si on a assez de produits en stock
        if (prod.HasEnoughQuantity(quantity))
        {
            // Calcule le gain
            int earnings = prod.sellPrice * quantity;
        
            // Retire le produit du stock
            prod.RemoveQuantity(quantity);
        
            // Ajoute l'argent
            AddMoney(earnings);
        
            Debug.Log("✅ Vente réussie : " + quantity + "x " + prod.productName + " pour " + earnings + "€");
            // Track pour les objectifs
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnProductSold(quantity);
            }
            
            // Son de vente
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySell();
            }
            
            // Donne de l'XP pour la vente
            if (progressionManager != null)
            {
                progressionManager.OnProductSold(earnings);
            }
        
            // Met à jour toute l'interface
            RefreshAllUI();
        }
        else
        {
            Debug.LogWarning("⚠️ Stock insuffisant de " + prod.productName + " (stock: " + prod.quantity + ")");
        }
    }
    
    // === FONCTIONS D'AMÉLIORATIONS ===

// Fonction pour acheter une amélioration
public void BuyUpgrade(int upgradeIndex)
{
    // Vérifie que l'index existe
    if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
    {
        Debug.LogError("❌ Index d'amélioration invalide !");
        return;
    }
    
    Upgrade upg = upgrades[upgradeIndex];
    
    // Vérifie si déjà acheté
    if (upg.isPurchased)
    {
        Debug.LogWarning("⚠️ Amélioration déjà achetée : " + upg.upgradeName);
        return;
    }
    
    // Vérifie si on a assez d'argent
    if (HasEnoughMoney(upg.cost))
    {
        // Retire l'argent
        RemoveMoney(upg.cost);
        
        // Achète l'amélioration
        upg.Purchase();
        
        // Applique l'effet
        ApplyUpgradeEffect(upg);
        
        Debug.Log("✅ Amélioration achetée : " + upg.upgradeName + " pour " + upg.cost + "€");
        // Track pour les objectifs
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnUpgradeBought(1);
        }
        
        // Son de succès
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySuccess();
        }
        
        // Donne de l'XP pour l'amélioration
        if (progressionManager != null)
        {
            progressionManager.OnUpgradeBought();
        }
        
        // Met à jour l'interface
        RefreshAllUI();
    }
    else
    {
        Debug.LogWarning("⚠️ Pas assez d'argent pour " + upg.upgradeName + " ! Il manque " + (upg.cost - playerMoney) + "€");
        // Son d'erreur
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayError();
        }
    }
}

// Fonction pour appliquer l'effet d'une amélioration
void ApplyUpgradeEffect(Upgrade upg)
{
    TimeManager timeManager = FindObjectOfType<TimeManager>();
    
    switch (upg.type)
    {
        case UpgradeType.MaterialDiscount:
            // Réduit le prix de tous les matériaux
            foreach (CraftingMaterial mat in craftingMaterials)
            {
                int reduction = Mathf.RoundToInt(mat.price * upg.value / 100f);
                mat.price -= reduction;
                if (mat.price < 1) mat.price = 1; // Minimum 1€
            }
            Debug.Log("📉 Prix des matériaux réduits de " + upg.value + "%");
            break;
            
        case UpgradeType.SalesBonus:
            // Augmente le prix de vente de tous les produits
            foreach (Product prod in products)
            {
                int bonus = Mathf.RoundToInt(prod.sellPrice * upg.value / 100f);
                prod.sellPrice += bonus;
            }
            Debug.Log("📈 Prix de vente augmentés de " + upg.value + "%");
            break;
            
        case UpgradeType.ProductionSpeed:
            // À implémenter plus tard avec un vrai système de temps de production
            Debug.Log("⚙️ Vitesse de production augmentée (fonctionnalité à venir)");
            break;
            
        case UpgradeType.DailyIncomeBoost:
            // Augmente les revenus quotidiens
            if (timeManager != null)
            {
                timeManager.dailyIncome += upg.value;
                Debug.Log("💵 Revenus quotidiens augmentés de " + upg.value + "€ (nouveau total: " + timeManager.dailyIncome + "€/jour)");
            }
            break;
            
        case UpgradeType.WeeklyCostReduction:
            // Réduit les charges hebdomadaires
            if (timeManager != null)
            {
                timeManager.weeklyCost -= upg.value;
                if (timeManager.weeklyCost < 0) timeManager.weeklyCost = 0;
                Debug.Log("💸 Charges hebdomadaires réduites de " + upg.value + "€ (nouveau total: " + timeManager.weeklyCost + "€/semaine)");
            }
            break;
    }
}

// Fonction publique pour réappliquer un effet d'amélioration (utilisée au chargement)
public void ReapplyUpgradeEffect(Upgrade upg)
{
    // Ne réapplique que les effets permanents (pas les bonus d'argent)
    TimeManager timeManager = FindObjectOfType<TimeManager>();
    
    switch (upg.type)
    {
        case UpgradeType.MaterialDiscount:
            // Réduit le prix de tous les matériaux
            foreach (CraftingMaterial mat in craftingMaterials)
            {
                int reduction = Mathf.RoundToInt(mat.price * upg.value / 100f);
                mat.price -= reduction;
                if (mat.price < 1) mat.price = 1;
            }
            break;
            
        case UpgradeType.SalesBonus:
            // Augmente le prix de vente de tous les produits
            foreach (Product prod in products)
            {
                int bonus = Mathf.RoundToInt(prod.sellPrice * upg.value / 100f);
                prod.sellPrice += bonus;
            }
            break;
            
        case UpgradeType.DailyIncomeBoost:
            // Augmente les revenus quotidiens
            if (timeManager != null)
            {
                timeManager.dailyIncome += upg.value;
            }
            break;
            
        case UpgradeType.WeeklyCostReduction:
            // Réduit les charges hebdomadaires
            if (timeManager != null)
            {
                timeManager.weeklyCost -= upg.value;
                if (timeManager.weeklyCost < 0) timeManager.weeklyCost = 0;
            }
            break;
    }
}

// Fonction pour obtenir une amélioration par son index
public Upgrade GetUpgrade(int index)
{
    if (index >= 0 && index < upgrades.Count)
    {
        return upgrades[index];
    }
    return null;
}

// Fonction pour obtenir un produit par son index
    public Product GetProduct(int index)
    {
        if (index >= 0 && index < products.Count)
        {
            return products[index];
        }
        return null;
    }
    
    // Fonction pour mettre à jour toute l'interface
    public void RefreshAllUI()
    {
        // Met à jour les matériaux
        if (woodItemUI != null)
            woodItemUI.UpdateDisplay(craftingMaterials[0]);
        if (pineItemUI != null)
            pineItemUI.UpdateDisplay(craftingMaterials[1]);
        if (varnishItemUI != null)
            varnishItemUI.UpdateDisplay(craftingMaterials[2]);
        if (metalItemUI != null)
            metalItemUI.UpdateDisplay(craftingMaterials[3]);
        if (fabricItemUI != null)
            fabricItemUI.UpdateDisplay(craftingMaterials[4]);
        if (leatherItemUI != null)
            leatherItemUI.UpdateDisplay(craftingMaterials[5]);
        

        // Met à jour les produits (atelier)
        if (tableItemUI != null)
            tableItemUI.UpdateDisplay(products[0]);
        if (chairItemUI != null)
            chairItemUI.UpdateDisplay(products[1]);
        if (shelfItemUI != null)
            shelfItemUI.UpdateDisplay(products[2]);
        if (lampItemUI != null)
            lampItemUI.UpdateDisplay(products[3]);     
        if (armchairItemUI != null)
            armchairItemUI.UpdateDisplay(products[4]);  
        if (deskItemUI != null)
            deskItemUI.UpdateDisplay(products[5]);     
        if (sofaItemUI != null)
            sofaItemUI.UpdateDisplay(products[6]);     
        if (wardrobeItemUI != null)
            wardrobeItemUI.UpdateDisplay(products[7]);  
    
        // Met à jour les produits (vente)
        if (saleTableItemUI != null)
            saleTableItemUI.UpdateDisplay(products[0]);
        if (saleChairItemUI != null)
            saleChairItemUI.UpdateDisplay(products[1]);
        if (saleShelfItemUI != null)
            saleShelfItemUI.UpdateDisplay(products[2]);
        if (saleLampItemUI != null)
            saleLampItemUI.UpdateDisplay(products[3]);
        if (saleArmchairItemUI != null)
            saleArmchairItemUI.UpdateDisplay(products[4]);
        if (saleDeskItemUI != null)
            saleDeskItemUI.UpdateDisplay(products[5]);
        if (saleSofaItemUI != null)
            saleSofaItemUI.UpdateDisplay(products[6]);
        if (saleWardrobefItemUI != null)
            saleWardrobefItemUI.UpdateDisplay(products[7]);
    
        // Met à jour les améliorations
        if (upgrade1ItemUI != null)
            upgrade1ItemUI.UpdateDisplay(upgrades[0]);
        if (upgrade2ItemUI != null)
            upgrade2ItemUI.UpdateDisplay(upgrades[1]);
        if (upgrade3ItemUI != null)
            upgrade3ItemUI.UpdateDisplay(upgrades[2]);
        if (upgrade4ItemUI != null)
            upgrade4ItemUI.UpdateDisplay(upgrades[3]);
        if (upgrade5ItemUI != null)
            upgrade5ItemUI.UpdateDisplay(upgrades[4]);
    
        // Met à jour les notifications (NOUVEAU)
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.UpdateAllNotifications();
        }
    }
    
    // Sauvegarde automatiquement quand on quitte le jeu
    void OnApplicationQuit()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            TimeManager timeManager = FindObjectOfType<TimeManager>();
        
            if (timeManager != null && progressionManager != null)
            {
                saveManager.SaveGame(this, timeManager, progressionManager);
                Debug.Log("💾 Sauvegarde automatique à la fermeture");
            }
        }
    }

// Sauvegarde aussi quand l'application perd le focus (mobile)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Le jeu est mis en pause (on quitte l'app sur mobile)
        {
            SaveManager saveManager = SaveManager.Instance;
            if (saveManager != null)
            {
                TimeManager timeManager = FindObjectOfType<TimeManager>();
            
                if (timeManager != null && progressionManager != null)
                {
                    saveManager.SaveGame(this, timeManager, progressionManager);
                    Debug.Log("💾 Sauvegarde automatique (pause)");
                }
            }
        }
    }
}