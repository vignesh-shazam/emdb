using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    // =========================================================
    // EVENTS
    // =========================================================

    public static event Action OnShopUpdated;
    public static event Action OnShopUpgrade;

    // =========================================================
    // SHOP STATUS
    // =========================================================

    [Header("Shop Status")]
    [SerializeField]
    private bool isShopOpen = true;

    // =========================================================
    // SHOP ITEMS
    // =========================================================

    [Header("Shop Items")]
    [SerializeField]
    private List<ShopItem> shopItems = new List<ShopItem>();

    // =========================================================
    // SHOP UPGRADE
    // =========================================================

    [Header("Shop Upgrade")]
    [SerializeField]
    private int shopUpgradeLevel = 1;

    [SerializeField]
    private int maximumUpgradeLevel = 10;

    [SerializeField]
    private int baseUpgradeCost = 1000;

    [SerializeField]
    private float upgradeCostMultiplier = 1.5f;

    // =========================================================
    // REVENUE
    // =========================================================

    [Header("Revenue Improvement")]
    [SerializeField]
    private float baseRevenueMultiplier = 1f;

    [SerializeField]
    private float revenueIncreasePerUpgrade = 0.10f;

    // =========================================================
    // CUSTOMER GROWTH
    // =========================================================

    [Header("Customer Growth")]
    [SerializeField]
    private float baseCustomerGrowthMultiplier = 1f;

    [SerializeField]
    private float customerGrowthPerUpgrade = 0.10f;

    // =========================================================
    // PURCHASE TRACKING
    // =========================================================

    private readonly Dictionary<string, int> purchasedItems =
        new Dictionary<string, int>();

    private readonly Dictionary<string, int> currentCustomerPurchases =
        new Dictionary<string, int>();

    // =========================================================
    // PUBLIC PROPERTIES
    // =========================================================

    public bool IsShopOpen => isShopOpen;

    public IReadOnlyList<ShopItem> ShopItems => shopItems;

    public int ShopUpgradeLevel => shopUpgradeLevel;

    public int MaximumUpgradeLevel => maximumUpgradeLevel;

    public bool CanUpgradeShop =>
        shopUpgradeLevel < maximumUpgradeLevel;

    public float RevenueMultiplier =>
        baseRevenueMultiplier +
        ((shopUpgradeLevel - 1) * revenueIncreasePerUpgrade);

    public float CustomerGrowthMultiplier =>
        baseCustomerGrowthMultiplier +
        ((shopUpgradeLevel - 1) * customerGrowthPerUpgrade);

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                $"Duplicate ShopManager found and destroyed. " +
                $"Object: {gameObject.name}"
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log(
            $"ShopManager Instance assigned successfully. " +
            $"Object: {gameObject.name}"
        );

        InitializeShop();
    }

    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;

            Debug.Log(
                "ShopManager Instance cleared."
            );
        }
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    private void InitializeShop()
    {
        if (shopItems == null)
        {
            shopItems = new List<ShopItem>();
        }

        if (shopUpgradeLevel < 1)
        {
            shopUpgradeLevel = 1;
        }

        if (maximumUpgradeLevel < 1)
        {
            maximumUpgradeLevel = 10;
        }

        if (shopUpgradeLevel > maximumUpgradeLevel)
        {
            shopUpgradeLevel = maximumUpgradeLevel;
        }

        if (baseUpgradeCost < 0)
        {
            baseUpgradeCost = 0;
        }

        if (upgradeCostMultiplier < 1f)
        {
            upgradeCostMultiplier = 1f;
        }

        if (baseRevenueMultiplier < 0f)
        {
            baseRevenueMultiplier = 1f;
        }

        if (revenueIncreasePerUpgrade < 0f)
        {
            revenueIncreasePerUpgrade = 0f;
        }

        if (baseCustomerGrowthMultiplier < 0f)
        {
            baseCustomerGrowthMultiplier = 1f;
        }

        if (customerGrowthPerUpgrade < 0f)
        {
            customerGrowthPerUpgrade = 0f;
        }

        // =====================================================
        // DEFAULT SHOP ITEMS
        //
        // food_001 -> Milk
        // food_002 -> Bread
        // tool_001 -> Tool Kit
        // =====================================================

        if (shopItems.Count == 0)
        {
            shopItems.Add(
                new ShopItem(
                    "food_001",
                    "Milk",
                    200,
                    80
                )
            );

            shopItems.Add(
                new ShopItem(
                    "food_002",
                    "Bread",
                    300,
                    40
                )
            );

            shopItems.Add(
                new ShopItem(
                    "tool_001",
                    "Tool Kit",
                    500,
                    500
                )
            );
        }

        Debug.Log(
            $"Shop initialized | " +
            $"Status: {(IsShopOpen ? "Open" : "Closed")} | " +
            $"Level: {shopUpgradeLevel} | " +
            $"Items available: {shopItems.Count} | " +
            $"Revenue Multiplier: {RevenueMultiplier:F2}x | " +
            $"Customer Growth: {CustomerGrowthMultiplier:F2}x"
        );

        OnShopUpdated?.Invoke();
    }

    // =========================================================
    // SHOP OPEN / CLOSE
    // =========================================================

    public void OpenShop()
    {
        if (isShopOpen)
        {
            Debug.LogWarning(
                "Shop is already open."
            );

            return;
        }

        isShopOpen = true;

        Debug.Log(
            "SHOP OPENED."
        );

        OnShopUpdated?.Invoke();
    }

    public void CloseShop()
    {
        if (!isShopOpen)
        {
            Debug.LogWarning(
                "Shop is already closed."
            );

            return;
        }

        isShopOpen = false;

        Debug.Log(
            "SHOP CLOSED."
        );

        OnShopUpdated?.Invoke();
    }

    // =========================================================
    // SHOP UPGRADE
    // =========================================================

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

    public bool UpgradeShop()
    {
        if (!CanUpgradeShop)
        {
            Debug.LogWarning(
                "Shop upgrade failed: Maximum shop level reached."
            );

            return false;
        }

        int upgradeCost = GetUpgradeCost();

        if (ExpenseManager.Instance == null)
        {
            Debug.LogError(
                "Shop upgrade failed: ExpenseManager not found."
            );

            return false;
        }

        bool expenseSuccessful =
            ExpenseManager.Instance.Spend(
                upgradeCost,
                ExpenseCategory.Shopping,
                FinanceAccountType.Current,
                "Shop Upgrade"
            );

        if (!expenseSuccessful)
        {
            Debug.LogWarning(
                $"Shop upgrade failed: Insufficient funds. " +
                $"Required: Rs. {upgradeCost:N0}"
            );

            return false;
        }

        shopUpgradeLevel++;

        Debug.Log(
            $"SHOP UPGRADED | " +
            $"New Level: {shopUpgradeLevel} | " +
            $"Cost: Rs. {upgradeCost:N0} | " +
            $"Revenue Multiplier: {RevenueMultiplier:F2}x | " +
            $"Customer Growth: {CustomerGrowthMultiplier:F2}x"
        );

        OnShopUpgrade?.Invoke();
        OnShopUpdated?.Invoke();

        return true;
    }

    public int GetShopLevel()
    {
        return shopUpgradeLevel;
    }

    public int GetNextUpgradeCost()
    {
        return GetUpgradeCost();
    }

    public float GetRevenueMultiplier()
    {
        return RevenueMultiplier;
    }

    public float GetCustomerGrowthMultiplier()
    {
        return CustomerGrowthMultiplier;
    }

    // =========================================================
    // GET SHOP ITEM
    // =========================================================

    public ShopItem GetItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (ShopItem item in shopItems)
        {
            if (item == null)
            {
                continue;
            }

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
        if (!IsShopOpen)
        {
            Debug.LogWarning(
                "Buy failed: Shop is currently closed."
            );

            return false;
        }

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

        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "Buy failed: InventoryManager not found."
            );

            return false;
        }

        Customer currentCustomer = GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.LogWarning(
                "Buy failed: No active customer."
            );

            return false;
        }

        if (currentCustomer.RequestedItemId != itemId)
        {
            Debug.LogWarning(
                $"Buy failed: Current customer wants " +
                $"{currentCustomer.RequestedItemName}, " +
                $"not {item.ItemName}."
            );

            return false;
        }

        int inventoryQuantity =
            InventoryManager.Instance.GetQuantity(itemId);

        if (inventoryQuantity >= currentCustomer.RequestedQuantity)
        {
            Debug.LogWarning(
                $"Buy failed: Required quantity reached. " +
                $"Item: {item.ItemName} | " +
                $"Required: {currentCustomer.RequestedQuantity} | " +
                $"Current: {inventoryQuantity}"
            );

            return false;
        }

        if (!MoneyManager.Instance.CanAfford(item.BuyPrice))
        {
            Debug.LogWarning(
                $"Buy failed: Insufficient money. " +
                $"Item: {item.ItemName} | " +
                $"Required: Rs. {item.BuyPrice:N0}"
            );

            return false;
        }

        bool moneyRemoved =
            MoneyManager.Instance.RemoveMoney(item.BuyPrice);

        if (!moneyRemoved)
        {
            Debug.LogWarning(
                $"Buy failed: Could not remove money. " +
                $"Item: {item.ItemName}"
            );

            return false;
        }

        bool inventoryAdded =
            InventoryManager.Instance.AddItem(
                item.ItemId,
                item.ItemName,
                1
            );

        if (!inventoryAdded)
        {
            MoneyManager.Instance.AddMoney(item.BuyPrice);

            Debug.LogWarning(
                $"Buy failed: Could not add " +
                $"{item.ItemName} to inventory."
            );

            return false;
        }

        RecordShopPurchase(item);
        AddPurchasedItem(item.ItemId);
        AddCurrentCustomerPurchase(item.ItemId);

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
    // RECORD SHOP PURCHASE
    // =========================================================

    private void RecordShopPurchase(ShopItem item)
    {
        if (item == null)
        {
            return;
        }

        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "ShopManager: FinanceTransactionManager not found. " +
                "Shop purchase ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordExpense(
            FinanceAccountType.Current,
            item.BuyPrice,
            "Shop Purchase"
        );

        Debug.Log(
            $"Shop purchase recorded | " +
            $"Item: {item.ItemName} | " +
            $"Amount: Rs. {item.BuyPrice:N0}"
        );
    }

    // =========================================================
    // SELL / UNDO CURRENT BUY
    // =========================================================

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

        int customerPurchaseQuantity =
            GetCurrentCustomerPurchaseQuantity(itemId);

        if (customerPurchaseQuantity <= 0)
        {
            Debug.LogWarning(
                $"Sell failed: No current customer purchase " +
                $"to undo. Item: {item.ItemName}"
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
                $"Sell failed: Could not remove " +
                $"{item.ItemName} from inventory."
            );

            return false;
        }

        MoneyManager.Instance.AddMoney(item.BuyPrice);

        RemovePurchasedItem(item.ItemId);
        RemoveCurrentCustomerPurchase(item.ItemId);

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
    // COMPLETE CUSTOMER PURCHASE
    // =========================================================

    public void CompleteCurrentCustomerPurchase()
    {
        currentCustomerPurchases.Clear();

        Debug.Log(
            "Current customer purchases completed."
        );
    }

    // =========================================================
    // CANCEL CUSTOMER PURCHASE
    // =========================================================

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
            string itemId = purchase.Key;
            int quantity = purchase.Value;

            if (quantity <= 0)
            {
                continue;
            }

            ShopItem item = GetItem(itemId);

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
                int refund = item.BuyPrice * quantity;

                MoneyManager.Instance.AddMoney(refund);

                Debug.Log(
                    $"Expired customer purchase cancelled | " +
                    $"Item: {item.ItemName} | " +
                    $"Quantity: {quantity} | " +
                    $"Refund: Rs. {refund:N0}"
                );

                RemovePurchasedItem(itemId, quantity);
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

    public int GetPurchasedQuantity(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        if (purchasedItems.TryGetValue(itemId, out int quantity))
        {
            return quantity;
        }

        return 0;
    }

    public int GetCurrentCustomerPurchaseQuantity(string itemId)
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
    // TOTAL PURCHASE TRACKING
    // =========================================================

    private void AddPurchasedItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (purchasedItems.ContainsKey(itemId))
        {
            purchasedItems[itemId]++;
        }
        else
        {
            purchasedItems.Add(itemId, 1);
        }
    }

    private void RemovePurchasedItem(
        string itemId,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (!purchasedItems.ContainsKey(itemId))
        {
            return;
        }

        purchasedItems[itemId] -= quantity;

        if (purchasedItems[itemId] <= 0)
        {
            purchasedItems.Remove(itemId);
        }
    }

    // =========================================================
    // CURRENT CUSTOMER PURCHASE TRACKING
    // =========================================================

    private void AddCurrentCustomerPurchase(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (currentCustomerPurchases.ContainsKey(itemId))
        {
            currentCustomerPurchases[itemId]++;
        }
        else
        {
            currentCustomerPurchases.Add(itemId, 1);
        }
    }

    private void RemoveCurrentCustomerPurchase(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (!currentCustomerPurchases.ContainsKey(itemId))
        {
            return;
        }

        currentCustomerPurchases[itemId]--;

        if (currentCustomerPurchases[itemId] <= 0)
        {
            currentCustomerPurchases.Remove(itemId);
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

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return null;
        }

        return CustomerManager.Instance.ActiveCustomers[0];
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST - Open Shop")]
    private void TestOpenShop()
    {
        OpenShop();
    }

    [ContextMenu("TEST - Close Shop")]
    private void TestCloseShop()
    {
        CloseShop();
    }

    [ContextMenu("TEST - Verify ShopManager Instance")]
    private void TestVerifyShopManagerInstance()
    {
        if (Instance == null)
        {
            Debug.LogError(
                "ShopManager TEST FAILED: Instance is NULL."
            );

            return;
        }

        Debug.Log(
            $"ShopManager TEST PASSED | " +
            $"Object: {Instance.gameObject.name} | " +
            $"Shop Open: {Instance.IsShopOpen} | " +
            $"Items: {Instance.ShopItems.Count}"
        );
    }
}