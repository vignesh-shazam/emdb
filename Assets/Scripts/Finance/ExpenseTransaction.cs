using System;

[Serializable]
public class ExpenseTransaction
{
    public ExpenseCategory Category;
    public int Amount;
    public int Day;
    public int Hour;
    public int Minute;

    public ExpenseTransaction(
        ExpenseCategory category,
        int amount,
        int day,
        int hour,
        int minute)
    {
        Category = category;
        Amount = amount;
        Day = day;
        Hour = hour;
        Minute = minute;
    }
}