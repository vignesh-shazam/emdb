using System;

[Serializable]
public class ShopItem
{
    public string ItemId;
    public string ItemName;
    public int BuyPrice;
    public int SellPrice;

    public ShopItem(
        string itemId,
        string itemName,
        int buyPrice,
        int sellPrice)
    {
        ItemId = itemId;
        ItemName = itemName;
        BuyPrice = Math.Max(0, buyPrice);
        SellPrice = Math.Max(0, sellPrice);
    }
}