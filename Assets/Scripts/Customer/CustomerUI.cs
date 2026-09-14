using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerUI : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private TMP_Text customerNameText;
    [SerializeField] private TMP_Text customerRequestText;
    [SerializeField] private TMP_Text customerPatienceText;

    [Header("Actions")]
    [SerializeField] private Button serveCustomerButton;

    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        CustomerManager.OnCustomerListChanged += RefreshUI;
        InventoryManager.OnInventoryChanged += RefreshUI;
        BillingManager.OnBillingUpdated += RefreshUI;
    }

    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        CustomerManager.OnCustomerListChanged -= RefreshUI;
        InventoryManager.OnInventoryChanged -= RefreshUI;
        BillingManager.OnBillingUpdated -= RefreshUI;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        InitializeUI();
        RefreshUI();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        RefreshPatienceUI();
    }

    // =========================================================
    // INITIALIZE UI
    // =========================================================

    private void InitializeUI()
    {
        if (serveCustomerButton == null)
        {
            return;
        }

        serveCustomerButton.onClick.RemoveAllListeners();
        serveCustomerButton.onClick.AddListener(ProcessCustomerFlow);
    }

    // =========================================================
    // REFRESH UI
    // =========================================================

    public void RefreshUI()
    {
        if (CustomerManager.Instance == null)
        {
            ClearUI();
            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null ||
            CustomerManager.Instance.ActiveCustomers.Count == 0)
        {
            ClearUI();
            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        if (customer == null)
        {
            ClearUI();
            return;
        }

        if (customerNameText != null)
        {
            customerNameText.text =
                customer.CustomerName;
        }

        if (customerRequestText != null)
        {
            customerRequestText.text =
                $"Wants: {customer.RequestedItemName} x " +
                $"{customer.RequestedQuantity}";
        }

        RefreshPatienceUI();
        UpdateActionButton(customer);
    }

    // =========================================================
    // REFRESH PATIENCE
    // =========================================================

    private void RefreshPatienceUI()
    {
        if (customerPatienceText == null)
        {
            return;
        }

        if (CustomerManager.Instance == null ||
            CustomerManager.Instance.ActiveCustomers == null ||
            CustomerManager.Instance.ActiveCustomers.Count == 0)
        {
            customerPatienceText.text = "Patience: 0";
            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        if (customer == null)
        {
            customerPatienceText.text = "Patience: 0";
            return;
        }

        customerPatienceText.text =
            $"Patience: {customer.Patience:0}";
    }

    // =========================================================
    // UPDATE ACTION BUTTON
    // =========================================================

    private void UpdateActionButton(Customer customer)
    {
        if (serveCustomerButton == null)
        {
            return;
        }

        if (ShopManager.Instance == null ||
            !ShopManager.Instance.IsShopOpen)
        {
            serveCustomerButton.interactable = false;
            return;
        }

        if (BillingManager.Instance != null &&
            BillingManager.Instance.HasBill)
        {
            serveCustomerButton.interactable = false;
            return;
        }

        if (customer.HasCollectedAllItems)
        {
            serveCustomerButton.interactable = true;
            return;
        }

        bool canCollectItems = false;

        if (InventoryManager.Instance != null)
        {
            int availableQuantity =
                InventoryManager.Instance.GetQuantity(
                    customer.RequestedItemId
                );

            canCollectItems =
                availableQuantity >=
                customer.RemainingQuantity;
        }

        serveCustomerButton.interactable =
            canCollectItems;
    }

    // =========================================================
    // CUSTOMER FLOW
    // =========================================================

    private void ProcessCustomerFlow()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogError(
                "Customer flow failed: CustomerManager not found."
            );

            return;
        }

        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "Customer flow failed: ShopManager not found."
            );

            return;
        }

        if (!ShopManager.Instance.IsShopOpen)
        {
            Debug.LogWarning(
                "Customer flow failed: Shop is currently closed."
            );

            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null ||
            CustomerManager.Instance.ActiveCustomers.Count == 0)
        {
            Debug.LogWarning(
                "Customer flow failed: No active customer."
            );

            RefreshUI();
            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        if (customer == null)
        {
            RefreshUI();
            return;
        }

        // =====================================================
        // STEP 1: COLLECT ITEMS
        // =====================================================

        if (!customer.HasCollectedAllItems)
        {
            bool collected =
                CustomerManager.Instance.CollectCustomerItems(
                    customer.CustomerId
                );

            if (!collected)
            {
                Debug.LogWarning(
                    "Customer flow stopped: Item collection failed."
                );

                RefreshUI();
                return;
            }

            Debug.Log(
                $"CustomerUI: Items collected for " +
                $"{customer.CustomerName}."
            );
        }

        // =====================================================
        // STEP 2: CREATE BILL
        // =====================================================

        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "Customer flow failed: BillingManager not found."
            );

            return;
        }

        bool billCreated =
            BillingManager.Instance.CreateBill(
                customer.CustomerId
            );

        if (!billCreated)
        {
            Debug.LogWarning(
                "Customer flow stopped: Bill creation failed."
            );

            RefreshUI();
            return;
        }

        // =====================================================
        // STEP 3: OPEN PAYMENT UI
        // =====================================================

        if (PaymentUI.Instance == null)
        {
            Debug.LogError(
                "Customer flow failed: PaymentUI not found."
            );

            return;
        }

        PaymentUI.Instance.ShowPaymentPanel();

        Debug.Log(
            $"CustomerUI: Payment panel opened for " +
            $"{customer.CustomerName}."
        );

        RefreshUI();
    }

    // =========================================================
    // CLEAR UI
    // =========================================================

    private void ClearUI()
    {
        if (customerNameText != null)
        {
            customerNameText.text = "No Customer";
        }

        if (customerRequestText != null)
        {
            customerRequestText.text = "No request";
        }

        if (customerPatienceText != null)
        {
            customerPatienceText.text = "Patience: 0";
        }

        if (serveCustomerButton != null)
        {
            serveCustomerButton.interactable = false;
        }
    }
}