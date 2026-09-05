using System;

[Serializable]
public class StoreRoomStock
{
    // =========================
    // ITEM INFORMATION
    // =========================

    public string ItemId;
    public string ItemName;

    // =========================
    // PACKAGING
    // =========================

    // Number of individual units
    // contained in one box.
    public int UnitsPerBox;

    // =========================
    // STORE ROOM QUANTITY
    // =========================

    // Total individual units currently
    // available in the Store Room.
    public int Quantity;

    // =========================
    // CONSTRUCTOR
    // =========================

    public StoreRoomStock(
        string itemId,
        string itemName,
        int unitsPerBox,
        int quantity = 0)
    {
        ItemId =
            string.IsNullOrWhiteSpace(itemId)
                ? "UNKNOWN"
                : itemId;

        ItemName =
            string.IsNullOrWhiteSpace(itemName)
                ? "Unknown Item"
                : itemName;

        UnitsPerBox =
            Math.Max(
                1,
                unitsPerBox
            );

        Quantity =
            Math.Max(
                0,
                quantity
            );
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
    // LOOSE UNIT COUNT
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
    // STOCK DISPLAY
    // =========================

    public string GetStockDisplay()
    {
        int boxes =
            BoxCount;

        int looseUnits =
            LooseUnitCount;

        if (boxes > 0 &&
            looseUnits > 0)
        {
            return
                $"{boxes} Box + {looseUnits} Nos";
        }

        if (boxes > 0)
        {
            return
                $"{boxes} Box";
        }

        return
            $"{looseUnits} Nos";
    }
}