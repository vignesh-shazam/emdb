using System.Collections.Generic;
using UnityEngine;

public class ExpenseManager : MonoBehaviour
{
    public static ExpenseManager Instance { get; private set; }

    private readonly List<ExpenseTransaction> transactions =
        new List<ExpenseTransaction>();

    public IReadOnlyList<ExpenseTransaction> Transactions =>
        transactions;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================
    // CAN AFFORD
    // =========================

    public bool CanAfford(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "ExpenseManager: MoneyManager not found."
            );

            return false;
        }

        return MoneyManager.Instance.CanAfford(amount);
    }

    // =========================
    // SPEND
    // =========================

    public bool Spend(int amount)
    {
        return Spend(
            amount,
            ExpenseCategory.Other,
            FinanceAccountType.Employee
        );
    }

    // =========================
    // SPEND - CATEGORY
    // =========================

    public bool Spend(
        int amount,
        ExpenseCategory category)
    {
        return Spend(
            amount,
            category,
            FinanceAccountType.Employee
        );
    }

    // =========================
    // SPEND - ACCOUNT
    // =========================

    public bool Spend(
        int amount,
        ExpenseCategory category,
        FinanceAccountType accountType)
    {
        return Spend(
            amount,
            category,
            accountType,
            category.ToString()
        );
    }

    // =========================
    // SPEND - CUSTOM DESCRIPTION
    // =========================

    public bool Spend(
        int amount,
        ExpenseCategory category,
        FinanceAccountType accountType,
        string description)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "ExpenseManager: Expense amount must be greater than zero."
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "ExpenseManager: MoneyManager not found."
            );

            return false;
        }

        if (!MoneyManager.Instance.CanAfford(amount))
        {
            Debug.Log(
                $"Expense rejected: {description}. " +
                $"Insufficient funds for Rs. {amount:N0}."
            );

            return false;
        }

        bool success =
            MoneyManager.Instance.RemoveMoney(amount);

        if (!success)
        {
            return false;
        }

        // =========================
        // EXISTING EXPENSE RECORD
        // =========================

        RecordTransaction(
            amount,
            category
        );

        // =========================
        // FINANCE LEDGER RECORD
        // =========================

        RecordFinanceTransaction(
            amount,
            accountType,
            description
        );

        Debug.Log(
            $"Expense completed | " +
            $"Account: {accountType} | " +
            $"Description: {description} | " +
            $"Category: {category} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Remaining: Rs. " +
            $"{MoneyManager.Instance.CurrentMoney:N0}"
        );

        return true;
    }

    // =========================
    // RECORD EXISTING EXPENSE
    // =========================

    private void RecordTransaction(
        int amount,
        ExpenseCategory category)
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogWarning(
                "ExpenseManager: GameTimeManager not found. " +
                "Transaction will use default time values."
            );

            transactions.Add(
                new ExpenseTransaction(
                    category,
                    amount,
                    0,
                    0,
                    0
                )
            );

            return;
        }

        ExpenseTransaction transaction =
            new ExpenseTransaction(
                category,
                amount,
                GameTimeManager.Instance.CurrentDay,
                GameTimeManager.Instance.CurrentHour,
                GameTimeManager.Instance.CurrentMinute
            );

        transactions.Add(transaction);
    }

    // =========================
    // RECORD FINANCE LEDGER
    // =========================

    private void RecordFinanceTransaction(
        int amount,
        FinanceAccountType accountType,
        string description)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "ExpenseManager: " +
                "FinanceTransactionManager not found. " +
                "Finance ledger entry skipped."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            description = "Expense";
        }

        FinanceTransactionManager.Instance.RecordExpense(
            accountType,
            amount,
            description
        );
    }
}