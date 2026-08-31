using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance { get; private set; }

    // =========================================================
    // FINANCE UI
    // =========================================================

    [Header("Finance UI")]
    [SerializeField]
    private GameObject financePanel;

    // =========================================================
    // INPUT
    // =========================================================

    [Header("Input")]
    [SerializeField]
    private Key toggleKey = Key.Q;

    // =========================================================
    // SAVINGS ACCOUNT
    // =========================================================

    [Header("Savings Account")]
    [SerializeField]
    private TextMeshProUGUI savingsBalanceText;

    [SerializeField]
    private TextMeshProUGUI savingsIncomeText;

    [SerializeField]
    private TextMeshProUGUI savingsExpenseText;

    [SerializeField]
    private TextMeshProUGUI savingsNetText;

    // =========================================================
    // CURRENT ACCOUNT
    // =========================================================

    [Header("Current Account")]
    [SerializeField]
    private TextMeshProUGUI currentBalanceText;

    [SerializeField]
    private TextMeshProUGUI currentIncomeText;

    [SerializeField]
    private TextMeshProUGUI currentExpenseText;

    [SerializeField]
    private TextMeshProUGUI currentNetText;

    // =========================================================
    // SECTIONS
    // =========================================================

    [Header("Sections")]
    [SerializeField]
    private GameObject savingsSection;

    [SerializeField]
    private GameObject currentSection;

    // =========================================================
    // PUBLIC
    // =========================================================

    public bool IsOpen =>
        financePanel != null &&
        financePanel.activeSelf;

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

        InitializeUI();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

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

        ClearUI();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleFinanceUI();
        }

        if (IsOpen)
        {
            UpdateFinanceUI();
        }
    }

    // =========================================================
    // TOGGLE
    // =========================================================

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

    // =========================================================
    // OPEN
    // =========================================================

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

        UpdateFinanceUI();

        Debug.Log(
            "Finance UI opened."
        );
    }

    // =========================================================
    // CLOSE
    // =========================================================

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

    // =========================================================
    // SET VISIBILITY
    // =========================================================

    public void SetFinanceUIVisible(
        bool visible)
    {
        if (financePanel == null)
        {
            return;
        }

        financePanel.SetActive(visible);

        if (visible)
        {
            UpdateFinanceUI();
        }
    }

    // =========================================================
    // UPDATE FINANCE UI
    // =========================================================

    private void UpdateFinanceUI()
    {
        if (FinanceAccountManager.Instance == null)
        {
            ClearUI();
            return;
        }

        UpdateAccountSections();

        UpdateSavingsAccount();

        UpdateCurrentAccount();
    }

    // =========================================================
    // ACCOUNT SECTIONS
    // =========================================================

    private void UpdateAccountSections()
    {
        FinanceAccountType selectedAccount =
            FinanceAccountManager.Instance.AccountType;

        bool savings =
            selectedAccount == FinanceAccountType.Savings;

        bool current =
            selectedAccount == FinanceAccountType.Current;

        if (savingsSection != null)
        {
            savingsSection.SetActive(savings);
        }

        if (currentSection != null)
        {
            currentSection.SetActive(current);
        }
    }

    // =========================================================
    // SAVINGS ACCOUNT
    // =========================================================

    private void UpdateSavingsAccount()
    {
        FinanceAccountType account =
            FinanceAccountType.Savings;

        int balance =
            FinanceAccountManager.Instance
                .SavingsBalance;

        int income =
            GetTotalIncome(account);

        int expense =
            GetTotalExpense(account);

        int net =
            income - expense;

        if (savingsBalanceText != null)
        {
            savingsBalanceText.text =
                $"Balance: Rs. {balance:N0}";
        }

        if (savingsIncomeText != null)
        {
            savingsIncomeText.text =
                $"Income: Rs. {income:N0}";
        }

        if (savingsExpenseText != null)
        {
            savingsExpenseText.text =
                $"Expense: Rs. {expense:N0}";
        }

        if (savingsNetText != null)
        {
            savingsNetText.text =
                $"Net: Rs. {net:N0}";
        }
    }

    // =========================================================
    // CURRENT ACCOUNT
    // =========================================================

    private void UpdateCurrentAccount()
    {
        FinanceAccountType account =
            FinanceAccountType.Current;

        int balance =
            FinanceAccountManager.Instance
                .CurrentBalance;

        int income =
            GetTotalIncome(account);

        int expense =
            GetTotalExpense(account);

        int net =
            income - expense;

        if (currentBalanceText != null)
        {
            currentBalanceText.text =
                $"Balance: Rs. {balance:N0}";
        }

        if (currentIncomeText != null)
        {
            currentIncomeText.text =
                $"Income: Rs. {income:N0}";
        }

        if (currentExpenseText != null)
        {
            currentExpenseText.text =
                $"Expense: Rs. {expense:N0}";
        }

        if (currentNetText != null)
        {
            currentNetText.text =
                $"Net: Rs. {net:N0}";
        }
    }

    // =========================================================
    // TOTAL INCOME
    // =========================================================

    private int GetTotalIncome(
        FinanceAccountType accountType)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            return 0;
        }

        return FinanceTransactionManager.Instance
            .GetTotalIncome(accountType);
    }

    // =========================================================
    // TOTAL EXPENSE
    // =========================================================

    private int GetTotalExpense(
        FinanceAccountType accountType)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            return 0;
        }

        return FinanceTransactionManager.Instance
            .GetTotalExpense(accountType);
    }

    // =========================================================
    // CLEAR UI
    // =========================================================

    private void ClearUI()
    {
        if (savingsBalanceText != null)
        {
            savingsBalanceText.text =
                "Balance: Rs. 0";
        }

        if (savingsIncomeText != null)
        {
            savingsIncomeText.text =
                "Income: Rs. 0";
        }

        if (savingsExpenseText != null)
        {
            savingsExpenseText.text =
                "Expense: Rs. 0";
        }

        if (savingsNetText != null)
        {
            savingsNetText.text =
                "Net: Rs. 0";
        }

        if (currentBalanceText != null)
        {
            currentBalanceText.text =
                "Balance: Rs. 0";
        }

        if (currentIncomeText != null)
        {
            currentIncomeText.text =
                "Income: Rs. 0";
        }

        if (currentExpenseText != null)
        {
            currentExpenseText.text =
                "Expense: Rs. 0";
        }

        if (currentNetText != null)
        {
            currentNetText.text =
                "Net: Rs. 0";
        }
    }
}