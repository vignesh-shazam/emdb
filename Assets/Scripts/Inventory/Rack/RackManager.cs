using System;
using System.Collections.Generic;
using UnityEngine;

public class RackManager : MonoBehaviour
{
    public static RackManager Instance { get; private set; }

    public static event Action OnRackChanged;

    // =========================
    // RACK SETTINGS
    // =========================

    [Header("Rack Settings")]
    [SerializeField]
    private int shelfCount = 5;

    [SerializeField]
    private int shelfCapacity = 10;

    // =========================
    // RACK SHELVES
    // =========================

    [Header("Rack Shelves")]
    [SerializeField]
    private List<RackShelf> shelves =
        new List<RackShelf>();

    public IReadOnlyList<RackShelf> Shelves =>
        shelves;

    public int ShelfCount =>
        shelfCount;

    public int ShelfCapacity =>
        shelfCapacity;

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

        InitializeRack();
    }

    // =========================
    // ON VALIDATE
    // =========================

    private void OnValidate()
    {
        if (shelfCount < 1)
        {
            shelfCount = 1;
        }

        if (shelfCapacity < 1)
        {
            shelfCapacity = 1;
        }

        EnsureShelfConfiguration();
    }

    // =========================================================
    // INITIALIZE RACK
    // =========================================================

    private void InitializeRack()
    {
        EnsureShelfConfiguration();

        Debug.Log(
            $"Rack initialized | " +
            $"Shelves: {shelves.Count} | " +
            $"Capacity/Shelf: {shelfCapacity} | " +
            $"Total Capacity: {GetTotalCapacity()}"
        );

        LogAllShelves();

        NotifyRackChanged();
    }

    // =========================================================
    // ENSURE SHELF CONFIGURATION
    // =========================================================

    private void EnsureShelfConfiguration()
    {
        if (shelves == null)
        {
            shelves =
                new List<RackShelf>();
        }

        // =========================
        // CREATE MISSING SHELVES
        // =========================

        while (shelves.Count <
               shelfCount)
        {
            int shelfNumber =
                shelves.Count + 1;

            shelves.Add(
                new RackShelf(
                    shelfNumber,
                    shelfCapacity
                )
            );
        }

        // =========================
        // REMOVE EXTRA SHELVES
        // =========================

        while (shelves.Count >
               shelfCount)
        {
            shelves.RemoveAt(
                shelves.Count - 1
            );
        }

        // =========================
        // FIX EVERY SHELF
        // =========================

        for (
            int i = 0;
            i < shelves.Count;
            i++)
        {
            if (shelves[i] == null)
            {
                shelves[i] =
                    new RackShelf(
                        i + 1,
                        shelfCapacity
                    );

                continue;
            }

            shelves[i].ShelfNumber =
                i + 1;

            shelves[i].Capacity =
                shelfCapacity;

            if (shelves[i].Quantity < 0)
            {
                shelves[i].Quantity = 0;
            }

            if (shelves[i].Quantity >
                shelfCapacity)
            {
                shelves[i].Quantity =
                    shelfCapacity;
            }

            if (shelves[i].Quantity == 0)
            {
                shelves[i].ItemId =
                    string.Empty;

                shelves[i].ItemName =
                    string.Empty;
            }
        }
    }

    // =========================================================
    // GET SHELF
    // =========================================================

    public RackShelf GetShelf(
        int shelfNumber)
    {
        EnsureShelfConfiguration();

        if (shelfNumber < 1 ||
            shelfNumber > shelves.Count)
        {
            return null;
        }

        return shelves[
            shelfNumber - 1
        ];
    }

    // =========================================================
    // GET SHELF FOR ITEM
    // =========================================================

    public RackShelf GetShelfForItem(
        string itemId)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null ||
                shelf.IsEmpty)
            {
                continue;
            }

            if (shelf.ItemId == itemId)
            {
                return shelf;
            }
        }

        return null;
    }

    // =========================================================
    // GET ITEM QUANTITY
    // =========================================================

    public int GetQuantity(
        string itemId)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        int total = 0;

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            if (shelf.ItemId != itemId)
            {
                continue;
            }

            total +=
                shelf.Quantity;
        }

        return total;
    }

    // =========================================================
    // HAS ITEM
    // =========================================================

    public bool HasItem(
        string itemId)
    {
        return GetQuantity(itemId) > 0;
    }

    // =========================================================
    // EMPTY SHELF
    // =========================================================

    public RackShelf GetEmptyShelf()
    {
        EnsureShelfConfiguration();

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            if (shelf.IsEmpty)
            {
                return shelf;
            }
        }

        return null;
    }

    // =========================================================
    // AVAILABLE SHELF
    // =========================================================

    public RackShelf GetAvailableShelf(
        string itemId)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            if (shelf.ItemId != itemId)
            {
                continue;
            }

            if (!shelf.IsFull)
            {
                return shelf;
            }
        }

        return null;
    }

    // =========================================================
    // ADD STOCK
    // =========================================================

    public int AddStock(
        string itemId,
        string itemName,
        int quantity)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Rack: Add stock failed. " +
                "Item ID is empty."
            );

            return 0;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning(
                "Rack: Add stock failed. " +
                "Item name is empty."
            );

            return 0;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                $"Rack: Invalid quantity: {quantity}"
            );

            return 0;
        }

        int remaining =
            quantity;

        // =====================================================
        // FILL EXISTING ITEM SHELVES
        // =====================================================

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (remaining <= 0)
            {
                break;
            }

            if (shelf == null)
            {
                continue;
            }

            if (shelf.IsEmpty)
            {
                continue;
            }

            if (shelf.ItemId != itemId)
            {
                continue;
            }

            int amountToAdd =
                Mathf.Min(
                    remaining,
                    shelf.AvailableSpace
                );

            shelf.Quantity +=
                amountToAdd;

            remaining -=
                amountToAdd;

            Debug.Log(
                $"Rack stock added | " +
                $"Shelf {shelf.ShelfNumber} | " +
                $"Item: {itemName} | " +
                $"Added: {amountToAdd} | " +
                $"Shelf: {shelf.Quantity}/{shelf.Capacity}"
            );
        }

        // =====================================================
        // USE EMPTY SHELVES
        // =====================================================

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (remaining <= 0)
            {
                break;
            }

            if (shelf == null)
            {
                continue;
            }

            if (!shelf.IsEmpty)
            {
                continue;
            }

            int amountToAdd =
                Mathf.Min(
                    remaining,
                    shelf.Capacity
                );

            shelf.ItemId =
                itemId;

            shelf.ItemName =
                itemName;

            shelf.Quantity =
                amountToAdd;

            remaining -=
                amountToAdd;

            Debug.Log(
                $"Rack shelf assigned | " +
                $"Shelf {shelf.ShelfNumber} | " +
                $"Item: {itemName} | " +
                $"Quantity: {shelf.Quantity}/{shelf.Capacity}"
            );
        }

        int added =
            quantity - remaining;

        if (added > 0)
        {
            Debug.Log(
                $"Rack stock result | " +
                $"Item: {itemName} | " +
                $"Requested: {quantity} | " +
                $"Added: {added} | " +
                $"Remaining: {remaining}"
            );

            NotifyRackChanged();
        }

        if (remaining > 0)
        {
            Debug.LogWarning(
                $"Rack capacity reached | " +
                $"Item: {itemName} | " +
                $"Requested: {quantity} | " +
                $"Added: {added} | " +
                $"Remaining: {remaining}"
            );
        }

        LogAllShelves();

        return added;
    }

    // =========================================================
    // REMOVE STOCK
    // =========================================================

    public bool RemoveStock(
        string itemId,
        int quantity)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Rack: Remove stock failed. " +
                "Item ID is empty."
            );

            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                $"Rack: Invalid quantity: {quantity}"
            );

            return false;
        }

        int available =
            GetQuantity(itemId);

        if (available < quantity)
        {
            Debug.LogWarning(
                $"Rack: Insufficient stock | " +
                $"Item: {itemId} | " +
                $"Available: {available} | " +
                $"Required: {quantity}"
            );

            return false;
        }

        int remaining =
            quantity;

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (remaining <= 0)
            {
                break;
            }

            if (shelf == null)
            {
                continue;
            }

            if (shelf.ItemId != itemId)
            {
                continue;
            }

            int amountToRemove =
                Mathf.Min(
                    remaining,
                    shelf.Quantity
                );

            shelf.Quantity -=
                amountToRemove;

            remaining -=
                amountToRemove;

            Debug.Log(
                $"Rack stock removed | " +
                $"Shelf {shelf.ShelfNumber} | " +
                $"Item: {shelf.ItemName} | " +
                $"Removed: {amountToRemove} | " +
                $"Remaining: " +
                $"{shelf.Quantity}/{shelf.Capacity}"
            );

            if (shelf.Quantity <= 0)
            {
                Debug.Log(
                    $"Shelf {shelf.ShelfNumber} " +
                    $"is now empty."
                );

                shelf.Clear();
            }
        }

        if (remaining > 0)
        {
            Debug.LogError(
                $"Rack: Removal failed unexpectedly | " +
                $"Item: {itemId} | " +
                $"Remaining: {remaining}"
            );

            return false;
        }

        Debug.Log(
            $"Rack sale/removal completed | " +
            $"Item: {itemId} | " +
            $"Removed: {quantity} | " +
            $"Rack Total: {GetQuantity(itemId)}"
        );

        LogAllShelves();

        NotifyRackChanged();

        return true;
    }

    // =========================================================
    // AVAILABLE SPACE
    // =========================================================

    public int GetAvailableSpace(
        string itemId)
    {
        EnsureShelfConfiguration();

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        int totalSpace = 0;

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            if (shelf.IsEmpty)
            {
                totalSpace +=
                    shelf.Capacity;

                continue;
            }

            if (shelf.ItemId == itemId)
            {
                totalSpace +=
                    shelf.AvailableSpace;
            }
        }

        return totalSpace;
    }

    // =========================================================
    // TOTAL CAPACITY
    // =========================================================

    public int GetTotalCapacity()
    {
        return
            shelfCount *
            shelfCapacity;
    }

    // =========================================================
    // TOTAL QUANTITY
    // =========================================================

    public int GetTotalQuantity()
    {
        EnsureShelfConfiguration();

        int total = 0;

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            total +=
                shelf.Quantity;
        }

        return total;
    }

    // =========================================================
    // LOW STOCK
    // =========================================================

    public bool IsLowStock(
        string itemId,
        int threshold = 5)
    {
        return
            GetQuantity(itemId) <=
            threshold;
    }

    // =========================================================
    // LOW STOCK ITEMS
    // =========================================================

    public List<string>
        GetLowStockItemIds(
            int threshold = 5)
    {
        EnsureShelfConfiguration();

        List<string> itemIds =
            new List<string>();

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null ||
                shelf.IsEmpty)
            {
                continue;
            }

            if (shelf.Quantity <=
                threshold)
            {
                if (!itemIds.Contains(
                        shelf.ItemId))
                {
                    itemIds.Add(
                        shelf.ItemId
                    );
                }
            }
        }

        return itemIds;
    }

    // =========================================================
    // CLEAR RACK
    // =========================================================

    public void ClearRack()
    {
        EnsureShelfConfiguration();

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            shelf.Clear();
        }

        Debug.Log(
            "Rack stock cleared."
        );

        LogAllShelves();

        NotifyRackChanged();
    }

    // =========================================================
    // LOG ALL SHELVES
    // =========================================================

    private void LogAllShelves()
    {
        EnsureShelfConfiguration();

        Debug.Log(
            "========== RACK STATUS =========="
        );

        foreach (
            RackShelf shelf
            in shelves)
        {
            if (shelf == null)
            {
                continue;
            }

            Debug.Log(
                shelf.GetDisplay()
            );
        }

        Debug.Log(
            "================================="
        );
    }

    // =========================================================
    // EVENT
    // =========================================================

    private void NotifyRackChanged()
    {
        OnRackChanged?.Invoke();
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST 1 - Clear Rack")]
    private void TestClearRack()
    {
        EnsureShelfConfiguration();

        ClearRack();
    }

    [ContextMenu("TEST 2 - Add Milk 10")]
    private void TestAddMilk()
    {
        EnsureShelfConfiguration();

        // food_001 = Milk
        AddStock(
            "food_001",
            "Milk",
            10
        );
    }

    [ContextMenu("TEST 3 - Add Bread 10")]
    private void TestAddBread()
    {
        EnsureShelfConfiguration();

        // food_002 = Bread
        AddStock(
            "food_002",
            "Bread",
            10
        );
    }

    [ContextMenu("TEST 4 - Add Tool Kit 10")]
    private void TestAddToolKit()
    {
        EnsureShelfConfiguration();

        // tool_001 = Tool Kit
        AddStock(
            "tool_001",
            "Tool Kit",
            10
        );
    }

    [ContextMenu("TEST 5 - Remove 6 Milk")]
    private void TestRemoveMilk()
    {
        EnsureShelfConfiguration();

        // food_001 = Milk
        RemoveStock(
            "food_001",
            6
        );
    }

    [ContextMenu("TEST 6 - Show Rack")]
    private void TestShowRack()
    {
        EnsureShelfConfiguration();

        LogAllShelves();
    }

    [ContextMenu("TEST 7 - Initialize Shelves")]
    private void TestInitializeShelves()
    {
        EnsureShelfConfiguration();

        LogAllShelves();
    }

    [ContextMenu("TEST 8 - Remove 5 Milk")]
    private void TestRemove5Milk()
    {
        EnsureShelfConfiguration();

        // food_001 = Milk
        bool removed =
            RemoveStock(
                "food_001",
                5
            );

        if (removed)
        {
            Debug.Log(
                "Rack Test: Successfully removed 5 Milk."
            );
        }
        else
        {
            Debug.LogWarning(
                "Rack Test: Failed to remove 5 Milk."
            );
        }
    }

    [ContextMenu("TEST 9 - Remove 4 Milk")]
    private void TestRemove4Milk()
    {
        RemoveStock("food_001", 4);
    }

    [ContextMenu("TEST 10 - Remove 6 Bread")]
    private void TestRemove6Bread()
    {
        RemoveStock("food_002", 6);
    }

    [ContextMenu("TEST 11 - Remove 6 ToolKit")]
    private void TestRemove6Toolkit()
    {
        RemoveStock("tool_001", 6);
    }
}