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
        
        // Génère l'UI des employés
        RefreshEmployeesDisplay();
    }
    
    // Rafraîchit l'affichage des employés
    public void RefreshEmployeesDisplay()
    {
        if (employeeManager == null || employeesContainer == null || employeeItemPrefab == null)
        {
            return;
        }
        
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
        
        Debug.Log("" + employeeItems.Count + " employés affichés");
    }
    
    // Met à jour tous les affichages (appelé périodiquement)
    void Update()
    {
        // Met à jour l'affichage toutes les 0.5 secondes
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
            employeeItems[i].UpdateDisplay(employeeManager.employees[i]);
        }
    }
}
