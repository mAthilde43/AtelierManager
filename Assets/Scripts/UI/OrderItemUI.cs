using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI clientNameText;
    public TextMeshProUGUI orderIDText;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timeText;
    public Slider timeProgressBar;
    public Button deliverButton;
    
    private Order currentOrder;
    private OrderManager orderManager;
    private GameManager gameManager;
    
    // Initialise l'item
    public void Initialize(Order order, OrderManager om, GameManager gm)
    {
        currentOrder = order;
        orderManager = om;
        gameManager = gm;
        
        // Configure le bouton
        deliverButton.onClick.RemoveAllListeners();
        deliverButton.onClick.AddListener(OnDeliverClicked);
        
        // Affiche les infos
        UpdateDisplay();
    }
    
    // Met à jour l'affichage
    public void UpdateDisplay()
    {
        if (currentOrder == null) return;
        
        // Nom du client
        if (clientNameText != null)
        {
            clientNameText.text = currentOrder.clientName;
        }
        
        // ID de commande
        if (orderIDText != null)
        {
            orderIDText.text = currentOrder.orderID;
        }
        
        // Liste des produits demandés
        if (requirementsText != null && gameManager != null)
        {
            string reqText = "";
            foreach (OrderRequirement req in currentOrder.requirements)
            {
                Product prod = gameManager.GetProduct(req.productIndex);
                if (prod != null)
                {
                    reqText += "• " + req.quantity + "x " + prod.productName + "\n";
                }
            }
            requirementsText.text = reqText.TrimEnd('\n');
        }
        
        // Récompense
        if (rewardText != null)
        {
            rewardText.text = "Récompense: " + currentOrder.reward + "€";
        }
        
        // Temps restant
        if (timeText != null)
        {
            timeText.text = "" + currentOrder.GetFormattedTimeRemaining();
            
            // Change la couleur selon le temps restant
            float progress = currentOrder.GetTimeProgress();
            if (progress > 0.5f)
            {
                timeText.color = new Color(0f, 0.8f, 0f);  // Vert
            }
            else if (progress > 0.25f)
            {
                timeText.color = new Color(1f, 0.6f, 0f);  // Orange
            }
            else
            {
                timeText.color = new Color(1f, 0.2f, 0.2f);  // Rouge
            }
        }
        
        // Barre de temps
        if (timeProgressBar != null)
        {
            timeProgressBar.value = currentOrder.GetTimeProgress();
            
            // Change la couleur de la barre selon le temps
            Image fillImage = timeProgressBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float progress = currentOrder.GetTimeProgress();
                if (progress > 0.5f)
                {
                    fillImage.color = new Color(0f, 0.8f, 0f);  // Vert
                }
                else if (progress > 0.25f)
                {
                    fillImage.color = new Color(1f, 0.6f, 0f);  // Orange
                }
                else
                {
                    fillImage.color = new Color(1f, 0.2f, 0.2f);  // Rouge
                }
            }
        }
        
        // Bouton Livrer
        if (deliverButton != null && gameManager != null)
        {
            // Vérifie si on peut livrer (stock suffisant)
            bool canDeliver = currentOrder.CanBeCompleted(gameManager);
            deliverButton.interactable = canDeliver;
            
            // Change le texte si pas possible
            TextMeshProUGUI buttonText = deliverButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = canDeliver ? "Livrer" : "Stock insuffisant";
            }
        }
    }
    
    // Bouton Livrer cliqué
    void OnDeliverClicked()
    {
        if (orderManager != null && currentOrder != null)
        {
            orderManager.CompleteOrder(currentOrder);
        }
    }
    
    // Met à jour périodiquement (appelé par OrdersUI)
    void Update()
    {
        UpdateDisplay();
    }
}
