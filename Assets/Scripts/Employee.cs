using UnityEngine;

// Types d'employés
public enum EmployeeType
{
    Crafter,        // Fabrique des produits automatiquement
    Seller,         // Vend des produits automatiquement
    Gatherer        // Achète des matériaux automatiquement
}

[System.Serializable]
public class Employee
{
    public string employeeName;           // Nom de l'employé
    public EmployeeType type;             // Type d'employé
    public int level;                     // Niveau de l'employé
    public int hireCost;                  // Coût d'embauche
    public int salaryPerWeek;             // Salaire hebdomadaire
    public float productionSpeed;         // Vitesse de production (en secondes)
    public bool isHired;                  // Employé embauché ?
    public bool isActive;                 // Employé actif (travaille) ?
    
    public float productionTimer;        // Timer interne
    
    // Constructeur
    public Employee(string name, EmployeeType empType, int cost, int salary, float speed)
    {
        employeeName = name;
        type = empType;
        level = 1;
        hireCost = cost;
        salaryPerWeek = salary;
        productionSpeed = speed;
        isHired = false;
        isActive = false;
        productionTimer = 0f;
    }
    
    // Met à jour l'employé (appelé chaque frame)
    public void Update(float deltaTime)
    {
        Debug.Log($" {employeeName}.Update() - isHired={isHired}, isActive={isActive}, deltaTime={deltaTime:F4}");
    
        if (!isHired || !isActive)
        {
            Debug.LogWarning($"⏸️ {employeeName} - Sortie early (not hired or not active)");
            return;
        }
    
        Debug.Log($"{employeeName} - Passage du test, incrémentation du timer");
        productionTimer += deltaTime;
        Debug.Log($"{employeeName} - Timer après ajout: {productionTimer:F2} / {productionSpeed}");
    
        //if (productionTimer >= productionSpeed)
        //{
          //  Debug.Log($"🎉 {employeeName} - Timer >= speed ! Réinitialisation.");
            //productionTimer = 0f;
        //}
    }

    
    // Vérifie si l'employé est prêt à effectuer une action
    public bool IsReadyToWork()
    {
        return isHired && isActive && productionTimer >= productionSpeed;
    }
    
    // Réinitialise le timer de production
    public void ResetTimer()
    {
        productionTimer = 0f;
    }
    
    // Embauche l'employé
    public void Hire()
    {
        isHired = true;
        isActive = true;
        productionTimer = 0f;
        Debug.Log("" + employeeName + " embauché !");
    }
    
    // Active/désactive l'employé
    public void ToggleActive()
    {
        isActive = !isActive;
        Debug.Log(employeeName + (isActive ? "activé" : "désactivé"));
    }
    
    // Améliore l'employé (augmente vitesse, réduit coût)
    public void LevelUp()
    {
        level++;
        productionSpeed *= 0.9f; // 10% plus rapide
        Debug.Log("" + employeeName + " niveau " + level);
    }
}
