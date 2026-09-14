using System;
using UnityEngine;

public class PaymentManager : MonoBehaviour
{
    public static PaymentManager Instance { get; private set; }

    public static event Action OnPaymentCompleted;

    public enum PaymentMethod
    {
        None,
        Cash,
        Card,
        UPI
    }

    public PaymentMethod LastPaymentMethod { get; private set; }

    public int LastPaymentAmount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LastPaymentMethod = PaymentMethod.None;
        LastPaymentAmount = 0;
    }

    public bool ProcessPayment(
        string customerId,
        PaymentMethod paymentMethod)
    {
        // =========================
        // SHOP STATUS VALIDATION
        // =========================

        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "Payment failed: ShopManager not found."
            );

            return false;
        }

        if (!ShopManager.Instance.IsShopOpen)
        {
            Debug.LogWarning(
                "Payment failed: Shop is currently closed."
            );

            return false;
        }

        // =========================
        // VALIDATE PAYMENT METHOD
        // =========================

        if (paymentMethod == PaymentMethod.None)
        {
            Debug.LogWarning(
                "Payment failed: Payment method not selected."
            );

            return false;
        }

        // =========================
        // BILLING MANAGER
        // =========================

        if (BillingManager.Instance == null)
        {
            Debug.LogError(
                "Payment failed: BillingManager not found."
            );

            return false;
        }

        if (!BillingManager.Instance.HasBill)
        {
            Debug.LogWarning(
                "Payment failed: No active bill."
            );

            return false;
        }

        if (BillingManager.Instance.CurrentCustomerId != customerId)
        {
            Debug.LogWarning(
                "Payment failed: Customer ID does not match the current bill."
            );

            return false;
        }

        // =========================
        // CUSTOMER MANAGER
        // =========================

        if (CustomerManager.Instance == null)
        {
            Debug.LogError(
                "Payment failed: CustomerManager not found."
            );

            return false;
        }

        Customer customer =
            CustomerManager.Instance.GetCustomer(customerId);

        if (customer == null)
        {
            Debug.LogWarning(
                $"Payment failed: Customer not found. ID: {customerId}"
            );

            return false;
        }

        if (!customer.HasValidRequest())
        {
            Debug.LogWarning(
                $"Payment failed: Invalid customer request. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        if (!customer.HasCollectedAllItems)
        {
            Debug.LogWarning(
                $"Payment failed: Customer items are not fully collected. " +
                $"Customer: {customer.CustomerName}"
            );

            return false;
        }

        // =========================
        // FINANCE MANAGER
        // =========================

        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogError(
                "Payment failed: FinanceTransactionManager not found."
            );

            return false;
        }

        int paymentAmount =
            BillingManager.Instance.CurrentTotalAmount;

        if (paymentAmount <= 0)
        {
            Debug.LogWarning(
                "Payment failed: Payment amount must be greater than zero."
            );

            return false;
        }

        // =========================
        // RECORD SHOP INCOME
        // =========================

        FinanceTransactionManager.Instance.RecordIncome(
            FinanceAccountType.Current,
            paymentAmount,
            "Shop Sale - " + paymentMethod
        );

        // =========================
        // MARK CUSTOMER AS SERVED
        // =========================

        customer.MarkServed();

        // =========================
        // UPDATE SATISFACTION
        // =========================

        if (CustomerSatisfactionManager.Instance != null)
        {
            CustomerSatisfactionManager.Instance.CustomerServed(
                customer
            );
        }

        // =========================
        // GENERATE TIP
        // =========================

        if (CustomerTipManager.Instance != null)
        {
            CustomerTipManager.Instance.AddTip(
                customer
            );
        }

        // =========================
        // COMPLETE SHOP PURCHASE TRACKING
        // =========================

        ShopManager.Instance.CompleteCurrentCustomerPurchase();

        // =========================
        // SAVE LAST PAYMENT
        // =========================

        LastPaymentMethod = paymentMethod;
        LastPaymentAmount = paymentAmount;

        // =========================
        // CLEAR BILL
        // =========================

        BillingManager.Instance.ClearBill();

        // =========================
        // REMOVE CUSTOMER
        // =========================

        CustomerManager.Instance.RemoveCustomer(
            customerId
        );

        Debug.Log(
            $"PAYMENT COMPLETED | " +
            $"Customer: {customer.CustomerName} | " +
            $"Payment Method: {paymentMethod} | " +
            $"Amount: Rs. {paymentAmount:N0} | " +
            $"Account: Current"
        );

        OnPaymentCompleted?.Invoke();

        return true;
    }

    [ContextMenu("TEST - Show Last Payment")]
    private void TestShowLastPayment()
    {
        Debug.Log(
            $"LAST PAYMENT | " +
            $"Method: {LastPaymentMethod} | " +
            $"Amount: Rs. {LastPaymentAmount:N0}"
        );
    }

    [ContextMenu("TEST - Reset Last Payment")]
    private void TestResetLastPayment()
    {
        LastPaymentMethod = PaymentMethod.None;
        LastPaymentAmount = 0;

        Debug.Log(
            "PaymentManager: Last payment reset."
        );
    }
}