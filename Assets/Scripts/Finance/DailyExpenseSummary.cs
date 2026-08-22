using System.Collections.Generic;
using UnityEngine;

public class DailyExpenseSummary : MonoBehaviour
{
    public static DailyExpenseSummary Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int GetTotalForDay(int day)
    {
        if (ExpenseManager.Instance == null)
        {
            Debug.LogError(
                "DailyExpenseSummary: ExpenseManager not found."
            );

            return 0;
        }

        int total = 0;

        foreach (ExpenseTransaction transaction
                 in ExpenseManager.Instance.Transactions)
        {
            if (transaction.Day == day)
            {
                total += transaction.Amount;
            }
        }

        return total;
    }

    public int GetTotalForCategory(
        int day,
        ExpenseCategory category)
    {
        if (ExpenseManager.Instance == null)
        {
            Debug.LogError(
                "DailyExpenseSummary: ExpenseManager not found."
            );

            return 0;
        }

        int total = 0;

        foreach (ExpenseTransaction transaction
                 in ExpenseManager.Instance.Transactions)
        {
            if (transaction.Day == day &&
                transaction.Category == category)
            {
                total += transaction.Amount;
            }
        }

        return total;
    }

    public Dictionary<ExpenseCategory, int>
        GetCategoryTotals(int day)
    {
        Dictionary<ExpenseCategory, int> totals =
            new Dictionary<ExpenseCategory, int>();

        if (ExpenseManager.Instance == null)
        {
            Debug.LogError(
                "DailyExpenseSummary: ExpenseManager not found."
            );

            return totals;
        }

        foreach (ExpenseTransaction transaction
                 in ExpenseManager.Instance.Transactions)
        {
            if (transaction.Day != day)
            {
                continue;
            }

            if (!totals.ContainsKey(transaction.Category))
            {
                totals[transaction.Category] = 0;
            }

            totals[transaction.Category] +=
                transaction.Amount;
        }

        return totals;
    }
}