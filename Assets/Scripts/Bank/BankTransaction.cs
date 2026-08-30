using System;

[Serializable]
public class BankTransaction
{
    public enum TransactionType
    {
        Deposit,
        Withdraw,
        EmiDebit,
        BounceCharge
    }

    public TransactionType Type;

    public int Amount;

    public int BalanceAfterTransaction;

    public BankTransaction(
        TransactionType type,
        int amount,
        int balanceAfterTransaction)
    {
        Type = type;

        Amount = amount;

        BalanceAfterTransaction =
            balanceAfterTransaction;
    }
}