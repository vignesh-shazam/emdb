using System;

[Serializable]
public class FinanceTransaction
{
    public FinanceAccountType AccountType;

    public bool IsIncome;

    public int Amount;

    public string Description;

    public int Day;

    public int Hour;

    public int Minute;

    public FinanceTransaction(
        FinanceAccountType accountType,
        bool isIncome,
        int amount,
        string description,
        int day,
        int hour,
        int minute)
    {
        AccountType = accountType;
        IsIncome = isIncome;
        Amount = amount;
        Description = description;
        Day = day;
        Hour = hour;
        Minute = minute;
    }
}