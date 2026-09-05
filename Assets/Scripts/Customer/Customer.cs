using System;

[Serializable]
public class Customer
{
    public enum CustomerResult
    {
        None,
        Served,
        Left
    }

    public string CustomerId { get; private set; }

    public string CustomerName { get; private set; }

    public CustomerRequest Request { get; private set; }

    public float Patience { get; private set; }

    public CustomerResult Result { get; private set; }

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
    // COLLECTION
    // =========================

    public int CollectedQuantity { get; private set; }

    public bool HasCollectedItems =>
        CollectedQuantity > 0;

    public bool HasCollectedAllItems =>
        Request != null &&
        CollectedQuantity >=
        Request.Quantity;

    public int RemainingQuantity =>
        Request != null
            ? Math.Max(
                0,
                Request.Quantity -
                CollectedQuantity
            )
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

        Result =
            CustomerResult.None;

        CollectedQuantity = 0;
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
    // COLLECTION
    // =========================

    public bool CollectItems(
        int quantity)
    {
        if (!HasValidRequest())
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (quantity >
            RemainingQuantity)
        {
            return false;
        }

        CollectedQuantity +=
            quantity;

        return true;
    }

    // =========================
    // PATIENCE
    // =========================

    public void ReducePatience(
        float amount)
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

    // =========================
    // RESULT
    // =========================

    public void MarkServed()
    {
        Result =
            CustomerResult.Served;
    }

    public void MarkLeft()
    {
        Result =
            CustomerResult.Left;
    }

    public bool WasServed()
    {
        return Result ==
               CustomerResult.Served;
    }

    public bool LeftWithoutService()
    {
        return Result ==
               CustomerResult.Left;
    }
}