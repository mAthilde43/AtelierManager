using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    // Singleton
    private static OrderManager instance;
    public static OrderManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<OrderManager>();  
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
        DontDestroyOnLoad(gameObject);
    }
    
    // === COMMANDES ===
    public List<Order> activeOrders = new List<Order>();
    public int maxActiveOrders = 3;
    
    // === GÉNÉRATION ===
    public float orderSpawnInterval = 30f;
    private float orderSpawnTimer = 0f;
    
    // === NOMS DE CLIENTS ===
    private string[] clientNames = {
        "M. Dupont", "Mme Martin", "M. Bernard", "Mme Dubois",
        "M. Thomas", "Mme Petit", "M. Robert", "Mme Richard",
        "M. Durand", "Mme Moreau", "M. Simon", "Mme Laurent"
    };
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private int orderCounter = 0;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); 
        activeOrders.Clear();
        Debug.Log("✅ OrderManager initialisé");
        
        GenerateRandomOrder();
    }
    
    void Update()
    {
        UpdateActiveOrders();
        HandleOrderGeneration();
    }
    
    void UpdateActiveOrders()
    {
        List<Order> ordersToRemove = new List<Order>();
        
        foreach (Order order in activeOrders)
        {
            if (order.isCompleted || order.isFailed)
            {
                ordersToRemove.Add(order);
                continue;
            }
            
            order.UpdateTimer(Time.deltaTime);
            
            if (order.isFailed)
            {
                OnOrderFailed(order);
            }
        }
        
        foreach (Order order in ordersToRemove)
        {
            activeOrders.Remove(order);
        }
    }
    
    void HandleOrderGeneration()
    {
        // Compte seulement les commandes actives (pas complétées/échouées)
        int activeCount = 0;
        foreach (Order order in activeOrders)
        {
            if (!order.isCompleted && !order.isFailed)
            {
                activeCount++;
            }
        }
        
        if (activeCount >= maxActiveOrders)
        {
            orderSpawnTimer = 0f;
            return;
        }
        
        orderSpawnTimer += Time.deltaTime;
        
        if (orderSpawnTimer >= orderSpawnInterval)
        {
            orderSpawnTimer = 0f;
            GenerateRandomOrder();
        }
    }
    
    void GenerateRandomOrder()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();  
            if (gameManager == null) return;
        }
        
        orderCounter++;
        string orderID = "CMD_" + orderCounter.ToString("D3");
        string clientName = clientNames[Random.Range(0, clientNames.Length)];
        
        ProgressionManager pm = FindFirstObjectByType<ProgressionManager>();  
        int playerLevel = pm != null ? pm.currentLevel : 1;
        
        // ===== TEMPS AUGMENTÉ =====
        float timeLimit = 600f + (playerLevel * 60f);  // 10 min de base + 1 min par niveau
        // ==========================
        
        int baseReward = 200 + (playerLevel * 50);
        
        Order newOrder = new Order(orderID, clientName, baseReward, timeLimit);
        
        // ===== FILTRE LES PRODUITS DÉBLOQUÉS =====
        List<int> availableProducts = new List<int>();

        for (int i = 0; i < gameManager.products.Count; i++)
        {
            Product prod = gameManager.products[i];
    
            // Ajoute seulement les produits débloqués
            if (prod.isUnlocked)
            {
                availableProducts.Add(i);
            }
        }

        // Si aucun produit débloqué, annule la génération
        if (availableProducts.Count == 0)
        {
            Debug.LogWarning("⚠️ Aucun produit débloqué pour générer une commande !");
            return;
        }

        Debug.Log("📦 " + availableProducts.Count + " produits débloqués disponibles");
        // ==========================================
        
        // Nombre de produits dans la commande (limité par les produits disponibles)
        int numProducts = Mathf.Min(
            1 + (playerLevel / 3),    // Augmente avec le niveau
            4,                         // Max 4 produits différents
            availableProducts.Count    // Limité par les produits disponibles
        );

        for (int i = 0; i < numProducts; i++)
        {
            // ===== CHOISIT PARMI LES PRODUITS DÉBLOQUÉS =====
            if (availableProducts.Count == 0)
            {
                Debug.LogWarning("⚠️ Plus de produits disponibles pour cette commande");
                break;
            }
    
            int randomIndex = Random.Range(0, availableProducts.Count);
            int productIndex = availableProducts[randomIndex];
    
            // Retire de la liste pour éviter les doublons
            availableProducts.RemoveAt(randomIndex);
            // ================================================
            
            // Quantité demandée
            int quantity = Random.Range(1, Mathf.Min(2 + (playerLevel / 5), 4));
            
            newOrder.AddRequirement(productIndex, quantity);
            
            // Augmente la récompense
            Product prod = gameManager.GetProduct(productIndex);
            if (prod != null)
            {
                newOrder.reward += prod.sellPrice * quantity / 2;
            }
        }
        
        activeOrders.Add(newOrder);
        
        Debug.Log("📦 Nouvelle commande : " + orderID + " de " + clientName);
        Debug.Log("   💰 Récompense : " + newOrder.reward + "€");
        Debug.Log("   ⏰ Temps limite : " + newOrder.GetFormattedTimeRemaining());
        Debug.Log("   📋 Produits demandés : " + newOrder.requirements.Count);
        
        RefreshOrdersUI();
    }
    
    public void CompleteOrder(Order order)
    {
        if (order == null) return;
        
        if (!order.CanBeCompleted(gameManager))
        {
            Debug.LogWarning("⚠️ Impossible de compléter la commande : stock insuffisant");
            return;
        }
        
        order.Complete(gameManager);
        
        ProgressionManager pm = FindFirstObjectByType<ProgressionManager>();
        if (pm != null)
        {
            int xpBonus = order.reward / 2;
            pm.AddExperience(xpBonus);
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnOrderCompleted(1);
        }
        
        RefreshOrdersUI();
        if (gameManager != null)
        {
            gameManager.RefreshAllUI();
        }
    }
    
    void OnOrderFailed(Order order)
    {
        Debug.Log("❌ Commande échouée : " + order.orderID + " de " + order.clientName);
        RefreshOrdersUI();
    }
    
    void RefreshOrdersUI()
    {
        OrdersUI ordersUI = FindFirstObjectByType<OrdersUI>();
        if (ordersUI != null)
        {
            ordersUI.RefreshOrdersDisplay();
        }
    }
    
    public Order GetOrder(string orderID)
    {
        foreach (Order order in activeOrders)
        {
            if (order.orderID == orderID)
            {
                return order;
            }
        }
        return null;
    }
}
