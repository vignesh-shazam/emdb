using System;
using UnityEngine;

public class StockerManager : MonoBehaviour
{
    public static StockerManager Instance { get; private set; }

    public static event Action OnStockerAction;

    // =========================
    // SETTINGS
    // =========================

    [Header("Stocker Settings")]
    [SerializeField]
    private int lowStockLimit = 5;

    public int LowStockLimit =>
        lowStockLimit;

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

        InitializeStocker();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeStocker()
    {
        if (lowStockLimit < 0)
        {
            lowStockLimit = 0;
        }

        Debug.Log(
            $"Stocker initialized | " +
            $"Low Stock Limit: {lowStockLimit}"
        );
    }

    // =========================================================
    // CHECK SPECIFIC ITEM
    // =========================================================

    public bool IsItemLowStock(
        string itemId)
    {
        if (RackManager.Instance == null)
        {
            Debug.LogError(
                "Stocker: RackManager not found."
            );

            return false;
        }

        int rackQuantity =
            RackManager.Instance.GetQuantity(
                itemId
            );

        return rackQuantity <=
               lowStockLimit;
    }

    // =========================================================
    // RESTOCK SPECIFIC ITEM
    // =========================================================

    public bool RestockItem(
        string itemId)
    {
        if (RackManager.Instance == null)
        {
            Debug.LogError(
                "Stocker: RackManager not found."
            );

            return false;
        }

        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "Stocker: StoreRoomManager not found."
            );

            return false;
        }

        // =====================================================
        // FIND RACK SHELF
        // =====================================================

        RackShelf shelf =
            RackManager.Instance.GetShelfForItem(
                itemId
            );

        if (shelf == null)
        {
            Debug.LogWarning(
                $"Stocker: Item not found on rack. " +
                $"Item ID: {itemId}"
            );

            return false;
        }

        // =====================================================
        // CHECK LOW STOCK
        // =====================================================

        if (shelf.Quantity >
            lowStockLimit)
        {
            Debug.Log(
                $"Stocker: No restock required | " +
                $"Shelf: {shelf.ShelfNumber} | " +
                $"Item: {shelf.ItemName} | " +
                $"Quantity: {shelf.Quantity}"
            );

            return false;
        }

        // =====================================================
        // CALCULATE REQUIRED QUANTITY
        // =====================================================

        int requiredQuantity =
            shelf.Capacity -
            shelf.Quantity;

        if (requiredQuantity <= 0)
        {
            return false;
        }

        // =====================================================
        // CHECK STORE ROOM
        // =====================================================

        int storeRoomQuantity =
            StoreRoomManager.Instance.GetQuantity(
                itemId
            );

        if (storeRoomQuantity <
            requiredQuantity)
        {
            Debug.LogWarning(
                $"Stocker: Not enough stock in Store Room | " +
                $"Item: {shelf.ItemName} | " +
                $"Required: {requiredQuantity} | " +
                $"Available: {storeRoomQuantity}"
            );

            return false;
        }

        // =====================================================
        // REMOVE EXACT QUANTITY FROM STORE ROOM
        // =====================================================

        bool removed =
            StoreRoomManager.Instance.RemoveStock(
                itemId,
                requiredQuantity
            );

        if (!removed)
        {
            Debug.LogWarning(
                $"Stocker: Failed to take stock " +
                $"from Store Room | " +
                $"Item: {shelf.ItemName}"
            );

            return false;
        }

        // =====================================================
        // ADD EXACT QUANTITY TO RACK
        // =====================================================

        int added =
            RackManager.Instance.AddStock(
                itemId,
                shelf.ItemName,
                requiredQuantity
            );

        // =====================================================
        // SAFETY CHECK
        // =====================================================

        if (added !=
            requiredQuantity)
        {
            Debug.LogError(
                $"Stocker ERROR | " +
                $"Expected: {requiredQuantity} | " +
                $"Added: {added}"
            );

            return false;
        }

        // =====================================================
        // SUCCESS
        // =====================================================

        Debug.Log(
            $"STOCKER RESTOCK COMPLETED | " +
            $"Shelf: {shelf.ShelfNumber} | " +
            $"Item: {shelf.ItemName} | " +
            $"Taken From Store Room: {requiredQuantity} | " +
            $"Rack Quantity: " +
            $"{RackManager.Instance.GetQuantity(itemId)} | " +
            $"Store Room Remaining: " +
            $"{StoreRoomManager.Instance.GetQuantity(itemId)}"
        );

        OnStockerAction?.Invoke();

        return true;
    }

    // =========================================================
    // RESTOCK ALL LOW ITEMS
    // =========================================================

    public void RestockAllLowStockItems()
    {
        if (RackManager.Instance == null)
        {
            Debug.LogError(
                "Stocker: RackManager not found."
            );

            return;
        }

        foreach (
            RackShelf shelf
            in RackManager.Instance.Shelves)
        {
            if (shelf == null ||
                shelf.IsEmpty)
            {
                continue;
            }

            if (shelf.Quantity <=
                lowStockLimit)
            {
                RestockItem(
                    shelf.ItemId
                );
            }
        }
    }

    // =========================================================
    // DEVELOPMENT TEST
    // =========================================================

    [ContextMenu("TEST - Restock Milk")]
    private void TestRestockMilk()
    {
        RestockItem(
            "food_002"
        );
    }

    [ContextMenu("TEST - Restock All Low Stock")]
    private void TestRestockAll()
    {
        RestockAllLowStockItems();
    }
}