using TMPro;
using UnityEngine;

public abstract class Tower_Base : MonoBehaviour
{
    private static Tower_Base currentInteractable;
    [Header("Tower UI")]
    [SerializeField] private TextMeshProUGUI towerText;

    [Header("Tower Info")]
    [SerializeField] protected string towerName = "Tower";
    [SerializeField] protected string rewardText = "+20% Stat";

    protected bool playerInRange = false;
    protected Stats currentStats;
    private bool used = false;
    private void Start()
    {
        if (towerText != null)
        {
            towerText.text = towerName;
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
            towerText.text = rewardText;
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
