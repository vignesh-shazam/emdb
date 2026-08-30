using System.Collections.Generic;
using UnityEngine;

public class FinanceTransactionManager : MonoBehaviour
{
    public static FinanceTransactionManager Instance { get; private set; }

    private readonly List<FinanceTransaction> transactions =
        new List<FinanceTransaction>();

    public IReadOnlyList<FinanceTransaction> Transactions =>
        transactions;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================
    // RECORD INCOME
    // =========================

    public void RecordIncome(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "FinanceTransactionManager: " +
                "Income amount must be greater than zero."
            );

            return;
        }

        AddTransaction(
            accountType,
            true,
            amount,
            description
        );
    }

    // =========================
    // RECORD EXPENSE
    // =========================

    public void RecordExpense(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "FinanceTransactionManager: " +
                "Expense amount must be greater than zero."
            );

            return;
        }

        AddTransaction(
            accountType,
            false,
            amount,
            description
        );
    }

    // =========================
    // ADD TRANSACTION
    // =========================

    private void AddTransaction(
        FinanceAccountType accountType,
        bool isIncome,
        int amount,
        string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            description = "Transaction";
        }

        int day = 0;
        int hour = 0;
        int minute = 0;

        if (GameTimeManager.Instance != null)
        {
            day =
                GameTimeManager.Instance.CurrentDay;

            hour =
                GameTimeManager.Instance.CurrentHour;

            minute =
                GameTimeManager.Instance.CurrentMinute;
        }

        FinanceTransaction transaction =
            new FinanceTransaction(
                accountType,
                isIncome,
                amount,
                description,
                day,
                hour,
                minute
            );

        transactions.Add(transaction);

        Debug.Log(
            $"Finance transaction recorded | " +
            $"Account: {accountType} | " +
            $"Type: {(isIncome ? "Income" : "Expense")} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Description: {description} | " +
            $"Day: {day} | " +
            $"Time: {hour:D2}:{minute:D2}"
        );
    }

    // =========================
    // TOTAL INCOME
    // =========================

    public int GetTotalIncome(
        FinanceAccountType accountType)
    {
        int total = 0;

        foreach (
            FinanceTransaction transaction
            in transactions)
        {
            if (transaction.AccountType !=
                accountType)
            {
                continue;
            }

            if (!transaction.IsIncome)
            {
                continue;
            }

            total += transaction.Amount;
        }

        return total;
    }

    // =========================
    // TOTAL EXPENSE
    // =========================

    public int GetTotalExpense(
        FinanceAccountType accountType)
    {
        int total = 0;

        foreach (
            FinanceTransaction transaction
            in transactions)
        {
            if (transaction.AccountType !=
                accountType)
            {
                continue;
            }

            if (transaction.IsIncome)
            {
                continue;
            }

            total += transaction.Amount;
        }

        return total;
    }

    // =========================
    // NET BALANCE
    // =========================

    public int GetNetBalance(
        FinanceAccountType accountType)
    {
        return
            GetTotalIncome(accountType) -
            GetTotalExpense(accountType);
    }

    // =========================
    // TODAY INCOME
    // =========================

    public int GetIncomeForDay(
        FinanceAccountType accountType,
        int day)
    {
        int total = 0;

        foreach (
            FinanceTransaction transaction
            in transactions)
        {
            if (transaction.AccountType !=
                accountType)
            {
                continue;
            }

            if (!transaction.IsIncome)
            {
                continue;
            }

            if (transaction.Day != day)
            {
                continue;
            }

            total += transaction.Amount;
        }

        return total;
    }

    // =========================
    // TODAY EXPENSE
    // =========================

    public int GetExpenseForDay(
        FinanceAccountType accountType,
        int day)
    {
        int total = 0;

        foreach (
            FinanceTransaction transaction
            in transactions)
        {
            if (transaction.AccountType !=
                accountType)
            {
                continue;
            }

            if (transaction.IsIncome)
            {
                continue;
            }

            if (transaction.Day != day)
            {
                continue;
            }

            total += transaction.Amount;
        }

        return total;
    }

    // =========================
    // TODAY NET
    // =========================

    public int GetNetForDay(
        FinanceAccountType accountType,
        int day)
    {
        return
            GetIncomeForDay(
                accountType,
                day
            ) -
            GetExpenseForDay(
                accountType,
                day
            );
    }

    // =========================
    // TRANSACTION COUNT
    // =========================

    public int GetTransactionCount(
        FinanceAccountType accountType)
    {
        int count = 0;

        foreach (
            FinanceTransaction transaction
            in transactions)
        {
            if (transaction.AccountType ==
                accountType)
            {
                count++;
            }
        }

        return count;
    }

    // =========================
    // CLEAR HISTORY
    // =========================

    public void ClearHistory()
    {
        transactions.Clear();

        Debug.Log(
            "Finance transaction history cleared."
        );
    }
}