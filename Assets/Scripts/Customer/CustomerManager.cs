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
        activeCustomers.Count;

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
    // GET CUSTOMER
    // =========================================================

    public Customer GetCustomer(
        string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return null;
        }

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

        Debug.Log(
            $"Customer added | " +
            $"Name: {customer.CustomerName} | " +
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
            // Another system may have already removed this customer.
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
    // CLEAR CUSTOMERS
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
            "All active customers cleared."
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
            "CUS-TEST-" + System.Guid.NewGuid().ToString("N")[..6];

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
            "CUS-TEST-" + System.Guid.NewGuid().ToString("N")[..6];

        CreateCustomer(
            customerId,
            "Test Customer",
            "food_002",
            "Bread",
            3,
            60f
        );
    }

    [ContextMenu("CreateToolkitCustomer")]
    private void CreateToolKitCustomer()
    {
        string customerId =
            "CUS-TEST-" + System.Guid.NewGuid().ToString("N")[..6];

        CreateCustomer(
            customerId,
            "Test Customer",
            "tool_001",
            "Tool Kit",
            3,
            60f
        );
    }

    [ContextMenu("TEST - Collect Current Customer Items")]
    private void TestCollectCurrentCustomerItems()
    {
        if (activeCustomers == null ||
            activeCustomers.Count == 0)
        {
            Debug.LogWarning(
                "Customer Test: No active customer."
            );

            return;
        }

        Customer customer =
            activeCustomers[0];

        if (customer == null)
        {
            Debug.LogWarning(
                "Customer Test: Current customer is null."
            );

            return;
        }

        CollectCustomerItems(customer.CustomerId);
    }

    [ContextMenu("TEST - Show Customer")]
    private void TestShowCustomer()
    {
        if (activeCustomers == null ||
            activeCustomers.Count == 0)
        {
            Debug.Log(
                "Customer Test: No active customers."
            );

            return;
        }

        foreach (Customer customer in activeCustomers)
        {
            if (customer == null)
            {
                continue;
            }

            Debug.Log(
                $"Customer | " +
                $"Name: {customer.CustomerName} | " +
                $"Request: {customer.RequestedItemName} x" +
                $"{customer.RequestedQuantity} | " +
                $"Collected: {customer.CollectedQuantity} | " +
                $"Remaining: {customer.RemainingQuantity}"
            );
        }
    }

    [ContextMenu("TEST - Clear Customers")]
    private void TestClearCustomers()
    {
        ClearCustomers();
    }
}