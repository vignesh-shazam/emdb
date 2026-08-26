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

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerManager.OnCustomerListChanged +=
            RefreshUI;

        InventoryManager.OnInventoryChanged +=
            RefreshUI;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerManager.OnCustomerListChanged -=
            RefreshUI;

        InventoryManager.OnInventoryChanged -=
            RefreshUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        InitializeUI();

        RefreshUI();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        RefreshPatienceUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (serveCustomerButton != null)
        {
            serveCustomerButton.onClick.RemoveAllListeners();

            serveCustomerButton.onClick.AddListener(
                ServeCustomer
            );
        }
    }

    // =========================
    // REFRESH UI
    // =========================

    public void RefreshUI()
    {
        if (CustomerManager.Instance == null)
        {
            ClearUI();
            return;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            ClearUI();
            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            ClearUI();
            return;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
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

        // =========================
        // CUSTOMER NAME
        // =========================

        if (customerNameText != null)
        {
            customerNameText.text =
                customer.CustomerName;
        }

        // =========================
        // CUSTOMER REQUEST
        // =========================

        if (customerRequestText != null)
        {
            customerRequestText.text =
                $"Wants: {customer.RequestedItemName} x " +
                $"{customer.RequestedQuantity}";
        }

        // =========================
        // PATIENCE
        // =========================

        RefreshPatienceUI();

        // =========================
        // SERVE BUTTON
        // =========================

        UpdateServeButton(
            customer
        );
    }

    // =========================
    // REFRESH PATIENCE
    // =========================

    private void RefreshPatienceUI()
    {
        if (customerPatienceText == null)
        {
            return;
        }

        if (CustomerManager.Instance == null)
        {
            return;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            customerPatienceText.text =
                "Patience: 0";

            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        if (customer == null)
        {
            return;
        }

        customerPatienceText.text =
            $"Patience: {customer.Patience:0}";
    }

    // =========================
    // SERVE BUTTON
    // =========================

    private void UpdateServeButton(
        Customer customer)
    {
        if (serveCustomerButton == null)
        {
            return;
        }

        bool canServe = false;

        if (InventoryManager.Instance != null)
        {
            int availableQuantity =
                InventoryManager.Instance.GetQuantity(
                    customer.RequestedItemId
                );

            canServe =
                availableQuantity >=
                customer.RequestedQuantity;
        }

        serveCustomerButton.interactable =
            canServe;
    }

    // =========================
    // CLEAR UI
    // =========================

    private void ClearUI()
    {
        if (customerNameText != null)
        {
            customerNameText.text =
                "No Customer";
        }

        if (customerRequestText != null)
        {
            customerRequestText.text =
                "No request";
        }

        if (customerPatienceText != null)
        {
            customerPatienceText.text =
                "Patience: 0";
        }

        if (serveCustomerButton != null)
        {
            serveCustomerButton.interactable =
                false;
        }
    }

    // =========================
    // SERVE CUSTOMER
    // =========================

    private void ServeCustomer()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogError(
                "Serve failed: CustomerManager not found."
            );

            return;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            Debug.LogWarning(
                "Serve failed: No active customer."
            );

            return;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return;
        }

        Customer customer =
            CustomerManager.Instance.ActiveCustomers[0];

        if (customer == null)
        {
            return;
        }

        bool success =
            CustomerManager.Instance.ServeCustomer(
                customer.CustomerId
            );

        if (!success)
        {
            RefreshUI();
            return;
        }

        RefreshUI();

        Debug.Log(
            "Customer served from UI."
        );
    }
}