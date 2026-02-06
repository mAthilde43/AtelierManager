using UnityEngine;

// Classe pour représenter une matière première
[System.Serializable]
public class CraftingMaterial
{
    public string materialName;      // Nom du matériau (ex: "Bois de chêne")
    public int price;                // Prix d'achat unitaire
    public int quantity;             // Quantité en stock
    public Sprite icon;              // Icône visuelle (pour plus tard)
    
    // Constructeur : pour créer un nouveau matériau facilement
    public CraftingMaterial(string name, int buyPrice)
    {
        materialName = name;
        price = buyPrice;
        quantity = 0;
        icon = null;
    }
    
    // Fonction pour ajouter du stock
    public void AddQuantity(int amount)
    {
        quantity += amount;
        Debug.Log("+" + amount + " " + materialName + " | Stock total: " + quantity);
    }
    
    // Fonction pour retirer du stock
    public bool RemoveQuantity(int amount)
    {
        if (quantity >= amount)
        {
            quantity -= amount;
            Debug.Log("-" + amount + " " + materialName + " | Stock restant: " + quantity);
            return true;
        }
        else
        {
            Debug.LogWarning("Stock insuffisant de " + materialName);
            return false;
        }
    }
    
    // Fonction pour vérifier si on a assez de stock
    public bool HasEnoughQuantity(int amount)
    {
        return quantity >= amount;
    }
}