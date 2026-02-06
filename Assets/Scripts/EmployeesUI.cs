using UnityEngine;
using System.Collections.Generic;

public class EmployeesUI : MonoBehaviour
{
    [Header("References")]
    public Transform employeesContainer;  
    public GameObject employeeItemPrefab;
    
    private EmployeeManager employeeManager;
    private List<EmployeeItemUI> employeeItems = new List<EmployeeItemUI>();
    
    void Start()
    {
        employeeManager = EmployeeManager.Instance;
        
        RefreshEmployeesDisplay();
    }
    
    public void RefreshEmployeesDisplay()
    {
        if (employeeManager == null || employeesContainer == null || employeeItemPrefab == null)
        {
            Debug.LogWarning("EmployeesUI : Références manquantes !");
            return;
        }
        
        ProgressionManager pm = FindObjectOfType<ProgressionManager>();
        int playerLevel = pm != null ? pm.currentLevel : 1;
        
        int unlockedCount = 0;
        foreach (Employee emp in employeeManager.employees)
        {
            if (emp.unlockLevel <= playerLevel)
            {
                unlockedCount++;
            }
        }
        
        Debug.Log("Affichage des employés (Niveau " + playerLevel + ")");
        Debug.Log("" + unlockedCount + " / " + employeeManager.employees.Count + " employés débloqués");
        
        // Nettoie les anciens items
        foreach (EmployeeItemUI item in employeeItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        employeeItems.Clear();
        
        // Crée les items pour chaque employé
        for (int i = 0; i < employeeManager.employees.Count; i++)
        {
            GameObject itemObj = Instantiate(employeeItemPrefab, employeesContainer);
            EmployeeItemUI itemUI = itemObj.GetComponent<EmployeeItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(i, employeeManager);
                itemUI.UpdateDisplay(employeeManager.employees[i]);
                employeeItems.Add(itemUI);
            }
        }
        
        Debug.Log("" + employeeItems.Count + " employés affichés dans l'UI");
        
        // Force la mise à jour du Content Size Fitter
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(employeesContainer.GetComponent<RectTransform>());
    }
    
    void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            UpdateAllDisplays();
        }
    }
    
    void UpdateAllDisplays()
    {
        if (employeeManager == null) return;
        
        for (int i = 0; i < employeeItems.Count && i < employeeManager.employees.Count; i++)
        {
            if (employeeItems[i] != null)
            {
                employeeItems[i].UpdateDisplay(employeeManager.employees[i]);
            }
        }
    }
}
