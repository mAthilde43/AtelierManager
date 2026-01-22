using UnityEngine;

// Classe pour représenter un événement de jeu
[System.Serializable]
public class GameEvent
{
    public string eventName;           // Nom de l'événement
    public string description;         // Description
    public EventType type;             // Type d'événement
    public int value;                  // Valeur de l'effet
    public bool requiresChoice;        // L'événement nécessite-t-il un choix ?
    
    // Pour les événements à choix
    public string choice1Text;         // Texte du choix 1
    public string choice2Text;         // Texte du choix 2
    public int choice1Cost;            // Coût du choix 1 (peut être négatif = gain)
    public int choice2Cost;            // Coût du choix 2
    public int choice1Effect;          // Effet du choix 1
    public int choice2Effect;          // Effet du choix 2
    
    // Constructeur pour événements simples (sans choix)
    public GameEvent(string name, string desc, EventType eventType, int effectValue)
    {
        eventName = name;
        description = desc;
        type = eventType;
        value = effectValue;
        requiresChoice = false;
    }
    
    // Constructeur pour événements avec choix
    public GameEvent(string name, string desc, EventType eventType, 
                     string c1Text, int c1Cost, int c1Effect,
                     string c2Text, int c2Cost, int c2Effect)
    {
        eventName = name;
        description = desc;
        type = eventType;
        requiresChoice = true;
        
        choice1Text = c1Text;
        choice1Cost = c1Cost;
        choice1Effect = c1Effect;
        
        choice2Text = c2Text;
        choice2Cost = c2Cost;
        choice2Effect = c2Effect;
    }
}

// Types d'événements possibles
public enum EventType
{
    MoneyGain,           // Gain d'argent
    MoneyLoss,           // Perte d'argent
    MaterialGain,        // Gain de matériaux
    MaterialLoss,        // Perte de matériaux
    SpecialOrder,        // Commande spéciale
    Breakdown,           // Panne/problème
    Opportunity          // Opportunité
}