using UnityEngine;
using TMPro;

public class NotificationBadge : MonoBehaviour
{
    public TextMeshProUGUI badgeText;
    public GameObject badgeObject;
    
    private int notificationCount = 0;
    
    void Start()
    {
        // Cache le badge au départ
        Hide();
    }
    
    // Affiche le badge avec un nombre
    public void Show(int count = 0)
    {
        notificationCount = count;
        
        if (badgeObject != null)
        {
            badgeObject.SetActive(true);
            
            // Affiche le nombre si > 0
            if (badgeText != null)
            {
                if (count > 0)
                {
                    badgeText.text = count.ToString();
                }
                else
                {
                    badgeText.text = "!";
                }
            }
        }
    }
    
    // Cache le badge
    public void Hide()
    {
        notificationCount = 0;
        
        if (badgeObject != null)
        {
            badgeObject.SetActive(false);
        }
    }
    
    // Incrémente le compteur
    public void Increment()
    {
        notificationCount++;
        Show(notificationCount);
    }
    
    // Décrémente le compteur
    public void Decrement()
    {
        notificationCount--;
        
        if (notificationCount <= 0)
        {
            Hide();
        }
        else
        {
            Show(notificationCount);
        }
    }
    
    // Obtenir le compteur actuel
    public int GetCount()
    {
        return notificationCount;
    }
}