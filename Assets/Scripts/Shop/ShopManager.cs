using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Items")]
    [SerializeField]
    private List<ShopItem> shopItems =
        new List<ShopItem>();

    private readonly Dictionary<string, int> purchasedItems =
        new Dictionary<string, int>();

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

    private void InitializeShop()
    {
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
            $"Shop initialized | " +
            $"Items available: {shopItems.Count}"
        );
    }

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

    public bool BuyItem(string itemId)
    {
        ShopItem item = GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"Buy failed: Item not found. ID: {itemId}"
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

        if (!MoneyManager.Instance.CanAfford(
                item.BuyPrice))
        {
            Debug.LogWarning(
                $"Buy failed: Insufficient money. " +
                $"Item: {item.ItemName} | " +
                $"Required: Rs. {item.BuyPrice:N0}"
            );

            return false;
        }

        MoneyManager.Instance.RemoveMoney(
            item.BuyPrice
        );

        AddPurchasedItem(item.ItemId);

        Debug.Log(
            $"Purchase successful | " +
            $"Item: {item.ItemName} | " +
            $"Price: Rs. {item.BuyPrice:N0} | " +
            $"Quantity: {GetPurchasedQuantity(item.ItemId)}"
        );

        return true;
    }

    public bool SellItem(string itemId)
    {
        ShopItem item = GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"Sell failed: Item not found. ID: {itemId}"
            );

            return false;
        }

        int currentQuantity =
            GetPurchasedQuantity(itemId);

        if (currentQuantity <= 0)
        {
            Debug.LogWarning(
                $"Sell failed: Player does not own " +
                $"{item.ItemName}."
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

        RemovePurchasedItem(item.ItemId);

        MoneyManager.Instance.AddMoney(
            item.SellPrice
        );

        Debug.Log(
            $"Sale successful | " +
            $"Item: {item.ItemName} | " +
            $"Price: Rs. {item.SellPrice:N0} | " +
            $"Quantity: {GetPurchasedQuantity(item.ItemId)}"
        );

        return true;
    }

    private void AddPurchasedItem(string itemId)
    {
        if (purchasedItems.ContainsKey(itemId))
        {
            purchasedItems[itemId]++;
        }
        else
        {
            purchasedItems.Add(
                itemId,
                1
            );
        }
    }

    private void RemovePurchasedItem(string itemId)
    {
        if (!purchasedItems.ContainsKey(itemId))
        {
            return;
        }

        purchasedItems[itemId]--;

        if (purchasedItems[itemId] <= 0)
        {
            purchasedItems.Remove(itemId);
        }
    }

    public int GetPurchasedQuantity(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        if (purchasedItems.TryGetValue(
                itemId,
                out int quantity))
        {
            return quantity;
        }

        return 0;
    }
}