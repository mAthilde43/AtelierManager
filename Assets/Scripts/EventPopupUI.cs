using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventPopupUI : MonoBehaviour
{
    // === UI ===
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button choice1Button;
    public Button choice2Button;
    public Button okButton;
    
    private EventManager eventManager;
    private GameEvent currentEvent;
    
    // Affiche un événement
    public void DisplayEvent(GameEvent gameEvent, EventManager manager)
    {
        currentEvent = gameEvent;
        eventManager = manager;

        // Affiche le titre et la description
        titleText.text = gameEvent.eventName;
        descriptionText.text = gameEvent.description;

        if (gameEvent.requiresChoice)
        {
            // Événement avec choix : affiche les 2 boutons
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(true);
            okButton.gameObject.SetActive(false);

            // Configure le texte des boutons avec les gains/pertes affichés
            TextMeshProUGUI choice1Text = choice1Button.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI choice2Text = choice2Button.GetComponentInChildren<TextMeshProUGUI>();

            // Construit le texte du choix 1 avec coût et effet
            string choice1String = gameEvent.choice1Text + "\n";
            if (gameEvent.choice1Cost < 0)
            {
                choice1String += "💸 Coût: " + (-gameEvent.choice1Cost) + "€";
            }
            else if (gameEvent.choice1Cost > 0)
            {
                choice1String += "💰 Gain: +" + gameEvent.choice1Cost + "€";
            }

            if (gameEvent.choice1Effect > 0)
            {
                choice1String += " → +" + gameEvent.choice1Effect + "€";
            }
            else if (gameEvent.choice1Effect < 0)
            {
                choice1String += " → " + gameEvent.choice1Effect + "€";
            }

            // Construit le texte du choix 2 avec coût et effet
            string choice2String = gameEvent.choice2Text + "\n";
            if (gameEvent.choice2Cost < 0)
            {
                choice2String += "💸 Coût: " + (-gameEvent.choice2Cost) + "€";
            }
            else if (gameEvent.choice2Cost > 0)
            {
                choice2String += "💰 Gain: +" + gameEvent.choice2Cost + "€";
            }

            if (gameEvent.choice2Effect > 0)
            {
                choice2String += " → +" + gameEvent.choice2Effect + "€";
            }
            else if (gameEvent.choice2Effect < 0)
            {
                choice2String += " → " + gameEvent.choice2Effect + "€";
            }

            choice1Text.text = choice1String;
            choice2Text.text = choice2String;

            // Configure les listeners
            choice1Button.onClick.RemoveAllListeners();
            choice2Button.onClick.RemoveAllListeners();

            choice1Button.onClick.AddListener(() => OnChoice1Clicked());
            choice2Button.onClick.AddListener(() => OnChoice2Clicked());
        }
        else
        {
            // ← CETTE PARTIE MANQUAIT DANS TON CODE !
            
            // Événement automatique : affiche juste le bouton OK
            choice1Button.gameObject.SetActive(false);
            choice2Button.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);

            // Configure le listener
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() => OnOKClicked());
        }
    }

    void OnChoice1Clicked()
    {
        Debug.Log("🔵 Choix 1 cliqué");
        if (eventManager != null)
        {
            eventManager.OnChoiceMade(1);
        }
        else
        {
            Debug.LogError("❌ EventManager est null !");
        }
    }
    
    void OnChoice2Clicked()
    {
        Debug.Log("🔵 Choix 2 cliqué");
        if (eventManager != null)
        {
            eventManager.OnChoiceMade(2);
        }
        else
        {
            Debug.LogError("❌ EventManager est null !");
        }
    }
    
    void OnOKClicked()
    {
        Debug.Log("🔵 OK cliqué");
        if (eventManager != null)
        {
            eventManager.OnChoiceMade(0);
        }
        else
        {
            Debug.LogError("❌ EventManager est null !");
        }
    }
}