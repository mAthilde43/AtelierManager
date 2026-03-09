using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MaterialItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public Button buyButton;
    public Slider holdProgressBar;  // Barre de progression (optionnel)
    
    // === DONNÉES ===
    private int materialIndex;
    private GameManager gameManager;
    private CraftingMaterial currentMaterial;
    
    // === SYSTÈME D'ACHAT MULTIPLE ===
    private int purchaseQuantity = 1;        // Quantité actuelle (1, 5 ou 10)
    private float holdTimer = 0f;            // Timer pour le maintien
    private bool isHolding = false;          // Bouton maintenu ?
    private float holdDuration = 1f;         // Durée requise (2 secondes)
    
    void Start()
    {
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(false);
    }
    // Initialise l'élément
    public void Setup(int index, CraftingMaterial craftingMaterial, GameManager gm)
    {
        materialIndex = index;
        gameManager = gm;
        currentMaterial = craftingMaterial;
        
        // Affiche les informations
        UpdateDisplay(craftingMaterial);
        
        // Configure le bouton avec EventTrigger
        if (buyButton != null)
        {
            // Retire les listeners existants
            buyButton.onClick.RemoveAllListeners();
            
            // Ajoute/récupère l'EventTrigger
            EventTrigger trigger = buyButton.gameObject.GetComponent<EventTrigger>();
            
            if (trigger == null)
            {
                trigger = buyButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // Efface les événements existants
            trigger.triggers.Clear();
            
            // Pointer Down (début du maintien)
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnPointerDown(); });
            trigger.triggers.Add(pointerDown);
            
            // Pointer Up (fin du maintien)
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnPointerUp(); });
            trigger.triggers.Add(pointerUp);
            
            // Pointer Exit (sort du bouton)
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(pointerExit);
        }
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(CraftingMaterial craftingMaterial)
    {
        if (craftingMaterial == null) return;
        
        currentMaterial = craftingMaterial;
        
        // Nom du matériau
        if (nameText != null)
            nameText.text = craftingMaterial.materialName;
        
        // Prix avec réduction
        float displayPrice = craftingMaterial.price;

        if (BuildingManager.Instance != null)
        {
            displayPrice *= BuildingManager.Instance.GetMaterialDiscountMultiplier();
        }

        int finalPrice = Mathf.RoundToInt(displayPrice);

        // Affiche le prix réduit si différent
        if (priceText != null)
        {
            if (finalPrice < craftingMaterial.price)
            {
                priceText.text = "<s>" + craftingMaterial.price + "€</s> " + finalPrice + "€";
            }
            else
            {
                priceText.text = finalPrice + "€";
            }
        }
        
        // Stock
        if (stockText != null)
            stockText.text = "Stock: " + craftingMaterial.quantity;
        
        // Désactive le bouton si pas assez d'argent
        if (buyButton != null && gameManager != null)
        {
            // Calcule le coût total pour la quantité actuelle
            int totalCost = finalPrice * purchaseQuantity;
            bool canAfford = gameManager.HasEnoughMoney(totalCost);
            buyButton.interactable = canAfford;
        }
        
        // Met à jour le texte du bouton
        UpdateButtonText();
    }
    
    // ===== GESTION DU MAINTIEN =====
    
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            
            // Met à jour la barre de progression
            if (holdProgressBar != null)
            {
                holdProgressBar.value = holdTimer / holdDuration;
            }
            
            // Si maintenu 3 secondes → Change de mode
            if (holdTimer >= holdDuration)
            {
                CyclePurchaseMode();
                ResetHold();
            }
        }
    }
    
    void OnPointerDown()
    {
        isHolding = true;
        holdTimer = 0f;
        
        // Active la barre si elle existe
        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(true);
        }
    }
    
    void OnPointerUp()
    {
        // Si maintenu moins de 3 secondes → Achat normal
        if (holdTimer < holdDuration)
        {
            OnBuyButtonClicked();
        }
        
        ResetHold();
    }
    
    void OnPointerExit()
    {
        // Si on sort du bouton, annule le maintien
        ResetHold();
    }
    
    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;
        
        // Reset la barre
        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0f;
            holdProgressBar.gameObject.SetActive(false);
        }
    }
    
    // Change de mode d'achat (x1 → x5 → x10 → x1)
    void CyclePurchaseMode()
    {
        if (purchaseQuantity == 1)
            purchaseQuantity = 5;
        else if (purchaseQuantity == 5)
            purchaseQuantity = 10;
        else
            purchaseQuantity = 1;
        
        // Met à jour l'affichage
        UpdateButtonText();
        UpdateDisplay(currentMaterial);
        
        // Feedback sonore
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchase();
        }
        
        // Feedback visuel (flash)
        if (FeedbackManager.Instance != null && buyButton != null)
        {
            FeedbackManager.Instance.ShowSuccess("Mode x" + purchaseQuantity, buyButton.transform.position);
        }
        
        Debug.Log("Mode d'achat changé : x" + purchaseQuantity);
    }
    
    // Met à jour le texte du bouton
    void UpdateButtonText()
    {
        if (buyButton != null)
        {
            TextMeshProUGUI buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Acheter x" + purchaseQuantity;
            }
        }
    }
    
    // Achat au clic
    void OnBuyButtonClicked()
    {
        if (gameManager != null && currentMaterial != null)
        {
            // Calcule le prix avec bonus
            float baseCost = currentMaterial.price * purchaseQuantity;
            
            if (BuildingManager.Instance != null)
            {
                baseCost *= BuildingManager.Instance.GetMaterialDiscountMultiplier();
            }
            
            int finalCost = Mathf.RoundToInt(baseCost);
            
            Debug.Log("Achat x" + purchaseQuantity + " : " + currentMaterial.price + "€ → " + finalCost + "€");
            
            // Achète la quantité sélectionnée
            gameManager.BuyMaterial(materialIndex, purchaseQuantity);
        }
    }
}
