using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    // === TEMPS ===
    public int currentDay = 1;           // Jour actuel
    public int currentWeek = 1;          // Semaine actuelle
    public float dayDuration = 10f;      // Durée d'un jour en secondes (10s pour tester)
    
    private float dayTimer = 0f;         // Timer interne
    private bool timeIsRunning = true;   // Le temps s'écoule-t-il ?
    
    // === ÉVÉNEMENTS ===
    public int dailyIncome = 50;        // Revenu quotidien automatique
    public int weeklyCost = 200;        // Coût hebdomadaire (loyer, etc.)
    
    // === UI ===
    public TextMeshProUGUI dayText;      // Affichage du jour
    public TextMeshProUGUI weekText;     // Affichage de la semaine
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        // Récupère le GameManager
        gameManager = FindObjectOfType<GameManager>();
        
        // Affiche le jour initial
        UpdateTimeDisplay();
        
        Debug.Log("TimeManager initialisé - Jour " + currentDay + ", Semaine " + currentWeek);
    }

	public void OnGameLoaded()
{
    // Met à jour l'affichage avec les valeurs chargées
    UpdateTimeDisplay();
    Debug.Log("📅 Temps chargé : Jour " + currentDay + ", Semaine " + currentWeek);
}
    
    void Update()
    {
        if (!timeIsRunning) return;
        
        // Incrémente le timer
        dayTimer += Time.deltaTime;
        
        // Si un jour est passé
        if (dayTimer >= dayDuration)
        {
            dayTimer = 0f;
            AdvanceDay();
        }
    }
    
    // Fonction pour avancer d'un jour
    void AdvanceDay()
    {
        currentDay++;
        
        // Tous les 7 jours = nouvelle semaine
        if (currentDay > 7)
        {
            currentDay = 1;
            currentWeek++;
            OnNewWeek();
        }
        
        OnNewDay();
    }
    
    // Appelée quand un nouveau jour commence
void OnNewDay()
{
    UpdateTimeDisplay();
    Debug.Log("Nouveau jour : Jour " + currentDay + " de la semaine " + currentWeek);

    // Revenu quotidien
    if (gameManager != null)
    {
        gameManager.AddMoney(dailyIncome);
        Debug.Log("Revenu quotidien : +" + dailyIncome + "€");
    }
    
    // Reset les objectifs quotidiens
    if (ObjectiveManager.Instance != null)
    {
        ObjectiveManager.Instance.ResetDailyObjectives();
    }
    
    // Commence un nouveau jour pour les stats
    if (StatsManager.Instance != null)
    {
        StatsManager.Instance.StartNewDay();
    }
}
    
    // Appelée quand une nouvelle semaine commence
void OnNewWeek()
{
    Debug.Log("Nouvelle semaine " + currentWeek + " !");

    // Coût hebdomadaire (loyer, salaires, etc.)
    if (gameManager != null)
    {
        gameManager.RemoveMoney(weeklyCost);
        Debug.Log("Charges hebdomadaires : -" + weeklyCost + "€");
    }
    
    // Reset les objectifs hebdomadaires
    if (ObjectiveManager.Instance != null)
    {
        ObjectiveManager.Instance.ResetWeeklyObjectives();
    }
    
    // Incrémente le compteur de semaines
    if (StatsManager.Instance != null)
    {
        StatsManager.Instance.OnNewWeek();
    }
    
    // Paie les salaires des employés
    if (EmployeeManager.Instance != null)
    {
        EmployeeManager.Instance.PaySalaries();
    }
    // ================================
}

    
    // Met à jour l'affichage du temps
    void UpdateTimeDisplay()
    {
        if (dayText != null)
        {
            dayText.text = "Jour " + currentDay;
        }
        
        if (weekText != null)
        {
            weekText.text = "Semaine " + currentWeek;
        }
    }
    
    // Fonction pour mettre en pause le temps
    public void PauseTime()
    {
        timeIsRunning = false;
        Debug.Log("Temps mis en pause");
    }
    
    // Fonction pour reprendre le temps
    public void ResumeTime()
    {
        timeIsRunning = true;
        Debug.Log("Temps repris");
    }
    
    // Fonction pour passer au jour suivant immédiatement (pour tester)
    public void SkipDay()
    {
        AdvanceDay();
        Debug.Log("Jour passé manuellement");
    }
    
    
}