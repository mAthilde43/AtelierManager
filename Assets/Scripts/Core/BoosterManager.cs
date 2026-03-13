using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Énumération des types de boosters
public enum BoosterType
{
    MoneyBoost,        // Multiplicateur d'argent
    XPBoost,           // Multiplicateur d'XP
    InstantCraft       // Production instantanée
}

// Classe pour représenter un booster
[System.Serializable]
public class Booster
{
    public string boosterName;
    public string description;
    public int cost;
    public float duration;         // Durée en secondes
    public BoosterType type;
    public float multiplier;       // Multiplicateur (ex: 1.5 pour +50%)
    public string icon;            // Emoji pour l'icône
    
    public Booster(string name, string desc, int price, float dur, BoosterType t, float mult, string ico)
    {
        boosterName = name;
        description = desc;
        cost = price;
        duration = dur;
        type = t;
        multiplier = mult;
        icon = ico;
    }
}

// Classe pour représenter un booster actif
public class ActiveBooster
{
    public BoosterType type;
    public float timeRemaining;
    public float multiplier;
    public string icon;
    
    public ActiveBooster(BoosterType t, float duration, float mult, string ico)
    {
        type = t;
        timeRemaining = duration;
        multiplier = mult;
        icon = ico;
    }
}

public class BoosterManager : MonoBehaviour
{
    // Singleton
    private static BoosterManager instance;
    public static BoosterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BoosterManager>();
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
    
    // === BOOSTERS DISPONIBLES ===
    public List<Booster> availableBoosters = new List<Booster>();
    
    // === BOOSTERS ACTIFS ===
    public List<ActiveBooster> activeBoosters = new List<ActiveBooster>();
    
    // === APPARITION DE BOOSTER ===
    private float spawnTimer = 0f;
    private float nextSpawnTime = 120f;  // Prochain spawn dans 2 minutes
    private Booster currentOffer = null; // Booster actuellement proposé
    private float offerTimer = 0f;       // Temps restant pour l'offre
    
    // === UI - BOOSTER OFFER (côté écran) ===
    public GameObject boosterOfferPanel;     // Panneau qui slide depuis le côté
    public TextMeshProUGUI offerIconText;    // Emoji du booster
    public TextMeshProUGUI offerNameText;    // Nom du booster
    public TextMeshProUGUI offerTimerText;   // "45s restantes"
    public Button offerButton;               // Bouton pour ouvrir la popup
    
    // === UI - POPUP CONFIRMATION ===
    public GameObject boosterPopup;          // Popup de confirmation
    public TextMeshProUGUI popupIconText;    // Emoji
    public TextMeshProUGUI popupNameText;    // Nom
    public TextMeshProUGUI popupDescText;    // Description
    public TextMeshProUGUI popupCostText;    // Coût
    public Button buyButton;                 // Bouton "Acheter"
    public Button cancelButton;              // Bouton "Ignorer"
    
    // === UI - BADGES ACTIFS ===
    public Transform badgeContainer;         // Container pour les badges
    public GameObject badgePrefab;           // Prefab d'un badge
    private List<GameObject> activeBadges = new List<GameObject>();
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        InitializeBoosters();
        
        // Cache tout au démarrage
        if (boosterOfferPanel != null)
            boosterOfferPanel.SetActive(false);
        if (boosterPopup != null)
            boosterPopup.SetActive(false);
        
