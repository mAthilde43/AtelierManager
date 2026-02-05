using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    // Singleton
    private static ComboManager instance;
    public static ComboManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ComboManager>();
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
    
    // === COMBO ===
    public int currentCombo = 0;           // Combo actuel
    public float comboTimer = 0f;          // Temps restant
    public float comboTimeLimit = 5f;     // Temps max entre 2 ventes
    
    // === BONUS ===
    public float[] comboMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.5f }; // Bonus par niveau de combo
    
    // === UI ===
    public GameObject comboPanel;          // Panneau de combo
    public TextMeshProUGUI comboText;      // Texte "COMBO x3 !"
    public TextMeshProUGUI bonusText;      // Texte "+30% argent"
    
    void Start()
    {
        // Cache le panneau au démarrage
        if (comboPanel != null)
        {
            comboPanel.SetActive(false);
        }
        
        Debug.Log("🔥 ComboManager initialisé");
    }
    
    void Update()
    {
        // Décompte du timer si combo actif
        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            
            // Si le temps est écoulé, reset le combo
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }
    
    // Appelée quand on vend un produit
    public void OnProductSold()
    {
        // Augmente le combo
        currentCombo++;
        
        // Reset le timer
        comboTimer = comboTimeLimit;
        
        // Affiche le combo
        ShowCombo();
        
        Debug.Log("🔥 COMBO x" + currentCombo + " !");
    }
    
    // Retourne le multiplicateur actuel
    public float GetComboMultiplier()
    {
        if (currentCombo <= 0) return 1.0f;
        
        // Limite au max du tableau
        int index = Mathf.Min(currentCombo - 1, comboMultipliers.Length - 1);
        return comboMultipliers[index];
    }
    
    // Affiche le combo à l'écran
    void ShowCombo()
    {
        if (comboPanel != null && currentCombo >= 2)
        {
            comboPanel.SetActive(true);
            
            if (comboText != null)
            {
                comboText.text = "🔥 COMBO x" + currentCombo + " !";
            }
            
            if (bonusText != null)
            {
                float bonusPercent = (GetComboMultiplier() - 1f) * 100f;
                bonusText.text = "+" + Mathf.RoundToInt(bonusPercent) + "% argent";
            }
        }
    }
    
    // Reset le combo
    void ResetCombo()
    {
        if (currentCombo >= 2)
        {
            Debug.Log("❌ Combo perdu ! (était à x" + currentCombo + ")");
        
            // Son d'échec si disponible
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    
        currentCombo = 0;
        comboTimer = 0f;
    
        // Cache le panneau
        if (comboPanel != null)
        {
            comboPanel.SetActive(false);
        }
    }

}
