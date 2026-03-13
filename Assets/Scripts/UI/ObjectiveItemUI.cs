using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Slider progressBar;
    public GameObject checkmarkIcon;
    
    private Objective currentObjective;
    
    // Affiche un objectif
    public void DisplayObjective(Objective obj)
    {
        if (obj == null) return;
        
        currentObjective = obj;
        
        // Nom
        if (objectiveNameText != null)
        {
            objectiveNameText.text = obj.objectiveName;
        }
        
        // Description
        if (descriptionText != null)
        {
            descriptionText.text = obj.description;
        }
        
        // Progression
        UpdateProgress();
    }
    
    // Met à jour la progression
    public void UpdateProgress()
    {
        if (currentObjective == null) return;
        
        // Texte de progression
        if (progressText != null)
        {
            progressText.text = currentObjective.currentProgress + " / " + currentObjective.targetAmount;
        }
        
        // Barre de progression
        if (progressBar != null)
        {
            progressBar.value = currentObjective.GetProgressPercentage();
        }
        
        // Icône de validation si complété
        if (checkmarkIcon != null)
        {
            checkmarkIcon.SetActive(currentObjective.isCompleted);
        }
        
        // Change la couleur si complété
        if (currentObjective.isCompleted && objectiveNameText != null)
        {
            objectiveNameText.color = new Color(0.2f, 0.8f, 0.2f); // Vert
        }
    }
}