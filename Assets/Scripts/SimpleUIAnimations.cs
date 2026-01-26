using UnityEngine;

public class SimpleUIAnimations : MonoBehaviour
{
    public float bobSpeed = 4f;      // Vitesse du mouvement
    public float bobAmount = 15f;    // Amplitude du mouvement
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.localPosition;
    }
    
    void Update()
    {
        // Mouvement de haut en bas (bobbing)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}