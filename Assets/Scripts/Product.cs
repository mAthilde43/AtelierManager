using UnityEngine;
using System.Collections.Generic;

// Classe pour représenter un produit fini
[System.Serializable]
public class Product
{
    public string productName;           // Nom du produit (ex: "Table en chêne")
    public int sellPrice;                // Prix de vente
    public int quantity;                 // Quantité en stock (produits finis)
    public int productionTime;           // Temps de production en secondes
    public Sprite icon;                  // Icône visuelle
    
    // Recette : liste des matériaux nécessaires
    public List<MaterialRequirement> recipe = new List<MaterialRequirement>();
    
    public bool isUnlocked;              // Produit débloqué ?
    public int unlockLevel;              // Niveau requis (0 = débloqué dès le début)
    public string unlockConditionText;
    
    // Constructeur
    public Product(string name, int price, int prodTime)
    {
        productName = name;
        sellPrice = price;
        quantity = 0;
        productionTime = prodTime;
        icon = null;
        recipe = new List<MaterialRequirement>();
        
        isUnlocked = false;
        unlockLevel = 0;
        unlockConditionText = "";
    
    }
    
    // Ajoute un matériau requis à la recette
    public void AddMaterialRequirement(int materialIndex, int amount)
    {
        recipe.Add(new MaterialRequirement(materialIndex, amount));
    }
    
    // Ajoute du stock de produits finis
    public void AddQuantity(int amount)
    {
        quantity += amount;
        Debug.Log("📦 +" + amount + " " + productName + " produit(s) | Stock total: " + quantity);
    }
    
    // Retire du stock de produits finis (lors de la vente)
    public bool RemoveQuantity(int amount)
    {
        if (quantity >= amount)
        {
            quantity -= amount;
            Debug.Log("📤 -" + amount + " " + productName + " vendu(s) | Stock restant: " + quantity);
            return true;
        }
        else
        {
            Debug.LogWarning("⚠️ Stock insuffisant de " + productName);
            return false;
        }
    }
    
    // Vérifie si on a assez de stock
    public bool HasEnoughQuantity(int amount)
    {
        return quantity >= amount;
    }
}

// Classe pour représenter un matériau requis dans une recette
[System.Serializable]
public class MaterialRequirement
{
    public int materialIndex;  // Index du matériau dans la liste du GameManager
    public int amount;         // Quantité nécessaire
    
    public MaterialRequirement(int index, int qty)
    {
        materialIndex = index;
        amount = qty;
    }
}