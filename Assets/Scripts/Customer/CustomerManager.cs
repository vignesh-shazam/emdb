using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    public static event Action OnCustomerListChanged;

    [Header("Customers")]
    [SerializeField]
    private List<Customer> activeCustomers =
        new List<Customer>();

    public IReadOnlyList<Customer> ActiveCustomers =>
        activeCustomers;

    public int CustomerCount =>
        activeCustomers != null
            ? activeCustomers.Count
            : 0;

    // =========================================================
    // AWAKE
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

        InitializeCustomers();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    private void InitializeCustomers()
    {
        if (activeCustomers == null)
        {
            activeCustomers =
                new List<Customer>();
        }

        Debug.Log(
            $"CustomerManager initialized | " +
            $"Customers: {activeCustomers.Count}"
        );
    }

    // =========================================================
    // GET CURRENT CUSTOMER
    // =========================================================

    /// <summary>
    /// Returns the First customer in the queue.
    /// The First customer is the current customer.
    /// </summary>
    public Customer GetCurrentCustomer()
    {
        RemoveNullCustomers();

        if (activeCustomers.Count == 0)
        {
            return null;
        }

        return activeCustomers[0];
    }

    // =========================================================
    // GET CUSTOMER BY QUEUE POSITION
    // =========================================================

    /// <summary>
    /// Queue position starts from 1.
    /// Position 1 is the current customer.
    /// </summary>
    public Customer GetCustomerAtQueuePosition(
        int queuePosition)
    {
        RemoveNullCustomers();

        if (queuePosition <= 0)
        {
            Debug.LogWarning(
                "Get customer failed: " +
                "Queue position must start from 1."
            );

            return null;
        }

        int index =
            queuePosition - 1;

        if (index >= activeCustomers.Count)
        {
            Debug.LogWarning(
                $"Get customer failed: " +
                $"Queue position {queuePosition} does not exist."
            );

            return null;
        }

        return activeCustomers[index];
    }

    // =========================================================
    // GET QUEUE POSITION
    // =========================================================

    /// <summary>
    /// Returns the customer's queue position.
    /// Returns -1 when the customer is not in the queue.
    /// </summary>
    public int GetQueuePosition(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return -1;
        }

        RemoveNullCustomers();

        for (int i = 0; i < activeCustomers.Count; i++)
        {
            Customer customer =
                activeCustomers[i];

            if (customer == null)
            {
                continue;
            }

            if (customer.CustomerId == customerId)
            {
                return i + 1;
            }
        }

        return -1;
    }

    // =========================================================
    // GET CUSTOMER
    // =========================================================

    public Customer GetCustomer(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return null;
        }

        RemoveNullCustomers();

        foreach (Customer customer in activeCustomers)
        {
            if (customer == null)
            {
                continue;
            }

            if (customer.CustomerId == customerId)
            {
                return customer;
            }
        }

        return null;
    }

    // =========================================================
    // ADD CUSTOMER
    // =========================================================

    public bool AddCustomer(
        Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Add customer failed: Customer is null."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(customer.CustomerId))
        {
            Debug.LogWarning(
                "Add customer failed: Customer ID is empty."
            );

            return false;
        }

        if (GetCustomer(customer.CustomerId) != null)
        {
            Debug.LogWarning(
                $"Add customer failed: " +
                $"Customer already exists: " +
                $"{customer.CustomerId}"
            );

            return false;
        }

        activeCustomers.Add(customer);

        int queuePosition =
            activeCustomers.Count;

        Debug.Log(
            $"Customer added | " +
            $"Queue Position: {queuePosition} | " +
            $"Name: {customer.CustomerName} | " +
            $"Item: {customer.RequestedItemName} x" +
            $"{customer.RequestedQuantity} | " +
            $"ID: {customer.CustomerId}"
        );

        OnCustomerListChanged?.Invoke();

        return true;
    }

    // =========================================================
    // REMOVE CUSTOMER
    // =========================================================

    public bool RemoveCustomer(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            Debug.LogWarning(
                "Remove customer skipped: Customer ID is empty."
            );

            return false;
        }

        Customer customer =
            GetCustomer(customerId);

        if (customer == null)
        {
            Debug.Log(
                $"Remove customer skipped: " +
                $"Customer already removed or no longer active. " +
                $"ID: {customerId}"
            );

            return false;
        }

        bool removed =
            activeCustomers.Remove(customer);

        if (!removed)
        {
            Debug.LogWarning(
                $"Remove customer failed: " +
                $"Could not remove customer from active list. " +
                $"ID: {customerId}"
            );

            return false;
        }

        Debug.Log(
            $"Customer removed | " +
            $"Name: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Result: {customer.Result}"
        );

        LogCurrentCustomer();

        OnCustomerListChanged?.Invoke();

        return true;
    }

    // =========================================================
    // HAS CUSTOMER
    // =========================================================

    public bool HasCustomer(
        string customerId)
    {
        return GetCustomer(customerId) != null;
    }

    // =========================================================
    // CLEAR Customers
    // =========================================================

    public void ClearCustomers()
    {
        if (activeCustomers == null)
        {
            activeCustomers =
                new List<Customer>();
        }

        activeCustomers.Clear();

        OnCustomerListChanged?.Invoke();

        Debug.Log(
            "All active Customers cleared."
        );
    }

    // =========================================================
    // CREATE CUSTOMER
    // =========================================================

    public Customer CreateCustomer(
        string customerId,
        string customerName,
        string itemId,
        string itemName,
        int quantity = 1,
        float patience = 100f)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            Debug.LogWarning(
                "Create customer failed: Customer ID is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            Debug.LogWarning(
                "Create customer failed: Customer name is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Create customer failed: Item ID is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning(
                "Create customer failed: Item name is empty."
            );

            return null;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning(
                "Create customer failed: " +
                "Quantity must be greater than zero."
            );

            return null;
        }

        Customer customer =
            new Customer(
                customerId,
                customerName,
                itemId,
                itemName,
                quantity,
                patience
            );

        bool added =
            AddCustomer(customer);

        if (!added)
        {
            return null;
        }

        Debug.Log(
            $"Customer request created | " +
            $"Customer: {customer.CustomerName} | " +
            $"Request: {customer.RequestedItemName} x" +
            $"{customer.RequestedQuantity} | " +
            $"Queue Position: " +
            $"{GetQueuePosition(customer.CustomerId)} | " +
            $"Result: {customer.Result}"
        );

        return customer;
    }

    // =========================================================
    // COLLECT CUSTOMER ITEMS FROM RACK
    // =========================================================

    public bool CollectCustomerItems(
        string customerId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "Collection failed: ShopManager not found."
            );

            return false;
        }

        if (!ShopManager.Instance.IsShopOpen)
        {
            Debug.LogWarning(
                "Collection failed: Shop is currently closed."
            );

            return false;
        }

        Customer customer =
            GetCustomer(customerId);

        if (customer == null)
        {
            Debug.LogWarning(
                $"Collection failed: Customer not found. " +
                $"ID: {customerId}"
            );

            return false;
        }

        Customer currentCustomer =
            GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.LogWarning(
                "Collection failed: No current customer."
            );

            return false;
        }

        if (currentCustomer.CustomerId != customer.CustomerId)
        {
            Debug.LogWarning(
                $"Collection failed: Only the current customer " +
                $"can collect items. " +
                $"Current Customer: {currentCustomer.CustomerName} | " +
                $"Requested Customer: {customer.CustomerName}"
            );

            return false;
        }

        if (!customer.HasValidRequest())
        {
            Debug.LogWarning(
                $"Collection failed: Invalid customer request. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        if (RackManager.Instance == null)
        {
            Debug.LogError(
                "Collection failed: RackManager not found."
            );

            return false;
        }

        int remainingQuantity =
            customer.RemainingQuantity;

        if (remainingQuantity <= 0)
        {
            Debug.LogWarning(
                $"Collection failed: " +
                $"Customer already collected all requested items. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        int rackQuantity =
            RackManager.Instance.GetQuantity(
                customer.RequestedItemId
            );

        if (rackQuantity < remainingQuantity)
        {
            Debug.LogWarning(
                $"Collection failed: Not enough rack stock. " +
                $"Item: {customer.RequestedItemName} | " +
                $"Item ID: {customer.RequestedItemId} | " +
                $"Required: {remainingQuantity} | " +
                $"Rack Available: {rackQuantity}"
            );

            return false;
        }

        bool removed =
            RackManager.Instance.RemoveStock(
                customer.RequestedItemId,
                remainingQuantity
            );

        if (!removed)
        {
            Debug.LogWarning(
                $"Collection failed: Could not remove items " +
                $"from rack. " +
                $"Item: {customer.RequestedItemName}"
            );

            return false;
        }

        bool collected =
            customer.CollectItems(remainingQuantity);

        if (!collected)
        {
            Debug.LogError(
                $"Collection ERROR: Rack stock was removed " +
                $"but customer collection failed. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        Debug.Log(
            $"CUSTOMER ITEMS COLLECTED | " +
            $"Queue Position: " +
            $"{GetQueuePosition(customer.CustomerId)} | " +
            $"Customer: {customer.CustomerName} | " +
            $"Item: {customer.RequestedItemName} | " +
            $"Quantity: {remainingQuantity} | " +
            $"Collected: {customer.CollectedQuantity} | " +
            $"Remaining: {customer.RemainingQuantity} | " +
            $"Rack Remaining: " +
            $"{RackManager.Instance.GetQuantity(customer.RequestedItemId)}"
        );

        OnCustomerListChanged?.Invoke();

        return true;
    }

    // =========================================================
    // LOG CURRENT CUSTOMER
    // =========================================================

    private void LogCurrentCustomer()
    {
        Customer currentCustomer =
            GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.Log(
                "Queue status: No active Customers Queue is empty."
            );

            return;
        }

        Debug.Log(
            $"New current customer | " +
            $"Name: {currentCustomer.CustomerName} | " +
            $"Item: {currentCustomer.RequestedItemName} x" +
            $"{currentCustomer.RequestedQuantity} | " +
            $"Queue Count: {activeCustomers.Count}"
        );
    }

    // =========================================================
    // REMOVE NULL Customers
    // =========================================================

    private void RemoveNullCustomers()
    {
        if (activeCustomers == null)
        {
            activeCustomers =
                new List<Customer>();

            return;
        }

        int removedCount =
            activeCustomers.RemoveAll(
                customer => customer == null
            );

        if (removedCount > 0)
        {
            Debug.LogWarning(
                $"Removed {removedCount} null customer(s) " +
                $"from the active queue."
            );

            OnCustomerListChanged?.Invoke();
        }
    }

    // =========================================================
    // LEGACY SERVE CUSTOMER - DISABLED
    // =========================================================

    [Obsolete(
        "ServeCustomer() is disabled. " +
        "Use CollectCustomerItems() -> BillingManager -> PaymentManager instead."
    )]
    public bool ServeCustomer(
        string customerId)
    {
        Debug.LogWarning(
            "ServeCustomer() is disabled. " +
            "Use the new flow: " +
            "CollectCustomerItems() -> Billing -> Payment."
        );

        return false;
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("Create Milk Customer")]
    private void CreateMilkCustomer()
    {
        string customerId =
            "CUS-TEST-" +
            Guid.NewGuid().ToString("N")[..6];

        CreateCustomer(
            customerId,
            "Test Customer",
            "food_001",
            "Milk",
            3,
            60f
        );
    }

    [ContextMenu("Create Bread Customer")]
    private void CreateBreadCustomer()
    {
        string customerId =
            "CUS-TEST-" +
            Guid.NewGuid().ToString("N")[..6];

        CreateCustomer(
            customerId,
            "Test Customer",
            "food_002",
            "Bread",
            3,
            60f
        );
    }

    [ContextMenu("Create Tool Kit Customer")]
    private void CreateToolKitCustomer()
    {
        string customerId =
            "CUS-TEST-" +
            Guid.NewGuid().ToString("N")[..6];

        CreateCustomer(
            customerId,
            "Test Customer",
            "tool_001",
            "Tool Kit",
            3,
            60f
        );
    }

    // =========================================================
    // TEST - COLLECT CURRENT CUSTOMER ITEMS
    // =========================================================

    [ContextMenu("TEST - Collect Current Customer Items")]
    private void TestCollectCurrentCustomerItems()
    {
        Customer currentCustomer =
            GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.LogWarning(
                "Customer Test: No active current customer."
            );

            return;
        }

        Debug.Log(
            $"Testing collection for current customer | " +
            $"Name: {currentCustomer.CustomerName} | " +
            $"Item: {currentCustomer.RequestedItemName} | " +
            $"Queue Position: 1"
        );

        CollectCustomerItems(
            currentCustomer.CustomerId
        );
    }

    // =========================================================
    // TEST - SHOW CUSTOMER DETAILS
    // =========================================================

    [ContextMenu("TEST - Show Customer")]
    private void TestShowCustomer()
    {
        RemoveNullCustomers();

        if (activeCustomers.Count == 0)
        {
            Debug.Log(
                "Customer Test: No active Customers"
            );

            return;
        }

        for (int i = 0; i < activeCustomers.Count; i++)
        {
            Customer customer =
                activeCustomers[i];

            if (customer == null)
            {
                continue;
            }

            Debug.Log(
                $"Customer | " +
                $"Queue Position: {i + 1} | " +
                $"Name: {customer.CustomerName} | " +
                $"ID: {customer.CustomerId} | " +
                $"Item ID: {customer.RequestedItemId} | " +
                $"Request: {customer.RequestedItemName} x" +
                $"{customer.RequestedQuantity} | " +
                $"Collected: {customer.CollectedQuantity} | " +
                $"Remaining: {customer.RemainingQuantity} | " +
                $"Result: {customer.Result}"
            );
        }
    }

    // =========================================================
    // TEST - DEBUG CUSTOMER QUEUE
    // =========================================================

    [ContextMenu("TEST - Debug Customer Queue")]
    private void DebugCustomerQueue()
    {
        RemoveNullCustomers();

        Debug.Log(
            "========== CUSTOMER QUEUE =========="
        );

        if (activeCustomers.Count == 0)
        {
            Debug.Log(
                "Queue is empty."
            );

            Debug.Log(
                "===================================="
            );

            return;
        }

        for (int i = 0; i < activeCustomers.Count; i++)
        {
            Customer customer =
                activeCustomers[i];

            if (customer == null)
            {
                continue;
            }

            string Customerstatus =
                i == 0
                    ? "CURRENT CUSTOMER"
                    : "WAITING CUSTOMER";

            Debug.Log(
                $"Position {i + 1} | " +
                $"{Customerstatus} | " +
                $"Name: {customer.CustomerName} | " +
                $"ID: {customer.CustomerId} | " +
                $"Item: {customer.RequestedItemName} | " +
                $"Item ID: {customer.RequestedItemId} | " +
                $"Requested: {customer.RequestedQuantity} | " +
                $"Collected: {customer.CollectedQuantity} | " +
                $"Remaining: {customer.RemainingQuantity} | " +
                $"Result: {customer.Result}"
            );
        }

        Debug.Log(
            $"Total Queue Customers: {activeCustomers.Count}"
        );

        Debug.Log(
            "===================================="
        );
    }

    // =========================================================
    // TEST - SHOW CURRENT CUSTOMER
    // =========================================================

    [ContextMenu("TEST - Show Current Customer")]
    private void TestShowCurrentCustomer()
    {
        Customer currentCustomer =
            GetCurrentCustomer();

        if (currentCustomer == null)
        {
            Debug.Log(
                "Current customer: None. Queue is empty."
            );

            return;
        }

        Debug.Log(
            $"Current customer | " +
            $"Position: 1 | " +
            $"Name: {currentCustomer.CustomerName} | " +
            $"Item: {currentCustomer.RequestedItemName} x" +
            $"{currentCustomer.RequestedQuantity} | " +
            $"Collected: {currentCustomer.CollectedQuantity} | " +
            $"Remaining: {currentCustomer.RemainingQuantity}"
        );
    }

    // =========================================================
    // TEST - CLEAR Customers
    // =========================================================

    [ContextMenu("TEST - Clear Customers")]
    private void TestClearCustomers()
    {
        ClearCustomers();
    }
}