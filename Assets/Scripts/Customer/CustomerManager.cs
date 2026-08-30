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

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeCustomers();
    }

    // =========================
    // INITIALIZE
    // =========================

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

    // =========================
    // GET CUSTOMER
    // =========================

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

    // =========================
    // ADD CUSTOMER
    // =========================

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

        if (string.IsNullOrWhiteSpace(
                customer.CustomerId))
        {
            Debug.LogWarning(
                "Add customer failed: Customer ID is empty."
            );

            return false;
        }

        if (GetCustomer(
                customer.CustomerId) != null)
        {
            Debug.LogWarning(
                $"Add customer failed: " +
                $"Customer already exists: " +
                $"{customer.CustomerId}"
            );

            return false;
        }

        activeCustomers.Add(
            customer
        );

        Debug.Log(
            $"Customer added | " +
            $"Name: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId}"
        );

        OnCustomerListChanged?.Invoke();

        return true;
    }

    // =========================
    // REMOVE CUSTOMER
    // =========================

    public bool RemoveCustomer(
        string customerId)
    {
        Customer customer =
            GetCustomer(customerId);

        if (customer == null)
        {
            Debug.LogWarning(
                $"Remove customer failed: " +
                $"Customer not found: {customerId}"
            );

            return false;
        }

        activeCustomers.Remove(
            customer
        );

        Debug.Log(
            $"Customer removed | " +
            $"Name: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Result: {customer.Result}"
        );

        OnCustomerListChanged?.Invoke();

        return true;
    }

    // =========================
    // HAS CUSTOMER
    // =========================

    public bool HasCustomer(
        string customerId)
    {
        return GetCustomer(
            customerId) != null;
    }

    // =========================
    // CLEAR CUSTOMERS
    // =========================

    public void ClearCustomers()
    {
        activeCustomers.Clear();

        OnCustomerListChanged?.Invoke();

        Debug.Log(
            "All active customers cleared."
        );
    }

    // =========================
    // CREATE CUSTOMER
    // =========================

    public Customer CreateCustomer(
        string customerId,
        string customerName,
        string itemId,
        string itemName,
        int quantity = 1,
        float patience = 100f)
    {
        if (string.IsNullOrWhiteSpace(
                customerId))
        {
            Debug.LogWarning(
                "Create customer failed: " +
                "Customer ID is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(
                customerName))
        {
            Debug.LogWarning(
                "Create customer failed: " +
                "Customer name is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(
                itemId))
        {
            Debug.LogWarning(
                "Create customer failed: " +
                "Item ID is empty."
            );

            return null;
        }

        if (string.IsNullOrWhiteSpace(
                itemName))
        {
            Debug.LogWarning(
                "Create customer failed: " +
                "Item name is empty."
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

    // =========================
    // SERVE CUSTOMER
    // =========================

    public bool ServeCustomer(
        string customerId)
    {
        Customer customer =
            GetCustomer(customerId);

        if (customer == null)
        {
            Debug.LogWarning(
                $"Serve failed: Customer not found. " +
                $"ID: {customerId}"
            );

            return false;
        }

        // =========================
        // VALIDATE REQUEST
        // =========================

        if (!customer.HasValidRequest())
        {
            Debug.LogWarning(
                $"Serve failed: Invalid customer request. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        // =========================
        // INVENTORY MANAGER
        // =========================

        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "Serve failed: InventoryManager not found."
            );

            return false;
        }

        // =========================
        // SHOP MANAGER
        // =========================

        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "Serve failed: ShopManager not found."
            );

            return false;
        }

        // =========================
        // MONEY MANAGER
        // =========================

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Serve failed: MoneyManager not found."
            );

            return false;
        }

        // =========================
        // CHECK INVENTORY
        // =========================

        int availableQuantity =
            InventoryManager.Instance.GetQuantity(
                customer.RequestedItemId
            );

        if (availableQuantity <
            customer.RequestedQuantity)
        {
            Debug.LogWarning(
                $"Serve failed: Not enough inventory. " +
                $"Item: {customer.RequestedItemName} | " +
                $"Required: {customer.RequestedQuantity} | " +
                $"Available: {availableQuantity}"
            );

            return false;
        }

        // =========================
        // GET SHOP ITEM
        // =========================

        ShopItem shopItem =
            ShopManager.Instance.GetItem(
                customer.RequestedItemId
            );

        if (shopItem == null)
        {
            Debug.LogWarning(
                $"Serve failed: Shop item not found. " +
                $"ID: {customer.RequestedItemId}"
            );

            return false;
        }

        // =========================
        // REMOVE REQUESTED ITEMS
        // =========================

        bool removed =
            InventoryManager.Instance.RemoveItem(
                customer.RequestedItemId,
                customer.RequestedQuantity
            );

        if (!removed)
        {
            Debug.LogWarning(
                "Serve failed: Could not remove " +
                "requested items from inventory."
            );

            return false;
        }

        // =========================
        // CALCULATE PAYMENT
        // =========================

        int payment =
            shopItem.SellPrice *
            customer.RequestedQuantity;

        // =========================
        // ADD PAYMENT
        // =========================

        MoneyManager.Instance.AddMoney(
            payment
        );

        // =========================
        // RECORD SHOP INCOME
        // =========================

        RecordShopSale(
            payment,
            customer
        );

        // =========================
        // COMPLETE PURCHASE
        // =========================

        ShopManager.Instance
            .CompleteCurrentCustomerPurchase();

        // =========================
        // MARK AS SERVED
        // =========================

        customer.MarkServed();

        // =========================
        // UPDATE SATISFACTION
        // =========================

        if (CustomerSatisfactionManager.Instance != null)
        {
            CustomerSatisfactionManager.Instance
                .CustomerServed(customer);
        }

        // =========================
        // GENERATE TIP
        // =========================

        if (CustomerTipManager.Instance != null)
        {
            CustomerTipManager.Instance
                .AddTip(customer);
        }

        // =========================
        // SUCCESS LOG
        // =========================

        Debug.Log(
            $"Customer served successfully | " +
            $"Customer: {customer.CustomerName} | " +
            $"ID: {customer.CustomerId} | " +
            $"Item: {customer.RequestedItemName} x" +
            $"{customer.RequestedQuantity} | " +
            $"Payment: Rs. {payment:N0} | " +
            $"Result: {customer.Result}"
        );

        // =========================
        // REMOVE CUSTOMER
        // =========================

        RemoveCustomer(
            customerId
        );

        return true;
    }

    // =========================
    // RECORD SHOP SALE
    // =========================

    private void RecordShopSale(
        int payment,
        Customer customer)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "CustomerManager: " +
                "FinanceTransactionManager not found. " +
                "Shop sale ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordIncome(
            FinanceAccountType.Shop,
            payment,
            "Shop Sale"
        );

        Debug.Log(
            $"Shop sale recorded | " +
            $"Customer: {customer.CustomerName} | " +
            $"Amount: Rs. {payment:N0}"
        );
    }
}