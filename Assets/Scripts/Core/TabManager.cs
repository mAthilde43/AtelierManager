using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
{
    // Références vers les panneaux
    public GameObject shopPanel;
    public GameObject workshopPanel;
    public GameObject salesPanel;
    public GameObject upgradesPanel;
    public GameObject statsPanel;
    public GameObject employeesPanel;
    public GameObject ordersPanel;
    public GameObject buildingPanel;

    // Références vers les boutons d'onglets
    public Button shopTabButton;
    public Button workshopTabButton;
    public Button salesTabButton;
    public Button upgradesTabButton;
    public Button statsTabButton;
    public Button employeesTabButton;
    public Button ordersTabButton;
    public Button buildingTabButton;
    
    // === DÉBLOCAGE ONGLETS ===
    public int buildingUnlockLevel = 10;  // Niveau requis pour débloquer l'atelier
    
    // Couleurs pour les boutons actifs/inactifs
    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    private GameManager gameManager;
    
    void Start()
    {
        // Récupère le GameManager
        gameManager = FindObjectOfType<GameManager>();
        
        // Configure les boutons
        shopTabButton.onClick.AddListener(() => ShowTab("shop"));
        workshopTabButton.onClick.AddListener(() => ShowTab("workshop"));
        salesTabButton.onClick.AddListener(() => ShowTab("sales"));
        upgradesTabButton.onClick.AddListener(() => ShowTab("upgrades"));
        statsTabButton.onClick.AddListener(() => ShowTab("stats"));
        employeesTabButton.onClick.AddListener(() => ShowTab("employees"));
        ordersTabButton.onClick.AddListener(() => ShowTab("orders"));  
        buildingTabButton.onClick.AddListener(() => ShowTab("building"));  
        
        // Vérifie l'accès à l'onglet Atelier
        Invoke("UpdateBuildingTabAccess", 0.1f);
        
        // Affiche la boutique par défaut au démarrage
        ShowTab("shop");
    }
    
    // Met à jour l'accès à l'onglet Atelier
    public void UpdateBuildingTabAccess()
    {
        ProgressionManager progressionManager = FindObjectOfType<ProgressionManager>();
        
        if (progressionManager != null && buildingTabButton != null)
        {
            bool canAccessBuilding = progressionManager.currentLevel >= buildingUnlockLevel;
            
            // Active/désactive le bouton
            buildingTabButton.interactable = canAccessBuilding;
            
            // Change le texte du bouton
            TextMeshProUGUI buttonText = buildingTabButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (canAccessBuilding)
                {
                    buttonText.text = "Atelier";
                }
                else
                {
                    buttonText.text = "Niv." + buildingUnlockLevel;
                }
            }
            
            Debug.Log("Atelier " + (canAccessBuilding ? "ACCESSIBLE" : "VERROUILLÉ (Niv." + buildingUnlockLevel + " requis)"));
        }
    }
    
    // Fonction pour afficher un onglet
    public void ShowTab(string tabName)
    {
        // Cache tous les panneaux
        shopPanel.SetActive(false);
        workshopPanel.SetActive(false);
        salesPanel.SetActive(false);
        upgradesPanel.SetActive(false);
        statsPanel.SetActive(false);
        employeesPanel.SetActive(false);
        ordersPanel.SetActive(false);
        buildingPanel.SetActive(false);
    
        // Réinitialise la couleur de tous les boutons
        ResetButtonColors();
    
        // Affiche le panneau demandé et met en surbrillance son bouton
        switch (tabName)
        {
            case "shop":
                shopPanel.SetActive(true);
                HighlightButton(shopTabButton);
                Debug.Log("Onglet Boutique ouvert");
                break;
            
            case "workshop":
                workshopPanel.SetActive(true);
                HighlightButton(workshopTabButton);
                Debug.Log("Onglet Atelier ouvert");
                break;
            
            case "sales":
                salesPanel.SetActive(true);
                HighlightButton(salesTabButton);
                Debug.Log("Onglet Vente ouvert");
                break;
            
            case "upgrades":
                upgradesPanel.SetActive(true);
                HighlightButton(upgradesTabButton);
                Debug.Log("Onglet Améliorations ouvert");
                break;
            
            case "stats":
                statsPanel.SetActive(true);
                HighlightButton(statsTabButton);
            
                StatsUI statsUI = statsPanel.GetComponent<StatsUI>();
                if (statsUI != null)
                {
                    statsUI.RefreshStatsDisplay();
                }
            
                Debug.Log("Onglet Statistiques ouvert");
                break;
            
            case "employees":
                employeesPanel.SetActive(true);
                HighlightButton(employeesTabButton);
    
                EmployeesUI employeesUI = employeesPanel.GetComponent<EmployeesUI>();
                if (employeesUI != null)
                {
                    employeesUI.RefreshEmployeesDisplay();
                }
    
                Debug.Log("Onglet Employés ouvert");
                break;
            
            case "orders":
                ordersPanel.SetActive(true);
                HighlightButton(ordersTabButton);
    
                OrdersUI ordersUI = ordersPanel.GetComponent<OrdersUI>();
                if (ordersUI != null)
                {
                    ordersUI.RefreshOrdersDisplay();
                }
    
                Debug.Log("Onglet Commandes ouvert");
                break;
            
            case "building":
                buildingPanel.SetActive(true);
                HighlightButton(buildingTabButton);  
                Debug.Log("Onglet Atelier affiché");
                break;

            default:
                Debug.LogWarning("Onglet inconnu : " + tabName);
                shopPanel.SetActive(true);
                break;
        }
        
        // Rafraîchit l'UI pour mettre à jour les boutons
        if (gameManager != null)
        {
            gameManager.RefreshAllUI();
            Debug.Log("UI rafraîchie après changement d'onglet");
        }
    }
    
    // Réinitialise la couleur de tous les boutons (inactifs)
    void ResetButtonColors()
    {
        ColorBlock cb;
    
        cb = shopTabButton.colors;
        cb.normalColor = inactiveColor;
        shopTabButton.colors = cb;
    
        cb = workshopTabButton.colors;
        cb.normalColor = inactiveColor;
        workshopTabButton.colors = cb;
    
        cb = salesTabButton.colors;
        cb.normalColor = inactiveColor;
        salesTabButton.colors = cb;
    
        cb = upgradesTabButton.colors;
        cb.normalColor = inactiveColor;
        upgradesTabButton.colors = cb;
        
        cb = statsTabButton.colors;
        cb.normalColor = inactiveColor;
        statsTabButton.colors = cb;
        
        cb = employeesTabButton.colors;
        cb.normalColor = inactiveColor;
        employeesTabButton.colors = cb;
        
        cb = ordersTabButton.colors;
        cb.normalColor = inactiveColor;
        ordersTabButton.colors = cb;
        
        cb = buildingTabButton.colors;
        cb.normalColor = inactiveColor;
        buildingTabButton.colors = cb;
    }
    
    // Met en surbrillance un bouton (actif)
    void HighlightButton(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = activeColor;
        button.colors = cb;
    }
}
