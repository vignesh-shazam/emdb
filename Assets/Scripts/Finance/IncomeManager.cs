using System.Collections.Generic;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    public static IncomeManager Instance { get; private set; }

    private readonly List<IncomeTransaction> transactions =
        new List<IncomeTransaction>();

    public IReadOnlyList<IncomeTransaction> Transactions =>
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
    // ADD INCOME - DEFAULT
    // =========================================================

    public bool AddIncome(
        int amount,
        string source)
    {
        return AddIncome(
            amount,
            source,
            FinanceAccountType.Savings
        );
    }

    // =========================================================
    // ADD INCOME - ACCOUNT
    // =========================================================

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

        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "IncomeManager: BankManager not found."
            );

            return false;
        }

        // =====================================================
        // BANK CREDIT
        // =====================================================

        bool success =
            BankManager.Instance.Credit(
                accountType,
                amount,
                source
            );

        if (!success)
        {
            Debug.LogWarning(
                $"Income rejected | " +
                $"Account: {accountType} | " +
                $"Source: {source} | " +
                $"Amount: Rs. {amount:N0}"
            );

            return false;
        }

        // =====================================================
        // EXISTING INCOME RECORD
        // =====================================================

        RecordTransaction(
            amount,
            source
        );

        Debug.Log(
            $"Income completed | " +
            $"Account: {accountType} | " +
            $"Source: {source} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. " +
            $"{BankManager.Instance.GetBalance(accountType):N0}"
        );

        return true;
    }

    // =========================================================
    // RECORD INCOME
    // =========================================================

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
}