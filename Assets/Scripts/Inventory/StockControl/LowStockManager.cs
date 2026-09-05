using System;
using System.Collections.Generic;
using UnityEngine;

public class LowStockManager : MonoBehaviour
{
    public static LowStockManager Instance { get; private set; }

    // --------------------------------------------------
    // LOW STOCK SETTINGS
    // --------------------------------------------------

    [Header("Low Stock Settings")]
    [SerializeField] private int minimumBoxCount = 1;
    [SerializeField] private int minimumLooseUnits = 5;

    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    public event Action OnLowStockChanged;

    // --------------------------------------------------
    // UNITY
    // --------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StoreRoomManager.OnStoreRoomChanged +=
            HandleStoreRoomChanged;
    }

    private void OnDestroy()
    {
        StoreRoomManager.OnStoreRoomChanged -=
            HandleStoreRoomChanged;
    }

    // --------------------------------------------------
    // STORE ROOM CHANGE
    // --------------------------------------------------

    private void HandleStoreRoomChanged()
    {
        OnLowStockChanged?.Invoke();
    }

    // --------------------------------------------------
    // CHECK ITEM LOW STOCK
    // --------------------------------------------------

    public bool IsItemLowStock(string itemId)
    {
        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock: StoreRoomManager not found."
            );

            return false;
        }

        StoreRoomStock stock =
            StoreRoomManager.Instance.GetStock(itemId);

        if (stock == null)
        {
            return false;
        }

        int threshold =
            CalculateLowStockThreshold(
                stock.UnitsPerBox
            );

        return stock.Quantity <= threshold;
    }

    // --------------------------------------------------
    // CALCULATE LOW STOCK THRESHOLD
    // --------------------------------------------------

    public int CalculateLowStockThreshold(
        int unitsPerBox)
    {
        if (unitsPerBox <= 0)
        {
            unitsPerBox = 1;
        }

        return
            (unitsPerBox * minimumBoxCount) +
            minimumLooseUnits;
    }

    // --------------------------------------------------
    // GET LOW STOCK ITEMS
    // --------------------------------------------------

    public List<StoreRoomStock> GetLowStockItems()
    {
        List<StoreRoomStock> lowStockItems =
            new List<StoreRoomStock>();

        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock: StoreRoomManager not found."
            );

            return lowStockItems;
        }

        List<StoreRoomStock> storeRoomItems =
            StoreRoomManager.Instance.GetLowStockItems();

        if (storeRoomItems == null)
        {
            return lowStockItems;
        }

        for (int i = 0; i < storeRoomItems.Count; i++)
        {
            StoreRoomStock stock =
                storeRoomItems[i];

            if (stock == null)
                continue;

            if (IsItemLowStock(stock.ItemId))
            {
                lowStockItems.Add(stock);
            }
        }

        return lowStockItems;
    }

    // --------------------------------------------------
    // LOW STOCK COUNT
    // --------------------------------------------------

    public int GetLowStockCount()
    {
        return GetLowStockItems().Count;
    }

    // --------------------------------------------------
    // HAS ANY LOW STOCK
    // --------------------------------------------------

    public bool HasAnyLowStock()
    {
        return GetLowStockCount() > 0;
    }

    // --------------------------------------------------
    // LOG LOW STOCK ITEMS
    // --------------------------------------------------

    public void LogLowStockItems()
    {
        List<StoreRoomStock> lowStockItems =
            GetLowStockItems();

        Debug.Log(
            "========== LOW STOCK =========="
        );

        if (lowStockItems.Count == 0)
        {
            Debug.Log(
                "LowStock: No low-stock items."
            );

            Debug.Log(
                "==============================="
            );

            return;
        }

        for (int i = 0; i < lowStockItems.Count; i++)
        {
            StoreRoomStock stock =
                lowStockItems[i];

            int threshold =
                CalculateLowStockThreshold(
                    stock.UnitsPerBox
                );

            Debug.Log(
                $"Low Stock | " +
                $"{stock.ItemName} | " +
                $"Stock: {stock.GetStockDisplay()} | " +
                $"Units: {stock.Quantity} | " +
                $"Threshold: {threshold} Nos"
            );
        }

        Debug.Log(
            $"Low Stock Items: {lowStockItems.Count}"
        );

        Debug.Log(
            "==============================="
        );
    }

    // --------------------------------------------------
    // TEST 1
    // 15 UNITS = LOW STOCK
    // food_001 → Milk
    // --------------------------------------------------

    [ContextMenu("TEST 1 - Milk 15 Units")]
    private void TestMilk15()
    {
        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock Test: " +
                "StoreRoomManager not found."
            );

            return;
        }

        StoreRoomManager.Instance.ClearStoreRoom();

        StoreRoomManager.Instance.AddStock(
            "food_001",
            "Milk",
            10,
            15
        );

        bool isLowStock =
            IsItemLowStock("food_001");

        Debug.Log(
            $"LowStock Test: " +
            $"Milk = 15 Nos | " +
            $"Low Stock = {isLowStock}"
        );
    }

    // --------------------------------------------------
    // TEST 2
    // 16 UNITS = NORMAL
    // food_001 → Milk
    // --------------------------------------------------

    [ContextMenu("TEST 2 - Milk 16 Units")]
    private void TestMilk16()
    {
        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock Test: " +
                "StoreRoomManager not found."
            );

            return;
        }

        StoreRoomManager.Instance.ClearStoreRoom();

        StoreRoomManager.Instance.AddStock(
            "food_001",
            "Milk",
            10,
            16
        );

        bool isLowStock =
            IsItemLowStock("food_001");

        Debug.Log(
            $"LowStock Test: " +
            $"Milk = 16 Nos | " +
            $"Low Stock = {isLowStock}"
        );
    }

    // --------------------------------------------------
    // TEST 3
    // SHOW LOW STOCK
    // --------------------------------------------------

    [ContextMenu("TEST 3 - Show Low Stock Items")]
    private void TestShowLowStock()
    {
        LogLowStockItems();
    }

    // --------------------------------------------------
    // TEST 4
    // MULTIPLE LOW STOCK ITEMS
    // --------------------------------------------------

    [ContextMenu("TEST 4 - Multiple Low Stock Items")]
    private void TestMultipleLowStock()
    {
        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock Test: " +
                "StoreRoomManager not found."
            );

            return;
        }

        StoreRoomManager.Instance.ClearStoreRoom();

        // food_001 → Milk
        // 15 → LOW
        StoreRoomManager.Instance.AddStock(
            "food_001",
            "Milk",
            10,
            15
        );

        // food_002 → Bread
        // 20 → NORMAL
        StoreRoomManager.Instance.AddStock(
            "food_002",
            "Bread",
            10,
            20
        );

        // tool_001 → Tool Kit
        // 10 → LOW
        StoreRoomManager.Instance.AddStock(
            "tool_001",
            "Tool Kit",
            10,
            10
        );

        LogLowStockItems();
    }

    // --------------------------------------------------
    // TEST 5
    // CLEAR STORE ROOM
    // --------------------------------------------------

    [ContextMenu("TEST 5 - Clear Store Room")]
    private void TestClearStoreRoom()
    {
        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "LowStock Test: " +
                "StoreRoomManager not found."
            );

            return;
        }

        StoreRoomManager.Instance.ClearStoreRoom();

        Debug.Log(
            "LowStock Test: Store Room cleared."
        );
    }
}