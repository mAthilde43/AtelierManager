using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // === ARGENT ===
    public int playerMoney = 1000;
    public TextMeshProUGUI moneyText;
    
    // === MATÉRIAUX ===
    public List<CraftingMaterial> craftingMaterials = new List<CraftingMaterial>();
    
    // === PRODUITS ===
    public List<Product> products = new List<Product>();
    
    // === UI BOUTIQUE ===
    public MaterialItemUI woodItemUI;
    public MaterialItemUI pineItemUI;
    public MaterialItemUI varnishItemUI;
    public MaterialItemUI metalItemUI;
    public MaterialItemUI fabricItemUI;
    public MaterialItemUI leatherItemUI;
    
    // === UI ATELIER ===
    public ProductItemUI tableItemUI;
    public ProductItemUI chairItemUI;
    public ProductItemUI shelfItemUI;
    public ProductItemUI lampItemUI;
    public ProductItemUI armchairItemUI;
    public ProductItemUI deskItemUI;
    public ProductItemUI sofaItemUI;
    public ProductItemUI wardrobeItemUI;
    
    // === UI VENTE ===
    public SaleItemUI saleTableItemUI;
    public SaleItemUI saleChairItemUI;
    public SaleItemUI saleShelfItemUI;
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
        Debug.Log("Atelier Manager démarré avec succès !");

        InitializeMaterials();
        InitializeProducts();
        InitializeUpgrades();
        UpdateMoneyDisplay();
        InitializeUI();
    
        progressionManager = FindObjectOfType<ProgressionManager>();
    
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            TimeManager timeManager = FindObjectOfType<TimeManager>();
        
            if (saveManager.HasSaveData())
{
    saveManager.LoadGame(this, timeManager, progressionManager);
    Debug.Log("Partie chargée !");
}
else
{
    saveManager.ResetGame();
    Debug.Log("Nouvelle partie - Données réinitialisées");
}

        }
    }
    
    void InitializeMaterials()
    {
        craftingMaterials.Add(new CraftingMaterial("Bois de chêne", 50));
        craftingMaterials.Add(new CraftingMaterial("Bois de pin", 30));
        craftingMaterials.Add(new CraftingMaterial("Vernis", 20));
        craftingMaterials.Add(new CraftingMaterial("Métal", 80));
        craftingMaterials.Add(new CraftingMaterial("Tissu", 40));
        craftingMaterials.Add(new CraftingMaterial("Cuir", 60));
        
        Debug.Log("" + craftingMaterials.Count + " types de matériaux initialisés");
    }
    
    void InitializeProducts()
    {
        Product tableChene = new Product("Table en chêne", 200, 5);
        tableChene.AddMaterialRequirement(0, 3);
        tableChene.AddMaterialRequirement(2, 1);
        tableChene.isUnlocked = true;
        tableChene.unlockLevel = 0;
        tableChene.unlockConditionText = "Disponible dès le début";
        products.Add(tableChene);

        Product chaisePine = new Product("Chaise en pin", 80, 3);
        chaisePine.AddMaterialRequirement(1, 2);
        chaisePine.AddMaterialRequirement(2, 1);
        chaisePine.isUnlocked = true;
        chaisePine.unlockLevel = 0;
        chaisePine.unlockConditionText = "Disponible dès le début";
        products.Add(chaisePine);

        Product etagere = new Product("Étagère mixte", 150, 4);
        etagere.AddMaterialRequirement(0, 2);
        etagere.AddMaterialRequirement(1, 2);
        etagere.AddMaterialRequirement(2, 2);
        etagere.isUnlocked = true;
        etagere.unlockLevel = 0;
        etagere.unlockConditionText = "Disponible dès le début";
        products.Add(etagere);
        
        Product lamp = new Product("Lampe", 180, 25);
        lamp.AddMaterialRequirement(3, 1);
        lamp.AddMaterialRequirement(4, 1);
        lamp.isUnlocked = false;
        lamp.unlockLevel = 7;
        lamp.unlockConditionText = "Niveau 3 requis";
        products.Add(lamp);

        Product armchair = new Product("Fauteuil", 280, 35);
        armchair.AddMaterialRequirement(1, 2);
        armchair.AddMaterialRequirement(4, 2);
        armchair.AddMaterialRequirement(5, 1);
        armchair.isUnlocked = false;
        armchair.unlockLevel = 10;
        armchair.unlockConditionText = "Niveau 5 requis";
        products.Add(armchair);

        Product desk = new Product("Bureau", 450, 40);
        desk.AddMaterialRequirement(0, 4);
        desk.AddMaterialRequirement(3, 2);
        desk.AddMaterialRequirement(2, 1);
        desk.isUnlocked = false;
        desk.unlockLevel = 15;
        desk.unlockConditionText = "Niveau 7 requis";
        products.Add(desk);

        Product sofa = new Product("Canapé", 520, 50);
        sofa.AddMaterialRequirement(1, 3);
        sofa.AddMaterialRequirement(4, 4);
        sofa.AddMaterialRequirement(5, 2);
        sofa.isUnlocked = false;
        sofa.unlockLevel = 20;
        sofa.unlockConditionText = "Niveau 10 requis";
        products.Add(sofa);
        
        Product wardrobe = new Product("Armoire", 650, 60);
        wardrobe.AddMaterialRequirement(0, 5);
        wardrobe.AddMaterialRequirement(3, 3);
        wardrobe.AddMaterialRequirement(2, 2);
        wardrobe.isUnlocked = false;
        wardrobe.unlockLevel = 25;
        wardrobe.unlockConditionText = "Niveau 15 requis";
        products.Add(wardrobe);

        Debug.Log("" + products.Count + " types de produits initialisés");
    }

    void InitializeUpgrades()
    {
        upgrades.Add(new Upgrade(
            "Fournisseur de confiance",
            "Réduit le coût des matériaux de 10%",
            300,
            UpgradeType.MaterialDiscount,
            10
        ));
    
        upgrades.Add(new Upgrade(
            "Meilleure réputation",
            "Augmente le prix de vente de 15%",
            500,
            UpgradeType.SalesBonus,
            15
        ));
    
        upgrades.Add(new Upgrade(
            "Outils perfectionnés",
            "Production plus rapide",
            400,
            UpgradeType.ProductionSpeed,
            20
        ));
    
        upgrades.Add(new Upgrade(
            "Contrat régulier",
            "Augmente les revenus quotidiens de 30€",
            600,
            UpgradeType.DailyIncomeBoost,
            30
        ));
    
        upgrades.Add(new Upgrade(
            "Local optimisé",
            "Réduit les charges hebdomadaires de 50€",
            800,
            UpgradeType.WeeklyCostReduction,
            50
        ));
    
        Debug.Log("" + upgrades.Count + " améliorations disponibles");
    }
    
    public void UpdateMoneyDisplay()
{
    if (moneyText != null)
    {
        moneyText.text = FormatMoney(playerMoney) + " €";
    }
    else
    {
        Debug.LogWarning("MoneyText n'est pas assigné !");
    }
}

