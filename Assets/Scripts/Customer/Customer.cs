using System;

[Serializable]
public class Customer
{
    public string CustomerId;
    public string CustomerName;

    public string RequestedItemId;
    public string RequestedItemName;

    public int RequestedQuantity;

    public float Patience;

    public Customer(
        string customerId,
        string customerName,
        string requestedItemId,
        string requestedItemName,
        int requestedQuantity = 1,
        float patience = 100f)
    {
        CustomerId = customerId;
        CustomerName = customerName;

        RequestedItemId = requestedItemId;
        RequestedItemName = requestedItemName;

        RequestedQuantity =
            Math.Max(1, requestedQuantity);

        Patience =
            Math.Max(0f, patience);
    }
}