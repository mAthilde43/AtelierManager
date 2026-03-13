using UnityEngine;
using TMPro;

public class ObjectivesPanelToggle : MonoBehaviour
{
    public RectTransform objectivesPanel;
    public TextMeshProUGUI buttonText;
    
    private bool isPanelVisible = true;
    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;
    
    void Start()
    {
        
        isPanelVisible = false;
        if (objectivesPanel != null)
        {
            // Sauvegarde les positions
            visiblePosition = objectivesPanel.anchoredPosition;
            hiddenPosition = new Vector2(visiblePosition.x + 400f, visiblePosition.y); // Décale de 400px à droite
            
            objectivesPanel.anchoredPosition = hiddenPosition;
        }
        
        UpdateButtonText();
    }
    
    public void TogglePanel()
    {
        if (objectivesPanel == null) return;
        
        isPanelVisible = !isPanelVisible;
        
        // Anime le déplacement (simple)
        objectivesPanel.anchoredPosition = isPanelVisible ? visiblePosition : hiddenPosition;
        
        UpdateButtonText();
        
        Debug.Log("Panneau objectifs : " + (isPanelVisible ? "Affiché" : "Caché"));
    }
    
    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isPanelVisible ? "►" : "◄";
        }
    }
}