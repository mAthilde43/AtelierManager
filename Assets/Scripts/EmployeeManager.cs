using UnityEngine;
using System.Collections.Generic;

public class EmployeeManager : MonoBehaviour
{
    // Singleton
    private static EmployeeManager instance;
    public static EmployeeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EmployeeManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        Debug.Log("EmployeeManager.Awake() appelé sur " + gameObject.name);
    
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Instance d'EmployeeManager déjà existante ! Destruction de " + gameObject.name);
            Destroy(gameObject);
            return;
        }
    
        Debug.Log("EmployeeManager devient l'instance singleton");
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // === LISTE DES EMPLOYÉS ===
    public List<Employee> employees = new List<Employee>();
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Initialise les employés disponibles
        InitializeEmployees();
        
        // Charge les employés sauvegardés
        LoadEmployees();
        
        Debug.Log("EmployeeManager initialisé avec " + employees.Count + " employés");
    }
    
    void Update()
    {
        // Met à jour tous les employés
        foreach (Employee emp in employees)
        {
            if (emp.isHired && emp.isActive)
            {
                emp.Update(Time.deltaTime);
            
                // Si l'employé est prêt à travailler
                if (emp.IsReadyToWork())
                {
                    Debug.Log($"⚡ {emp.employeeName} est prêt à travailler !");
                    PerformEmployeeAction(emp);
                    emp.ResetTimer();
                }
            }
        }
    }
    
    // Initialise la liste des employés disponibles
    void InitializeEmployees()
{
    employees.Clear();
    
    // ========================================
    // NIVEAU 1-3 : EMPLOYÉS DE BASE
    // ========================================
    
    employees.Add(new Employee(
        "Jean - Apprenti Menuisier",
        EmployeeType.Crafter,
        300,    // Coût d'embauche
        30,     // Salaire hebdomadaire
        15f,    // Fabrique toutes les 15 secondes
        1       // Niveau requis
    ));
    
    employees.Add(new Employee(
        "Paul - Vendeur Débutant",
        EmployeeType.Seller,
        250,
        25,
        18f,    // Vend toutes les 18 secondes
        1
    ));
    
    employees.Add(new Employee(
        "Luc - Acheteur Junior",
        EmployeeType.Gatherer,
        200,
        20,
        20f,    // Achète toutes les 20 secondes
        2
    ));
    
    // ========================================
    // NIVEAU 4-6 : EMPLOYÉS INTERMÉDIAIRES
    // ========================================
    
    employees.Add(new Employee(
        "Marie - Artisane Confirmée",
        EmployeeType.Crafter,
        600,
        60,
        10f,    // Plus rapide
        4
    ));
    
    employees.Add(new Employee(
        "Sophie - Vendeuse Experte",
        EmployeeType.Seller,
        550,
        55,
        12f,
        4
    ));
    
    employees.Add(new Employee(
        "Thomas - Acheteur Confirmé",
        EmployeeType.Gatherer,
        500,
        50,
        15f,
        5
    ));
    
    // ========================================
    // NIVEAU 7-9 : EMPLOYÉS AVANCÉS
    // ========================================
    
    employees.Add(new Employee(
        "Pierre - Maître Artisan",
        EmployeeType.Crafter,
        1000,
        100,
        7f,     // Très rapide
        7
    ));
    
    employees.Add(new Employee(
        "Claire - Directrice des Ventes",
        EmployeeType.Seller,
        950,
        95,
        8f,
        7
    ));
    
    employees.Add(new Employee(
        "Marc - Chef Acheteur",
        EmployeeType.Gatherer,
        900,
        90,
        10f,
        8
    ));
    
    // ========================================
    // NIVEAU 10+ : EMPLOYÉS D'ÉLITE
    // ========================================
    
    employees.Add(new Employee(
        "Antoine - Artisan Légendaire",
        EmployeeType.Crafter,
        2000,
        200,
        5f,     // Ultra rapide
        10
    ));
    
    employees.Add(new Employee(
        "Isabelle - Vendeuse Experte",
        EmployeeType.Seller,
        1900,
        190,
        6f,
        10
    ));
    
    employees.Add(new Employee(
        "François - Négociateur Expert",
        EmployeeType.Gatherer,
        1800,
        180,
        7f,
        11
    ));
    
    // ========================================
    // NIVEAU 13+ : EMPLOYÉS PREMIUM
    // ========================================
    
    employees.Add(new Employee(
        "Élise - Maître d'Œuvre",
        EmployeeType.Crafter,
        3500,
        350,
        4f,     // Extrêmement rapide
        13
    ));
    
    employees.Add(new Employee(
        "Victor - Directeur Commercial",
        EmployeeType.Seller,
        3300,
        330,
        4.5f,
        13
    ));
    
    employees.Add(new Employee(
        "Camille - Expert en Logistique",
        EmployeeType.Gatherer,
        3000,
        300,
        5f,
        14
    ));
    
    // ========================================
    // NIVEAU 17+ : EMPLOYÉS ULTIME
    // ========================================
    
    employees.Add(new Employee(
        "Alexandre - Génie de l'Artisanat",
        EmployeeType.Crafter,
        5000,
        500,
        3f,     // Super rapide
        17
    ));
    
    employees.Add(new Employee(
        "Charlotte - Reine des Ventes",
        EmployeeType.Seller,
        4800,
        480,
        3.5f,
        17
    ));
    
    employees.Add(new Employee(
        "Maxime - Roi des Négociations",
        EmployeeType.Gatherer,
        4500,
        450,
        4f,
        18
    ));
    
    Debug.Log("" + employees.Count + " employés initialisés");
}

    
    // Effectue l'action de l'employé
    void PerformEmployeeAction(Employee emp)
    {
        // Rafraîchit la référence si elle est nulle
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("GameManager rafraîchi dans EmployeeManager");
        }
    
        if (gameManager == null)
        {
            Debug.LogError("GameManager introuvable !");
            return;
        }
    
        switch (emp.type)
        {
            case EmployeeType.Crafter:
                CraftRandomProduct();
                break;
        
            case EmployeeType.Seller:
                SellRandomProduct();
                break;
        
            case EmployeeType.Gatherer:
                BuyRandomMaterial();
                break;
        }
    }
    
    // Fabrique un produit aléatoire
    void CraftRandomProduct()
    {
        Debug.Log("CraftRandomProduct appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("gameManager est NULL dans CraftRandomProduct !");
            return;
        }
    
        Debug.Log($"Nombre de produits : {gameManager.products.Count}");
    
        // Trouve un produit qu'on peut fabriquer
        List<int> craftableProducts = new List<int>();
    
        for (int i = 0; i < gameManager.products.Count; i++)
        {
            Product prod = gameManager.products[i];
            bool canCraft = true;
        
            foreach (MaterialRequirement req in prod.recipe)
            {
                CraftingMaterial mat = gameManager.GetMaterial(req.materialIndex);
                if (mat == null || !mat.HasEnoughQuantity(req.amount))
                {
                    canCraft = false;
                    break;
                }
            }
        
            if (canCraft)
            {
                craftableProducts.Add(i);
            }
        }
    
        Debug.Log($"Produits fabricables : {craftableProducts.Count}");
    
        // Si au moins un produit peut être fabriqué
        if (craftableProducts.Count > 0)
        {
            int randomIndex = craftableProducts[Random.Range(0, craftableProducts.Count)];
            gameManager.CraftProduct(randomIndex);
            Debug.Log("Employé a fabriqué : " + gameManager.products[randomIndex].productName);
        }
        else
        {
            Debug.LogWarning("Aucun produit ne peut être fabriqué (manque de matériaux)");
        }
    }
    
    // Vend un produit aléatoire
    void SellRandomProduct()
    {
        Debug.Log("🔍 SellRandomProduct appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("gameManager est NULL dans SellRandomProduct !");
            return;
        }
    
        // Trouve un produit en stock
        List<int> availableProducts = new List<int>();
    
        for (int i = 0; i < gameManager.products.Count; i++)
        {
            if (gameManager.products[i].quantity > 0)
            {
                availableProducts.Add(i);
            }
        }
    
        Debug.Log($"Produits disponibles à la vente : {availableProducts.Count}");
    
        // Si au moins un produit est disponible
        if (availableProducts.Count > 0)
        {
            int randomIndex = availableProducts[Random.Range(0, availableProducts.Count)];
            gameManager.SellProduct(randomIndex, 1);
            Debug.Log("Employé a vendu : " + gameManager.products[randomIndex].productName);
        }
        else
        {
            Debug.LogWarning("Aucun produit en stock à vendre");
        }
    }
    
    // Achète un matériau aléatoire
    void BuyRandomMaterial()
    {
        Debug.Log("🔍 BuyRandomMaterial appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("gameManager est NULL dans BuyRandomMaterial !");
            return;
        }
    
        Debug.Log($"Argent disponible : {gameManager.playerMoney}€");
    
        // Achète un matériau aléatoire si assez d'argent
        int randomIndex = Random.Range(0, gameManager.craftingMaterials.Count);
        CraftingMaterial mat = gameManager.craftingMaterials[randomIndex];
    
        Debug.Log($"Tentative d'achat : {mat.materialName} pour {mat.price}€");
    
        if (gameManager.HasEnoughMoney(mat.price))
        {
            gameManager.BuyMaterial(randomIndex, 1);
            Debug.Log("Employé a acheté : " + mat.materialName);
        }
        else
        {
            Debug.LogWarning($"Pas assez d'argent pour acheter {mat.materialName} ({mat.price}€)");
        }
    }
    
    // Embauche un employé
    public void HireEmployee(int employeeIndex)
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        if (employeeIndex < 0 || employeeIndex >= employees.Count)
        {
            Debug.LogError("Index d'employé invalide !");
            return;
        }
        
        Employee emp = employees[employeeIndex];
        
        if (emp.isHired)
        {
            Debug.LogWarning("" + emp.employeeName + " est déjà embauché !");
            return;
        }
        
        if (!gameManager.HasEnoughMoney(emp.hireCost))
        {
            Debug.LogWarning("Pas assez d'argent pour embaucher " + emp.employeeName + " !");
            return;
        }
        
        gameManager.RemoveMoney(emp.hireCost);
        emp.Hire();
        
        // Sauvegarde après embauche
        SaveEmployees();
        
        Debug.Log("" + emp.employeeName + " embauché pour " + emp.hireCost + "€ !");
    }
    
    // Active/désactive un employé
    public void ToggleEmployee(int employeeIndex)
    {
        if (employeeIndex < 0 || employeeIndex >= employees.Count)
        {
            Debug.LogError("Index d'employé invalide !");
            return;
        }
        
        Employee emp = employees[employeeIndex];
        
        if (!emp.isHired)
        {
            Debug.LogWarning("" + emp.employeeName + " n'est pas embauché !");
            return;
        }
        
        emp.ToggleActive();
        
        // Sauvegarde après changement d'état
        SaveEmployees();
    }
    
    // Paie tous les salaires (appelé chaque semaine)
    public void PaySalaries()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        int totalSalaries = 0;
        
        foreach (Employee emp in employees)
        {
            if (emp.isHired)
            {
                totalSalaries += emp.salaryPerWeek;
            }
        }
        
        if (totalSalaries > 0)
        {
            gameManager.RemoveMoney(totalSalaries);
            Debug.Log("Salaires payés : " + totalSalaries + "€");
        }
    }
    
    // Obtenir un employé
    public Employee GetEmployee(int index)
    {
        if (index >= 0 && index < employees.Count)
        {
            return employees[index];
        }
        return null;
    }
    
    // ===== SAUVEGARDE ET CHARGEMENT =====
    
    // Sauvegarde les employés
    public void SaveEmployees()
    {
        int hiredCount = 0;
        
        for (int i = 0; i < employees.Count; i++)
        {
            Employee emp = employees[i];
            
            if (emp.isHired)
            {
                string key = "Employee_" + i;
                PlayerPrefs.SetInt(key + "_Hired", 1);
                PlayerPrefs.SetInt(key + "_Active", emp.isActive ? 1 : 0);
                hiredCount++;
            }
            else
            {
                string key = "Employee_" + i;
                PlayerPrefs.SetInt(key + "_Hired", 0);
            }
        }
        
        PlayerPrefs.SetInt("EmployeeCount", hiredCount);
        PlayerPrefs.Save();
        
        Debug.Log("Employés sauvegardés (" + hiredCount + " embauchés)");
    }
    
    // Charge les employés
    public void LoadEmployees()
    {
        if (!PlayerPrefs.HasKey("EmployeeCount"))
        {
            Debug.Log("Aucune sauvegarde d'employés trouvée");
            return;
        }
        
        int savedCount = PlayerPrefs.GetInt("EmployeeCount");
        Debug.Log("Chargement de " + savedCount + " employés...");
        
        for (int i = 0; i < employees.Count; i++)
        {
            string key = "Employee_" + i;
            
            if (PlayerPrefs.HasKey(key + "_Hired"))
            {
                bool isHired = PlayerPrefs.GetInt(key + "_Hired") == 1;
                
                if (isHired)
                {
                    employees[i].isHired = true;
                    employees[i].isActive = PlayerPrefs.GetInt(key + "_Active") == 1;
                    
                    Debug.Log("" + employees[i].employeeName + " chargé (Actif: " + employees[i].isActive + ")");
                }
            }
        }
        
        Debug.Log("Employés chargés avec succès !");
    }
    
    // Réinitialise tous les employés (nouvelle partie)
    public void ResetEmployees()
    {
        Debug.Log("Réinitialisation des employés...");
        
        // Supprime toutes les sauvegardes d'employés
        PlayerPrefs.DeleteKey("EmployeeCount");
        
        for (int i = 0; i < employees.Count; i++)
        {
            string key = "Employee_" + i;
            PlayerPrefs.DeleteKey(key + "_Hired");
            PlayerPrefs.DeleteKey(key + "_Active");
            
            // Réinitialise l'employé en mémoire
            employees[i].isHired = false;
            employees[i].isActive = false;
            employees[i].productionTimer = 0f;
        }
        
        PlayerPrefs.Save();
        
        Debug.Log("Employés réinitialisés !");
    }
}
