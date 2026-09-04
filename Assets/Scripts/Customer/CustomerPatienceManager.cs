using UnityEngine;

public class CustomerPatienceManager : MonoBehaviour
{
    [Header("Patience")]
    [SerializeField]
    private float patienceDecreasePerSecond = 1f;

    private float debugTimer;

    private bool customerExpiryInProgress;

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (CustomerManager.Instance == null)
        {
            return;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
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

        // =========================
        // REDUCE PATIENCE
        // =========================

        float decreaseAmount =
            patienceDecreasePerSecond *
            Time.deltaTime;

        customer.ReducePatience(
            decreaseAmount
        );

        // =========================
        // DEBUG PATIENCE
        // =========================

        debugTimer += Time.deltaTime;

        if (debugTimer >= 1f)
        {
            debugTimer = 0f;

            //Debug.Log(
            //    $"Customer patience updated | " +
            //    $"Customer: {customer.CustomerName} | " +
            //    $"ID: {customer.CustomerId} | " +
            //    $"Patience: {customer.Patience:0.00}"
            //);
        }

        // =========================
        // CUSTOMER EXPIRED
        // =========================

        if (customer.Patience <= 0f)
        {
            if (customerExpiryInProgress)
            {
                return;
            }

            customerExpiryInProgress = true;

            CustomerExpired(
                customer
            );

            customerExpiryInProgress = false;
        }
    }

    // =========================
    // CUSTOMER EXPIRED
    // =========================

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

        // =========================
        // MARK AS LEFT
        // =========================

        customer.MarkLeft();

        // =========================
        // UPDATE SATISFACTION
        // =========================

        if (CustomerSatisfactionManager.Instance != null)
        {
            CustomerSatisfactionManager.Instance
                .CustomerLeft(customer);
        }

        // =========================
        // RESULT LOG
        // =========================

        Debug.Log(
            $"Customer left due to patience reaching zero | " +
            $"Name: {customerName} | " +
            $"ID: {customerId} | " +
            $"Result: {customer.Result}"
        );

        // =========================
        // CANCEL PURCHASES
        // =========================

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance
                .CancelCurrentCustomerPurchases();
        }

        // =========================
        // REMOVE CUSTOMER
        // =========================

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.RemoveCustomer(
                customerId
            );
        }
    }
}