        // Configure les boutons
        if (offerButton != null)
            offerButton.onClick.AddListener(OpenBoosterPopup);
        if (buyButton != null)
            buyButton.onClick.AddListener(BuyCurrentBooster);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseBoosterPopup);
        
        Debug.Log("⚡ BoosterManager initialisé");
    }
    
    void Update()
    {
        // Gestion de l'apparition des boosters
        if (currentOffer == null)
        {
            spawnTimer += Time.deltaTime;
            
            if (spawnTimer >= nextSpawnTime)
            {
                SpawnRandomBooster();
                spawnTimer = 0f;
                nextSpawnTime = Random.Range(1800f, 3600f); // Entre 30 min et 1h
            }
        }
        else
        {
            // Décompte du timer de l'offre
            offerTimer -= Time.deltaTime;
            
            if (offerTimerText != null)
            {
                offerTimerText.text = Mathf.CeilToInt(offerTimer) + "s";
            }
            
            // Si le temps est écoulé, retire l'offre
            if (offerTimer <= 0f)
            {
                HideBoosterOffer();
            }
        }
        
        // Décompte des boosters actifs
        for (int i = activeBoosters.Count - 1; i >= 0; i--)
        {
            activeBoosters[i].timeRemaining -= Time.deltaTime;
            
            // Met à jour le badge correspondant
            if (i < activeBadges.Count && activeBadges[i] != null)
            {
                UpdateBadge(activeBadges[i], activeBoosters[i]);
            }
            
            // Si le booster est terminé
            if (activeBoosters[i].timeRemaining <= 0f)
            {
                OnBoosterExpired(activeBoosters[i]);
                
                // Détruit le badge
                if (i < activeBadges.Count && activeBadges[i] != null)
                {
                    Destroy(activeBadges[i]);
                    activeBadges.RemoveAt(i);
                }
                
                activeBoosters.RemoveAt(i);
            }
        }
    }
    
    // Initialise les boosters disponibles
    void InitializeBoosters()
    {
        availableBoosters.Clear();
        
        // Booster d'argent
        availableBoosters.Add(new Booster(
            "Boost d'argent",
            "Gagnez x1.5 d'argent sur toutes les ventes pendant 60 secondes !",
            500,
            60f,
            BoosterType.MoneyBoost,
            1.5f,
            ""
        ));
        
        // Booster d'XP
        availableBoosters.Add(new Booster(
            "Boost d'XP",
            "Gagnez x2 d'XP sur toutes les actions pendant 60 secondes !",
            400,
            60f,
            BoosterType.XPBoost,
            2.0f,
            ""
        ));
        
        // Booster de production
        availableBoosters.Add(new Booster(
            "Production éclair",
            "Fabrication instantanée pendant 60 secondes !",
            600,
            60f,
            BoosterType.InstantCraft,
            1.0f,
            ""
        ));
        
        Debug.Log("" + availableBoosters.Count + " boosters disponibles");
    }
    
    // Fait apparaître un booster aléatoire
    void SpawnRandomBooster()
    {
        if (availableBoosters.Count == 0) return;
        
        // Choisis un booster aléatoire
        currentOffer = availableBoosters[Random.Range(0, availableBoosters.Count)];
        offerTimer = 60f; // 60 secondes pour décider
        
        // Affiche le panneau
        if (boosterOfferPanel != null)
        {
            boosterOfferPanel.SetActive(true);
            
            if (offerIconText != null)
                offerIconText.text = currentOffer.icon;
            if (offerNameText != null)
                offerNameText.text = currentOffer.boosterName;
        }
        
        Debug.Log("Booster disponible : " + currentOffer.boosterName + " pour " + offerTimer + "s");
    }
    
    // Cache l'offre de booster
    void HideBoosterOffer()
    {
        if (boosterOfferPanel != null)
            boosterOfferPanel.SetActive(false);
        
        currentOffer = null;
        Debug.Log("Offre de booster expirée");
    }
    
    // Ouvre la popup de confirmation
    void OpenBoosterPopup()
    {
        if (currentOffer == null || boosterPopup == null) return;
        
        boosterPopup.SetActive(true);
        
        // Remplit les infos
        if (popupIconText != null)
            popupIconText.text = currentOffer.icon;
        if (popupNameText != null)
            popupNameText.text = currentOffer.boosterName;
        if (popupDescText != null)
            popupDescText.text = currentOffer.description;
        if (popupCostText != null)
            popupCostText.text = currentOffer.cost + " €";
        
        // Son d'ouverture
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchase();
        }
    }
    
    // Ferme la popup et retire l'offre
    void CloseBoosterPopup()
    {
        if (boosterPopup != null)
            boosterPopup.SetActive(false);
    
        // Cache aussi l'offre de booster (on a refusé)
        HideBoosterOffer();
    
        Debug.Log("Booster ignoré");
    }

    
    // Achète le booster actuel
    // Achète le booster actuel
    void BuyCurrentBooster()
    {
        // ===== PROTECTION CONTRE NULL =====
        if (currentOffer == null)
        {
            Debug.LogWarning("⚠️ Aucun booster à acheter !");
            CloseBoosterPopup();
            return;
        }
    
        // Sauvegarde le booster dans une variable locale
        Booster boosterToActivate = currentOffer;
    
        // Vérifie si on a assez d'argent
        if (gameManager != null && gameManager.HasEnoughMoney(boosterToActivate.cost))
        {
            // Retire l'argent
            gameManager.RemoveMoney(boosterToActivate.cost);
        
            // Active le booster
            ActivateBooster(boosterToActivate);
        
            Debug.Log("✅ Booster acheté : " + boosterToActivate.boosterName);
        
            // Son de succès
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySuccess();
            }
        
            // Feedback visuel
            if (FeedbackManager.Instance != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                FeedbackManager.Instance.ShowSuccess("⚡ " + boosterToActivate.boosterName.ToUpper() + " ACTIVÉ !", screenCenter);
            }
        
            // Cache la popup et l'offre APRÈS avoir utilisé currentOffer
            CloseBoosterPopup();
            HideBoosterOffer();
            
        }
        else
        {
            Debug.LogWarning("⚠️ Pas assez d'argent pour " + boosterToActivate.boosterName);
        
            // Son d'erreur
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    }

    
    // Active un booster
    void ActivateBooster(Booster booster)
    {
        // Ajoute le booster aux boosters actifs
        ActiveBooster activeBooster = new ActiveBooster(booster.type, booster.duration, booster.multiplier, booster.icon);
        activeBoosters.Add(activeBooster);
        
        // Crée le badge
        CreateBadge(activeBooster);
        
        Debug.Log("" + booster.boosterName + " actif pour " + booster.duration + " secondes");
    }
    
    // Crée un badge pour un booster actif
    void CreateBadge(ActiveBooster booster)
    {
        if (badgeContainer == null || badgePrefab == null) return;
        
        GameObject badge = Instantiate(badgePrefab, badgeContainer);
        activeBadges.Add(badge);
        
        // Configure le badge
        UpdateBadge(badge, booster);
    }
    
    // Met à jour un badge
    void UpdateBadge(GameObject badge, ActiveBooster booster)
    {
        if (badge == null) return;
        
        // Trouve les composants
        TextMeshProUGUI iconText = badge.transform.Find("IconText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timerText = badge.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
        
        if (iconText != null)
            iconText.text = booster.icon;
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(booster.timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(booster.timeRemaining % 60f);
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }
    
    // Appelé quand un booster expire
    void OnBoosterExpired(ActiveBooster booster)
    {
        Debug.Log("Booster expiré : " + booster.type);
        
        // Son d'expiration
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayError();
        }
    }
    
    // Retourne le multiplicateur d'argent actuel
    public float GetMoneyMultiplier()
    {
        float multiplier = 1.0f;
        
        foreach (ActiveBooster booster in activeBoosters)
        {
            if (booster.type == BoosterType.MoneyBoost)
            {
                multiplier *= booster.multiplier;
            }
        }
        
        return multiplier;
    }
    
    // Retourne le multiplicateur d'XP actuel
    public float GetXPMultiplier()
    {
        float multiplier = 1.0f;
        
        foreach (ActiveBooster booster in activeBoosters)
        {
            if (booster.type == BoosterType.XPBoost)
            {
                multiplier *= booster.multiplier;
            }
        }
        
        return multiplier;
    }
    
    // Vérifie si le boost de production instantanée est actif
    public bool IsInstantCraftActive()
    {
        foreach (ActiveBooster booster in activeBoosters)
        {
            if (booster.type == BoosterType.InstantCraft)
            {
                return true;
            }
        }
        
        return false;
    }
}
