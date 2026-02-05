using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingUI : MonoBehaviour
{
    // === RÉFÉRENCES UI ===
    public Transform elementsContainer;      // Le Content de la ScrollView
    public GameObject elementPrefab;         // Le prefab BuildingElementPrefab
    public TextMeshProUGUI progressText;     // "Éléments construits : X / Y"
    
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
        
        // Initialise l'affichage
        RefreshDisplay();
        
        Debug.Log("🎨 BuildingUI initialisé");
    }
    
    // Rafraîchit l'affichage complet
    public void RefreshDisplay()
    {
        if (buildingManager == null || elementsContainer == null || elementPrefab == null)
        {
            Debug.LogError("❌ BuildingUI : Références manquantes !");
            return;
        }
        
        // Détruit les anciens éléments
        ClearElements();
        
        // Crée un élément UI pour chaque élément constructible
        foreach (BuildingElement element in buildingManager.allElements)
        {
            CreateElementUI(element);
        }
        
        // Met à jour le texte de progression
        UpdateProgressText();
        
        Debug.Log("🔄 BuildingUI rafraîchi : " + elementUIList.Count + " éléments affichés");
    }
    
    // Crée un élément UI
    void CreateElementUI(BuildingElement element)
    {
        // Instancie le prefab
        GameObject elementUI = Instantiate(elementPrefab, elementsContainer);
        
        // Couleur selon la catégorie
        Image panelImage = elementUI.GetComponent<Image>();
        if (panelImage != null)
        {
            switch (element.category)
            {
                case BuildingCategory.Structure:
                    panelImage.color = new Color(1f, 0.95f, 0.9f); // Beige
                    break;
                case BuildingCategory.Equipment:
                    panelImage.color = new Color(0.9f, 0.95f, 1f); // Bleu clair
                    break;
                case BuildingCategory.Furniture:
                    panelImage.color = new Color(0.95f, 1f, 0.95f); // Vert clair
                    break;
                case BuildingCategory.Decoration:
                    panelImage.color = new Color(1f, 0.95f, 1f); // Rose clair
                    break;
            }
        }
        
        elementUIList.Add(elementUI);
        
        // Trouve les composants
        TextMeshProUGUI iconText = elementUI.transform.Find("IconText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = elementUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI infoText = elementUI.transform.Find("InfoText")?.GetComponent<TextMeshProUGUI>();
        Button buyButton = elementUI.transform.Find("BuyButton")?.GetComponent<Button>();
        GameObject purchasedOverlay = elementUI.transform.Find("PurchasedOverlay")?.gameObject;
        
        // Remplit les infos
        if (iconText != null)
            iconText.text = element.icon;
        
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
        
        // Configure le bouton
        if (buyButton != null)
        {
            // Vérifie si l'élément est déjà acheté
            if (element.isPurchased)
            {
                buyButton.gameObject.SetActive(false);
                if (purchasedOverlay != null)
                    purchasedOverlay.SetActive(true);
            }
            else
            {
                // Vérifie si débloqué
                bool isUnlocked = (progressionManager != null && progressionManager.currentLevel >= element.unlockLevel);
                bool canAfford = (gameManager != null && gameManager.HasEnoughMoney(element.cost));
                
                // Active/désactive le bouton selon les conditions
                buyButton.interactable = isUnlocked && canAfford;
                
                // Change la couleur si verrouillé
                if (!isUnlocked)
                {
                    Image buttonImage = buyButton.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = new Color(0.5f, 0.5f, 0.5f); // Gris
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
                        buttonImage.color = new Color(0.8f, 0.4f, 0.4f); // Rouge pâle
                    }
                }
                
                // Ajoute l'événement de clic
                int elementIndex = buildingManager.allElements.IndexOf(element);
                buyButton.onClick.AddListener(() => OnBuyButtonClicked(elementIndex));
            }
        }
    }
    
    // Appelé quand on clique sur "Construire"
    void OnBuyButtonClicked(int elementIndex)
    {
        if (buildingManager != null)
        {
            buildingManager.PurchaseElement(elementIndex);
            
            // Rafraîchit l'affichage après l'achat
            RefreshDisplay();
        }
    }
    
    // Met à jour le texte de progression
    void UpdateProgressText()
    {
        if (progressText != null && buildingManager != null)
        {
            int purchased = buildingManager.GetTotalPurchased();
            int total = buildingManager.allElements.Count;
            
            progressText.text = "Éléments construits : " + purchased + " / " + total;
        }
    }
    
    // Détruit tous les éléments UI
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
    
    // Appelé quand l'onglet devient actif (pour rafraîchir)
    void OnEnable()
    {
        if (buildingManager != null)
        {
            RefreshDisplay();
        }
    }
}
