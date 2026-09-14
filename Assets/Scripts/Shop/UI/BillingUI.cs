using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BillingUI : MonoBehaviour
{
    public static BillingUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject billingPanel;

    [Header("Text References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text customerNameText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text unitPriceText;
    [SerializeField] private TMP_Text totalText;

    [Header("Button References")]
    [SerializeField] private Button proceedButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        BillingManager.OnBillingUpdated += RefreshUI;

        if (proceedButton != null)
        {
            proceedButton.onClick.AddListener(OnProceedButtonClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        RefreshUI();
    }

    private void OnDisable()
    {
        BillingManager.OnBillingUpdated -= RefreshUI;

        if (proceedButton != null)
        {
            proceedButton.onClick.RemoveListener(OnProceedButtonClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePanel);
        }
    }

    // =========================================================
    // SHOW BILL FOR CUSTOMER
    // =========================================================

    public bool ShowBillForCustomer(string customerId)
    {
        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "BillingUI failed: BillingManager not found."
            );

            return false;
        }

        bool billCreated =
            BillingManager.Instance.CreateBill(customerId);

        if (!billCreated)
        {
            Debug.LogWarning(
                "BillingUI: Bill could not be created."
            );

            return false;
        }

        if (billingPanel != null)
        {
            billingPanel.SetActive(true);
        }

        RefreshUI();

        return true;
    }

    // =========================================================
    // REFRESH UI
    // =========================================================

    public void RefreshUI()
    {
        if (BillingManager.Instance == null)
        {
            ClearUI();
            return;
        }

        BillingManager billingManager =
            BillingManager.Instance;

        if (!billingManager.HasBill)
        {
            ClearUI();
            return;
        }

        Customer customer = null;

        if (CustomerManager.Instance != null)
        {
            customer =
                CustomerManager.Instance.GetCustomer(
                    billingManager.CurrentCustomerId
                );
        }

        if (titleText != null)
        {
            titleText.text = "BILLING";
        }

        if (customerNameText != null)
        {
            customerNameText.text =
                customer != null
                    ? $"Customer: {customer.CustomerName}"
                    : "Customer: -";
        }

        if (itemNameText != null)
        {
            string itemName =
                customer != null
                    ? customer.RequestedItemName
                    : "Unknown Item";

            itemNameText.text =
                $"Item: {itemName}";
        }

        if (quantityText != null)
        {
            quantityText.text =
                $"Quantity: {billingManager.CurrentQuantity}";
        }

        if (unitPriceText != null)
        {
            unitPriceText.text =
                $"Unit Price: RS.{billingManager.CurrentUnitPrice:N0}";
        }

        if (totalText != null)
        {
            totalText.text =
                $"TOTAL: RS.{billingManager.CurrentTotalAmount:N0}";
        }

        if (billingPanel != null &&
            !billingPanel.activeSelf)
        {
            billingPanel.SetActive(true);
        }
    }

    // =========================================================
    // CLEAR UI
    // =========================================================

    private void ClearUI()
    {
        if (titleText != null)
        {
            titleText.text = "BILLING";
        }

        if (customerNameText != null)
        {
            customerNameText.text = "Customer: -";
        }

        if (itemNameText != null)
        {
            itemNameText.text = "Item: -";
        }

        if (quantityText != null)
        {
            quantityText.text = "Quantity: -";
        }

        if (unitPriceText != null)
        {
            unitPriceText.text = "Unit Price: RS.0";
        }

        if (totalText != null)
        {
            totalText.text = "TOTAL: RS.0";
        }
    }

    // =========================================================
    // CLOSE BILLING PANEL
    // =========================================================

    public void ClosePanel()
    {
        if (billingPanel != null)
        {
            billingPanel.SetActive(false);
        }
    }

    // =========================================================
    // PROCEED TO PAYMENT
    // =========================================================

    private void OnProceedButtonClicked()
    {
        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "BillingUI failed: BillingManager not found."
            );

            return;
        }

        if (!BillingManager.Instance.HasBill)
        {
            Debug.LogWarning(
                "BillingUI: Cannot proceed without an active bill."
            );

            return;
        }

        if (PaymentUI.Instance == null)
        {
            Debug.LogError(
                "BillingUI failed: PaymentUI not found."
            );

            return;
        }

        Debug.Log(
            "BillingUI: Proceed pressed. Opening PaymentUI."
        );

        bool paymentPanelOpened =
            TryOpenPaymentPanel();

        if (paymentPanelOpened)
        {
            ClosePanel();
        }
    }

    private bool TryOpenPaymentPanel()
    {
        PaymentUI.Instance.ShowPaymentPanel();

        Debug.Log(
            "BillingUI: PaymentUI.ShowPaymentPanel() called."
        );

        return true;
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST - Show Current Customer Bill")]
    private void TestShowCurrentCustomerBill()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogWarning(
                "BillingUI Test: CustomerManager not found."
            );

            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null ||
            CustomerManager.Instance.ActiveCustomers.Count == 0)
        {
            Debug.LogWarning(
                "BillingUI Test: No active customers."
            );

            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        ShowBillForCustomer(customer.CustomerId);
    }

    [ContextMenu("TEST - Refresh Billing UI")]
    private void TestRefreshBillingUI()
    {
        RefreshUI();

        Debug.Log(
            "BillingUI Test: UI refreshed."
        );
    }

    [ContextMenu("TEST - Hide Billing Panel")]
    private void TestHideBillingPanel()
    {
        ClosePanel();

        Debug.Log(
            "BillingUI Test: Billing panel hidden."
        );
    }
}