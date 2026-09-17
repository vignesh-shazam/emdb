using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaymentUI : MonoBehaviour
{
    public static PaymentUI Instance { get; private set; }

    [Header("Panel Reference")]
    [SerializeField]
    private GameObject paymentPanel;

    [Header("Text References")]
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text amountText;

    [SerializeField]
    private TMP_Text paymentMethodText;

    [Header("Payment Method Buttons")]
    [SerializeField]
    private Button cashButton;

    [SerializeField]
    private Button cardButton;

    [SerializeField]
    private Button upiButton;

    [Header("Action Buttons")]
    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Button cancelButton;

    private PaymentManager.PaymentMethod selectedPaymentMethod =
        PaymentManager.PaymentMethod.None;

    public PaymentManager.PaymentMethod SelectedPaymentMethod =>
        selectedPaymentMethod;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("PaymentUI initialized.");
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ValidateReferences();

        RefreshUI();

        Debug.Log(
            $"PaymentUI startup completed | " +
            $"Panel Assigned: {paymentPanel != null} | " +
            $"ShopManager Found: {ShopManager.Instance != null} | " +
            $"BillingManager Found: {BillingManager.Instance != null}"
        );
    }

    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        BillingManager.OnBillingUpdated += RefreshUI;

        RegisterButtonListeners();

        RefreshUI();
    }

    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        BillingManager.OnBillingUpdated -= RefreshUI;

        RemoveButtonListeners();
    }

    // =========================================================
    // VALIDATE REFERENCES
    // =========================================================

    private void ValidateReferences()
    {
        if (paymentPanel == null)
        {
            Debug.LogError(
                "PaymentUI setup error: Payment Panel is not assigned."
            );
        }

        if (titleText == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Title Text is not assigned."
            );
        }

        if (amountText == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Amount Text is not assigned."
            );
        }

        if (paymentMethodText == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Payment Method Text is not assigned."
            );
        }

        if (cashButton == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Cash Button is not assigned."
            );
        }

        if (cardButton == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Card Button is not assigned."
            );
        }

        if (upiButton == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: UPI Button is not assigned."
            );
        }

        if (confirmButton == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Confirm Button is not assigned."
            );
        }

        if (cancelButton == null)
        {
            Debug.LogWarning(
                "PaymentUI setup warning: Cancel Button is not assigned."
            );
        }
    }

    // =========================================================
    // REGISTER BUTTON LISTENERS
    // =========================================================

    private void RegisterButtonListeners()
    {
        if (cashButton != null)
        {
            cashButton.onClick.RemoveListener(SelectCash);
            cashButton.onClick.AddListener(SelectCash);
        }

        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(SelectCard);
            cardButton.onClick.AddListener(SelectCard);
        }

        if (upiButton != null)
        {
            upiButton.onClick.RemoveListener(SelectUPI);
            upiButton.onClick.AddListener(SelectUPI);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(ConfirmPayment);
            confirmButton.onClick.AddListener(ConfirmPayment);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(ClosePanel);
            cancelButton.onClick.AddListener(ClosePanel);
        }
    }

    // =========================================================
    // REMOVE BUTTON LISTENERS
    // =========================================================

    private void RemoveButtonListeners()
    {
        if (cashButton != null)
        {
            cashButton.onClick.RemoveListener(SelectCash);
        }

        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(SelectCard);
        }

        if (upiButton != null)
        {
            upiButton.onClick.RemoveListener(SelectUPI);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(ConfirmPayment);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(ClosePanel);
        }
    }

    // =========================================================
    // SHOP STATUS
    // =========================================================

    private bool IsShopOpen()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "PaymentUI failed: ShopManager.Instance is NULL."
            );

            return false;
        }

        return ShopManager.Instance.IsShopOpen;
    }

    // =========================================================
    // SHOW PAYMENT PANEL
    // =========================================================

    public void ShowPaymentPanel()
    {
        Debug.Log("PaymentUI: ShowPaymentPanel() called.");

        if (paymentPanel == null)
        {
            Debug.LogError(
                "PaymentUI failed: Payment Panel reference is not assigned."
            );

            return;
        }

        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "PaymentUI failed: ShopManager.Instance is NULL."
            );

            return;
        }

        if (!ShopManager.Instance.IsShopOpen)
        {
            Debug.LogWarning(
                "PaymentUI failed: Shop is currently closed."
            );

            return;
        }

        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "PaymentUI failed: BillingManager.Instance is NULL."
            );

            return;
        }

        if (!BillingManager.Instance.HasBill)
        {
            Debug.LogWarning(
                "PaymentUI failed: No active bill is available."
            );

            return;
        }

        selectedPaymentMethod =
            PaymentManager.PaymentMethod.None;

        paymentPanel.SetActive(true);

        RefreshUI();

        Debug.Log(
            $"Payment panel opened successfully | " +
            $"Amount: ₹{BillingManager.Instance.CurrentTotalAmount:N0}"
        );
    }

    // =========================================================
    // REFRESH UI
    // =========================================================

    public void RefreshUI()
    {
        if (titleText != null)
        {
            titleText.text = "PAYMENT";
        }

        if (ShopManager.Instance == null)
        {
            SetPaymentButtonsInteractable(false);

            if (amountText != null)
            {
                amountText.text = "Shop unavailable.";
            }

            if (paymentMethodText != null)
            {
                paymentMethodText.text =
                    "Payment Method: Not Available";
            }

            return;
        }

        if (!ShopManager.Instance.IsShopOpen)
        {
            if (amountText != null)
            {
                amountText.text = "Shop is currently closed.";
            }

            if (paymentMethodText != null)
            {
                paymentMethodText.text =
                    "Payment is unavailable.";
            }

            SetPaymentButtonsInteractable(false);

            return;
        }

        if (BillingManager.Instance == null)
        {
            if (amountText != null)
            {
                amountText.text = "Billing system unavailable.";
            }

            if (paymentMethodText != null)
            {
                paymentMethodText.text =
                    "Payment Method: Not Available";
            }

            SetPaymentButtonsInteractable(false);

            return;
        }

        if (!BillingManager.Instance.HasBill)
        {
            if (amountText != null)
            {
                amountText.text = "Amount: ₹0";
            }

            if (paymentMethodText != null)
            {
                paymentMethodText.text =
                    "Payment Method: Not Selected";
            }

            SetPaymentButtonsInteractable(false);

            return;
        }

        if (amountText != null)
        {
            amountText.text =
                $"Amount: ₹{BillingManager.Instance.CurrentTotalAmount:N0}";
        }

        if (paymentMethodText != null)
        {
            paymentMethodText.text =
                $"Payment Method: {GetPaymentMethodDisplay()}";
        }

        SetPaymentButtonsInteractable(true);
    }

    // =========================================================
    // BUTTON STATE
    // =========================================================

    private void SetPaymentButtonsInteractable(
        bool isInteractable)
    {
        if (cashButton != null)
        {
            cashButton.interactable = isInteractable;
        }

        if (cardButton != null)
        {
            cardButton.interactable = isInteractable;
        }

        if (upiButton != null)
        {
            upiButton.interactable = isInteractable;
        }

        bool canConfirm =
            isInteractable &&
            selectedPaymentMethod !=
            PaymentManager.PaymentMethod.None;

        if (confirmButton != null)
        {
            confirmButton.interactable = canConfirm;
        }
    }

    // =========================================================
    // PAYMENT METHOD SELECTION
    // =========================================================

    private void SelectCash()
    {
        SelectPaymentMethod(
            PaymentManager.PaymentMethod.Cash
        );
    }

    private void SelectCard()
    {
        SelectPaymentMethod(
            PaymentManager.PaymentMethod.Card
        );
    }

    private void SelectUPI()
    {
        SelectPaymentMethod(
            PaymentManager.PaymentMethod.UPI
        );
    }

    private void SelectPaymentMethod(
        PaymentManager.PaymentMethod paymentMethod)
    {
        if (!IsShopOpen())
        {
            Debug.LogWarning(
                "PaymentUI: Cannot select payment method. " +
                "Shop is closed."
            );

            return;
        }

        selectedPaymentMethod =
            paymentMethod;

        Debug.Log(
            $"PaymentUI: Selected payment method: " +
            $"{GetPaymentMethodDisplay()}"
        );

        RefreshUI();
    }

    // =========================================================
    // PAYMENT METHOD DISPLAY
    // =========================================================

    private string GetPaymentMethodDisplay()
    {
        switch (selectedPaymentMethod)
        {
            case PaymentManager.PaymentMethod.Cash:
                return "Cash";

            case PaymentManager.PaymentMethod.Card:
                return "Card";

            case PaymentManager.PaymentMethod.UPI:
                return "UPI";

            default:
                return "Not Selected";
        }
    }

    // =========================================================
    // CONFIRM PAYMENT
    // =========================================================

    private void ConfirmPayment()
    {
        Debug.Log("PaymentUI: ConfirmPayment() called.");

        if (!IsShopOpen())
        {
            Debug.LogWarning(
                "PaymentUI: Cannot confirm payment. " +
                "Shop is closed."
            );

            RefreshUI();

            return;
        }

        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "PaymentUI failed: BillingManager.Instance is NULL."
            );

            return;
        }

        if (!BillingManager.Instance.HasBill)
        {
            Debug.LogWarning(
                "PaymentUI: Cannot confirm payment without a bill."
            );

            return;
        }

        if (selectedPaymentMethod ==
            PaymentManager.PaymentMethod.None)
        {
            Debug.LogWarning(
                "PaymentUI: Please select a payment method first."
            );

            return;
        }

        if (PaymentManager.Instance == null)
        {
            Debug.LogError(
                "PaymentUI failed: PaymentManager.Instance is NULL."
            );

            return;
        }

        string customerId =
            BillingManager.Instance.CurrentCustomerId;

        bool paymentSuccessful =
            PaymentManager.Instance.ProcessPayment(
                customerId,
                selectedPaymentMethod
            );

        if (paymentSuccessful)
        {
            Debug.Log(
                "PaymentUI: Payment completed successfully."
            );

            ClosePanel();
        }
        else
        {
            Debug.LogWarning(
                "PaymentUI: Payment processing failed."
            );
        }
    }

    // =========================================================
    // CLOSE PANEL
    // =========================================================

    public void ClosePanel()
    {
        if (paymentPanel == null)
        {
            Debug.LogWarning(
                "PaymentUI: Cannot close panel. " +
                "Payment Panel reference is missing."
            );

            return;
        }

        paymentPanel.SetActive(false);

        Debug.Log(
            "PaymentUI: Payment panel closed."
        );
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST - Show Payment Panel")]
    private void TestShowPaymentPanel()
    {
        ShowPaymentPanel();
    }

    [ContextMenu("TEST - Refresh Payment UI")]
    private void TestRefreshPaymentUI()
    {
        RefreshUI();

        Debug.Log(
            "PaymentUI Test: UI refreshed."
        );
    }

    [ContextMenu("TEST - Hide Payment Panel")]
    private void TestHidePaymentPanel()
    {
        ClosePanel();

        Debug.Log(
            "PaymentUI Test: Payment panel hidden."
        );
    }

    [ContextMenu("TEST - Reset Selected Payment Method")]
    private void TestResetSelectedPaymentMethod()
    {
        selectedPaymentMethod =
            PaymentManager.PaymentMethod.None;

        RefreshUI();

        Debug.Log(
            "PaymentUI Test: Selected payment method reset."
        );
    }
}