using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    // Singleton
    private static NotificationManager instance;
    public static NotificationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NotificationManager>();
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
    
    // === RÉFÉRENCES AUX BADGES ===
    public NotificationBadge workshopBadge;    // Badge Atelier
    public NotificationBadge salesBadge;       // Badge Vente
    public NotificationBadge upgradesBadge;    // Badge Améliorations
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Cache tous les badges au départ
        HideAllBadges();
        
        Debug.Log("🔔 NotificationManager initialisé");
    }
    
    // Cache tous les badges
    public void HideAllBadges()
    {
        if (workshopBadge != null) workshopBadge.Hide();
        if (salesBadge != null) salesBadge.Hide();
        if (upgradesBadge != null) upgradesBadge.Hide();
    }
    
    // Vérifie et met à jour tous les badges
    public void UpdateAllNotifications()
    {
        UpdateWorkshopNotification();
        UpdateSalesNotification();
        UpdateUpgradesNotification();
    }
    
    // === BADGE ATELIER (quand des matériaux sont disponibles) ===
    void UpdateWorkshopNotification()
    {
        if (workshopBadge == null || gameManager == null) return;
        
        // Compte combien de produits peuvent être fabriqués
        int craftableProducts = 0;
        
        foreach (Product prod in gameManager.products)
        {
            // Vérifie si on a tous les matériaux pour ce produit
            bool canCraft = true;
            foreach (MaterialRequirement req in prod.recipe)
            {
                CraftingMaterial mat = gameManager.GetMaterial(req.materialIndex);
                if (mat == null || !mat.HasEnoughQuantity(req.amount))
                {
                    canCraft = false;
                    break;
                }
            }
            
            if (canCraft)
            {
                craftableProducts++;
            }
        }
        
        // Affiche le badge si on peut fabriquer au moins 1 produit
        if (craftableProducts > 0)
        {
            workshopBadge.Show(craftableProducts);
        }
        else
        {
            workshopBadge.Hide();
        }
    }
    
    // === BADGE VENTE (quand des produits sont en stock) ===
    void UpdateSalesNotification()
    {
        if (salesBadge == null || gameManager == null) return;
        
        // Compte combien de produits sont disponibles à la vente
        int productsToSell = 0;
        
        foreach (Product prod in gameManager.products)
        {
            productsToSell += prod.quantity;
        }
        
        // Affiche le badge si au moins 1 produit en stock
        if (productsToSell > 0)
        {
            salesBadge.Show(productsToSell);
        }
        else
        {
            salesBadge.Hide();
        }
    }
    
    // === BADGE AMÉLIORATIONS (quand on peut acheter une amélioration) ===
    void UpdateUpgradesNotification()
    {
        if (upgradesBadge == null || gameManager == null) return;
        
        // Compte combien d'améliorations on peut acheter
        int affordableUpgrades = 0;
        
        foreach (Upgrade upg in gameManager.upgrades)
        {
            // Si pas encore acheté ET on a assez d'argent
            if (!upg.isPurchased && gameManager.HasEnoughMoney(upg.cost))
            {
                affordableUpgrades++;
            }
        }
        
        // Affiche le badge si au moins 1 amélioration disponible
        if (affordableUpgrades > 0)
        {
            upgradesBadge.Show(affordableUpgrades);
        }
        else
        {
            upgradesBadge.Hide();
        }
    }
}