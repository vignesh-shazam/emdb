using TMPro;
using UnityEngine;

public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance { get; private set; }

    [Header("Finance UI")]
    [SerializeField]
    private GameObject financePanel;

    [Header("Input")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.Q;

    [Header("Finance Summary")]
    [SerializeField]
    private TextMeshProUGUI incomeText;

    [SerializeField]
    private TextMeshProUGUI expenseText;

    [SerializeField]
    private TextMeshProUGUI netText;

    public bool IsOpen =>
        financePanel != null &&
        financePanel.activeSelf;

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

        InitializeUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        financePanel.SetActive(false);

        ClearSummaryUI();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFinanceUI();
        }

        if (IsOpen)
        {
            UpdateFinanceSummary();
        }
    }

    // =========================
    // TOGGLE
    // =========================

    public void ToggleFinanceUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        if (IsOpen)
        {
            CloseFinanceUI();
        }
        else
        {
            OpenFinanceUI();
        }
    }

    // =========================
    // OPEN
    // =========================

    public void OpenFinanceUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        financePanel.SetActive(true);

        UpdateFinanceSummary();

        Debug.Log(
            "Finance UI opened."
        );
    }

    // =========================
    // CLOSE
    // =========================

    public void CloseFinanceUI()
    {
        if (financePanel == null)
        {
            return;
        }

        financePanel.SetActive(false);

        Debug.Log(
            "Finance UI closed."
        );
    }

    // =========================
    // SET VISIBILITY
    // =========================

    public void SetFinanceUIVisible(bool visible)
    {
        if (financePanel == null)
        {
            return;
        }

        financePanel.SetActive(visible);

        if (visible)
        {
            UpdateFinanceSummary();
        }
    }

    // =========================================================
    // FINANCE SUMMARY
    // =========================================================

    private void UpdateFinanceSummary()
    {
        if (GameTimeManager.Instance == null)
        {
            return;
        }

        int currentDay =
            GameTimeManager.Instance.CurrentDay;

        int totalIncome =
            GetTotalIncome(currentDay);

        int totalExpense =
            GetTotalExpense(currentDay);

        int netAmount =
            totalIncome - totalExpense;

        // =========================
        // INCOME
        // =========================

        if (incomeText != null)
        {
            incomeText.text =
                $"Income: Rs. {totalIncome:N0}";
        }

        // =========================
        // EXPENSE
        // =========================

        if (expenseText != null)
        {
            expenseText.text =
                $"Expenses: Rs. {totalExpense:N0}";
        }

        // =========================
        // NET
        // =========================

        if (netText != null)
        {
            netText.text =
                $"Net: Rs. {netAmount:N0}";
        }
    }

    // =========================================================
    // INCOME
    // =========================================================

    private int GetTotalIncome(int day)
    {
        if (IncomeManager.Instance == null)
        {
            return 0;
        }

        int total = 0;

        foreach (
            IncomeTransaction transaction
            in IncomeManager.Instance.Transactions)
        {
            if (transaction == null)
            {
                continue;
            }

            if (transaction.Day == day)
            {
                total += transaction.Amount;
            }
        }

        return total;
    }

    // =========================================================
    // EXPENSE
    // =========================================================

    private int GetTotalExpense(int day)
    {
        if (DailyExpenseSummary.Instance == null)
        {
            return 0;
        }

        return
            DailyExpenseSummary.Instance
                .GetTotalForDay(day);
    }

    // =========================================================
    // CLEAR UI
    // =========================================================

    private void ClearSummaryUI()
    {
        if (incomeText != null)
        {
            incomeText.text =
                "Income: Rs. 0";
        }

        if (expenseText != null)
        {
            expenseText.text =
                "Expenses: Rs. 0";
        }

        if (netText != null)
        {
            netText.text =
                "Net: Rs. 0";
        }
    }
}