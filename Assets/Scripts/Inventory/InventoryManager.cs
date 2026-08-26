using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public static event Action OnInventoryChanged;

    [Header("Inventory")]
    [SerializeField]
    private List<InventoryItem> inventoryItems =
        new List<InventoryItem>();

    public IReadOnlyList<InventoryItem> InventoryItems =>
        inventoryItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeInventory();
    }

    private void InitializeInventory()
    {
        if (inventoryItems == null)
        {
            inventoryItems =
                new List<InventoryItem>();
        }

        Debug.Log(
            $"Inventory initialized | " +
            $"Items: {inventoryItems.Count}"
        );
    }

    // =========================
    // GET ITEM
    // =========================

    public InventoryItem GetItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (InventoryItem item in inventoryItems)
        {
            if (item.ItemId == itemId)
            {
                return item;
            }
        }

        return null;
    }

    // =========================
    // HAS ITEM
    // =========================

    public bool HasItem(string itemId)
    {
        InventoryItem item =
            GetItem(itemId);

        return item != null &&
               item.Quantity > 0;
    }

    // =========================
    // GET QUANTITY
    // =========================

    public int GetQuantity(string itemId)
    {
        InventoryItem item =
            GetItem(itemId);

        if (item == null)
        {
            return 0;
        }

        return item.Quantity;
    }

    // =========================
    // ADD ITEM
    // =========================

    public bool AddItem(
        string itemId,
        string itemName,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Add item failed: Item ID is empty."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning(
                "Add item failed: Item name is empty."
            );

            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                $"Add item failed: Invalid quantity: {quantity}"
            );

            return false;
        }

        InventoryItem existingItem =
            GetItem(itemId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;

            Debug.Log(
                $"Item added | " +
                $"Item: {existingItem.ItemName} | " +
                $"Added: {quantity} | " +
                $"Quantity: {existingItem.Quantity}"
            );

            NotifyInventoryChanged();

            return true;
        }

        InventoryItem newItem =
            new InventoryItem(
                itemId,
                itemName,
                quantity
            );

        inventoryItems.Add(newItem);

        Debug.Log(
            $"Item added | " +
            $"Item: {newItem.ItemName} | " +
            $"Added: {quantity} | " +
            $"Quantity: {newItem.Quantity}"
        );

        NotifyInventoryChanged();

        return true;
    }

    // =========================
    // REMOVE ITEM
    // =========================

    public bool RemoveItem(
        string itemId,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Remove item failed: Item ID is empty."
            );

            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                $"Remove item failed: Invalid quantity: {quantity}"
            );

            return false;
        }

        InventoryItem existingItem =
            GetItem(itemId);

        if (existingItem == null)
        {
            Debug.LogWarning(
                $"Remove item failed: Item not found. " +
                $"ID: {itemId}"
            );

            return false;
        }

        if (existingItem.Quantity < quantity)
        {
            Debug.LogWarning(
                $"Remove item failed: Insufficient quantity. " +
                $"Item: {existingItem.ItemName} | " +
                $"Available: {existingItem.Quantity} | " +
                $"Required: {quantity}"
            );

            return false;
        }

        existingItem.Quantity -= quantity;

        Debug.Log(
            $"Item removed | " +
            $"Item: {existingItem.ItemName} | " +
            $"Removed: {quantity} | " +
            $"Quantity: {existingItem.Quantity}"
        );

        if (existingItem.Quantity <= 0)
        {
            inventoryItems.Remove(existingItem);

            Debug.Log(
                $"Inventory item removed completely | " +
                $"Item: {existingItem.ItemName}"
            );
        }

        NotifyInventoryChanged();

        return true;
    }

    // =========================
    // SET QUANTITY
    // =========================

    public bool SetQuantity(
        string itemId,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Set quantity failed: Item ID is empty."
            );

            return false;
        }

        if (quantity < 0)
        {
            Debug.LogWarning(
                $"Set quantity failed: " +
                $"Quantity cannot be negative: {quantity}"
            );

            return false;
        }

        InventoryItem existingItem =
            GetItem(itemId);

        if (existingItem == null)
        {
            Debug.LogWarning(
                $"Set quantity failed: Item not found. " +
                $"ID: {itemId}"
            );

            return false;
        }

        if (quantity == 0)
        {
            inventoryItems.Remove(existingItem);

            Debug.Log(
                $"Inventory item removed | " +
                $"Item: {existingItem.ItemName}"
            );

            NotifyInventoryChanged();

            return true;
        }

        existingItem.Quantity = quantity;

        Debug.Log(
            $"Quantity updated | " +
            $"Item: {existingItem.ItemName} | " +
            $"Quantity: {existingItem.Quantity}"
        );

        NotifyInventoryChanged();

        return true;
    }

    // =========================
    // INVENTORY CHANGE EVENT
    // =========================

    private void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}