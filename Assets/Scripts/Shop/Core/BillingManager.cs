using System;
using UnityEngine;

public class BillingManager : MonoBehaviour
{
    public static BillingManager Instance { get; private set; }

    // =========================================================
    // EVENTS
    // =========================================================

    public static event Action OnBillingUpdated;

    // =========================================================
    // CURRENT BILL
    // =========================================================

    private string currentCustomerId = string.Empty;

    private int currentQuantity = 0;

    private int currentUnitPrice = 0;

    private int currentTotalAmount = 0;

    // =========================================================
    // PUBLIC PROPERTIES
    // =========================================================

    public string CurrentCustomerId =>
        currentCustomerId;

    public int CurrentQuantity =>
        currentQuantity;

    public int CurrentUnitPrice =>
        currentUnitPrice;

    public int CurrentTotalAmount =>
        currentTotalAmount;

    public bool HasBill =>
        !string.IsNullOrWhiteSpace(
            currentCustomerId
        ) &&
        currentQuantity > 0;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ClearBill();
    }

    // =========================================================
    // CREATE BILL
    // =========================================================

    public bool CreateBill(
        string customerId)
    {
        // =====================================================
        // CUSTOMER MANAGER
        // =====================================================

        if (CustomerManager.Instance == null)
        {
            Debug.LogError(
                "Billing failed: " +
                "CustomerManager not found."
            );

            return false;
        }

        // =====================================================
        // GET CUSTOMER
        // =====================================================

        Customer customer =
            CustomerManager.Instance.GetCustomer(
                customerId
            );

        if (customer == null)
        {
            Debug.LogWarning(
                $"Billing failed: Customer not found. " +
                $"ID: {customerId}"
            );

            return false;
        }

        // =====================================================
        // VALIDATE REQUEST
        // =====================================================

        if (!customer.HasValidRequest())
        {
            Debug.LogWarning(
                $"Billing failed: Invalid customer request. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        // =====================================================
        // CHECK COLLECTED ITEMS
        // =====================================================

        if (customer.CollectedQuantity <= 0)
        {
            Debug.LogWarning(
                $"Billing failed: Customer has not collected " +
                $"any items. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        // =====================================================
        // CHECK ALL ITEMS COLLECTED
        // =====================================================

        if (!customer.HasCollectedAllItems)
        {
            Debug.LogWarning(
                $"Billing failed: Customer has not collected " +
                $"all requested items. " +
                $"Customer: {customer.CustomerName} | " +
                $"Requested: {customer.RequestedQuantity} | " +
                $"Collected: {customer.CollectedQuantity} | " +
                $"Remaining: {customer.RemainingQuantity}"
            );

            return false;
        }

        // =====================================================
        // SHOP MANAGER
        // =====================================================

        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "Billing failed: " +
                "ShopManager not found."
            );

            return false;
        }

        // =====================================================
        // GET SHOP ITEM
        // =====================================================

        ShopItem shopItem =
            ShopManager.Instance.GetItem(
                customer.RequestedItemId
            );

        if (shopItem == null)
        {
            Debug.LogWarning(
                $"Billing failed: Shop item not found. " +
                $"Item ID: {customer.RequestedItemId}"
            );

            return false;
        }

        // =====================================================
        // GET SELLING PRICE
        // =====================================================

        int quantity =
            customer.CollectedQuantity;

        int unitPrice =
            shopItem.SellPrice;

        if (unitPrice < 0)
        {
            Debug.LogWarning(
                $"Billing failed: Invalid selling price. " +
                $"Item: {shopItem.ItemName} | " +
                $"Price: Rs. {unitPrice:N0}"
            );

            return false;
        }

        // =====================================================
        // CALCULATE TOTAL
        // =====================================================

        int totalAmount =
            quantity *
            unitPrice;

        // =====================================================
        // STORE BILL
        // =====================================================

        currentCustomerId =
            customer.CustomerId;

        currentQuantity =
            quantity;

        currentUnitPrice =
            unitPrice;

        currentTotalAmount =
            totalAmount;

        // =====================================================
        // LOG BILL
        // =====================================================

        Debug.Log(
            $"BILL CREATED | " +
            $"Customer: {customer.CustomerName} | " +
            $"Item: {shopItem.ItemName} | " +
            $"Quantity: {quantity} | " +
            $"Unit Price: Rs. {unitPrice:N0} | " +
            $"Total: Rs. {totalAmount:N0}"
        );

        // =====================================================
        // EVENT
        // =====================================================

        OnBillingUpdated?.Invoke();

        return true;
    }

    // =========================================================
    // GET BILL AMOUNT
    // =========================================================

    public int GetBillAmount(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(
                customerId))
        {
            return 0;
        }

        if (currentCustomerId !=
            customerId)
        {
            return 0;
        }

        return currentTotalAmount;
    }

    // =========================================================
    // GET BILL QUANTITY
    // =========================================================

    public int GetBillQuantity(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(
                customerId))
        {
            return 0;
        }

        if (currentCustomerId !=
            customerId)
        {
            return 0;
        }

        return currentQuantity;
    }

    // =========================================================
    // GET UNIT PRICE
    // =========================================================

    public int GetBillUnitPrice(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(
                customerId))
        {
            return 0;
        }

        if (currentCustomerId !=
            customerId)
        {
            return 0;
        }

        return currentUnitPrice;
    }

    // =========================================================
    // CLEAR BILL
    // =========================================================

    public void ClearBill()
    {
        currentCustomerId =
            string.Empty;

        currentQuantity = 0;

        currentUnitPrice = 0;

        currentTotalAmount = 0;

        OnBillingUpdated?.Invoke();
    }

    // =========================================================
    // TEST - CREATE CURRENT CUSTOMER BILL
    // =========================================================

    [ContextMenu("TEST - Create Current Customer Bill")]
    private void TestCreateCurrentCustomerBill()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogWarning(
                "Billing Test: " +
                "CustomerManager not found."
            );

            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null ||
            CustomerManager.Instance.ActiveCustomers.Count == 0)
        {
            Debug.LogWarning(
                "Billing Test: No active customer."
            );

            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        CreateBill(
            customer.CustomerId
        );
    }

    // =========================================================
    // TEST - SHOW CURRENT BILL
    // =========================================================

    [ContextMenu("TEST - Show Current Bill")]
    private void TestShowCurrentBill()
    {
        if (!HasBill)
        {
            Debug.Log(
                "Billing Test: No current bill."
            );

            return;
        }

        Debug.Log(
            $"CURRENT BILL | " +
            $"Customer ID: {currentCustomerId} | " +
            $"Quantity: {currentQuantity} | " +
            $"Unit Price: Rs. {currentUnitPrice:N0} | " +
            $"Total: Rs. {currentTotalAmount:N0}"
        );
    }

    // =========================================================
    // TEST - CLEAR BILL
    // =========================================================

    [ContextMenu("TEST - Clear Bill")]
    private void TestClearBill()
    {
        ClearBill();

        Debug.Log(
            "Billing Test: Current bill cleared."
        );
    }
}