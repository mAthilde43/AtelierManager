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
        Debug.Log("🔍 EmployeeManager.Awake() appelé sur " + gameObject.name);
    
        if (instance != null && instance != this)
        {
            Debug.LogWarning("⚠️ Instance d'EmployeeManager déjà existante ! Destruction de " + gameObject.name);
            Destroy(gameObject);
            return;
        }
    
        Debug.Log("✅ EmployeeManager devient l'instance singleton");
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
        
        Debug.Log("🧑‍💼 EmployeeManager initialisé avec " + employees.Count + " employés");
    }
    
    void Update()
    {
        // Met à jour tous les employés
        foreach (Employee emp in employees)
        {
            if (emp.isHired && emp.isActive)
            {
                Debug.Log($"🔍 {emp.employeeName} est actif et travaille");  
            
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
        
        // === CRAFTERS (Fabricants) ===
        employees.Add(new Employee(
            "Jean - Menuisier",
            EmployeeType.Crafter,
            500,    // Coût d'embauche
            50,     // Salaire hebdomadaire
            10f     // Fabrique toutes les 10 secondes
        ));
        
        employees.Add(new Employee(
            "Marie - Artisane",
            EmployeeType.Crafter,
            1000,
            100,
            8f      // Plus rapide
        ));
        
        // === SELLERS (Vendeurs) ===
        employees.Add(new Employee(
            "Paul - Vendeur",
            EmployeeType.Seller,
            400,
            40,
            12f     // Vend toutes les 12 secondes
        ));
        
        employees.Add(new Employee(
            "Sophie - Vendeuse",
            EmployeeType.Seller,
            800,
            80,
            10f
        ));
        
        // === GATHERERS (Acheteurs) ===
        employees.Add(new Employee(
            "Luc - Acheteur",
            EmployeeType.Gatherer,
            300,
            30,
            15f     // Achète toutes les 15 secondes
        ));
    }
    
    // Effectue l'action de l'employé
    void PerformEmployeeAction(Employee emp)
    {
        // Rafraîchit la référence si elle est nulle
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            Debug.Log("🔄 GameManager rafraîchi dans EmployeeManager");
        }
    
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager introuvable !");
            return;
        }
    
        switch (emp.type)
        {
            case EmployeeType.Crafter:
                // Fabrique un produit aléatoire si possible
                CraftRandomProduct();
                break;
        
            case EmployeeType.Seller:
                // Vend un produit aléatoire si possible
                SellRandomProduct();
                break;
        
            case EmployeeType.Gatherer:
                // Achète un matériau aléatoire
                BuyRandomMaterial();
                break;
        }
    }

    
    // Fabrique un produit aléatoire
    void CraftRandomProduct()
    {
        Debug.Log("🔍 CraftRandomProduct appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("❌ gameManager est NULL dans CraftRandomProduct !");
            return;
        }
    
        Debug.Log($"🔍 Nombre de produits : {gameManager.products.Count}");
    
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
    
        Debug.Log($"🔍 Produits fabricables : {craftableProducts.Count}");
    
        // Si au moins un produit peut être fabriqué
        if (craftableProducts.Count > 0)
        {
            int randomIndex = craftableProducts[Random.Range(0, craftableProducts.Count)];
            gameManager.CraftProduct(randomIndex);
            Debug.Log("🔨 Employé a fabriqué : " + gameManager.products[randomIndex].productName);
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun produit ne peut être fabriqué (manque de matériaux)");
        }
    }

    
    // Vend un produit aléatoire
    void SellRandomProduct()
    {
        Debug.Log("🔍 SellRandomProduct appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("❌ gameManager est NULL dans SellRandomProduct !");
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
    
        Debug.Log($"🔍 Produits disponibles à la vente : {availableProducts.Count}");
    
        // Si au moins un produit est disponible
        if (availableProducts.Count > 0)
        {
            int randomIndex = availableProducts[Random.Range(0, availableProducts.Count)];
            gameManager.SellProduct(randomIndex, 1);
            Debug.Log("💼 Employé a vendu : " + gameManager.products[randomIndex].productName);
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun produit en stock à vendre");
        }
    }

    
    // Achète un matériau aléatoire
    void BuyRandomMaterial()
    {
        Debug.Log("🔍 BuyRandomMaterial appelé");
    
        if (gameManager == null)
        {
            Debug.LogError("❌ gameManager est NULL dans BuyRandomMaterial !");
            return;
        }
    
        Debug.Log($"🔍 Argent disponible : {gameManager.playerMoney}€");
    
        // Achète un matériau aléatoire si assez d'argent
        int randomIndex = Random.Range(0, gameManager.craftingMaterials.Count);
        CraftingMaterial mat = gameManager.craftingMaterials[randomIndex];
    
        Debug.Log($"🔍 Tentative d'achat : {mat.materialName} pour {mat.price}€");
    
        if (gameManager.HasEnoughMoney(mat.price))
        {
            gameManager.BuyMaterial(randomIndex, 1);
            Debug.Log("📦 Employé a acheté : " + mat.materialName);
        }
        else
        {
            Debug.LogWarning($"⚠️ Pas assez d'argent pour acheter {mat.materialName} ({mat.price}€)");
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
            Debug.LogError("❌ Index d'employé invalide !");
            return;
        }
        
        Employee emp = employees[employeeIndex];
        
        // Vérifie si déjà embauché
        if (emp.isHired)
        {
            Debug.LogWarning("⚠️ " + emp.employeeName + " est déjà embauché !");
            return;
        }
        
        // Vérifie si assez d'argent
        if (!gameManager.HasEnoughMoney(emp.hireCost))
        {
            Debug.LogWarning("⚠️ Pas assez d'argent pour embaucher " + emp.employeeName + " !");
            return;
        }
        
        // Retire l'argent
        gameManager.RemoveMoney(emp.hireCost);
        
        // Embauche
        emp.Hire();
        
        Debug.Log("✅ " + emp.employeeName + " embauché pour " + emp.hireCost + "€ !");
    }
    
    // Active/désactive un employé
    public void ToggleEmployee(int employeeIndex)
    {
        if (employeeIndex < 0 || employeeIndex >= employees.Count)
        {
            Debug.LogError("❌ Index d'employé invalide !");
            return;
        }
        
        Employee emp = employees[employeeIndex];
        
        if (!emp.isHired)
        {
            Debug.LogWarning("⚠️ " + emp.employeeName + " n'est pas embauché !");
            return;
        }
        
        emp.ToggleActive();
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
            Debug.Log("💸 Salaires payés : " + totalSalaries + "€");
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
}
