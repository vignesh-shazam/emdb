using System;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    // --------------------------------------------------
    // DELIVERY TIMES
    // --------------------------------------------------

    private const float SingleItemDeliveryTime = 30f;
    private const float MultipleItemDeliveryTime = 45f;

    // --------------------------------------------------
    // DELIVERY STATE
    // --------------------------------------------------

    [Header("Delivery State")]
    [SerializeField] private bool deliveryInProgress;
    [SerializeField] private float remainingTime;

    public bool IsDeliveryInProgress
    {
        get { return deliveryInProgress; }
    }

    public float RemainingTime
    {
        get { return remainingTime; }
    }

    public float TotalDeliveryTime
    {
        get;
        private set;
    }

    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    public event Action OnDeliveryStarted;
    public event Action OnDeliveryCompleted;
    public event Action OnDeliveryCancelled;
    public event Action OnDeliveryUpdated;

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

    private void Update()
    {
        if (!deliveryInProgress)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            CompleteDelivery();

            return;
        }

        OnDeliveryUpdated?.Invoke();
    }

    // --------------------------------------------------
    // START DELIVERY
    // --------------------------------------------------

    public bool StartDelivery()
    {
        if (deliveryInProgress)
        {
            Debug.Log(
                "Delivery: A delivery is already in progress."
            );

            return false;
        }

        if (PurchaseOrderManager.Instance == null)
        {
            Debug.LogError(
                "Delivery: PurchaseOrderManager not found."
            );

            return false;
        }

        if (!PurchaseOrderManager.Instance.HasOrder)
        {
            Debug.Log(
                "Delivery: No purchase order available."
            );

            return false;
        }

        TotalDeliveryTime =
            PurchaseOrderManager.Instance.GetDeliveryTime();

        if (TotalDeliveryTime <= 0f)
        {
            Debug.LogError(
                "Delivery: Invalid delivery time."
            );

            return false;
        }

        remainingTime = TotalDeliveryTime;
        deliveryInProgress = true;

        Debug.Log(
            $"Delivery started. " +
            $"Delivery time: {TotalDeliveryTime:0} seconds."
        );

        OnDeliveryStarted?.Invoke();

        return true;
    }

    // --------------------------------------------------
    // COMPLETE DELIVERY
    // --------------------------------------------------

    private void CompleteDelivery()
    {
        if (!deliveryInProgress)
            return;

        bool deliverySuccessful =
            DeliverOrderToStoreRoom();

        if (!deliverySuccessful)
        {
            deliveryInProgress = false;
            remainingTime = 0f;
            TotalDeliveryTime = 0f;

            Debug.LogError(
                "Delivery failed. " +
                "The purchase order could not be delivered " +
                "to the Store Room."
            );

            return;
        }

        deliveryInProgress = false;
        remainingTime = 0f;

        Debug.Log(
            "Delivery completed successfully."
        );

        OnDeliveryCompleted?.Invoke();
    }

    // --------------------------------------------------
    // DELIVER ORDER TO STORE ROOM
    // --------------------------------------------------

    private bool DeliverOrderToStoreRoom()
    {
        if (PurchaseOrderManager.Instance == null)
        {
            Debug.LogError(
                "Delivery: PurchaseOrderManager not found."
            );

            return false;
        }

        if (StoreRoomManager.Instance == null)
        {
            Debug.LogError(
                "Delivery: StoreRoomManager not found."
            );

            return false;
        }

        if (!PurchaseOrderManager.Instance.HasOrder)
        {
            Debug.LogError(
                "Delivery: No purchase order available."
            );

            return false;
        }

        Debug.Log(
            "========== DELIVERY → STORE ROOM =========="
        );

        // --------------------------------------------------
        // MILK
        // food_001 → Milk
        // --------------------------------------------------

        DeliverItemToStoreRoom(
            "food_001",
            "Milk"
        );

        // --------------------------------------------------
        // BREAD
        // food_002 → Bread
        // --------------------------------------------------

        DeliverItemToStoreRoom(
            "food_002",
            "Bread"
        );

        // --------------------------------------------------
        // TOOL KIT
        // tool_001 → Tool Kit
        // --------------------------------------------------

        DeliverItemToStoreRoom(
            "tool_001",
            "Tool Kit"
        );

        Debug.Log(
            "==========================================="
        );

        // Clear order only after delivery processing.
        PurchaseOrderManager.Instance.ClearOrder();

        return true;
    }

    // --------------------------------------------------
    // DELIVER ONE ITEM
    // --------------------------------------------------

    private void DeliverItemToStoreRoom(
        string itemId,
        string itemName)
    {
        PurchaseOrderItem orderItem =
            PurchaseOrderManager.Instance.GetOrderItem(
                itemId
            );

        if (orderItem == null)
        {
            return;
        }

        int totalUnits =
            orderItem.TotalUnits;

        if (totalUnits <= 0)
        {
            return;
        }

        StoreRoomManager.Instance.AddStock(
            orderItem.ItemId,
            orderItem.ItemName,
            orderItem.UnitsPerBox,
            totalUnits
        );

        Debug.Log(
            $"Delivery → Store Room | " +
            $"{orderItem.ItemName} | " +
            $"{orderItem.BoxQuantity} Box(es) | " +
            $"{totalUnits} Nos"
        );
    }

    // --------------------------------------------------
    // CANCEL DELIVERY
    // --------------------------------------------------

    public void CancelDelivery()
    {
        if (!deliveryInProgress)
        {
            Debug.Log(
                "Delivery: No delivery is currently in progress."
            );

            return;
        }

        deliveryInProgress = false;
        remainingTime = 0f;
        TotalDeliveryTime = 0f;

        Debug.Log(
            "Delivery cancelled."
        );

        OnDeliveryCancelled?.Invoke();
    }

    // --------------------------------------------------
    // DISPLAY HELPERS
    // --------------------------------------------------

    public string GetRemainingTimeDisplay()
    {
        if (!deliveryInProgress)
            return "No Delivery";

        return $"{Mathf.CeilToInt(remainingTime)} sec";
    }

    public float GetProgress()
    {
        if (!deliveryInProgress ||
            TotalDeliveryTime <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(
            1f -
            (remainingTime / TotalDeliveryTime)
        );
    }

    public string GetStatusDisplay()
    {
        if (deliveryInProgress)
        {
            return
                $"Delivery in Progress\n" +
                $"{GetRemainingTimeDisplay()}";
        }

        return "No Delivery";
    }

    // --------------------------------------------------
    // TEST 1
    // SINGLE ITEM DELIVERY
    // --------------------------------------------------

    [ContextMenu("TEST 1 - Start Single Item Delivery")]
    private void TestSingleItemDelivery()
    {
        if (PurchaseOrderManager.Instance == null)
        {
            Debug.LogError(
                "Delivery Test: " +
                "PurchaseOrderManager not found."
            );

            return;
        }

        PurchaseOrderManager.Instance.ClearOrder();

        // food_001 → Milk
        PurchaseOrderManager.Instance.AddItem(
            "food_001",
            "Milk",
            2,
            10
        );

        Debug.Log(
            "Delivery Test: Milk × 2 Boxes added."
        );

        StartDelivery();
    }

    // --------------------------------------------------
    // TEST 2
    // MULTIPLE ITEM DELIVERY
    // --------------------------------------------------

    [ContextMenu("TEST 2 - Start Multiple Item Delivery")]
    private void TestMultipleItemDelivery()
    {
        if (PurchaseOrderManager.Instance == null)
        {
            Debug.LogError(
                "Delivery Test: " +
                "PurchaseOrderManager not found."
            );

            return;
        }

        PurchaseOrderManager.Instance.ClearOrder();

        // food_001 → Milk
        PurchaseOrderManager.Instance.AddItem(
            "food_001",
            "Milk",
            2,
            10
        );

        // food_002 → Bread
        PurchaseOrderManager.Instance.AddItem(
            "food_002",
            "Bread",
            2,
            10
        );

        Debug.Log(
            "Delivery Test: " +
            "Milk × 2 Boxes + Bread × 2 Boxes added."
        );

        StartDelivery();
    }

    // --------------------------------------------------
    // TEST 3
    // SHOW DELIVERY STATUS
    // --------------------------------------------------

    [ContextMenu("TEST 3 - Show Delivery Status")]
    private void TestShowDeliveryStatus()
    {
        Debug.Log(
            $"Delivery Status: {GetStatusDisplay()}"
        );

        Debug.Log(
            $"In Progress: {deliveryInProgress}"
        );

        Debug.Log(
            $"Remaining Time: {remainingTime:0.00} sec"
        );

        Debug.Log(
            $"Total Delivery Time: " +
            $"{TotalDeliveryTime:0.00} sec"
        );

        Debug.Log(
            $"Progress: " +
            $"{GetProgress() * 100f:0.0}%"
        );
    }

    // --------------------------------------------------
    // TEST 4
    // CANCEL DELIVERY
    // --------------------------------------------------

    [ContextMenu("TEST 4 - Cancel Delivery")]
    private void TestCancelDelivery()
    {
        CancelDelivery();
    }
}