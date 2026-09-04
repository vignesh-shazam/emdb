using System;

[Serializable]
public class PurchaseOrderItem
{
    // =========================
    // ITEM INFORMATION
    // =========================

    public string ItemId;
    public string ItemName;

    // =========================
    // ORDER QUANTITY
    // =========================

    // Number of boxes ordered.
    public int BoxQuantity;

    // =========================
    // UNITS PER BOX
    // =========================

    public int UnitsPerBox;

    // =========================
    // CONSTRUCTOR
    // =========================

    public PurchaseOrderItem(
        string itemId,
        string itemName,
        int boxQuantity,
        int unitsPerBox)
    {
        ItemId =
            string.IsNullOrWhiteSpace(itemId)
                ? string.Empty
                : itemId;

        ItemName =
            string.IsNullOrWhiteSpace(itemName)
                ? "Unknown Item"
                : itemName;

        BoxQuantity =
            Math.Max(
                0,
                boxQuantity
            );

        UnitsPerBox =
            Math.Max(
                1,
                unitsPerBox
            );
    }

    // =========================
    // TOTAL UNITS
    // =========================

    public int TotalUnits
    {
        get
        {
            return
                BoxQuantity *
                UnitsPerBox;
        }
    }

    // =========================
    // DISPLAY
    // =========================

    public string GetDisplay()
    {
        return
            $"{ItemName} × " +
            $"{BoxQuantity} Box" +
            (BoxQuantity == 1 ? "" : "es") +
            $" ({TotalUnits} Nos)";
    }
}