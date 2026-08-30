using System.Collections.Generic;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    public static IncomeManager Instance { get; private set; }

    private readonly List<IncomeTransaction> transactions =
        new List<IncomeTransaction>();

    public IReadOnlyList<IncomeTransaction> Transactions =>
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
    // ADD INCOME - DEFAULT
    // =========================

    public bool AddIncome(
        int amount,
        string source)
    {
        return AddIncome(
            amount,
            source,
            FinanceAccountType.Employee
        );
    }

    // =========================
    // ADD INCOME - ACCOUNT
    // =========================

    public bool AddIncome(
        int amount,
        string source,
        FinanceAccountType accountType)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "IncomeManager: Income amount must be greater than zero."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            Debug.LogWarning(
                "IncomeManager: Income source is required."
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "IncomeManager: MoneyManager not found."
            );

            return false;
        }

        // =========================
        // ADD MONEY
        // =========================

        MoneyManager.Instance.AddMoney(
            amount
        );

        // =========================
        // RECORD EXISTING INCOME
        // =========================

        RecordTransaction(
            amount,
            source
        );

        // =========================
        // RECORD FINANCE LEDGER
        // =========================

        RecordFinanceTransaction(
            amount,
            source,
            accountType
        );

        Debug.Log(
            $"Income completed | " +
            $"Account: {accountType} | " +
            $"Source: {source} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. " +
            $"{MoneyManager.Instance.CurrentMoney:N0}"
        );

        return true;
    }

    // =========================
    // RECORD INCOME
    // =========================

    private void RecordTransaction(
        int amount,
        string source)
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogWarning(
                "IncomeManager: GameTimeManager not found. " +
                "Transaction will use default time values."
            );

            transactions.Add(
                new IncomeTransaction(
                    source,
                    amount,
                    0,
                    0,
                    0
                )
            );

            return;
        }

        IncomeTransaction transaction =
            new IncomeTransaction(
                source,
                amount,
                GameTimeManager.Instance.CurrentDay,
                GameTimeManager.Instance.CurrentHour,
                GameTimeManager.Instance.CurrentMinute
            );

        transactions.Add(
            transaction
        );
    }

    // =========================
    // RECORD FINANCE LEDGER
    // =========================

    private void RecordFinanceTransaction(
        int amount,
        string source,
        FinanceAccountType accountType)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "IncomeManager: " +
                "FinanceTransactionManager not found. " +
                "Finance ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordIncome(
            accountType,
            amount,
            source
        );
    }
}