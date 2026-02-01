using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    // === PARAMÈTRES ===
    public float eventCheckInterval = 30f;    // Vérification tous les 30 secondes
    public float eventChance = 0.3f;          // 30% de chance qu'un événement arrive
    
    private float eventTimer = 0f;
    
    // === RÉFÉRENCES ===
    private GameManager gameManager;
    private TimeManager timeManager;
    
    // === ÉVÉNEMENTS ===
    private List<GameEvent> possibleEvents = new List<GameEvent>();
    private GameEvent currentEvent = null;    // Événement en cours
    
    // === UI ===
    public GameObject eventPopupPanel;        // Panneau popup pour les événements
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        timeManager = FindObjectOfType<TimeManager>();
        
        InitializeEvents();
        
        Debug.Log("🎲 EventManager initialisé avec " + possibleEvents.Count + " événements possibles");
    }
    
    void Update()
    {
        eventTimer += Time.deltaTime;
        
        // Vérifie périodiquement si un événement doit se produire
        if (eventTimer >= eventCheckInterval)
        {
            eventTimer = 0f;
            CheckForEvent();
        }
    }
    
    // Initialise tous les événements possibles
    void InitializeEvents()
    {
        // === ÉVÉNEMENTS SIMPLES (automatiques) ===
        
        // Événements positifs
        possibleEvents.Add(new GameEvent(
            "🎉 Client satisfait",
            "Un client régulier vous laisse un pourboire généreux !",
            EventType.MoneyGain,
            100
        ));
        
        possibleEvents.Add(new GameEvent(
            "📦 Livraison bonus",
            "Votre fournisseur vous offre des matériaux gratuits !",
            EventType.MaterialGain,
            2
        ));
        
        possibleEvents.Add(new GameEvent(
            "💰 Subvention",
            "Vous recevez une aide de l'artisanat local !",
            EventType.MoneyGain,
            200
        ));
        
        possibleEvents.Add(new GameEvent(
            "Salon du meuble",
            "Votre stand au salon attire beaucoup de clients !",
            EventType.MoneyGain,
            800
        ));
    
       
    
        possibleEvents.Add(new GameEvent(
            "Client influent",
            "Un influenceur partage votre travail sur les réseaux !",
            EventType.MoneyGain,
            400
        ));
        
        
        // Événements négatifs
        possibleEvents.Add(new GameEvent(
            "⚡ Panne électrique",
            "Une coupure d'électricité ralentit votre production.",
            EventType.MoneyLoss,
            50
        ));
        
        possibleEvents.Add(new GameEvent(
            "🔨 Outil cassé",
            "Un de vos outils s'est cassé, réparation nécessaire.",
            EventType.MoneyLoss,
            80
        ));
        possibleEvents.Add(new GameEvent(
            "Panne d'électricité",
            "Une panne ralentit votre production aujourd'hui.",
            EventType.MoneyLoss,
            150
        ));

        possibleEvents.Add(new GameEvent(
            "Concurrence déloyale",
            "Un concurrent vend moins cher, vous perdez des clients.",
            EventType.MoneyLoss,
            300));
        
        possibleEvents.Add(new GameEvent(
            "📉 Matériau défectueux",
            "Un lot de matériaux reçu était défectueux.",
            EventType.MaterialLoss,
            1
        ));
        
        
        // === ÉVÉNEMENTS AVEC CHOIX ===

        possibleEvents.Add(new GameEvent(
            "🎯 Commande urgente",
            "Un client propose une grosse commande urgente. C'est risqué mais potentiellement très rentable !",
            EventType.SpecialOrder,
            "Accepter la commande",  -150, 400,  // Coût 150€, gain 400€ = +250€ net
            "Refuser poliment", 0, 0
        ));

        possibleEvents.Add(new GameEvent(
            "🏪 Nouveau fournisseur",
            "Un nouveau fournisseur propose des prix très bas, mais la qualité est incertaine.",
            EventType.Opportunity,
            "Tester ce fournisseur", -100, 300,  // Coût 100€, gain 300€ = +200€ net
            "Rester fidèle", 0, 50  // Pas de coût, petit gain de fidélité
        ));

        possibleEvents.Add(new GameEvent(
            "🎓 Formation proposée",
            "Une formation professionnelle vous permettrait d'améliorer vos compétences. Investissement pour l'avenir ?",
            EventType.Opportunity,
            "Suivre la formation", -200, 0,  // Coût 200€, pas de gain immédiat (bénéfice à long terme)
            "Refuser", 0, 0
        ));

        possibleEvents.Add(new GameEvent(
            "🚨 Inspection surprise",
            "Une inspection surprise ! Vous devez mettre aux normes ou prendre le risque d'une amende.",
            EventType.Breakdown,
            "Mise aux normes", -150, 0,  // Coût 150€, évite l'amende
            "Prendre le risque", 0, -100  // Pas de coût initial mais amende possible de 100€
        ));

        possibleEvents.Add(new GameEvent(
            "💎 Matériaux premium",
            "Un lot de matériaux de qualité supérieure est disponible à prix réduit aujourd'hui seulement !",
            EventType.Opportunity,
            "Acheter le lot", -250, 150,  // Coût 250€, valeur 400€ donc gain net futur de 150€
            "Laisser passer", 0, 0
        ));
    }
    
    // Vérifie si un événement doit se produire
    void CheckForEvent()
    {
        // Pas d'événement si un événement est déjà en cours
        if (currentEvent != null) return;
        
        // Tire au hasard
        float roll = Random.Range(0f, 1f);
        
        if (roll <= eventChance)
        {
            TriggerRandomEvent();
        }
    }
    
    // Déclenche un événement aléatoire
    void TriggerRandomEvent()
    {
        if (possibleEvents.Count == 0) return;
        
        // Choisit un événement au hasard
        int randomIndex = Random.Range(0, possibleEvents.Count);
        currentEvent = possibleEvents[randomIndex];
        
        Debug.Log("🎲 Événement déclenché : " + currentEvent.eventName);
        
        // Affiche le popup d'événement
        ShowEventPopup();
    }
    
    // Affiche le popup d'événement
    void ShowEventPopup()
    {
        if (eventPopupPanel != null)
        {
            eventPopupPanel.SetActive(true);
        
            // Son d'événement
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEvent();
            }
        
            // Le script EventPopupUI gérera l'affichage
            EventPopupUI popupUI = eventPopupPanel.GetComponent<EventPopupUI>();
            if (popupUI != null)
            {
                popupUI.DisplayEvent(currentEvent, this);
            }
        }
    }
    
    // Appelée quand le joueur clique sur un bouton de choix
    public void OnChoiceMade(int choiceNumber)
    {
        if (currentEvent == null) return;
        
        if (currentEvent.requiresChoice)
        {
            // Applique l'effet du choix
            if (choiceNumber == 1)
            {
                ApplyChoiceEffect(currentEvent.choice1Cost, currentEvent.choice1Effect);
                Debug.Log("✅ Choix 1 : " + currentEvent.choice1Text);
            }
            else if (choiceNumber == 2)
            {
                ApplyChoiceEffect(currentEvent.choice2Cost, currentEvent.choice2Effect);
                Debug.Log("✅ Choix 2 : " + currentEvent.choice2Text);
            }
        }
        else
        {
            // Événement automatique
            ApplyEventEffect();
        }
        
        // Ferme le popup et réinitialise
        CloseEventPopup();
    }
    
    // Applique l'effet d'un événement automatique
    void ApplyEventEffect()
    {
        if (currentEvent == null) return;
        
        switch (currentEvent.type)
        {
            case EventType.MoneyGain:
                gameManager.AddMoney(currentEvent.value);
                break;
                
            case EventType.MoneyLoss:
                gameManager.RemoveMoney(currentEvent.value);
                break;
                
            case EventType.MaterialGain:
                // Ajoute des matériaux aléatoires
                AddRandomMaterials(currentEvent.value);
                break;
                
            case EventType.MaterialLoss:
                // Retire des matériaux aléatoires
                RemoveRandomMaterials(currentEvent.value);
                break;
        }
        
        gameManager.RefreshAllUI();
    }
    
    // Applique l'effet d'un choix
    // Applique l'effet d'un choix
    void ApplyChoiceEffect(int cost, int effect)
    {
        // Applique le coût (négatif) ou gain (positif) immédiat
        if (cost < 0)
        {
            // Coût négatif = dépense
            gameManager.RemoveMoney(-cost); // -(-150) = 150
            Debug.Log("💸 Dépense : " + (-cost) + "€");
        }
        else if (cost > 0)
        {
            // Coût positif = gain
            gameManager.AddMoney(cost);
            Debug.Log("💰 Gain immédiat : +" + cost + "€");
        }
    
        // Applique l'effet supplémentaire (gain/perte)
        if (effect != 0)
        {
            if (effect > 0)
            {
                gameManager.AddMoney(effect);
                Debug.Log("💰 Gain supplémentaire : +" + effect + "€");
            }
            else if (effect < 0)
            {
                gameManager.RemoveMoney(-effect);
                Debug.Log("💸 Perte supplémentaire : " + (-effect) + "€");
            }
        }
    
        gameManager.RefreshAllUI();
    }
    
    // Ajoute des matériaux aléatoires
    void AddRandomMaterials(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMatIndex = Random.Range(0, gameManager.craftingMaterials.Count);
            gameManager.craftingMaterials[randomMatIndex].AddQuantity(1);
        }
    }
    
    // Retire des matériaux aléatoires
    void RemoveRandomMaterials(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Trouve un matériau qui a du stock
            List<int> availableMats = new List<int>();
            for (int j = 0; j < gameManager.craftingMaterials.Count; j++)
            {
                if (gameManager.craftingMaterials[j].quantity > 0)
                {
                    availableMats.Add(j);
                }
            }
            
            if (availableMats.Count > 0)
            {
                int randomIndex = availableMats[Random.Range(0, availableMats.Count)];
                gameManager.craftingMaterials[randomIndex].RemoveQuantity(1);
            }
        }
    }
    
    // Ferme le popup d'événement
    void CloseEventPopup()
    {
        if (eventPopupPanel != null)
        {
            eventPopupPanel.SetActive(false);
        }
        
        currentEvent = null;
    }
    
    // Fonction pour forcer un événement (pour tester)
    public void ForceEvent()
    {
        TriggerRandomEvent();
    }
}