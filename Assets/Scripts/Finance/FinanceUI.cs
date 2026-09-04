using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance { get; private set; }

    // =========================================================
    // FINANCE PANEL
    // =========================================================

    [Header("Finance Panel")]
    [SerializeField]
    private GameObject financePanel;

    // =========================================================
    // INPUT
    // =========================================================

    [Header("Input")]
    [SerializeField]
    private Key toggleKey = Key.Q;

    // =========================================================
    // TITLE
    // =========================================================

    [Header("Title")]
    [SerializeField]
    private TextMeshProUGUI titleText;

    // =========================================================
    // ACCOUNT BUTTONS
    // =========================================================

    [Header("Account Buttons")]
    [SerializeField]
    private Button savingsButton;

    [SerializeField]
    private Button currentButton;

    // =========================================================
    // ACCOUNT INFORMATION
    // =========================================================

    [Header("Account Information")]
    [SerializeField]
    private TextMeshProUGUI accountNumberText;

    [SerializeField]
    private TextMeshProUGUI balanceText;

    [SerializeField]
    private TextMeshProUGUI incomeText;

    [SerializeField]
    private TextMeshProUGUI expenseText;

    [SerializeField]
    private TextMeshProUGUI netText;

    // =========================================================
    // CLOSE BUTTON
    // =========================================================

    [Header("Close Button")]
    [SerializeField]
    private Button closeButton;

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
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        InitializeUI();

        RegisterButtonEvents();
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

        UpdateFinanceUI();
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
    // REGISTER BUTTON EVENTS
    // =========================================================

    private void RegisterButtonEvents()
    {
        if (savingsButton != null)
        {
            savingsButton.onClick.RemoveListener(
                SelectSavingsAccount
            );

            savingsButton.onClick.AddListener(
                SelectSavingsAccount
            );
        }

        if (currentButton != null)
        {
            currentButton.onClick.RemoveListener(
                SelectCurrentAccount
            );

            currentButton.onClick.AddListener(
                SelectCurrentAccount
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                CloseFinanceUI
            );

            closeButton.onClick.AddListener(
                CloseFinanceUI
            );
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
    // SELECT SAVINGS
    // =========================================================

    public void SelectSavingsAccount()
    {
        if (FinanceAccountManager.Instance == null)
        {
            Debug.LogWarning(
                "FinanceUI: FinanceAccountManager not found."
            );

            return;
        }

        FinanceAccountManager.Instance
            .SwitchToSavingsAccount();

        UpdateFinanceUI();

        Debug.Log(
            "Finance UI: SAVINGS selected."
        );
    }

    // =========================================================
    // SELECT CURRENT
    // =========================================================

    public void SelectCurrentAccount()
    {
        if (FinanceAccountManager.Instance == null)
        {
            Debug.LogWarning(
                "FinanceUI: FinanceAccountManager not found."
            );

            return;
        }

        FinanceAccountManager.Instance
            .SwitchToCurrentAccount();

        UpdateFinanceUI();

        Debug.Log(
            "Finance UI: CURRENT selected."
        );
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

        FinanceAccountType selectedAccount =
            FinanceAccountManager.Instance.AccountType;

        if (selectedAccount ==
            FinanceAccountType.Savings)
        {
            UpdateSavingsAccount();
        }
        else if (selectedAccount ==
                 FinanceAccountType.Current)
        {
            UpdateCurrentAccount();
        }
    }

    // =========================================================
    // SAVINGS ACCOUNT
    // =========================================================

    private void UpdateSavingsAccount()
    {
        int balance =
            FinanceAccountManager.Instance
                .SavingsBalance;

        int income =
            GetTotalIncome(
                FinanceAccountType.Savings
            );

        int expense =
            GetTotalExpense(
                FinanceAccountType.Savings
            );

        int net =
            income -
            expense;

        if (titleText != null)
        {
            titleText.text =
                "SAVINGS ACCOUNT";
        }

        if (accountNumberText != null)
        {
            if (BankManager.Instance != null)
            {
                accountNumberText.text =
                    $"Account: {BankManager.Instance.SavingsAccountNumber}";
            }
            else
            {
                accountNumberText.text =
                    "Account: SV-1001";
            }
        }

        UpdateValues(
            balance,
            income,
            expense,
            net
        );
    }

    // =========================================================
    // CURRENT ACCOUNT
    // =========================================================

    private void UpdateCurrentAccount()
    {
        int balance =
            FinanceAccountManager.Instance
                .CurrentBalance;

        int income =
            GetTotalIncome(
                FinanceAccountType.Current
            );

        int expense =
            GetTotalExpense(
                FinanceAccountType.Current
            );

        int net =
            income -
            expense;

        if (titleText != null)
        {
            titleText.text =
                "CURRENT ACCOUNT";
        }

        if (accountNumberText != null)
        {
            if (BankManager.Instance != null)
            {
                accountNumberText.text =
                    $"Account: {BankManager.Instance.CurrentAccountNumber}";
            }
            else
            {
                accountNumberText.text =
                    "Account: CA-1001";
            }
        }

        UpdateValues(
            balance,
            income,
            expense,
            net
        );
    }

    // =========================================================
    // UPDATE COMMON VALUES
    // =========================================================

    private void UpdateValues(
        int balance,
        int income,
        int expense,
        int net)
    {
        if (balanceText != null)
        {
            balanceText.text =
                $"Balance: Rs. {balance:N0}";
        }

        if (incomeText != null)
        {
            incomeText.text =
                $"Income: Rs. {income:N0}";
        }

        if (expenseText != null)
        {
            expenseText.text =
                $"Expense: Rs. {expense:N0}";
        }

        if (netText != null)
        {
            netText.text =
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
        if (titleText != null)
        {
            titleText.text =
                "FINANCE";
        }

        if (accountNumberText != null)
        {
            accountNumberText.text =
                "Account: -";
        }

        if (balanceText != null)
        {
            balanceText.text =
                "Balance: Rs. 0";
        }

        if (incomeText != null)
        {
            incomeText.text =
                "Income: Rs. 0";
        }

        if (expenseText != null)
        {
            expenseText.text =
                "Expense: Rs. 0";
        }

        if (netText != null)
        {
            netText.text =
                "Net: Rs. 0";
        }
    }
}