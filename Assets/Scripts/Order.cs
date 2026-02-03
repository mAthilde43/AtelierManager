using UnityEngine;
using System.Collections.Generic;

// Classe pour représenter un produit demandé dans une commande
[System.Serializable]
public class OrderRequirement
{
    public int productIndex;  // Index du produit dans la liste du GameManager
    public int quantity;      // Quantité demandée
    
    public OrderRequirement(int index, int qty)
    {
        productIndex = index;
        quantity = qty;
    }
}

// Classe principale pour une commande client
[System.Serializable]
public class Order
{
    public string orderID;                              // ID unique de la commande
    public string clientName;                           // Nom du client
    public List<OrderRequirement> requirements;         // Liste des produits demandés
    public int reward;                                  // Récompense en argent
    public float timeLimit;                             // Temps limite en secondes
    public float timeRemaining;                         // Temps restant
    public bool isCompleted;                            // Commande complétée ?
    public bool isFailed;                               // Commande ratée ?
    
    // Constructeur
    public Order(string id, string client, int rewardAmount, float timeLimitSeconds)
    {
        orderID = id;
        clientName = client;
        reward = rewardAmount;
        timeLimit = timeLimitSeconds;
        timeRemaining = timeLimitSeconds;
        isCompleted = false;
        isFailed = false;
        requirements = new List<OrderRequirement>();
    }
    
    // Ajoute un produit à la commande
    public void AddRequirement(int productIndex, int quantity)
    {
        requirements.Add(new OrderRequirement(productIndex, quantity));
    }
    
    // Met à jour le timer (appelé chaque frame)
    public void UpdateTimer(float deltaTime)
    {
        if (isCompleted || isFailed) return;
        
        timeRemaining -= deltaTime;
        
        // Si le temps est écoulé
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isFailed = true;
            Debug.Log("❌ Commande échouée : " + orderID + " (temps écoulé)");
        }
    }
    
    // Vérifie si la commande peut être complétée
    public bool CanBeCompleted(GameManager gameManager)
    {
        if (gameManager == null) return false;
        
        // Vérifie que tous les produits demandés sont en stock
        foreach (OrderRequirement req in requirements)
        {
            Product prod = gameManager.GetProduct(req.productIndex);
            if (prod == null || !prod.HasEnoughQuantity(req.quantity))
            {
                return false;
            }
        }
        
        return true;
    }
    
    // Complète la commande (retire les produits du stock)
    public void Complete(GameManager gameManager)
    {
        if (gameManager == null) return;
        
        // Retire tous les produits du stock
        foreach (OrderRequirement req in requirements)
        {
            Product prod = gameManager.GetProduct(req.productIndex);
            if (prod != null)
            {
                prod.RemoveQuantity(req.quantity);
            }
        }
        
        // Ajoute la récompense
        gameManager.AddMoney(reward);
        
        // Marque comme complétée
        isCompleted = true;
        
        Debug.Log("✅ Commande complétée : " + orderID + " - Récompense : " + reward + "€");
    }
    
    // Obtient le temps restant formaté (MM:SS)
    public string GetFormattedTimeRemaining()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    // Obtient le pourcentage de temps restant (0-1)
    public float GetTimeProgress()
    {
        return Mathf.Clamp01(timeRemaining / timeLimit);
    }
}
