using System.Collections.Generic;
using UnityEngine;

public class ExpenseManager : MonoBehaviour
{
    public static ExpenseManager Instance { get; private set; }

    private readonly List<ExpenseTransaction> transactions =
        new List<ExpenseTransaction>();

    public IReadOnlyList<ExpenseTransaction> Transactions =>
        transactions;

    // =========================================================
    // AWAKE
    // =========================================================

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

    // =========================================================
    // CAN AFFORD
    // =========================================================

    public bool CanAfford(int amount)
    {
        return CanAfford(
            amount,
            FinanceAccountType.Savings
        );
    }

    // =========================================================
    // CAN AFFORD - ACCOUNT
    // =========================================================

    public bool CanAfford(
        int amount,
        FinanceAccountType accountType)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "ExpenseManager: BankManager not found."
            );

            return false;
        }

        return
            BankManager.Instance.GetBalance(
                accountType
            ) >= amount;
    }

    // =========================================================
    // SPEND
    // =========================================================

    public bool Spend(int amount)
    {
        return Spend(
            amount,
            ExpenseCategory.Other,
            FinanceAccountType.Savings
        );
    }

    // =========================================================
    // SPEND - CATEGORY
    // =========================================================

    public bool Spend(
        int amount,
        ExpenseCategory category)
    {
        return Spend(
            amount,
            category,
            FinanceAccountType.Savings
        );
    }

    // =========================================================
    // SPEND - ACCOUNT
    // =========================================================

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

    // =========================================================
    // SPEND - CUSTOM DESCRIPTION
    // =========================================================

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

        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "ExpenseManager: BankManager not found."
            );

            return false;
        }

        int currentBalance =
            BankManager.Instance.GetBalance(
                accountType
            );

        if (currentBalance < amount)
        {
            Debug.Log(
                $"Expense rejected: {description}. " +
                $"Account: {accountType} | " +
                $"Required: Rs. {amount:N0} | " +
                $"Available: Rs. {currentBalance:N0}"
            );

            return false;
        }

        // =====================================================
        // BANK DEBIT
        // =====================================================

        bool success =
            BankManager.Instance.Debit(
                accountType,
                amount,
                description
            );

        if (!success)
        {
            return false;
        }

        // =====================================================
        // EXISTING EXPENSE RECORD
        // =====================================================

        RecordTransaction(
            amount,
            category
        );

        Debug.Log(
            $"Expense completed | " +
            $"Account: {accountType} | " +
            $"Description: {description} | " +
            $"Category: {category} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Remaining: Rs. " +
            $"{BankManager.Instance.GetBalance(accountType):N0}"
        );

        return true;
    }

    // =========================================================
    // RECORD EXISTING EXPENSE
    // =========================================================

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

        transactions.Add(
            transaction
        );
    }
}