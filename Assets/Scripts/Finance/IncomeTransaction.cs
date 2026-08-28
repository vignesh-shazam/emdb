using System;

[Serializable]
public class IncomeTransaction
{
    public string Source;
    public int Amount;
    public int Day;
    public int Hour;
    public int Minute;

    public IncomeTransaction(
        string source,
        int amount,
        int day,
        int hour,
        int minute)
    {
        Source = source;
        Amount = amount;
        Day = day;
        Hour = hour;
        Minute = minute;
    }
}