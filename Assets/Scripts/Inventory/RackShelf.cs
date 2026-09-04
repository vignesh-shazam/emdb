using System;

[Serializable]
public class RackShelf
{
    // =========================
    // SHELF INFORMATION
    // =========================

    public int ShelfNumber;

    // =========================
    // ITEM INFORMATION
    // =========================

    public string ItemId;
    public string ItemName;

    // =========================
    // CAPACITY
    // =========================

    public int Capacity;

    // =========================
    // QUANTITY
    // =========================

    public int Quantity;

    // =========================
    // CONSTRUCTOR
    // =========================

    public RackShelf(
        int shelfNumber,
        int capacity = 10)
    {
        ShelfNumber =
            Math.Max(
                1,
                shelfNumber
            );

        Capacity =
            Math.Max(
                1,
                capacity
            );

        Quantity = 0;

        ItemId = string.Empty;
        ItemName = string.Empty;
    }

    // =========================
    // AVAILABLE SPACE
    // =========================

    public int AvailableSpace
    {
        get
        {
            return Math.Max(
                0,
                Capacity - Quantity
            );
        }
    }

    // =========================
    // EMPTY
    // =========================

    public bool IsEmpty
    {
        get
        {
            return Quantity <= 0;
        }
    }

    // =========================
    // FULL
    // =========================

    public bool IsFull
    {
        get
        {
            return Quantity >= Capacity;
        }
    }

    // =========================
    // CLEAR
    // =========================

    public void Clear()
    {
        ItemId = string.Empty;
        ItemName = string.Empty;
        Quantity = 0;
    }

    // =========================
    // DISPLAY
    // =========================

    public string GetDisplay()
    {
        string displayName =
            string.IsNullOrWhiteSpace(ItemName)
                ? "Empty"
                : ItemName;

        return
            $"Shelf {ShelfNumber} | " +
            $"{displayName} | " +
            $"{Quantity}/{Capacity}";
    }
}