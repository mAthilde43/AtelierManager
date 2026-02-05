using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // === PARAMÈTRES ===
    [Header("Scale Animation")]
    public float hoverScale = 1.1f;        // Taille au survol
    public float clickScale = 0.95f;       // Taille au clic
    public float animationSpeed = 10f;     // Vitesse d'animation
    
    // === ÉTAT ===
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovering = false;
    private bool isClicking = false;
    
    // === RÉFÉRENCES ===
    private Button button;
    
    void Start()
    {
        // Sauvegarde la taille originale
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        // Récupère le bouton
        button = GetComponent<Button>();
    }
    
    void Update()
    {
        // Animation fluide vers la taille cible
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }
    
    // Quand la souris entre sur le bouton
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isHovering = true;
            UpdateScale();
        }
    }
    
    // Quand la souris sort du bouton
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isClicking = false;
        UpdateScale();
    }
    
    // Quand on clique sur le bouton
    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isClicking = true;
            UpdateScale();
        }
    }
    
    // Quand on relâche le clic
    public void OnPointerUp(PointerEventData eventData)
    {
        isClicking = false;
        UpdateScale();
    }
    
    // Met à jour la taille cible selon l'état
    void UpdateScale()
    {
        if (isClicking)
        {
            // Clic : réduction
            targetScale = originalScale * clickScale;
        }
        else if (isHovering)
        {
            // Hover : agrandissement
            targetScale = originalScale * hoverScale;
        }
        else
        {
            // Normal : taille originale
            targetScale = originalScale;
        }
    }
}
