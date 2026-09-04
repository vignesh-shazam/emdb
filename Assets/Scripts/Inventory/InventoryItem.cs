using System;

[Serializable]
public class InventoryItem
{
    // =========================
    // ITEM INFORMATION
    // =========================

    public string ItemId;
    public string ItemName;

    // =========================
    // PACKAGING
    // =========================

    public int UnitsPerBox;

    // =========================
    // PRICING
    // =========================

    public int PurchasePricePerBox;
    public int SellingPricePerUnit;

    // =========================
    // STOCK
    // =========================

    public int Quantity;

    // =========================
    // STOCK RULES
    // =========================

    public int RackShelfCapacity;
    public int RackLowStockThreshold;
    public int StoreRoomReorderThreshold;

    // =========================
    // CONSTRUCTOR
    // =========================

    public InventoryItem(
        string itemId,
        string itemName,
        int unitsPerBox,
        int purchasePricePerBox,
        int sellingPricePerUnit,
        int quantity = 0,
        int rackShelfCapacity = 10,
        int rackLowStockThreshold = 5,
        int storeRoomReorderThreshold = 15)
    {
        ItemId = string.IsNullOrWhiteSpace(itemId)
            ? "UNKNOWN"
            : itemId;

        ItemName = string.IsNullOrWhiteSpace(itemName)
            ? "Unknown Item"
            : itemName;

        UnitsPerBox = Math.Max(
            1,
            unitsPerBox
        );

        PurchasePricePerBox = Math.Max(
            0,
            purchasePricePerBox
        );

        SellingPricePerUnit = Math.Max(
            0,
            sellingPricePerUnit
        );

        Quantity = Math.Max(
            0,
            quantity
        );

        RackShelfCapacity = Math.Max(
            1,
            rackShelfCapacity
        );

        RackLowStockThreshold = Math.Max(
            0,
            rackLowStockThreshold
        );

        StoreRoomReorderThreshold = Math.Max(
            0,
            storeRoomReorderThreshold
        );
    }

    // =========================
    // PURCHASE PRICE / UNIT
    // =========================

    public int PurchasePricePerUnit
    {
        get
        {
            if (UnitsPerBox <= 0)
            {
                return 0;
            }

            return PurchasePricePerBox /
                   UnitsPerBox;
        }
    }

    // =========================
    // BOX COUNT
    // =========================

    public int BoxCount
    {
        get
        {
            if (UnitsPerBox <= 0)
            {
                return 0;
            }

            return Quantity /
                   UnitsPerBox;
        }
    }

    // =========================
    // LOOSE UNITS
    // =========================

    public int LooseUnitCount
    {
        get
        {
            if (UnitsPerBox <= 0)
            {
                return Quantity;
            }

            return Quantity %
                   UnitsPerBox;
        }
    }

    // =========================
    // STOCK VALUE
    // =========================

    public int StockValue
    {
        get
        {
            return
                PurchasePricePerUnit *
                Quantity;
        }
    }

    // =========================
    // STOCK DISPLAY
    // =========================

    public string GetStockDisplay()
    {
        if (UnitsPerBox <= 0)
        {
            return $"{Quantity} Nos";
        }

        int boxes =
            BoxCount;

        int loose =
            LooseUnitCount;

        if (boxes > 0 && loose > 0)
        {
            return
                $"{boxes} Box + {loose} Nos";
        }

        if (boxes > 0)
        {
            return
                $"{boxes} Box";
        }

        return
            $"{loose} Nos";
    }
}