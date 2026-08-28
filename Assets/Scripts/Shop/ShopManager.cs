using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    // =========================
    // EVENTS
    // =========================

    public static event Action OnShopUpdated;
    public static event Action OnShopUpgrade;

    // =========================
    // SHOP ITEMS
    // =========================

    [Header("Shop Items")]
    [SerializeField]
    private List<ShopItem> shopItems =
        new List<ShopItem>();

    // =========================
    // SHOP UPGRADE
    // =========================

    [Header("Shop Upgrade")]
    [SerializeField]
    private int shopUpgradeLevel = 1;

    [SerializeField]
    private int maximumUpgradeLevel = 10;

    [SerializeField]
    private int baseUpgradeCost = 1000;

    [SerializeField]
    private float upgradeCostMultiplier = 1.5f;

    // =========================
    // REVENUE
    // =========================

    [Header("Revenue Improvement")]
    [SerializeField]
    private float baseRevenueMultiplier = 1f;

    [SerializeField]
    private float revenueIncreasePerUpgrade = 0.10f;

    // =========================
    // CUSTOMER GROWTH
    // =========================

    [Header("Customer Growth")]
    [SerializeField]
    private float baseCustomerGrowthMultiplier = 1f;

    [SerializeField]
    private float customerGrowthPerUpgrade = 0.10f;

    // =========================
    // PURCHASE TRACKING
    // =========================

    // Items currently owned by the player
    private readonly Dictionary<string, int> purchasedItems =
        new Dictionary<string, int>();

    // Items purchased specifically for the current customer
    private readonly Dictionary<string, int> currentCustomerPurchases =
        new Dictionary<string, int>();

    // =========================
    // PUBLIC PROPERTIES
    // =========================

    public IReadOnlyList<ShopItem> ShopItems =>
        shopItems;

    public int ShopUpgradeLevel =>
        shopUpgradeLevel;

    public int MaximumUpgradeLevel =>
        maximumUpgradeLevel;

    public bool CanUpgradeShop =>
        shopUpgradeLevel < maximumUpgradeLevel;

    public float RevenueMultiplier =>
        baseRevenueMultiplier +
        ((shopUpgradeLevel - 1) *
        revenueIncreasePerUpgrade);

    public float CustomerGrowthMultiplier =>
        baseCustomerGrowthMultiplier +
        ((shopUpgradeLevel - 1) *
        customerGrowthPerUpgrade);

    // =========================
    // AWAKE
    // =========================

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
    // INITIALIZE
    // =========================

    private void InitializeShop()
    {
        // -------------------------
        // SHOP LEVEL VALIDATION
        // -------------------------

        if (shopUpgradeLevel < 1)
        {
            shopUpgradeLevel = 1;
        }

        if (maximumUpgradeLevel < 1)
        {
            maximumUpgradeLevel = 10;
        }

        if (shopUpgradeLevel >
            maximumUpgradeLevel)
        {
            shopUpgradeLevel =
                maximumUpgradeLevel;
        }

        // -------------------------
        // UPGRADE COST VALIDATION
        // -------------------------

        if (baseUpgradeCost < 0)
        {
            baseUpgradeCost = 0;
        }

        if (upgradeCostMultiplier < 1f)
        {
            upgradeCostMultiplier = 1f;
        }

        // -------------------------
        // REVENUE VALIDATION
        // -------------------------

        if (baseRevenueMultiplier < 0f)
        {
            baseRevenueMultiplier = 1f;
        }

        if (revenueIncreasePerUpgrade < 0f)
        {
            revenueIncreasePerUpgrade = 0f;
        }

        // -------------------------
        // CUSTOMER GROWTH VALIDATION
        // -------------------------

        if (baseCustomerGrowthMultiplier < 0f)
        {
            baseCustomerGrowthMultiplier = 1f;
        }

        if (customerGrowthPerUpgrade < 0f)
        {
            customerGrowthPerUpgrade = 0f;
        }

        // -------------------------
        // DEFAULT ITEMS
        // -------------------------

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
            $"Level: {shopUpgradeLevel} | " +
            $"Items available: {shopItems.Count} | " +
            $"Revenue Multiplier: {RevenueMultiplier:F2}x | " +
            $"Customer Growth: {CustomerGrowthMultiplier:F2}x"
        );

        OnShopUpdated?.Invoke();
    }

    // =========================================================
    // SHOP UPGRADE SYSTEM
    // =========================================================

    // =========================
    // GET UPGRADE COST
    // =========================

    public int GetUpgradeCost()
    {
        if (!CanUpgradeShop)
        {
            return 0;
        }

        float cost =
            baseUpgradeCost *
            Mathf.Pow(
                upgradeCostMultiplier,
                shopUpgradeLevel - 1
            );

        return Mathf.RoundToInt(cost);
    }

    // =========================
    // UPGRADE SHOP
    // =========================

    public bool UpgradeShop()
    {
        if (!CanUpgradeShop)
        {
            Debug.LogWarning(
                "Shop upgrade failed: " +
                "Maximum shop level reached."
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Shop upgrade failed: " +
                "MoneyManager not found."
            );

            return false;
        }

        int upgradeCost =
            GetUpgradeCost();

        if (!MoneyManager.Instance.CanAfford(
                upgradeCost))
        {
            Debug.LogWarning(
                $"Shop upgrade failed: " +
                $"Insufficient money. " +
                $"Required: Rs. {upgradeCost:N0}"
            );

            return false;
        }

        // =========================
        // REMOVE MONEY
        // =========================

        MoneyManager.Instance.RemoveMoney(
            upgradeCost
        );

        // =========================
        // INCREASE LEVEL
        // =========================

        shopUpgradeLevel++;

        Debug.Log(
            $"⭐ SHOP UPGRADED | " +
            $"New Level: {shopUpgradeLevel} | " +
            $"Cost: Rs. {upgradeCost:N0} | " +
            $"Revenue Multiplier: {RevenueMultiplier:F2}x | " +
            $"Customer Growth: {CustomerGrowthMultiplier:F2}x"
        );

        OnShopUpgrade?.Invoke();
        OnShopUpdated?.Invoke();

        return true;
    }

    // =========================
    // GET SHOP LEVEL
    // =========================

    public int GetShopLevel()
    {
        return shopUpgradeLevel;
    }

    // =========================
    // GET NEXT UPGRADE COST
    // =========================

    public int GetNextUpgradeCost()
    {
        return GetUpgradeCost();
    }

    // =========================
    // GET REVENUE MULTIPLIER
    // =========================

    public float GetRevenueMultiplier()
    {
        return RevenueMultiplier;
    }

    // =========================
    // GET CUSTOMER GROWTH
    // =========================

    public float GetCustomerGrowthMultiplier()
    {
        return CustomerGrowthMultiplier;
    }

    // =========================================================
    // SHOP ITEMS
    // =========================================================

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

    // =========================================================
    // BUY ITEM
    // =========================================================

    public bool BuyItem(string itemId)
    {
        ShopItem item =
            GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"Buy failed: Item not found. " +
                $"ID: {itemId}"
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

        // =========================
        // CHECK CURRENT CUSTOMER
        // =========================

        Customer currentCustomer =
            GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.LogWarning(
                "Buy failed: No active customer."
            );

            return false;
        }

        // =========================
        // ONLY REQUESTED ITEM
        // =========================

        if (currentCustomer.RequestedItemId !=
            itemId)
        {
            Debug.LogWarning(
                $"Buy failed: Current customer wants " +
                $"{currentCustomer.RequestedItemName}, " +
                $"not {item.ItemName}."
            );

            return false;
        }

        // =========================
        // CHECK REQUIRED QUANTITY
        // =========================

        int inventoryQuantity =
            InventoryManager.Instance.GetQuantity(
                itemId
            );

        if (inventoryQuantity >=
            currentCustomer.RequestedQuantity)
        {
            Debug.LogWarning(
                $"Buy failed: Required quantity reached. " +
                $"Item: {item.ItemName} | " +
                $"Required: {currentCustomer.RequestedQuantity} | " +
                $"Current: {inventoryQuantity}"
            );

            return false;
        }

        // =========================
        // CHECK MONEY
        // =========================

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

        // =========================
        // REMOVE MONEY
        // =========================

        MoneyManager.Instance.RemoveMoney(
            item.BuyPrice
        );

        // =========================
        // ADD INVENTORY
        // =========================

        bool inventoryAdded =
            InventoryManager.Instance.AddItem(
                item.ItemId,
                item.ItemName,
                1
            );

        if (!inventoryAdded)
        {
            MoneyManager.Instance.AddMoney(
                item.BuyPrice
            );

            Debug.LogWarning(
                $"Buy failed: Could not add " +
                $"{item.ItemName} to inventory."
            );

            return false;
        }

        // =========================
        // TRACK TOTAL OWNERSHIP
        // =========================

        AddPurchasedItem(
            item.ItemId
        );

        // =========================
        // TRACK CURRENT CUSTOMER
        // =========================

        AddCurrentCustomerPurchase(
            item.ItemId
        );

        Debug.Log(
            $"Purchase successful | " +
            $"Item: {item.ItemName} | " +
            $"Price: Rs. {item.BuyPrice:N0} | " +
            $"Inventory: " +
            $"{InventoryManager.Instance.GetQuantity(item.ItemId)} | " +
            $"Customer Purchase: " +
            $"{GetCurrentCustomerPurchaseQuantity(item.ItemId)}"
        );

        return true;
    }

    // =========================================================
    // SELL / UNDO CURRENT BUY
    // =========================================================

    public bool SellItem(string itemId)
    {
        ShopItem item =
            GetItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"Sell failed: Item not found. " +
                $"ID: {itemId}"
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

        // =========================
        // ONLY UNDO CURRENT CUSTOMER BUY
        // =========================

        int customerPurchaseQuantity =
            GetCurrentCustomerPurchaseQuantity(
                itemId
            );

        if (customerPurchaseQuantity <= 0)
        {
            Debug.LogWarning(
                $"Sell failed: No current customer " +
                $"purchase to undo. Item: {item.ItemName}"
            );

            return false;
        }

        // =========================
        // REMOVE FROM INVENTORY
        // =========================

        bool inventoryRemoved =
            InventoryManager.Instance.RemoveItem(
                item.ItemId,
                1
            );

        if (!inventoryRemoved)
        {
            Debug.LogWarning(
                $"Sell failed: Could not remove " +
                $"{item.ItemName} from inventory."
            );

            return false;
        }

        // =========================
        // RETURN MONEY
        // =========================

        MoneyManager.Instance.AddMoney(
            item.BuyPrice
        );

        // =========================
        // REMOVE OWNERSHIP
        // =========================

        RemovePurchasedItem(
            item.ItemId
        );

        // =========================
        // REMOVE CUSTOMER PURCHASE
        // =========================

        RemoveCurrentCustomerPurchase(
            item.ItemId
        );

        Debug.Log(
            $"Customer purchase undone | " +
            $"Item: {item.ItemName} | " +
            $"Refund: Rs. {item.BuyPrice:N0} | " +
            $"Inventory: " +
            $"{InventoryManager.Instance.GetQuantity(item.ItemId)}"
        );

        return true;
    }

    // =========================================================
    // CUSTOMER PURCHASE
    // =========================================================

    // =========================
    // COMPLETE CUSTOMER PURCHASE
    // =========================

    public void CompleteCurrentCustomerPurchase()
    {
        currentCustomerPurchases.Clear();

        Debug.Log(
            "Current customer purchases completed."
        );
    }

    // =========================
    // CANCEL CUSTOMER PURCHASE
    // =========================

    public void CancelCurrentCustomerPurchases()
    {
        if (InventoryManager.Instance == null)
        {
            currentCustomerPurchases.Clear();
            return;
        }

        if (MoneyManager.Instance == null)
        {
            currentCustomerPurchases.Clear();
            return;
        }

        foreach (
            KeyValuePair<string, int> purchase
            in currentCustomerPurchases)
        {
            string itemId =
                purchase.Key;

            int quantity =
                purchase.Value;

            if (quantity <= 0)
            {
                continue;
            }

            ShopItem item =
                GetItem(itemId);

            if (item == null)
            {
                continue;
            }

            bool removed =
                InventoryManager.Instance.RemoveItem(
                    itemId,
                    quantity
                );

            if (removed)
            {
                int refund =
                    item.BuyPrice *
                    quantity;

                MoneyManager.Instance.AddMoney(
                    refund
                );

                Debug.Log(
                    $"Expired customer purchase cancelled | " +
                    $"Item: {item.ItemName} | " +
                    $"Quantity: {quantity} | " +
                    $"Refund: Rs. {refund:N0}"
                );

                RemovePurchasedItem(
                    itemId,
                    quantity
                );
            }
        }

        currentCustomerPurchases.Clear();

        Debug.Log(
            "Current customer purchases cancelled."
        );
    }

    // =========================================================
    // PURCHASE QUANTITIES
    // =========================================================

    // =========================
    // TOTAL PURCHASED QUANTITY
    // =========================

    public int GetPurchasedQuantity(
        string itemId)
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

    // =========================
    // CURRENT CUSTOMER QUANTITY
    // =========================

    public int GetCurrentCustomerPurchaseQuantity(
        string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        if (currentCustomerPurchases.TryGetValue(
                itemId,
                out int quantity))
        {
            return quantity;
        }

        return 0;
    }

    // =========================================================
    // PURCHASE TRACKING
    // =========================================================

    // =========================
    // ADD TOTAL PURCHASE
    // =========================

    private void AddPurchasedItem(
        string itemId)
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

    // =========================
    // REMOVE TOTAL PURCHASE
    // =========================

    private void RemovePurchasedItem(
        string itemId,
        int quantity = 1)
    {
        if (!purchasedItems.ContainsKey(itemId))
        {
            return;
        }

        purchasedItems[itemId] -=
            quantity;

        if (purchasedItems[itemId] <= 0)
        {
            purchasedItems.Remove(
                itemId
            );
        }
    }

    // =========================
    // ADD CURRENT CUSTOMER BUY
    // =========================

    private void AddCurrentCustomerPurchase(
        string itemId)
    {
        if (currentCustomerPurchases.ContainsKey(
                itemId))
        {
            currentCustomerPurchases[itemId]++;
        }
        else
        {
            currentCustomerPurchases.Add(
                itemId,
                1
            );
        }
    }

    // =========================
    // REMOVE CURRENT CUSTOMER BUY
    // =========================

    private void RemoveCurrentCustomerPurchase(
        string itemId)
    {
        if (!currentCustomerPurchases.ContainsKey(
                itemId))
        {
            return;
        }

        currentCustomerPurchases[itemId]--;

        if (currentCustomerPurchases[itemId] <= 0)
        {
            currentCustomerPurchases.Remove(
                itemId
            );
        }
    }

    // =========================================================
    // GET CURRENT CUSTOMER
    // =========================================================

    private Customer GetCurrentCustomer()
    {
        if (CustomerManager.Instance == null)
        {
            return null;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return null;
        }

        return
            CustomerManager.Instance.ActiveCustomers[0];
    }
}