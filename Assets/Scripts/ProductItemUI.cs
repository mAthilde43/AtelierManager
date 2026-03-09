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
    
    // === SYSTÈME X1/X5/X10 ===
    private int craftQuantity = 1;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private float holdDuration = 1f;
    public Slider holdProgressBar;
    // =========================
    
    // Données
    private int productIndex;
    private GameManager gameManager;
    
    void Start()
    {
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(false);
    }
    
    // Fonction appelée pour initialiser cet élément
    public void Setup(int index, Product product, GameManager gm)
    {
        productIndex = index;
        gameManager = gm;
        
        // Affiche les informations
        UpdateDisplay(product);
        
        // ===== AJOUTE L'EVENT TRIGGER =====
        if (craftButton != null)
        {
            // Ajoute EventTrigger
            UnityEngine.EventSystems.EventTrigger trigger = craftButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            
            if (trigger == null)
            {
                trigger = craftButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
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
        
        // Vérifie si on peut fabriquer
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
        
        // ===== MET À JOUR LE TEXTE DU BOUTON =====
        UpdateButtonText();
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
                CycleCraftMode();
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
            OnCraftButtonClicked();
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
    
    void CycleCraftMode()
    {
        if (craftQuantity == 1)
            craftQuantity = 5;
        else if (craftQuantity == 5)
            craftQuantity = 10;
        else
            craftQuantity = 1;
        
        UpdateButtonText();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySuccess();
        }
        
        if (FeedbackManager.Instance != null && craftButton != null)
        {
            FeedbackManager.Instance.ShowSuccess("Mode x" + craftQuantity, craftButton.transform.position);
        }
        
        Debug.Log("Mode de fabrication changé : x" + craftQuantity);
    }
    
    void UpdateButtonText()
    {
        if (craftButton != null)
        {
            TextMeshProUGUI buttonText = craftButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Fabriquer x" + craftQuantity;
            }
        }
    }
    
    void OnCraftButtonClicked()
    {
        if (gameManager != null)
        {
            Product prod = gameManager.GetProduct(productIndex);
            if (prod == null) return;
            
            // Fabrique X fois
            int crafted = 0;
            for (int i = 0; i < craftQuantity; i++)
            {
                // Vérifie si on a les matériaux
                bool hasAll = true;
                foreach (MaterialRequirement req in prod.recipe)
                {
                    CraftingMaterial mat = gameManager.GetMaterial(req.materialIndex);
                    if (mat == null || !mat.HasEnoughQuantity(req.amount))
                    {
                        hasAll = false;
                        break;
                    }
                }
                
                if (hasAll)
                {
                    gameManager.CraftProduct(productIndex);
                    crafted++;
                }
                else
                {
                    break;
                }
            }
            
            if (crafted < craftQuantity)
            {
                Debug.LogWarning("Seulement " + crafted + " produit(s) fabriqué(s) (manque de matériaux)");
                
                if (FeedbackManager.Instance != null && craftButton != null)
                {
                    FeedbackManager.Instance.ShowError("Matériaux insuffisants !", craftButton.transform.position);
                }
            }
            else
            {
                Debug.Log("" + crafted + " produit(s) fabriqué(s) !");
            }
            
            // Met à jour l'affichage
            UpdateDisplay(prod);
        }
    }
}