string FormatMoney(int amount)
{
    if (amount >= 1000000)
    {
        return (amount / 1000000f).ToString("F1") + "M";
    }
    else if (amount >= 100000)
    {
        return (amount / 1000f).ToString("F0") + "K";
    }
    else if (amount >= 10000)
    {
        return (amount / 1000f).ToString("F1") + "K";
    }
    else
    {
        return amount.ToString();
    }
}

    
    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyDisplay();
        Debug.Log("+" + amount + "€ | Total: " + playerMoney + "€");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMoneyGain();
        }
        
        if (FeedbackManager.Instance != null && moneyText != null)
        {
            Vector3 position = moneyText.transform.position;
            FeedbackManager.Instance.ShowMoneyGain(amount, position);
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnMoneyEarned(amount);
        }
        
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnMoneyEarned(amount);
        }
		
		if (NotificationManager.Instance != null)
    	{
    	    NotificationManager.Instance.UpdateAllNotifications();
    	}
    }
    
    public void RemoveMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyDisplay();
        Debug.Log("-" + amount + "€ | Total: " + playerMoney + "€");
        
        if (FeedbackManager.Instance != null && moneyText != null)
        {
            Vector3 position = moneyText.transform.position;
            FeedbackManager.Instance.ShowMoneyLoss(amount, position);
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnMoneySpent(amount);
        }
        
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnMoneySpent(amount);
        }
   		 if (NotificationManager.Instance != null)
   		 {
    	    NotificationManager.Instance.UpdateAllNotifications();
    	 }
    }
    
    public bool HasEnoughMoney(int amount)
    {
        return playerMoney >= amount;
    }
    
    public void BuyMaterial(int materialIndex, int quantity)
    {
        if (materialIndex < 0 || materialIndex >= craftingMaterials.Count)
        {
            Debug.LogError("Index de matériau invalide !");
            return;
        }
        
        CraftingMaterial mat = craftingMaterials[materialIndex];
        
        // ===== APPLIQUE LA RÉDUCTION DES MATÉRIAUX =====
        float baseCost = mat.price * quantity;
        
        // Bonus Building (réduction matériaux)
        if (BuildingManager.Instance != null)
        {
            baseCost *= BuildingManager.Instance.GetMaterialDiscountMultiplier();
        }
        
        int totalCost = Mathf.RoundToInt(baseCost);
        // ===============================================
        
        if (HasEnoughMoney(totalCost))
        {
            RemoveMoney(totalCost);
            mat.AddQuantity(quantity);
            
            Debug.Log("Achat : " + quantity + "x " + mat.materialName + " pour " + totalCost + "€");
            
            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.OnMaterialBought();
            }
            
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnMaterialBought(quantity);
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPurchase();
            }
            
            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.UpdateAllNotifications();
            }
        }
        else
        {
            Debug.LogWarning("Pas assez d'argent ! Manque " + (totalCost - playerMoney) + "€");
        }

		RefreshAllUI();
    }
    
    public CraftingMaterial GetMaterial(int index)
    {
        if (index >= 0 && index < craftingMaterials.Count)
        {
            return craftingMaterials[index];
        }
        return null;
    }
    
    void InitializeUI()
    {
        if (woodItemUI != null)
            woodItemUI.Setup(0, craftingMaterials[0], this);
        if (pineItemUI != null)
            pineItemUI.Setup(1, craftingMaterials[1], this);
        if (varnishItemUI != null)
            varnishItemUI.Setup(2, craftingMaterials[2], this);
        if (metalItemUI != null)
            metalItemUI.Setup(3, craftingMaterials[3], this);
        if (fabricItemUI != null)
            fabricItemUI.Setup(4, craftingMaterials[4], this);
        if (leatherItemUI != null)
            leatherItemUI.Setup(5, craftingMaterials[5], this);
    
        if (tableItemUI != null)
            tableItemUI.Setup(0, products[0], this);
        if (chairItemUI != null)
            chairItemUI.Setup(1, products[1], this);
        if (shelfItemUI != null)
            shelfItemUI.Setup(2, products[2], this);
        if (lampItemUI != null)
            lampItemUI.Setup(3, products[3], this);
        if (armchairItemUI != null)
            armchairItemUI.Setup(4, products[4], this);
        if (deskItemUI != null)
            deskItemUI.Setup(5, products[5], this);
        if (sofaItemUI != null)
            sofaItemUI.Setup(6, products[6], this);
        if (wardrobeItemUI != null)
            wardrobeItemUI.Setup(7, products[7], this);
    
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
    
    public void CraftProduct(int productIndex)
    {
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Debug.LogError("Index de produit invalide !");
            return;
        }
    
        Product prod = products[productIndex];
    
        bool hasAllMaterials = true;
        foreach (MaterialRequirement req in prod.recipe)
        {
            CraftingMaterial mat = GetMaterial(req.materialIndex);
            if (mat == null || !mat.HasEnoughQuantity(req.amount))
            {
                hasAllMaterials = false;
                if (mat != null)
                {
                    Debug.LogWarning("Pas assez de " + mat.materialName);
                }
                break;
            }
        }
    
        if (hasAllMaterials)
        {
            foreach (MaterialRequirement req in prod.recipe)
            {
                CraftingMaterial mat = GetMaterial(req.materialIndex);
                mat.RemoveQuantity(req.amount);
            }
        
            prod.AddQuantity(1);
        
            Debug.Log("Production : 1x " + prod.productName);

            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.OnProductCrafted();
            }

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnProductCrafted(1);
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCraft();
            }
            
            if (progressionManager != null)
            {
                progressionManager.OnProductCrafted(prod.sellPrice);
            }
        }
        else
        {
            Debug.LogWarning("Matériaux insuffisants pour " + prod.productName);
        }
    
        RefreshAllUI();
    }
    
    public void SellProduct(int productIndex, int quantity)
    {
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Debug.LogError("Index de produit invalide !");
            return;
        }
    
        Product prod = products[productIndex];
    
        if (prod.HasEnoughQuantity(quantity))
        {
            // ===== APPLIQUE TOUS LES BONUS DE VENTE =====
            float baseSellPrice = prod.sellPrice * quantity;
            
            // Bonus Building (prix de vente)
            if (BuildingManager.Instance != null)
            {
                baseSellPrice *= BuildingManager.Instance.GetSalesBonusMultiplier();
            }
            
            // Bonus Combo
            if (ComboManager.Instance != null)
            {
                baseSellPrice *= ComboManager.Instance.GetComboMultiplier();
            }
            
            // Bonus Booster
            if (BoosterManager.Instance != null)
            {
                baseSellPrice *= BoosterManager.Instance.GetMoneyMultiplier();
            }
            
            int earnings = Mathf.RoundToInt(baseSellPrice);
            // ===========================================
        
            prod.RemoveQuantity(quantity);
            AddMoney(earnings);
        
            Debug.Log("Vente : " + quantity + "x " + prod.productName + " = " + earnings + "€");

            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.OnProductSold();
            }

            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.OnProductSold(prod.sellPrice);
            }

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnProductSold(quantity);
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySell();
            }
            
            if (progressionManager != null)
            {
                progressionManager.OnProductSold(earnings);
            }
        
            RefreshAllUI();
        }
        else
        {
            Debug.LogWarning("Stock insuffisant de " + prod.productName);
        }
    }
    
    public void BuyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
        {
            Debug.LogError("Index d'amélioration invalide !");
            return;
        }
        
        Upgrade upg = upgrades[upgradeIndex];
        
        if (upg.isPurchased)
        {
            Debug.LogWarning("Déjà acheté : " + upg.upgradeName);
            return;
        }
        
        if (HasEnoughMoney(upg.cost))
        {
            RemoveMoney(upg.cost);
            upg.Purchase();
            ApplyUpgradeEffect(upg);
            
            Debug.Log("Amélioration : " + upg.upgradeName);
            
            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.OnUpgradeBought();
            }

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.OnUpgradeBought(1);
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccess();
            }
            
            if (progressionManager != null)
            {
                progressionManager.OnUpgradeBought();
            }
            
            RefreshAllUI();
        }
        else
        {
            Debug.LogWarning("Pas assez d'argent !");
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    }

    void ApplyUpgradeEffect(Upgrade upg)
    {
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        
        switch (upg.type)
        {
            case UpgradeType.MaterialDiscount:
                foreach (CraftingMaterial mat in craftingMaterials)
                {
                    int reduction = Mathf.RoundToInt(mat.price * upg.value / 100f);
                    mat.price -= reduction;
                    if (mat.price < 1) mat.price = 1;
                }
                Debug.Log("Prix matériaux réduits de " + upg.value + "%");
                break;
                
            case UpgradeType.SalesBonus:
                foreach (Product prod in products)
                {
                    int bonus = Mathf.RoundToInt(prod.sellPrice * upg.value / 100f);
                    prod.sellPrice += bonus;
                }
                Debug.Log("Prix vente augmentés de " + upg.value + "%");
                break;
                
            case UpgradeType.ProductionSpeed:
                Debug.Log("⚙Vitesse production augmentée");
                break;
                
            case UpgradeType.DailyIncomeBoost:
                if (timeManager != null)
                {
                    timeManager.dailyIncome += upg.value;
                    Debug.Log("Revenus quotidiens +" + upg.value + "€");
                }
                break;
                
            case UpgradeType.WeeklyCostReduction:
                if (timeManager != null)
                {
                    timeManager.weeklyCost -= upg.value;
                    if (timeManager.weeklyCost < 0) timeManager.weeklyCost = 0;
                    Debug.Log("Charges réduites de " + upg.value + "€");
                }
                break;
        }
    }

    public void ReapplyUpgradeEffect(Upgrade upg)
    {
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        
        switch (upg.type)
        {
            case UpgradeType.MaterialDiscount:
                foreach (CraftingMaterial mat in craftingMaterials)
                {
                    int reduction = Mathf.RoundToInt(mat.price * upg.value / 100f);
                    mat.price -= reduction;
                    if (mat.price < 1) mat.price = 1;
                }
                break;
                
            case UpgradeType.SalesBonus:
                foreach (Product prod in products)
                {
                    int bonus = Mathf.RoundToInt(prod.sellPrice * upg.value / 100f);
                    prod.sellPrice += bonus;
                }
                break;
                
            case UpgradeType.DailyIncomeBoost:
                if (timeManager != null)
                {
                    timeManager.dailyIncome += upg.value;
                }
                break;
                
            case UpgradeType.WeeklyCostReduction:
                if (timeManager != null)
                {
                    timeManager.weeklyCost -= upg.value;
                    if (timeManager.weeklyCost < 0) timeManager.weeklyCost = 0;
                }
                break;
        }
    }

    public Upgrade GetUpgrade(int index)
    {
        if (index >= 0 && index < upgrades.Count)
        {
            return upgrades[index];
        }
        return null;
    }

    public Product GetProduct(int index)
    {
        if (index >= 0 && index < products.Count)
        {
            return products[index];
        }
        return null;
    }
    
    public void RefreshAllUI()
    {
        if (woodItemUI != null && craftingMaterials.Count > 0)
            woodItemUI.UpdateDisplay(craftingMaterials[0]);
        if (pineItemUI != null && craftingMaterials.Count > 1)
            pineItemUI.UpdateDisplay(craftingMaterials[1]);
        if (varnishItemUI != null && craftingMaterials.Count > 2)
            varnishItemUI.UpdateDisplay(craftingMaterials[2]);
        if (metalItemUI != null && craftingMaterials.Count > 3)
            metalItemUI.UpdateDisplay(craftingMaterials[3]);
        if (fabricItemUI != null && craftingMaterials.Count > 4)
            fabricItemUI.UpdateDisplay(craftingMaterials[4]);
        if (leatherItemUI != null && craftingMaterials.Count > 5)
            leatherItemUI.UpdateDisplay(craftingMaterials[5]);

        if (tableItemUI != null && products.Count > 0)
            tableItemUI.UpdateDisplay(products[0]);
        if (chairItemUI != null && products.Count > 1)
            chairItemUI.UpdateDisplay(products[1]);
        if (shelfItemUI != null && products.Count > 2)
            shelfItemUI.UpdateDisplay(products[2]);
        if (lampItemUI != null && products.Count > 3)
            lampItemUI.UpdateDisplay(products[3]);
        if (armchairItemUI != null && products.Count > 4)
            armchairItemUI.UpdateDisplay(products[4]);
        if (deskItemUI != null && products.Count > 5)
            deskItemUI.UpdateDisplay(products[5]);
        if (sofaItemUI != null && products.Count > 6)
            sofaItemUI.UpdateDisplay(products[6]);
        if (wardrobeItemUI != null && products.Count > 7)
            wardrobeItemUI.UpdateDisplay(products[7]);

        if (saleTableItemUI != null && products.Count > 0)
            saleTableItemUI.UpdateDisplay(products[0]);
        if (saleChairItemUI != null && products.Count > 1)
            saleChairItemUI.UpdateDisplay(products[1]);
        if (saleShelfItemUI != null && products.Count > 2)
            saleShelfItemUI.UpdateDisplay(products[2]);
        if (saleLampItemUI != null && products.Count > 3)
            saleLampItemUI.UpdateDisplay(products[3]);
        if (saleArmchairItemUI != null && products.Count > 4)
            saleArmchairItemUI.UpdateDisplay(products[4]);
        if (saleDeskItemUI != null && products.Count > 5)
            saleDeskItemUI.UpdateDisplay(products[5]);
        if (saleSofaItemUI != null && products.Count > 6)
            saleSofaItemUI.UpdateDisplay(products[6]);
        if (saleWardrobefItemUI != null && products.Count > 7)
            saleWardrobefItemUI.UpdateDisplay(products[7]);

        if (upgrade1ItemUI != null && upgrades.Count > 0)
            upgrade1ItemUI.UpdateDisplay(upgrades[0]);
        if (upgrade2ItemUI != null && upgrades.Count > 1)
            upgrade2ItemUI.UpdateDisplay(upgrades[1]);
        if (upgrade3ItemUI != null && upgrades.Count > 2)
            upgrade3ItemUI.UpdateDisplay(upgrades[2]);
        if (upgrade4ItemUI != null && upgrades.Count > 3)
            upgrade4ItemUI.UpdateDisplay(upgrades[3]);
        if (upgrade5ItemUI != null && upgrades.Count > 4)
            upgrade5ItemUI.UpdateDisplay(upgrades[4]);

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.UpdateAllNotifications();
        }
    }
    
    void OnApplicationQuit()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            TimeManager timeManager = FindObjectOfType<TimeManager>();
        
            if (timeManager != null && progressionManager != null)
            {
                saveManager.SaveGame(this, timeManager, progressionManager);
                Debug.Log("Sauvegarde auto fermeture");
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveManager saveManager = SaveManager.Instance;
            if (saveManager != null)
            {
                TimeManager timeManager = FindObjectOfType<TimeManager>();
            
                if (timeManager != null && progressionManager != null)
                {
                    saveManager.SaveGame(this, timeManager, progressionManager);
                    Debug.Log("Sauvegarde auto pause");
                }
            }
        }
    }
}
