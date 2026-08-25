using UnityEngine;

public class ItemUsageManager : MonoBehaviour
{
    public static ItemUsageManager Instance { get; private set; }

    [Header("Bread Effect")]
    [SerializeField] private float breadHungerRestore = 20f;

    [Header("Milk Effect")]
    [SerializeField] private float milkHungerRestore = 10f;
    [SerializeField] private float milkEnergyRestore = 5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log(
            "ItemUsageManager initialized."
        );
    }

    // =========================
    // VALIDATION
    // =========================

    public bool CanUseItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "CanUseItem failed: Item ID is empty."
            );

            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "CanUseItem failed: InventoryManager not found."
            );

            return false;
        }

        bool hasItem =
            InventoryManager.Instance.HasItem(itemId);

        if (!hasItem)
        {
            Debug.LogWarning(
                $"Cannot use item. " +
                $"Item not available: {itemId}"
            );

            return false;
        }

        return true;
    }

    // =========================
    // USE ITEM
    // =========================

    public bool UseItem(string itemId)
    {
        if (!CanUseItem(itemId))
        {
            return false;
        }

        if (PlayerLifeManager.Instance == null)
        {
            Debug.LogError(
                "Use item failed: " +
                "PlayerLifeManager not found."
            );

            return false;
        }

        bool removed =
            InventoryManager.Instance.RemoveItem(
                itemId,
                1
            );

        if (!removed)
        {
            Debug.LogWarning(
                $"Use item failed: " +
                $"Could not remove item: {itemId}"
            );

            return false;
        }

        ApplyItemEffect(itemId);

        Debug.Log(
            $"Item used successfully | " +
            $"Item ID: {itemId} | " +
            $"Remaining Quantity: " +
            InventoryManager.Instance.GetQuantity(
                itemId
            )
        );

        return true;
    }

    // =========================
    // ITEM EFFECT
    // =========================

    private void ApplyItemEffect(string itemId)
    {
        switch (itemId)
        {
            case "food_001":
                UseBread();
                break;

            case "food_002":
                UseMilk();
                break;

            case "tool_001":
                UseToolKit();
                break;

            default:
                Debug.LogWarning(
                    $"No item effect configured for: {itemId}"
                );
                break;
        }
    }

    // =========================
    // BREAD
    // =========================

    private void UseBread()
    {
        PlayerLifeManager.Instance.IncreaseHunger(
            breadHungerRestore
        );

        Debug.Log(
            $"Bread used | " +
            $"Hunger +{breadHungerRestore}"
        );
    }

    // =========================
    // MILK
    // =========================

    private void UseMilk()
    {
        PlayerLifeManager.Instance.IncreaseHunger(
            milkHungerRestore
        );

        PlayerLifeManager.Instance.RestoreEnergy(
            milkEnergyRestore
        );

        Debug.Log(
            $"Milk used | " +
            $"Hunger +{milkHungerRestore} | " +
            $"Energy +{milkEnergyRestore}"
        );
    }

    // =========================
    // TOOL KIT
    // =========================

    private void UseToolKit()
    {
        Debug.Log(
            "Tool Kit used. " +
            "No effect configured yet."
        );
    }
}