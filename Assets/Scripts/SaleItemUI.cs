using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaleItemUI : MonoBehaviour
{
    // Références vers les éléments UI
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public Button sellButton;
    
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
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => OnSellButtonClicked());
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Product product)
    {
        // ===== VÉRIFICATION DÉVERROUILLAGE =====
        // Si le produit est verrouillé, cache complètement cet élément UI
        if (!product.isUnlocked)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        // =======================================
        
        productNameText.text = product.productName;
        priceText.text = "Prix: " + product.sellPrice + "€";
        stockText.text = "Dispo: " + product.quantity;
        
        // Désactive le bouton si pas de stock
        if (sellButton != null)
        {
            sellButton.interactable = (product.quantity > 0);
        }
    }
    
    // Fonction appelée quand on clique sur "Vendre"
    void OnSellButtonClicked()
    {
        gameManager.SellProduct(productIndex, 1);
        
        // Met à jour l'affichage
        Product prod = gameManager.GetProduct(productIndex);
        if (prod != null)
        {
            UpdateDisplay(prod);
        }
    }
}