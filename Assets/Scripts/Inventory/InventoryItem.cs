using System;

[Serializable]
public class InventoryItem
{
    public string ItemId;
    public string ItemName;
    public int Quantity;

    public InventoryItem(
        string itemId,
        string itemName,
        int quantity = 0)
    {
        ItemId = itemId;
        ItemName = itemName;
        Quantity = Math.Max(0, quantity);
    }
}