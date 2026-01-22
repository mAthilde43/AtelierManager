using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialItemUI : MonoBehaviour
{
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
        priceText.text = "Prix: " + craftingMaterial.price + "€";
        stockText.text = "Stock: " + craftingMaterial.quantity;
    }
    
    // Fonction appelée quand on clique sur le bouton
    void OnBuyButtonClicked()
    {
        // Achète 1 unité de ce matériau
        gameManager.BuyMaterial(materialIndex, 1);
        
        // Met à jour l'affichage
        CraftingMaterial mat = gameManager.GetMaterial(materialIndex);
        if (mat != null)
        {
            UpdateDisplay(mat);
        }
    }
}