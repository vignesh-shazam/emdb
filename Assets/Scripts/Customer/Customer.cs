using System;

[Serializable]
public class Customer
{
    public string CustomerId { get; private set; }

    public string CustomerName { get; private set; }

    public CustomerRequest Request { get; private set; }

    public float Patience { get; private set; }

    // =========================
    // REQUEST COMPATIBILITY
    // =========================

    public string RequestedItemId =>
        Request != null
            ? Request.ItemId
            : string.Empty;

    public string RequestedItemName =>
        Request != null
            ? Request.ItemName
            : string.Empty;

    public int RequestedQuantity =>
        Request != null
            ? Request.Quantity
            : 0;

    // =========================
    // CONSTRUCTOR
    // =========================

    public Customer(
        string customerId,
        string customerName,
        string itemId,
        string itemName,
        int quantity,
        float patience)
    {
        CustomerId = customerId;

        CustomerName = customerName;

        Request =
            new CustomerRequest(
                itemId,
                itemName,
                quantity
            );

        Patience = patience;
    }

    // =========================
    // REQUEST VALIDATION
    // =========================

    public bool HasValidRequest()
    {
        return Request != null &&
               Request.IsValid();
    }

    // =========================
    // PATIENCE
    // =========================

    public void ReducePatience(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        Patience -= amount;

        if (Patience < 0f)
        {
            Patience = 0f;
        }
    }

    public bool IsPatient()
    {
        return Patience > 0f;
    }
}