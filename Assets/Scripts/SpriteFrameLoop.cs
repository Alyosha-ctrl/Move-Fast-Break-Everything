using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFrameLoop : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesPerSecond = 12f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        int frameIndex = Mathf.FloorToInt(Time.time * framesPerSecond) % frames.Length;
        spriteRenderer.sprite = frames[frameIndex];
    }
}
