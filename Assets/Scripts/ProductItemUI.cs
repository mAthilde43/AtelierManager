using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductItemUI : MonoBehaviour
{
    // Références vers les éléments UI
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI recipeText;
    public TextMeshProUGUI sellPriceText;
    public TextMeshProUGUI productStockText;
    public Button craftButton;
    
    // Données
    private int productIndex;
    private GameManager gameManager;
    
    // Fonction appelée pour initialiser cet élément
    public void Setup(int index, Product product, GameManager gm)
    {
        productIndex = index;
        gameManager = gm;
        
        // Affiche les informations
        UpdateDisplay(product);
        
        // Configure le bouton
        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => OnCraftButtonClicked());
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Product product)
    {
        productNameText.text = product.productName;
        sellPriceText.text = "Vente: " + product.sellPrice + "€";
        productStockText.text = "Stock: " + product.quantity;
        
        // Construit le texte de la recette
        string recipeString = "Recette:\n";
        foreach (MaterialRequirement req in product.recipe)
        {
            CraftingMaterial mat = gameManager.GetMaterial(req.materialIndex);
            if (mat != null)
            {
                recipeString += req.amount + "x " + mat.materialName + "\n";
            }
        }
        recipeText.text = recipeString;
        
        // ===== AJOUTE CETTE SECTION =====
        // Vérifie si on peut fabriquer (tous les matériaux disponibles)
        if (craftButton != null && gameManager != null)
        {
            bool canCraft = true;
            
            foreach (MaterialRequirement req in product.recipe)
            {
                CraftingMaterial mat = gameManager.GetMaterial(req.materialIndex);
                if (mat == null || !mat.HasEnoughQuantity(req.amount))
                {
                    canCraft = false;
                    break;
                }
            }
            
            craftButton.interactable = canCraft;
        }
        // ================================
    }
    
    // Fonction appelée quand on clique sur "Fabriquer"
    void OnCraftButtonClicked()
    {
        gameManager.CraftProduct(productIndex);
        
        // Met à jour l'affichage
        Product prod = gameManager.GetProduct(productIndex);
        if (prod != null)
        {
            UpdateDisplay(prod);
        }
    }
}
