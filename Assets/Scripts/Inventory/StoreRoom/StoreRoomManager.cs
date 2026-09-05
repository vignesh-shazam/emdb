using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreRoomManager : MonoBehaviour
{
    public static StoreRoomManager Instance { get; private set; }

    public static event Action OnStoreRoomChanged;

    // =========================
    // STORE ROOM STOCK
    // =========================

    [Header("Store Room Stock")]
    [SerializeField]
    private List<StoreRoomStock> storeRoomStocks =
        new List<StoreRoomStock>();

    public IReadOnlyList<StoreRoomStock> StoreRoomStocks =>
        storeRoomStocks;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeStoreRoom();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeStoreRoom()
    {
        if (storeRoomStocks == null)
        {
            storeRoomStocks =
                new List<StoreRoomStock>();
        }

        Debug.Log(
            $"Store Room initialized | " +
            $"Items: {storeRoomStocks.Count}"
        );
    }

    // =========================================================
    // GET STOCK
    // =========================================================

    public StoreRoomStock GetStock(
        string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (
            StoreRoomStock stock
            in storeRoomStocks)
        {
            if (stock == null)
            {
                continue;
            }

            if (stock.ItemId == itemId)
            {
                return stock;
            }
        }

        return null;
    }

    // =========================================================
    // HAS STOCK
    // =========================================================

    public bool HasStock(
        string itemId)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        return stock != null &&
               stock.Quantity > 0;
    }

    // =========================================================
    // GET QUANTITY
    // =========================================================

    public int GetQuantity(
        string itemId)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            return 0;
        }

        return stock.Quantity;
    }

    // =========================================================
    // ADD STOCK
    // =========================================================

    public bool AddStock(
        string itemId,
        string itemName,
        int unitsPerBox,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Store Room: " +
                "Add stock failed. Item ID is empty."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning(
                "Store Room: " +
                "Add stock failed. Item name is empty."
            );

            return false;
        }

        if (unitsPerBox <= 0)
        {
            Debug.LogWarning(
                "Store Room: " +
                $"Invalid Units Per Box: {unitsPerBox}"
            );

            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                "Store Room: " +
                $"Invalid quantity: {quantity}"
            );

            return false;
        }

        StoreRoomStock existingStock =
            GetStock(itemId);

        // =========================
        // EXISTING STOCK
        // =========================

        if (existingStock != null)
        {
            existingStock.Quantity +=
                quantity;

            Debug.Log(
                $"Store Room stock added | " +
                $"Item: {existingStock.ItemName} | " +
                $"Added: {quantity} | " +
                $"Total: {existingStock.Quantity} | " +
                $"Display: {existingStock.GetStockDisplay()}"
            );

            NotifyStoreRoomChanged();

            return true;
        }

        // =========================
        // NEW STOCK
        // =========================

        StoreRoomStock newStock =
            new StoreRoomStock(
                itemId,
                itemName,
                unitsPerBox,
                quantity
            );

        storeRoomStocks.Add(
            newStock
        );

        Debug.Log(
            $"Store Room stock added | " +
            $"Item: {newStock.ItemName} | " +
            $"Added: {quantity} | " +
            $"Total: {newStock.Quantity} | " +
            $"Display: {newStock.GetStockDisplay()}"
        );

        NotifyStoreRoomChanged();

        return true;
    }

    // =========================================================
    // REMOVE STOCK
    // =========================================================

    public bool RemoveStock(
        string itemId,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Store Room: " +
                "Remove stock failed. Item ID is empty."
            );

            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                $"Store Room: " +
                $"Invalid remove quantity: {quantity}"
            );

            return false;
        }

        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            Debug.LogWarning(
                $"Store Room: " +
                $"Item not found. ID: {itemId}"
            );

            return false;
        }

        if (stock.Quantity < quantity)
        {
            Debug.LogWarning(
                $"Store Room: " +
                $"Insufficient stock. " +
                $"Item: {stock.ItemName} | " +
                $"Available: {stock.Quantity} | " +
                $"Required: {quantity}"
            );

            return false;
        }

        stock.Quantity -=
            quantity;

        Debug.Log(
            $"Store Room stock removed | " +
            $"Item: {stock.ItemName} | " +
            $"Removed: {quantity} | " +
            $"Remaining: {stock.Quantity} | " +
            $"Display: {stock.GetStockDisplay()}"
        );

        if (stock.Quantity <= 0)
        {
            storeRoomStocks.Remove(
                stock
            );

            Debug.Log(
                $"Store Room item completely empty | " +
                $"Item: {stock.ItemName}"
            );
        }

        NotifyStoreRoomChanged();

        return true;
    }

    // =========================================================
    // BOX COUNT
    // =========================================================

    public int GetBoxCount(
        string itemId)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            return 0;
        }

        return stock.BoxCount;
    }

    // =========================================================
    // LOOSE UNIT COUNT
    // =========================================================

    public int GetLooseUnitCount(
        string itemId)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            return 0;
        }

        return stock.LooseUnitCount;
    }

    // =========================================================
    // STOCK DISPLAY
    // =========================================================

    public string GetStockDisplay(
        string itemId)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            return "0 Nos";
        }

        return stock.GetStockDisplay();
    }

    // =========================================================
    // CHECK LOW STOCK
    // =========================================================

    public bool IsLowStock(
        string itemId,
        int reorderThreshold)
    {
        StoreRoomStock stock =
            GetStock(itemId);

        if (stock == null)
        {
            return true;
        }

        return stock.Quantity <=
               reorderThreshold;
    }

    // =========================================================
    // GET ALL LOW-STOCK ITEMS
    // =========================================================

    public List<StoreRoomStock>
        GetLowStockItems()
    {
        List<StoreRoomStock> lowStockItems =
            new List<StoreRoomStock>();

        foreach (
            StoreRoomStock stock
            in storeRoomStocks)
        {
            if (stock == null)
            {
                continue;
            }

            // Default rule:
            // 1 Box + 5 Nos
            //
            // Example:
            // 1 Box = 10
            // 10 + 5 = 15

            int reorderThreshold =
                (stock.UnitsPerBox * 1) +
                5;

            if (stock.Quantity <=
                reorderThreshold)
            {
                lowStockItems.Add(
                    stock
                );
            }
        }

        return lowStockItems;
    }

    // =========================================================
    // CLEAR STORE ROOM
    // =========================================================

    public void ClearStoreRoom()
    {
        storeRoomStocks.Clear();

        Debug.Log(
            "Store Room stock cleared."
        );

        NotifyStoreRoomChanged();
    }

    // =========================================================
    // EVENT
    // =========================================================

    private void NotifyStoreRoomChanged()
    {
        OnStoreRoomChanged?.Invoke();
    }

    // =========================================================
    // DEVELOPMENT TEST
    // =========================================================

    [ContextMenu("TEST - Add 24 Milk")]
    private void TestAddMilk()
    {
        // food_001 = Milk
        AddStock(
            "food_001",
            "Milk",
            10,
            24
        );
    }

    [ContextMenu("TEST - Remove 6 Milk")]
    private void TestRemoveMilk()
    {
        // food_001 = Milk
        RemoveStock(
            "food_001",
            6
        );
    }

    [ContextMenu("TEST - Clear Store Room")]
    private void TestClearStoreRoom()
    {
        ClearStoreRoom();
    }

    [ContextMenu("TEST - Add 5 Milk")]
    private void TestAdd5Milk()
    {
        // food_001 = Milk
        AddStock(
            "food_001",
            "Milk",
            10,
            5
        );
    }

    [ContextMenu("TEST - Add 10 Bread")]
    private void TestAdd10Bread()
    {
        // food_002 = Bread
        AddStock(
            "food_002",
            "Bread",
            10,
            10
        );
    }

    [ContextMenu("TEST - Add 10 Tool Kit")]
    private void TestAdd10ToolKit()
    {
        // tool_001 = Tool Kit
        AddStock(
            "tool_001",
            "Tool Kit",
            10,
            10
        );
    }
}