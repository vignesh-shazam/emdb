using System;

[Serializable]
public class BankAccount
{
    public string AccountNumber;
    public int Balance;

    public BankAccount(
        string accountNumber,
        int balance)
    {
        AccountNumber = accountNumber;
        Balance = Math.Max(0, balance);
    }
}