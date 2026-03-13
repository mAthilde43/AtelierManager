using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingUI : MonoBehaviour
{
    // === RÉFÉRENCES UI ===
    public Transform elementsContainer;
    public GameObject elementPrefab;
    public TextMeshProUGUI progressText;
    
    // === RÉFÉRENCES MANAGERS ===
    private BuildingManager buildingManager;
    private ProgressionManager progressionManager;
    private GameManager gameManager;
    
    // === LISTE DES ÉLÉMENTS UI ===
    private List<GameObject> elementUIList = new List<GameObject>();
    
    void Start()
    {
        buildingManager = BuildingManager.Instance;
        progressionManager = FindObjectOfType<ProgressionManager>();
        gameManager = FindObjectOfType<GameManager>();
        
        RefreshDisplay();
        
        Debug.Log("✅ BuildingUI initialisé");
    }
    
    public void RefreshDisplay()
    {
        if (buildingManager == null || elementsContainer == null || elementPrefab == null)
        {
            Debug.LogError("❌ BuildingUI : Références manquantes !");
            return;
        }
        
        ClearElements();
        
        foreach (BuildingElement element in buildingManager.allElements)
        {
            CreateElementUI(element);
        }
        
        UpdateProgressText();
        
        Debug.Log("BuildingUI rafraîchi : " + elementUIList.Count + " éléments affichés");
    }
    
    void CreateElementUI(BuildingElement element)
    {
        GameObject elementUI = Instantiate(elementPrefab, elementsContainer);
        
        // Couleur selon la catégorie
        Image panelImage = elementUI.GetComponent<Image>();
        if (panelImage != null)
        {
            switch (element.category)
            {
                case BuildingCategory.Structure:
                    panelImage.color = new Color(1f, 0.95f, 0.9f);
                    break;
                case BuildingCategory.Equipment:
                    panelImage.color = new Color(0.9f, 0.95f, 1f);
                    break;
                case BuildingCategory.Furniture:
                    panelImage.color = new Color(0.95f, 1f, 0.95f);
                    break;
                case BuildingCategory.Decoration:
                    panelImage.color = new Color(1f, 0.95f, 1f);
                    break;
            }
        }
        
        elementUIList.Add(elementUI);
        
        Image elementIcon = elementUI.transform.Find("ElementIcon")?.GetComponent<Image>();
        TextMeshProUGUI nameText = elementUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI infoText = elementUI.transform.Find("InfoText")?.GetComponent<TextMeshProUGUI>();
        Button buyButton = elementUI.transform.Find("BuyButton")?.GetComponent<Button>();
        GameObject purchasedOverlay = elementUI.transform.Find("PurchasedOverlay")?.gameObject;
        
        // ===== CHARGE ET AFFICHE L'ICÔNE =====
        if (elementIcon != null && !string.IsNullOrEmpty(element.iconName))
        {
            Sprite sprite = Resources.Load<Sprite>("Icons/" + element.iconName);
            
            if (sprite != null)
            {
                elementIcon.sprite = sprite;
                elementIcon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Icône introuvable : Icons/" + element.iconName);
                elementIcon.gameObject.SetActive(false);
            }
        }
        else if (elementIcon != null)
        {
            elementIcon.gameObject.SetActive(false);
        }
        
        
        if (nameText != null)
            nameText.text = element.elementName;
        
        if (infoText != null)
        {
            if (element.cost > 0)
            {
                infoText.text = element.cost + "€ · Niv." + element.unlockLevel;
            }
            else
            {
                infoText.text = "GRATUIT";
            }
        }
        
        if (buyButton != null)
        {
            if (element.isPurchased)
            {
                buyButton.gameObject.SetActive(false);
                if (purchasedOverlay != null)
                    purchasedOverlay.SetActive(true);
            }
            else
            {
                bool isUnlocked = (progressionManager != null && progressionManager.currentLevel >= element.unlockLevel);
                bool canAfford = (gameManager != null && gameManager.HasEnoughMoney(element.cost));
                
                buyButton.interactable = isUnlocked && canAfford;
                
                if (!isUnlocked)
                {
                    Image buttonImage = buyButton.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                    
                    TextMeshProUGUI buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = "🔒 Verrouillé";
                    }
                }
                else if (!canAfford)
                {
                    Image buttonImage = buyButton.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = new Color(0.8f, 0.4f, 0.4f);
                    }
                }
                
                int elementIndex = buildingManager.allElements.IndexOf(element);
                buyButton.onClick.AddListener(() => OnBuyButtonClicked(elementIndex));
            }
        }
    }
    
    void OnBuyButtonClicked(int elementIndex)
    {
        if (buildingManager != null)
        {
            buildingManager.PurchaseElement(elementIndex);
            RefreshDisplay();
        }
    }
    
    void UpdateProgressText()
    {
        if (progressText != null && buildingManager != null)
        {
            int purchased = buildingManager.GetTotalPurchased();
            int total = buildingManager.allElements.Count;
            
            progressText.text = "Éléments construits : " + purchased + " / " + total;
        }
    }
    
    void ClearElements()
    {
        foreach (GameObject element in elementUIList)
        {
            if (element != null)
            {
                Destroy(element);
            }
        }
        elementUIList.Clear();
    }
    
    void OnEnable()
    {
        if (buildingManager != null)
        {
            RefreshDisplay();
        }
    }
}
