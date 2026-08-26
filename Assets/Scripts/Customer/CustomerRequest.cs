using System;

[Serializable]
public class CustomerRequest
{
    public string ItemId { get; private set; }

    public string ItemName { get; private set; }

    public int Quantity { get; private set; }

    public CustomerRequest(
        string itemId,
        string itemName,
        int quantity)
    {
        ItemId = itemId;
        ItemName = itemName;
        Quantity = quantity;
    }

    public bool IsValid()
    {
        return
            !string.IsNullOrWhiteSpace(ItemId) &&
            !string.IsNullOrWhiteSpace(ItemName) &&
            Quantity > 0;
    }
}