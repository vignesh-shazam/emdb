using System.Collections.Generic;
using UnityEngine;

public class ExpenseManager : MonoBehaviour
{
    public static ExpenseManager Instance { get; private set; }

    private readonly List<ExpenseTransaction> transactions =
        new List<ExpenseTransaction>();

    public IReadOnlyList<ExpenseTransaction> Transactions =>
        transactions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

    public bool Spend(int amount)
    {
        return Spend(amount, ExpenseCategory.Other);
    }

    public bool Spend(
        int amount,
        ExpenseCategory category)
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
                $"Expense rejected: {category}. " +
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

        RecordTransaction(
            amount,
            category
        );

        Debug.Log(
            $"Expense completed | " +
            $"Category: {category} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Remaining: Rs. " +
            $"{MoneyManager.Instance.CurrentMoney:N0}"
        );

        return true;
    }

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
}