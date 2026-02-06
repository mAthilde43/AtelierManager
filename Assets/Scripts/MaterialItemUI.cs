using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialItemUI : MonoBehaviour
{
    [Header("UI References")]
    // Références vers les éléments UI
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public Button buyButton;
    
    // Données
    private int materialIndex; // Index du matériau dans la liste du GameManager
    private GameManager gameManager;
    
    // Fonction appelée pour initialiser cet élément
    public void Setup(int index, CraftingMaterial craftingMaterial, GameManager gm)
    {
        materialIndex = index;
        gameManager = gm;
        
        // Affiche les informations
        UpdateDisplay(craftingMaterial);
        
        // Configure le bouton
        buyButton.onClick.RemoveAllListeners(); // Nettoie les anciens listeners
        buyButton.onClick.AddListener(() => OnBuyButtonClicked());
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(CraftingMaterial craftingMaterial)
    {
        nameText.text = craftingMaterial.materialName;
        
// Calcule le prix réel avec réduction
        float displayPrice = craftingMaterial.price;

        if (BuildingManager.Instance != null)
        {
            displayPrice *= BuildingManager.Instance.GetMaterialDiscountMultiplier();
        }

        int finalPrice = Mathf.RoundToInt(displayPrice);

// Affiche le prix réduit si différent
        if (finalPrice < craftingMaterial.price)
        {
            priceText.text = "<s>" + craftingMaterial.price + "€</s> " + finalPrice + "€";
        }
        else
        {
            priceText.text = "Prix: " + craftingMaterial.price + "€";
        }
        
        stockText.text = "Stock: " + craftingMaterial.quantity;
    
        // ===== DEBUG =====
        Debug.Log("🔍 UpdateDisplay pour " + craftingMaterial.materialName);
        Debug.Log("   gameManager null ? " + (gameManager == null));
        Debug.Log("   buyButton null ? " + (buyButton == null));
    
        if (gameManager != null)
        {
            Debug.Log("   Argent joueur : " + gameManager.playerMoney);
            Debug.Log("   Prix matériau : " + craftingMaterial.price);
        }
        // =================
    
        // Désactive le bouton si pas assez d'argent
        if (buyButton != null && gameManager != null)
        {
            bool canAfford = gameManager.HasEnoughMoney(craftingMaterial.price);
            Debug.Log("   Can afford ? " + canAfford); // ← AJOUTE
            buyButton.interactable = canAfford;
        }
        else
        {
            Debug.LogWarning("buyButton ou gameManager est null !"); // ← AJOUTE
        }
    }
    
    // Fonction appelée quand on clique sur le bouton
    void OnBuyButtonClicked()
    {
        // ===== CALCULE LE PRIX AVEC BONUS =====
        CraftingMaterial mat = gameManager.GetMaterial(materialIndex);
        if (mat == null) return;
    
        float baseCost = mat.price;
    
        // Applique la réduction des matériaux du Building
        if (BuildingManager.Instance != null)
        {
            baseCost *= BuildingManager.Instance.GetMaterialDiscountMultiplier();
        }
    
        int finalCost = Mathf.RoundToInt(baseCost);
    
        Debug.Log("Prix affiché : " + mat.price + "€ → Prix réel : " + finalCost + "€");
    
        // Achète 1 unité de ce matériau
        gameManager.BuyMaterial(materialIndex, 1);
    
        // Met à jour l'affichage
        if (mat != null)
        {
            UpdateDisplay(mat);
        }
    }
}