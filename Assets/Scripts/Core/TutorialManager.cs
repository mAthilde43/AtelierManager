using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Singleton
    private static TutorialManager instance;
    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TutorialManager>();
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
    
    // === ÉTAT DU TUTORIEL ===
    public bool isTutorialActive = false;
    public int currentStep = 0;
    
    // === UI ===
    public GameObject tutorialPanel;         // Panel principal
    public TextMeshProUGUI tutorialTitle;    // Titre
    public TextMeshProUGUI tutorialText;     // Texte explicatif
    public TextMeshProUGUI stepCounter;      // "Étape 1/5"
    public Button nextButton;                // Bouton "Suivant"
    public Button skipButton;                // Bouton "Passer"
    
    // === HIGHLIGHT ===
    public GameObject highlightArrow;        // Flèche qui pointe (optionnel)
    
    // === ÉTAPES DU TUTORIEL ===
    private string[] stepTitles = {
        "Bienvenue dans votre atelier !",
        "• Acheter des matériaux",
        "• Fabriquer un produit",
        "• Vendre votre création",
        "Félicitations !"
    };
    
    private string[] stepTexts = {
        "Vous êtes maintenant propriétaire d'un atelier de meubles !\n\n" +
        "• Votre objectif : acheter des matériaux, fabriquer des produits et les vendre pour faire prospérer votre entreprise.\n\n" +
        "• Gagnez de l'XP pour débloquer de nouvelles recettes et progresser !",
        
        "Pour commencer, vous avez besoin de matériaux !\n\n" +
        "• Allez dans l'onglet BOUTIQUE\n" +
        "• Achetez du Bois de chêne (x3)\n" +
        "• Achetez du Vernis (x1)\n\n" +
        "Ces matériaux vous permettront de fabriquer votre première Table en chêne !",
        
        "Maintenant, passons à la fabrication !\n\n" +
        "• Allez dans l'onglet ATELIER\n" +
        "• Sélectionnez 'Table en chêne'\n" +
        "• Cliquez sur 'Fabriquer'\n\n" +
        "La table sera ajoutée à votre stock de produits finis !",
        
        "Il est temps de gagner de l'argent !\n\n" +
        "• Allez dans l'onglet VENTE\n" +
        "• Sélectionnez votre Table en chêne\n" +
        "• Cliquez sur 'Vendre'\n\n" +
        "Vous gagnerez de l'argent ET de l'XP pour progresser !",
        
        "Bravo ! Vous maîtrisez les bases !\n\n" +
        "• Continuez à acheter, fabriquer et vendre\n" +
        "• Montez de niveau pour débloquer de nouveaux produits\n" +
        "• Développez votre empire du meuble !\n\n" +
        "Bonne chance, artisan !"
    };
    
    void Start()
    {
        // Vérifie si c'est la première fois
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            StartTutorial();
        }
        else
        {
            // Cache tout
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
            if (highlightArrow != null)
                highlightArrow.SetActive(false);
        }
        
        // Configure les boutons
        if (nextButton != null)
            nextButton.onClick.AddListener(NextStep);
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);
        
        Debug.Log("TutorialManager initialisé");
    }
    
    // Démarre le tutoriel
    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = 0;
        
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
        
        // Met le jeu en pause
        Time.timeScale = 0f;
        
        ShowStep(currentStep);
        
        Debug.Log("Tutoriel démarré");
    }
    
    // Affiche une étape
    void ShowStep(int step)
    {
        if (step < 0 || step >= stepTitles.Length) return;
        
        // Met à jour le texte
        if (tutorialTitle != null)
            tutorialTitle.text = stepTitles[step];
        
        if (tutorialText != null)
            tutorialText.text = stepTexts[step];
        
        if (stepCounter != null)
            stepCounter.text = "Étape " + (step + 1) + "/" + stepTitles.Length;
        
        // Change le texte du bouton sur la dernière étape
        if (nextButton != null)
        {
            TextMeshProUGUI buttonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = (step == stepTitles.Length - 1) ? "COMMENCER !" : "Suivant";
            }
        }
        
        // Gère la flèche (optionnel)
        if (highlightArrow != null)
        {
            // Cache la flèche pour l'intro et la fin
            highlightArrow.SetActive(step >= 1 && step <= 3);
        }
        
        Debug.Log("Étape " + (step + 1) + "/" + stepTitles.Length);
    }
    
    // Passe à l'étape suivante
    void NextStep()
    {
        currentStep++;
        
        if (currentStep >= stepTitles.Length)
        {
            CompleteTutorial();
        }
        else
        {
            ShowStep(currentStep);
        }
        
        // Son
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchase();
        }
    }
    
    // Termine le tutoriel
    void CompleteTutorial()
    {
        isTutorialActive = false;
        
        // Cache tout
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
        if (highlightArrow != null)
            highlightArrow.SetActive(false);
        
        // Reprend le jeu
        Time.timeScale = 1f;
        
        // Sauvegarde que le tutoriel est terminé
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Tutoriel terminé !");
        
        // Feedback
        if (FeedbackManager.Instance != null)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            FeedbackManager.Instance.ShowSuccess("C'EST PARTI !", screenCenter);
        }
        
        // Son de succès
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySuccess();
        }
    }
    
    // Passe le tutoriel
    void SkipTutorial()
    {
        CompleteTutorial();
        Debug.Log("Tutoriel ignoré");
    }
    
    // Pour réinitialiser (debug)
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();
        Debug.Log("Tutoriel réinitialisé");
    }
}
