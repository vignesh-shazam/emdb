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

    private void OnEnable()
    {
        CustomerManager.OnCustomerListChanged +=
            RefreshUI;

        InventoryManager.OnInventoryChanged +=
            RefreshUI;
    }

    private void OnDisable()
    {
        CustomerManager.OnCustomerListChanged -=
            RefreshUI;

        InventoryManager.OnInventoryChanged -=
            RefreshUI;
    }

    private void Start()
    {
        InitializeUI();

        RefreshUI();
    }

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

        if (customerPatienceText != null)
        {
            customerPatienceText.text =
                $"Patience: {customer.Patience:0}";
        }

        if (serveCustomerButton != null)
        {
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
    }

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
            return;
        }

        RefreshUI();

        Debug.Log(
            "Customer served from UI."
        );
    }
}