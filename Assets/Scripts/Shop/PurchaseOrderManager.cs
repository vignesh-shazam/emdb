using System;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseOrderManager : MonoBehaviour
{
    public static PurchaseOrderManager Instance { get; private set; }

    // --------------------------------------------------
    // DELIVERY TIME
    // --------------------------------------------------

    private const float SingleItemDeliveryTime = 30f;
    private const float MultipleItemDeliveryTime = 45f;

    // --------------------------------------------------
    // ORDER DATA
    // --------------------------------------------------

    [Header("Purchase Order")]
    [SerializeField]
    private List<PurchaseOrderItem> orderItems =
        new List<PurchaseOrderItem>();

    // --------------------------------------------------
    // PROPERTIES
    // --------------------------------------------------

    public bool HasOrder
    {
        get
        {
            return orderItems.Count > 0;
        }
    }

    public int DifferentItemCount
    {
        get
        {
            return orderItems.Count;
        }
    }

    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    public event Action OnPurchaseOrderChanged;

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

    // --------------------------------------------------
    // ADD ITEM
    // --------------------------------------------------

    public void AddItem(
        string itemId,
        string itemName,
        int boxQuantity,
        int unitsPerBox)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "PurchaseOrder: Item ID is empty."
            );

            return;
        }

        if (boxQuantity <= 0)
        {
            Debug.LogWarning(
                $"PurchaseOrder: Invalid box quantity for {itemName}."
            );

            return;
        }

        PurchaseOrderItem existingItem =
            GetOrderItem(itemId);

        if (existingItem != null)
        {
            existingItem.BoxQuantity += boxQuantity;

            Debug.Log(
                $"PurchaseOrder: Added {boxQuantity} Box(es) " +
                $"to existing {itemName} order."
            );
        }
        else
        {
            PurchaseOrderItem newItem =
                new PurchaseOrderItem(
                    itemId,
                    itemName,
                    boxQuantity,
                    unitsPerBox
                );

            orderItems.Add(newItem);

            Debug.Log(
                $"PurchaseOrder: Added {newItem.GetDisplay()}."
            );
        }

        OnPurchaseOrderChanged?.Invoke();
    }

    // --------------------------------------------------
    // GET ITEM BY ITEM ID
    // --------------------------------------------------

    public PurchaseOrderItem GetOrderItem(
        string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return null;

        for (int i = 0; i < orderItems.Count; i++)
        {
            if (orderItems[i].ItemId == itemId)
            {
                return orderItems[i];
            }
        }

        return null;
    }

    // --------------------------------------------------
    // GET ITEM BY INDEX
    // --------------------------------------------------

    public PurchaseOrderItem GetOrderItemAt(
        int index)
    {
        if (index < 0 ||
            index >= orderItems.Count)
        {
            Debug.LogWarning(
                $"PurchaseOrder: Invalid item index {index}."
            );

            return null;
        }

        return orderItems[index];
    }

    // --------------------------------------------------
    // REMOVE ITEM
    // --------------------------------------------------

    public bool RemoveItem(
        string itemId)
    {
        PurchaseOrderItem item =
            GetOrderItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"PurchaseOrder: Item not found: {itemId}"
            );

            return false;
        }

        orderItems.Remove(item);

        Debug.Log(
            $"PurchaseOrder: Removed {item.ItemName}."
        );

        OnPurchaseOrderChanged?.Invoke();

        return true;
    }

    // --------------------------------------------------
    // SET BOX QUANTITY
    // --------------------------------------------------

    public bool SetBoxQuantity(
        string itemId,
        int boxQuantity)
    {
        PurchaseOrderItem item =
            GetOrderItem(itemId);

        if (item == null)
        {
            Debug.LogWarning(
                $"PurchaseOrder: Item not found: {itemId}"
            );

            return false;
        }

        if (boxQuantity <= 0)
        {
            return RemoveItem(itemId);
        }

        item.BoxQuantity = boxQuantity;

        Debug.Log(
            $"PurchaseOrder: {item.ItemName} " +
            $"set to {boxQuantity} Box(es)."
        );

        OnPurchaseOrderChanged?.Invoke();

        return true;
    }

    // --------------------------------------------------
    // GET BOX QUANTITY
    // --------------------------------------------------

    public int GetBoxQuantity(
        string itemId)
    {
        PurchaseOrderItem item =
            GetOrderItem(itemId);

        if (item == null)
            return 0;

        return item.BoxQuantity;
    }

    // --------------------------------------------------
    // GET TOTAL UNITS
    // --------------------------------------------------

    public int GetTotalUnits(
        string itemId)
    {
        PurchaseOrderItem item =
            GetOrderItem(itemId);

        if (item == null)
            return 0;

        return item.TotalUnits;
    }

    // --------------------------------------------------
    // GET TOTAL BOX COUNT
    // --------------------------------------------------

    public int GetTotalBoxCount()
    {
        int totalBoxes = 0;

        for (int i = 0; i < orderItems.Count; i++)
        {
            if (orderItems[i] == null)
                continue;

            totalBoxes +=
                orderItems[i].BoxQuantity;
        }

        return totalBoxes;
    }

    // --------------------------------------------------
    // GET TOTAL UNIT COUNT
    // --------------------------------------------------

    public int GetTotalUnitCount()
    {
        int totalUnits = 0;

        for (int i = 0; i < orderItems.Count; i++)
        {
            if (orderItems[i] == null)
                continue;

            totalUnits +=
                orderItems[i].TotalUnits;
        }

        return totalUnits;
    }

    // --------------------------------------------------
    // DELIVERY TIME
    // --------------------------------------------------

    public float GetDeliveryTime()
    {
        if (!HasOrder)
            return 0f;

        if (DifferentItemCount == 1)
        {
            return SingleItemDeliveryTime;
        }

        return MultipleItemDeliveryTime;
    }

    // --------------------------------------------------
    // DELIVERY TIME DISPLAY
    // --------------------------------------------------

    public string GetDeliveryTimeDisplay()
    {
        float deliveryTime =
            GetDeliveryTime();

        if (deliveryTime <= 0f)
            return "No Delivery";

        return
            $"{deliveryTime:0} sec";
    }

    // --------------------------------------------------
    // CLEAR ORDER
    // --------------------------------------------------

    public void ClearOrder()
    {
        if (orderItems.Count == 0)
            return;

        orderItems.Clear();

        Debug.Log(
            "PurchaseOrder: Order cleared."
        );

        OnPurchaseOrderChanged?.Invoke();
    }

    // --------------------------------------------------
    // LOG ORDER
    // --------------------------------------------------

    public void LogOrder()
    {
        Debug.Log(
            "========== PURCHASE ORDER =========="
        );

        if (!HasOrder)
        {
            Debug.Log(
                "PurchaseOrder: No items in order."
            );

            return;
        }

        for (int i = 0; i < orderItems.Count; i++)
        {
            PurchaseOrderItem item =
                orderItems[i];

            if (item == null)
                continue;

            Debug.Log(
                $"{i + 1}. {item.GetDisplay()}"
            );
        }

        Debug.Log(
            $"Different Items: {DifferentItemCount}"
        );

        Debug.Log(
            $"Total Boxes: {GetTotalBoxCount()}"
        );

        Debug.Log(
            $"Total Units: {GetTotalUnitCount()}"
        );

        Debug.Log(
            $"Delivery Time: {GetDeliveryTimeDisplay()}"
        );

        Debug.Log(
            "===================================="
        );
    }

    // --------------------------------------------------
    // TEST 1
    // --------------------------------------------------

    [ContextMenu("TEST 1 - Clear Order")]
    private void TestClearOrder()
    {
        ClearOrder();

        Debug.Log(
            "PurchaseOrder Test: Order cleared."
        );
    }

    // --------------------------------------------------
    // TEST 2
    // --------------------------------------------------

    [ContextMenu("TEST 2 - Milk 2 Boxes")]
    private void TestMilkOrder()
    {
        ClearOrder();

        AddItem(
            "food_002",
            "Milk",
            2,
            10
        );

        LogOrder();
    }

    // --------------------------------------------------
    // TEST 3
    // --------------------------------------------------

    [ContextMenu("TEST 3 - Bread 2 Boxes")]
    private void TestBreadOrder()
    {
        AddItem(
            "food_001",
            "Bread",
            2,
            10
        );

        LogOrder();
    }

    // --------------------------------------------------
    // TEST 4
    // --------------------------------------------------

    [ContextMenu("TEST 4 - Tool Kit 3 Boxes")]
    private void TestToolKitOrder()
    {
        AddItem(
            "tool_001",
            "Tool Kit",
            3,
            10
        );

        LogOrder();
    }

    // --------------------------------------------------
    // TEST 5
    // --------------------------------------------------

    [ContextMenu("TEST 5 - Show Order")]
    private void TestShowOrder()
    {
        LogOrder();
    }
}