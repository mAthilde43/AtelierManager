using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItemUI : MonoBehaviour
{
    // Références vers les éléments UI
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statusText;
    public Button buyButton;
    
    // Données
    private int upgradeIndex;
    private GameManager gameManager;
    
    // Fonction appelée pour initialiser cet élément
    public void Setup(int index, Upgrade upgrade, GameManager gm)
    {
        upgradeIndex = index;
        gameManager = gm;
        
        // Affiche les informations
        UpdateDisplay(upgrade);
        
        // Configure le bouton
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyButtonClicked());
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Upgrade upgrade)
    {
        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        costText.text = "Coût: " + upgrade.cost + "€";
        
        // Change le statut et l'apparence selon si c'est acheté
        if (upgrade.isPurchased)
        {
            statusText.text = "Acheté";
            statusText.color = new Color(0f, 0.7f, 0f); // Vert
            
            // Désactive le bouton
            if (buyButton != null)
            {
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Acheté";
            }
            
            // Grise légèrement le panneau
            GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        else
        {
            statusText.text = "Disponible";
            statusText.color = new Color(0f, 0.47f, 0f); // Vert foncé
            
            // Active le bouton SI on a assez d'argent
            if (buyButton != null)
            {
                bool canAfford = gameManager.HasEnoughMoney(upgrade.cost);
                buyButton.interactable = canAfford;
                
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Acheter";
            }
            
            // Couleur normale
            GetComponent<Image>().color = new Color(0.98f, 0.98f, 0.98f, 1f);
        }
    }
    
    // Fonction appelée quand on clique sur "Acheter"
    void OnBuyButtonClicked()
    {
        gameManager.BuyUpgrade(upgradeIndex);
        
        // Met à jour l'affichage
        Upgrade upg = gameManager.GetUpgrade(upgradeIndex);
        if (upg != null)
        {
            UpdateDisplay(upg);
        }
    }
}