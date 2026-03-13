using UnityEngine;
using System.Collections.Generic;

public class OrdersUI : MonoBehaviour
{
    [Header("References")]
    public Transform ordersContainer;
    public GameObject orderItemPrefab;
    
    private OrderManager orderManager;
    private GameManager gameManager;
    private List<OrderItemUI> orderItems = new List<OrderItemUI>();
    
    void Start()
    {
        orderManager = OrderManager.Instance;
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Génère l'UI des commandes
        RefreshOrdersDisplay();
    }
    
    // Rafraîchit l'affichage des commandes
    public void RefreshOrdersDisplay()
    {
        if (orderManager == null || ordersContainer == null || orderItemPrefab == null)
        {
            return;
        }
        
        // Nettoie les anciens items
        foreach (OrderItemUI item in orderItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        orderItems.Clear();
        
        // Crée les items pour chaque commande active
        foreach (Order order in orderManager.activeOrders)
        {
            // Ne pas afficher les commandes terminées ou échouées
            if (order.isCompleted || order.isFailed) continue;
            
            GameObject itemObj = Instantiate(orderItemPrefab, ordersContainer);
            OrderItemUI itemUI = itemObj.GetComponent<OrderItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Initialize(order, orderManager, gameManager);
                orderItems.Add(itemUI);
            }
        }
        
        Debug.Log("" + orderItems.Count + " commandes affichées");
    }
    
    // Met à jour tous les affichages (appelé périodiquement)
    void Update()
    {
        // Rafraîchit toutes les 0.5 secondes pour détecter les nouvelles commandes
        if (Time.frameCount % 30 == 0)
        {
            // Vérifie si le nombre de commandes a changé
            int activeOrdersCount = 0;
            foreach (Order order in orderManager.activeOrders)
            {
                if (!order.isCompleted && !order.isFailed)
                {
                    activeOrdersCount++;
                }
            }
            
            // Si le nombre a changé, rafraîchit l'affichage complet
            if (activeOrdersCount != orderItems.Count)
            {
                RefreshOrdersDisplay();
            }
        }
    }
}
