using TMPro;
using UnityEngine;

public abstract class Tower_Base : MonoBehaviour
{
    private static Tower_Base currentInteractable;
    private const float MinTowerCardWidth = 2f;
    private const float MaxTowerCardWidth = 3.2f;
    private const float MinTowerCardHeight = 0.5f;
    private const float TowerCardHorizontalPadding = 0.35f;
    private const float TowerCardVerticalPadding = 0.14f;

    [Header("Tower UI")]
    [SerializeField] private TextMeshProUGUI towerText;

    [Header("Tower Info")]
    [SerializeField] protected string towerName = "Tower";
    [SerializeField] protected string rewardText = "+20% Stat";

    protected bool playerInRange = false;
    protected Stats currentStats;
    private bool used = false;
    private RectTransform _towerTextRect;
    private RectTransform _towerBackgroundRect;

    private void Start()
    {
        if (towerText != null)
        {
            ConfigureTowerText();
            SetTowerText(towerName);
        }
    }

    private void ConfigureTowerText()
    {
        _towerTextRect = towerText.rectTransform;
        _towerBackgroundRect = towerText.transform.parent as RectTransform;

        towerText.textWrappingMode = TextWrappingModes.Normal;
        towerText.alignment = TextAlignmentOptions.Center;
    }

    private void SetTowerText(string text)
    {
        towerText.text = text;

        var preferredSize = towerText.GetPreferredValues(text, MaxTowerCardWidth - TowerCardHorizontalPadding, 0f);
        var cardWidth = Mathf.Clamp(preferredSize.x + TowerCardHorizontalPadding, MinTowerCardWidth, MaxTowerCardWidth);
        var cardHeight = Mathf.Max(preferredSize.y + TowerCardVerticalPadding, MinTowerCardHeight);
        var cardSize = new Vector2(cardWidth, cardHeight);
        var textSize = new Vector2(cardWidth - TowerCardHorizontalPadding, cardHeight - TowerCardVerticalPadding);

        if (_towerTextRect != null)
        {
            _towerTextRect.sizeDelta = textSize;
        }

        if (_towerBackgroundRect != null)
        {
            _towerBackgroundRect.sizeDelta = cardSize;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Tower_Base.TryInteractCurrent();
        }
    }


    public static void TryInteractCurrent()
    {
        if (currentInteractable != null)
        {
            currentInteractable.TryInteract();
        }
    }

    private void TryInteract()
    {
        if (!playerInRange || currentStats == null || used)
        {
            return;
        }

        ApplyEffect(currentStats);

        used = true;

        if (towerText != null)
        {
            SetTowerText(rewardText);
        }

        Destroy(gameObject, 1.5f);
    }

    protected abstract void ApplyEffect(Stats stats);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            currentStats = collision.GetComponent<Stats>();
            currentInteractable = this;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            currentStats = null;
            if (currentInteractable == this)
            {
                currentInteractable = null;
            }
        }
    }

    protected virtual void OnDisable()
    {
        if (currentInteractable == this)
        {
            currentInteractable = null;
        }
    }
}
