using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PulsingFloor : MonoBehaviour
{
    private const float PulseSpeed = 0.5f;
    private const float WaveFrequency = 0.15f;
    
    [SerializeField] private Color dimColor = Color.white;
    [SerializeField] private Color glowColor = Color.cyan;
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        float phase = Time.time * PulseSpeed - transform.position.y * WaveFrequency;
        float pulse = (Mathf.Sin(phase * Mathf.PI * 2f) + 1f) * 0.5f;
        spriteRenderer.color = Color.Lerp(dimColor, glowColor, pulse);
    }
    
    private void OnDisable()
    {
        spriteRenderer.color = dimColor;
    }
}
