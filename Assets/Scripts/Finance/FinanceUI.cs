using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance { get; private set; }

    // =========================
    // FINANCE UI
    // =========================

    [Header("Finance UI")]
    [SerializeField]
    private GameObject financePanel;

    // =========================
    // INPUT
    // =========================

    [Header("Input")]
    [SerializeField]
    private Key toggleKey = Key.Q;

    // =========================
    // EMPLOYEE ACCOUNT
    // =========================

    [Header("Employee Account")]
    [SerializeField]
    private TextMeshProUGUI employeeBalanceText;

    [SerializeField]
    private TextMeshProUGUI employeeIncomeText;

    [SerializeField]
    private TextMeshProUGUI employeeExpenseText;

    [SerializeField]
    private TextMeshProUGUI employeeNetText;

    // =========================
    // SHOP ACCOUNT
    // =========================

    [Header("Shop Account")]
    [SerializeField]
    private TextMeshProUGUI shopBalanceText;

    [SerializeField]
    private TextMeshProUGUI shopIncomeText;

    [SerializeField]
    private TextMeshProUGUI shopExpenseText;

    [SerializeField]
    private TextMeshProUGUI shopNetText;

    // =========================
    // MERGED ACCOUNT
    // =========================

    [Header("Merged Account")]
    [SerializeField]
    private TextMeshProUGUI mergedBalanceText;

    [SerializeField]
    private TextMeshProUGUI mergedIncomeText;

    [SerializeField]
    private TextMeshProUGUI mergedExpenseText;

    [SerializeField]
    private TextMeshProUGUI mergedNetText;

    // =========================
    // SECTIONS
    // =========================

    [Header("Sections")]
    [SerializeField]
    private GameObject employeeSection;

    [SerializeField]
    private GameObject shopSection;

    [SerializeField]
    private GameObject mergedSection;

    // =========================
    // PUBLIC
    // =========================

    public bool IsOpen =>
        financePanel != null &&
        financePanel.activeSelf;

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

        ClearUI();
    }

    // =========================
    // UPDATE
    // =========================

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

        UpdateFinanceUI();

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

        if (FinanceAccountManager.Instance.IsMergedAccount)
        {
            UpdateMergedAccount();
        }
        else
        {
            UpdateEmployeeAccount();
            UpdateShopAccount();
        }
    }

    // =========================================================
    // ACCOUNT SECTIONS
    // =========================================================

    private void UpdateAccountSections()
    {
        bool merged =
            FinanceAccountManager.Instance != null &&
            FinanceAccountManager.Instance.IsMergedAccount;

        if (employeeSection != null)
        {
            employeeSection.SetActive(!merged);
        }

        if (shopSection != null)
        {
            shopSection.SetActive(!merged);
        }

        if (mergedSection != null)
        {
            mergedSection.SetActive(merged);
        }
    }

    // =========================================================
    // EMPLOYEE ACCOUNT
    // =========================================================

    private void UpdateEmployeeAccount()
    {
        FinanceAccountType account =
            FinanceAccountType.Employee;

        int balance =
            FinanceAccountManager.Instance
                .EmployeeBalance;

        int income =
            GetTotalIncome(account);

        int expense =
            GetTotalExpense(account);

        int net =
            income - expense;

        if (employeeBalanceText != null)
        {
            employeeBalanceText.text =
                $"Balance: Rs. {balance:N0}";
        }

        if (employeeIncomeText != null)
        {
            employeeIncomeText.text =
                $"Income: Rs. {income:N0}";
        }

        if (employeeExpenseText != null)
        {
            employeeExpenseText.text =
                $"Expense: Rs. {expense:N0}";
        }

        if (employeeNetText != null)
        {
            employeeNetText.text =
                $"Net: Rs. {net:N0}";
        }
    }

    // =========================================================
    // SHOP ACCOUNT
    // =========================================================

    private void UpdateShopAccount()
    {
        FinanceAccountType account =
            FinanceAccountType.Shop;

        int balance =
            FinanceAccountManager.Instance
                .ShopBalance;

        int income =
            GetTotalIncome(account);

        int expense =
            GetTotalExpense(account);

        int net =
            income - expense;

        if (shopBalanceText != null)
        {
            shopBalanceText.text =
                $"Balance: Rs. {balance:N0}";
        }

        if (shopIncomeText != null)
        {
            shopIncomeText.text =
                $"Income: Rs. {income:N0}";
        }

        if (shopExpenseText != null)
        {
            shopExpenseText.text =
                $"Expense: Rs. {expense:N0}";
        }

        if (shopNetText != null)
        {
            shopNetText.text =
                $"Net: Rs. {net:N0}";
        }
    }

    // =========================================================
    // MERGED ACCOUNT
    // =========================================================

    private void UpdateMergedAccount()
    {
        int employeeBalance =
            FinanceAccountManager.Instance
                .EmployeeBalance;

        int shopBalance =
            FinanceAccountManager.Instance
                .ShopBalance;

        int mergedBalance =
            employeeBalance +
            shopBalance;

        int employeeIncome =
            GetTotalIncome(
                FinanceAccountType.Employee
            );

        int shopIncome =
            GetTotalIncome(
                FinanceAccountType.Shop
            );

        int employeeExpense =
            GetTotalExpense(
                FinanceAccountType.Employee
            );

        int shopExpense =
            GetTotalExpense(
                FinanceAccountType.Shop
            );

        int mergedIncome =
            employeeIncome +
            shopIncome;

        int mergedExpense =
            employeeExpense +
            shopExpense;

        int mergedNet =
            mergedIncome -
            mergedExpense;

        if (mergedBalanceText != null)
        {
            mergedBalanceText.text =
                $"Balance: Rs. {mergedBalance:N0}";
        }

        if (mergedIncomeText != null)
        {
            mergedIncomeText.text =
                $"Income: Rs. {mergedIncome:N0}";
        }

        if (mergedExpenseText != null)
        {
            mergedExpenseText.text =
                $"Expense: Rs. {mergedExpense:N0}";
        }

        if (mergedNetText != null)
        {
            mergedNetText.text =
                $"Net: Rs. {mergedNet:N0}";
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
        if (employeeBalanceText != null)
        {
            employeeBalanceText.text =
                "Balance: Rs. 0";
        }

        if (employeeIncomeText != null)
        {
            employeeIncomeText.text =
                "Income: Rs. 0";
        }

        if (employeeExpenseText != null)
        {
            employeeExpenseText.text =
                "Expense: Rs. 0";
        }

        if (employeeNetText != null)
        {
            employeeNetText.text =
                "Net: Rs. 0";
        }

        if (shopBalanceText != null)
        {
            shopBalanceText.text =
                "Balance: Rs. 0";
        }

        if (shopIncomeText != null)
        {
            shopIncomeText.text =
                "Income: Rs. 0";
        }

        if (shopExpenseText != null)
        {
            shopExpenseText.text =
                "Expense: Rs. 0";
        }

        if (shopNetText != null)
        {
            shopNetText.text =
                "Net: Rs. 0";
        }

        if (mergedBalanceText != null)
        {
            mergedBalanceText.text =
                "Balance: Rs. 0";
        }

        if (mergedIncomeText != null)
        {
            mergedIncomeText.text =
                "Income: Rs. 0";
        }

        if (mergedExpenseText != null)
        {
            mergedExpenseText.text =
                "Expense: Rs. 0";
        }

        if (mergedNetText != null)
        {
            mergedNetText.text =
                "Net: Rs. 0";
        }
    }
}