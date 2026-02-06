using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    // Singleton
    private static FeedbackManager instance;
    public static FeedbackManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FeedbackManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    // === PREFABS ===
    public GameObject floatingTextPrefab;
    
    // === CANVAS ===
    private Canvas canvas;
    
    void Start()
    {
        // Trouve le Canvas principal
        canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("Aucun Canvas trouvé !");
        }
        
        Debug.Log("FeedbackManager initialisé");
    }
    
    // === TEXTES FLOTTANTS ===
    
    public void ShowFloatingText(string message, Vector3 worldPosition, Color color)
    {
        if (floatingTextPrefab == null || canvas == null) return;
        
        // Instancie le texte
        GameObject textObj = Instantiate(floatingTextPrefab, canvas.transform);
        
        // Initialise
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.Initialize(message, color, worldPosition);
        }
    }
    
    // Fonctions pratiques pour différents types de feedback
    
    public void ShowMoneyGain(int amount, Vector3 position)
    {
        Color goldColor = new Color(1f, 0.84f, 0f); // Or
        ShowFloatingText("+" + amount + "€", position, goldColor);
    }
    
    public void ShowMoneyLoss(int amount, Vector3 position)
    {
        Color redColor = new Color(1f, 0.3f, 0.3f); // Rouge
        ShowFloatingText("-" + amount + "€", position, redColor);
    }
    
    public void ShowXPGain(int amount, Vector3 position)
    {
        Color purpleColor = new Color(0.7f, 0.4f, 1f); // Violet
        ShowFloatingText("+" + amount + " XP", position, purpleColor);
    }
    
    public void ShowSuccess(string message, Vector3 position)
    {
        Color greenColor = new Color(0.3f, 1f, 0.3f); // Vert
        ShowFloatingText(message, position, greenColor);
    }
    
    public void ShowError(string message, Vector3 position)
    {
        Color redColor = new Color(1f, 0.3f, 0.3f); // Rouge
        ShowFloatingText(message, position, redColor);
    }
    
    // === FLASH D'ÉCRAN ===
    
    public void FlashScreen(Color color, float duration = 0.2f)
    {
        // On implémentera ça dans la prochaine étape
    }
}