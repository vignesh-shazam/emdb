using UnityEngine;

public class CustomerPatienceManager : MonoBehaviour
{
    [Header("Patience Settings")]
    [SerializeField]
    private float patienceDecreasePerSecond = 0.10f;

    [SerializeField]
    private bool decreaseOnlyWhenShopIsOpen = true;

    [SerializeField]
    private bool pauseDuringBilling = true;

    [SerializeField]
    private bool pauseDuringPayment = true;

    [Header("Debug Settings")]
    [SerializeField]
    private bool enablePatienceLogs = false;

    [SerializeField]
    private float debugLogInterval = 5f;

    private float debugTimer;

    private bool customerExpiryInProgress;

    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    private void Awake()
    {
        if (patienceDecreasePerSecond < 0f)
        {
            patienceDecreasePerSecond = 0f;
        }

        if (debugLogInterval <= 0f)
        {
            debugLogInterval = 5f;
        }

        Debug.Log(
            $"CustomerPatienceManager initialized | " +
            $"Decrease Per Second: {patienceDecreasePerSecond:0.00} | " +
            $"Shop Open Only: {decreaseOnlyWhenShopIsOpen} | " +
            $"Pause During Billing: {pauseDuringBilling} | " +
            $"Pause During Payment: {pauseDuringPayment}"
        );
    }

    private void Update()
    {
        if (customerExpiryInProgress)
        {
            return;
        }

        if (CustomerManager.Instance == null)
        {
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

        if (ShouldPausePatience())
        {
            return;
        }

        ReduceCustomerPatience(customer);

        CheckCustomerExpiry(customer);
    }

    // =========================================================
    // CHECK WHETHER PATIENCE SHOULD PAUSE
    // =========================================================

    private bool ShouldPausePatience()
    {
        // -----------------------------------------------------
        // CHECK SHOP STATUS
        // -----------------------------------------------------

        if (decreaseOnlyWhenShopIsOpen)
        {
            if (ShopManager.Instance == null)
            {
                return true;
            }

            if (!ShopManager.Instance.IsShopOpen)
            {
                return true;
            }
        }

        // -----------------------------------------------------
        // CHECK BILLING / PAYMENT
        // -----------------------------------------------------

        if (BillingManager.Instance != null &&
            BillingManager.Instance.HasBill)
        {
            if (pauseDuringBilling || pauseDuringPayment)
            {
                return true;
            }
        }

        return false;
    }

    // =========================================================
    // REDUCE PATIENCE
    // =========================================================

    private void ReduceCustomerPatience(
        Customer customer)
    {
        if (customer == null)
        {
            return;
        }

        if (patienceDecreasePerSecond <= 0f)
        {
            return;
        }

        float decreaseAmount =
            patienceDecreasePerSecond *
            Time.deltaTime;

        customer.ReducePatience(
            decreaseAmount
        );

        // -----------------------------------------------------
        // DEBUG LOG
        // -----------------------------------------------------

        if (!enablePatienceLogs)
        {
            return;
        }

        debugTimer += Time.deltaTime;

        if (debugTimer >= debugLogInterval)
        {
            debugTimer = 0f;

            Debug.Log(
                $"Customer patience updated | " +
                $"Name: {customer.CustomerName} | " +
                $"ID: {customer.CustomerId} | " +
                $"Patience: {customer.Patience:0.00}"
            );
        }
    }

    // =========================================================
    // CHECK CUSTOMER EXPIRY
    // =========================================================

    private void CheckCustomerExpiry(
        Customer customer)
    {
        if (customer == null)
        {
            return;
        }

        if (customer.Patience > 0f)
        {
            return;
        }

        customerExpiryInProgress = true;

        CustomerExpired(customer);

        customerExpiryInProgress = false;
    }

    // =========================================================
    // CUSTOMER EXPIRED
    // =========================================================

    private void CustomerExpired(
        Customer customer)
    {
        if (customer == null)
        {
            return;
        }

        string customerId =
            customer.CustomerId;

        string customerName =
            customer.CustomerName;

        // =====================================================
        // MARK CUSTOMER AS LEFT
        // =====================================================

        customer.MarkLeft();

        // =====================================================
        // UPDATE SATISFACTION
        // =====================================================

        if (CustomerSatisfactionManager.Instance != null)
        {
            CustomerSatisfactionManager.Instance.CustomerLeft(
                customer
            );
        }

        // =====================================================
        // LOG RESULT
        // =====================================================

        Debug.Log(
            $"Customer left due to patience reaching zero | " +
            $"Name: {customerName} | " +
            $"ID: {customerId} | " +
            $"Result: {customer.Result}"
        );

        // =====================================================
        // CANCEL CURRENT CUSTOMER PURCHASES
        // =====================================================

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.CancelCurrentCustomerPurchases();
        }

        // =====================================================
        // REMOVE CUSTOMER
        // =====================================================

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.RemoveCustomer(
                customerId
            );
        }

        // =====================================================
        // RESET DEBUG TIMER
        // =====================================================

        debugTimer = 0f;
    }

    // =========================================================
    // PUBLIC SETTINGS
    // =========================================================

    public float GetPatienceDecreasePerSecond()
    {
        return patienceDecreasePerSecond;
    }

    public void SetPatienceDecreasePerSecond(
        float value)
    {
        patienceDecreasePerSecond =
            Mathf.Max(0f, value);

        Debug.Log(
            $"Customer patience decrease updated | " +
            $"Value: {patienceDecreasePerSecond:0.00} per second"
        );
    }

    public void EnablePatience()
    {
        enabled = true;

        Debug.Log(
            "Customer patience manager enabled."
        );
    }

    public void DisablePatience()
    {
        enabled = false;

        Debug.Log(
            "Customer patience manager disabled."
        );
    }

    // =========================================================
    // DEVELOPMENT TESTS
    // =========================================================

    [ContextMenu("TEST - Enable Patience Manager")]
    private void TestEnablePatienceManager()
    {
        EnablePatience();
    }

    [ContextMenu("TEST - Disable Patience Manager")]
    private void TestDisablePatienceManager()
    {
        DisablePatience();
    }

    [ContextMenu("TEST - Set Slow Patience")]
    private void TestSetSlowPatience()
    {
        SetPatienceDecreasePerSecond(0.10f);
    }

    [ContextMenu("TEST - Set Very Slow Patience")]
    private void TestSetVerySlowPatience()
    {
        SetPatienceDecreasePerSecond(0.05f);
    }

    [ContextMenu("TEST - Enable Patience Logs")]
    private void TestEnablePatienceLogs()
    {
        enablePatienceLogs = true;

        Debug.Log(
            "Customer patience debug logs enabled."
        );
    }

    [ContextMenu("TEST - Disable Patience Logs")]
    private void TestDisablePatienceLogs()
    {
        enablePatienceLogs = false;

        Debug.Log(
            "Customer patience debug logs disabled."
        );
    }
}