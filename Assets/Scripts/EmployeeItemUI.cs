using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmployeeItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI statusText;
    public Button hireButton;
    public Button toggleButton;
    public Slider workProgressBar;
    
    private int employeeIndex;
    private EmployeeManager employeeManager;
    
    // Initialise l'item
    public void Initialize(int index, EmployeeManager em)
    {
        employeeIndex = index;
        employeeManager = em;
        
        // Configure les boutons
        hireButton.onClick.RemoveAllListeners();
        hireButton.onClick.AddListener(OnHireClicked);
        
        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(OnToggleClicked);
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Employee emp)
    {
        if (emp == null) return;
        
        // Nom
        if (nameText != null)
        {
            nameText.text = emp.employeeName;
        }
        
        // Type
        if (typeText != null)
        {
            string typeString = "";
            switch (emp.type)
            {
                case EmployeeType.Crafter:
                    typeString = "🔨 Fabricant";
                    break;
                case EmployeeType.Seller:
                    typeString = "💼 Vendeur";
                    break;
                case EmployeeType.Gatherer:
                    typeString = "📦 Acheteur";
                    break;
            }
            typeText.text = "Type: " + typeString;
        }
        
        // Coût d'embauche
        if (costText != null)
        {
            costText.text = "Embauche: " + emp.hireCost + "€";
        }
        
        // Salaire
        if (salaryText != null)
        {
            salaryText.text = "Salaire: " + emp.salaryPerWeek + "€/semaine";
        }
        
        // Vitesse
        if (speedText != null)
        {
            speedText.text = "⚡ " + emp.productionSpeed.ToString("F0") + "s/action";
        }
        
        // Statut et boutons
        if (emp.isHired)
        {
            // Employé embauché
            if (statusText != null)
            {
                statusText.text = emp.isActive ? "🟢 Actif" : "🔴 Inactif";
                statusText.color = emp.isActive ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.3f, 0.3f);
            }
            
            // Cache le bouton embaucher
            if (hireButton != null)
            {
                hireButton.gameObject.SetActive(false);
            }
            
            // Affiche le bouton activer/désactiver
            if (toggleButton != null)
            {
                toggleButton.gameObject.SetActive(true);
                TextMeshProUGUI toggleText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
                if (toggleText != null)
                {
                    toggleText.text = emp.isActive ? "Désactiver" : "Activer";
                }
            }
            
        }
        else
        {
            // Employé non embauché
            if (statusText != null)
            {
                statusText.text = "Non embauché";
                statusText.color = new Color(0.5f, 0.5f, 0.5f);
            }
            
            // Affiche le bouton embaucher
            if (hireButton != null)
            {
                hireButton.gameObject.SetActive(true);
                
                // Désactive si pas assez d'argent
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    hireButton.interactable = gm.HasEnoughMoney(emp.hireCost);
                }
            }
            
            // Cache le bouton activer/désactiver
            if (toggleButton != null)
            {
                toggleButton.gameObject.SetActive(false);
            }
        }
        
        // Barre de progression
        if (workProgressBar != null)
        {
            if (emp.isHired && emp.isActive)
            {
                // Affiche la barre et met à jour sa valeur
                workProgressBar.gameObject.SetActive(true);
                
                // Calcule le pourcentage (productionTimer / productionSpeed)
                float progress = Mathf.Clamp01(emp.productionTimer / emp.productionSpeed);
                workProgressBar.value = progress;
            }
            else
            {
                // Cache la barre si pas embauché ou inactif
                workProgressBar.gameObject.SetActive(false);
            }
        }
    }
    
    // Bouton embaucher cliqué
    void OnHireClicked()
    {
        if (employeeManager != null)
        {
            employeeManager.HireEmployee(employeeIndex);
            
            // Met à jour l'affichage
            Employee emp = employeeManager.GetEmployee(employeeIndex);
            if (emp != null)
            {
                UpdateDisplay(emp);
            }
            
            // Rafraîchit l'UI globale
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.RefreshAllUI();
            }
        }
    }
    
    // Bouton activer/désactiver cliqué
    void OnToggleClicked()
    {
        if (employeeManager != null)
        {
            employeeManager.ToggleEmployee(employeeIndex);
            
            // Met à jour l'affichage
            Employee emp = employeeManager.GetEmployee(employeeIndex);
            if (emp != null)
            {
                UpdateDisplay(emp);
            }
        }
    }
}
