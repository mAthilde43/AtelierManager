using UnityEngine;

public class BadgePulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    
    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
    }
    
    void Update()
    {
        // Pulse (respiration)
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;
    }
}