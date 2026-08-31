using System;

[Serializable]
public class BankAccount
{
    // =========================================================
    // ACCOUNT DETAILS
    // =========================================================

    public string AccountNumber;

    public string AccountName;

    public FinanceAccountType AccountType;

    public int Balance;

    // =========================================================
    // ACCOUNT TYPE HELPERS
    // =========================================================

    public bool IsSavingsAccount =>
        AccountType == FinanceAccountType.Savings;

    public bool IsCurrentAccount =>
        AccountType == FinanceAccountType.Current;

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public BankAccount(
        string accountNumber,
        string accountName,
        FinanceAccountType accountType,
        int balance)
    {
        AccountNumber =
            string.IsNullOrWhiteSpace(accountNumber)
                ? "UNKNOWN"
                : accountNumber;

        AccountName =
            string.IsNullOrWhiteSpace(accountName)
                ? "Unknown Account"
                : accountName;

        AccountType =
            accountType;

        Balance =
            Math.Max(0, balance);
    }
}