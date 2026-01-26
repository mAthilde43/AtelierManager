using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 50f;          // Vitesse de montée
    public float fadeDuration = 1.5f;      // Durée avant disparition
    public float randomOffsetX = 30f;      // Décalage horizontal aléatoire
    
    private TextMeshProUGUI text;
    private float timer = 0f;
    private Vector3 moveDirection;
    
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        
        // Direction de déplacement avec un petit décalage aléatoire
        float randomX = Random.Range(-randomOffsetX, randomOffsetX);
        moveDirection = new Vector3(randomX, moveSpeed, 0f);
    }
    
    void Update()
    {
        // Monte et se déplace
        transform.position += moveDirection * Time.deltaTime;
        
        // Fade progressif
        timer += Time.deltaTime;
        float alpha = 1f - (timer / fadeDuration);
        
        if (text != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
        
        // Détruit après la durée
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
    
    // Fonction pour initialiser le texte
    public void Initialize(string message, Color color, Vector3 position)
    {
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }
        transform.position = position;
    }
}