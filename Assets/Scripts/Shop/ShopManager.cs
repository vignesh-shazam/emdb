using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Items")]
    [SerializeField]
    private List<ShopItem> shopItems =
        new List<ShopItem>();

    public IReadOnlyList<ShopItem> ShopItems =>
        shopItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeShop();
    }

    // =========================
    // INITIALIZE SHOP
    // =========================

    private void InitializeShop()
    {
        if (shopItems == null)
        {
            shopItems =
                new List<ShopItem>();
        }

        if (shopItems.Count == 0)
        {
            shopItems.Add(
                new ShopItem(
                    "food_001",
                    "Bread",
                    50,
                    25
                )
            );

            shopItems.Add(
                new ShopItem(
                    "food_002",
                    "Milk",
                    60,
                    30
                )
            );

            shopItems.Add(
                new ShopItem(
                    "tool_001",
                    "Tool Kit",
                    500,
                    250
                )
            );
        }

        Debug.Log(
            "Shop initialized | Items available: " +
            shopItems.Count
        );
    }

    // =========================
    // GET ITEM
    // =========================

    public ShopItem GetItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (ShopItem item in shopItems)
        {
            if (item.ItemId == itemId)
            {
                return item;
            }
        }

        return null;
    }

    // =========================
    // BUY ITEM
    // =========================

    public bool BuyItem(string itemId)
    {
        ShopItem item =
            GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                "Buy failed: Item not found. ID: " +
                itemId
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Buy failed: MoneyManager not found."
            );

            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "Buy failed: InventoryManager not found."
            );

            return false;
        }

        if (!MoneyManager.Instance.CanAfford(
                item.BuyPrice))
        {
            Debug.LogWarning(
                "Buy failed: Insufficient money. " +
                "Item: " +
                item.ItemName +
                " | Required: Rs. " +
                item.BuyPrice.ToString("N0")
            );

            return false;
        }

        // Add item to inventory first.
        bool inventoryAdded =
            InventoryManager.Instance.AddItem(
                item.ItemId,
                item.ItemName,
                1
            );

        if (!inventoryAdded)
        {
            Debug.LogWarning(
                "Buy failed: Could not add " +
                item.ItemName +
                " to inventory."
            );

            return false;
        }

        // Remove money only after inventory succeeds.
        MoneyManager.Instance.RemoveMoney(
            item.BuyPrice
        );

        int inventoryQuantity =
            InventoryManager.Instance.GetQuantity(
                item.ItemId
            );

        Debug.Log(
            "Purchase successful | " +
            "Item: " +
            item.ItemName +
            " | Price: Rs. " +
            item.BuyPrice.ToString("N0") +
            " | Inventory Quantity: " +
            inventoryQuantity
        );

        return true;
    }

    // =========================
    // SELL ITEM
    // =========================

    public bool SellItem(string itemId)
    {
        ShopItem item =
            GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                "Sell failed: Item not found. ID: " +
                itemId
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Sell failed: MoneyManager not found."
            );

            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "Sell failed: InventoryManager not found."
            );

            return false;
        }

        // InventoryManager is the source of truth.
        int inventoryQuantity =
            InventoryManager.Instance.GetQuantity(
                item.ItemId
            );

        if (inventoryQuantity <= 0)
        {
            Debug.LogWarning(
                "Sell failed: Player does not own " +
                item.ItemName +
                "."
            );

            return false;
        }

        bool inventoryRemoved =
            InventoryManager.Instance.RemoveItem(
                item.ItemId,
                1
            );

        if (!inventoryRemoved)
        {
            Debug.LogWarning(
                "Sell failed: Could not remove " +
                item.ItemName +
                " from inventory."
            );

            return false;
        }

        MoneyManager.Instance.AddMoney(
            item.SellPrice
        );

        int remainingQuantity =
            InventoryManager.Instance.GetQuantity(
                item.ItemId
            );

        Debug.Log(
            "Sale successful | " +
            "Item: " +
            item.ItemName +
            " | Price: Rs. " +
            item.SellPrice.ToString("N0") +
            " | Inventory Quantity: " +
            remainingQuantity
        );

        return true;
    }

    // =========================
    // PLAYER QUANTITY
    // =========================

    public int GetPurchasedQuantity(string itemId)
    {
        if (InventoryManager.Instance == null)
        {
            return 0;
        }

        return InventoryManager.Instance.GetQuantity(
            itemId
        );
    }
}