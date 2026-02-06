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
    
    // === SYSTÈME X1/X5/X10 ===
    private int sellQuantity = 1;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private float holdDuration = 1f;
    public Slider holdProgressBar;
    // =========================
    
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
        
        // ===== AJOUTE L'EVENT TRIGGER =====
        if (sellButton != null)
        {
            // Ajoute EventTrigger
            UnityEngine.EventSystems.EventTrigger trigger = sellButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            
            if (trigger == null)
            {
                trigger = sellButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            trigger.triggers.Clear();
            
            // Pointer Down
            UnityEngine.EventSystems.EventTrigger.Entry pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnPointerDown(); });
            trigger.triggers.Add(pointerDown);
            
            // Pointer Up
            UnityEngine.EventSystems.EventTrigger.Entry pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnPointerUp(); });
            trigger.triggers.Add(pointerUp);
            
            // Pointer Exit
            UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(pointerExit);
        }
        // ==================================
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Product product)
    {
        // ===== VÉRIFICATION DÉVERROUILLAGE =====
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
        
        // ===== MET À JOUR LE TEXTE DU BOUTON =====
        UpdateButtonText();
        // =========================================
    }
    
    // ===== SYSTÈME X1/X5/X10 =====
    
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            
            if (holdProgressBar != null)
            {
                holdProgressBar.value = holdTimer / holdDuration;
            }
            
            if (holdTimer >= holdDuration)
            {
                CycleSellMode();
                ResetHold();
            }
        }
    }
    
    void OnPointerDown()
    {
        isHolding = true;
        holdTimer = 0f;
        
        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(true);
        }
    }
    
    void OnPointerUp()
    {
        if (holdTimer < holdDuration)
        {
            OnSellButtonClicked();
        }
        
        ResetHold();
    }
    
    void OnPointerExit()
    {
        ResetHold();
    }
    
    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;
        
        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0f;
            holdProgressBar.gameObject.SetActive(false);
        }
    }
    
    void CycleSellMode()
    {
        if (sellQuantity == 1)
            sellQuantity = 5;
        else if (sellQuantity == 5)
            sellQuantity = 10;
        else
            sellQuantity = 1;
        
        UpdateButtonText();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySuccess();
        }
        
        if (FeedbackManager.Instance != null && sellButton != null)
        {
            FeedbackManager.Instance.ShowSuccess("Mode x" + sellQuantity, sellButton.transform.position);
        }
        
        Debug.Log("🔄 Mode de vente changé : x" + sellQuantity);
    }
    
    void UpdateButtonText()
    {
        if (sellButton != null)
        {
            TextMeshProUGUI buttonText = sellButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Vendre x" + sellQuantity;
            }
        }
    }
    
    void OnSellButtonClicked()
    {
        if (gameManager != null)
        {
            Product prod = gameManager.GetProduct(productIndex);
            if (prod == null) return;
            
            // Vend X fois (limité au stock)
            int actualQuantity = Mathf.Min(sellQuantity, prod.quantity);
            
            if (actualQuantity > 0)
            {
                gameManager.SellProduct(productIndex, actualQuantity);
                Debug.Log("✅ " + actualQuantity + " produit(s) vendu(s) !");
            }
            else
            {
                Debug.LogWarning("⚠️ Stock insuffisant !");
                
                if (FeedbackManager.Instance != null && sellButton != null)
                {
                    FeedbackManager.Instance.ShowError("Stock insuffisant !", sellButton.transform.position);
                }
            }
            
            // Met à jour l'affichage
            UpdateDisplay(prod);
        }
    }
    
    // ==============================
}
