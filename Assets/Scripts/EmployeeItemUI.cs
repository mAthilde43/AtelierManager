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
    private Employee currentEmployee;
    private EmployeeManager employeeManager;
    
    // Initialise l'item
    public void Initialize(int index, EmployeeManager em)
    {
        employeeIndex = index;
        employeeManager = em;
        
        // Configure les boutons
        if (hireButton != null)
        {
            hireButton.onClick.RemoveAllListeners();
            hireButton.onClick.AddListener(OnHireClicked);
        }
        
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(OnToggleClicked);
        }
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay(Employee emp)
    {
        if (emp == null) return;
        
        currentEmployee = emp;
        
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
            typeText.text = typeString;
        }
        
        // Coût d'embauche
        if (costText != null)
        {
            costText.text = "Embauche: " + emp.hireCost + "€";
        }
        
        // Salaire
        if (salaryText != null)
        {
            salaryText.text = "Salaire: " + emp.salaryPerWeek + "€/sem";
        }
        
        // Vitesse
        if (speedText != null)
        {
            speedText.text = "Vitesse: " + emp.productionSpeed.ToString("F0") + "s";
        }
        
        // ===== VÉRIFICATION DU NIVEAU =====
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
        int playerLevel = pm != null ? pm.currentLevel : 1;
        bool isUnlocked = emp.unlockLevel <= playerLevel;
        
        // Statut
        if (statusText != null)
        {
            if (!isUnlocked)
            {
                // Employé verrouillé
                statusText.text = "Niveau " + emp.unlockLevel + " requis";
                statusText.color = new Color(0.65f, 0.2f, 0.2f);  // Rouge
            }
            else if (emp.isHired)
            {
                // Employé embauché
                statusText.text = emp.isActive ? "Actif" : "Inactif";
                statusText.color = emp.isActive ? new Color(0.1f, 0.6f, 0.1f) : new Color(0.65f, 0.2f, 0.2f);
            }
            else
            {
                // Employé disponible
                statusText.text = "Disponible";
                statusText.color = new Color(1f, 1f, 1f);  // Blanc
            }
        }
        
        // Bouton embaucher
        if (hireButton != null)
        {
            if (!isUnlocked)
            {
                // Verrouillé
                hireButton.gameObject.SetActive(true);
                hireButton.interactable = false;
                
                TextMeshProUGUI buttonText = hireButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Verrouillé";
                }
            }
            else if (emp.isHired)
            {
                // Déjà embauché
                hireButton.gameObject.SetActive(false);
            }
            else
            {
                // Disponible à l'embauche
                hireButton.gameObject.SetActive(true);
                
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    hireButton.interactable = gm.HasEnoughMoney(emp.hireCost);
                }
                
                TextMeshProUGUI buttonText = hireButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Embaucher";
                }
            }
        }
        
        // Bouton toggle
        if (toggleButton != null)
        {
            if (emp.isHired && isUnlocked)
            {
                toggleButton.gameObject.SetActive(true);
                
                TextMeshProUGUI toggleText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
                if (toggleText != null)
                {
                    toggleText.text = emp.isActive ? "Désactiver" : "Activer";
                }
            }
            else
            {
                toggleButton.gameObject.SetActive(false);
            }
        }
        
        // Barre de progression
        if (workProgressBar != null)
        {
            if (emp.isHired && emp.isActive && isUnlocked)
            {
                workProgressBar.gameObject.SetActive(true);
                
                float progress = Mathf.Clamp01(emp.productionTimer / emp.productionSpeed);
                workProgressBar.value = progress;
            }
            else
            {
                workProgressBar.gameObject.SetActive(false);
            }
        }
    }
    
    // Met à jour seulement l'affichage (sans passer l'employé)
    public void UpdateDisplay()
    {
        if (currentEmployee != null)
        {
            UpdateDisplay(currentEmployee);
        }
    }
    
    // Bouton embaucher cliqué
    void OnHireClicked()
    {
        if (employeeManager != null)
        {
            employeeManager.HireEmployee(employeeIndex);
            
            Employee emp = employeeManager.GetEmployee(employeeIndex);
            if (emp != null)
            {
                UpdateDisplay(emp);
            }
            
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
            
            Employee emp = employeeManager.GetEmployee(employeeIndex);
            if (emp != null)
            {
                UpdateDisplay(emp);
            }
        }
    }
}
