using System.Collections.Generic;
using UnityEngine;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance { get; private set; }

    // =========================================================
    // SAVINGS ACCOUNT
    // =========================================================

    [Header("Savings Account")]
    [SerializeField]
    private string savingsAccountNumber = "SV-1001";

    [SerializeField]
    private string savingsAccountName = "Vignesh";

    [SerializeField]
    private int savingsStartingBalance = 10000;

    // =========================================================
    // CURRENT ACCOUNT
    // =========================================================

    [Header("Current Account")]
    [SerializeField]
    private string currentAccountNumber = "CA-1001";

    [SerializeField]
    private string currentAccountName = "Shop";

    [SerializeField]
    private int currentStartingBalance = 5000;

    // =========================================================
    // MONTHLY EMI
    // =========================================================

    [Header("Monthly EMI")]
    [SerializeField]
    private int monthlyEmiAmount = 3000;

    [SerializeField]
    private int emiDueDay = 3;

    [SerializeField]
    private int bounceCharge = 500;

    // =========================================================
    // EMI STATE
    // =========================================================

    private readonly List<int> overdueEmiMonths =
        new List<int>();

    private int lastEmiProcessedMonth = -1;

    // =========================================================
    // BANK ACCOUNTS
    // =========================================================

    private BankAccount savingsAccount;
    private BankAccount currentAccount;

    // =========================================================
    // BANK TRANSACTIONS
    // =========================================================

    private readonly List<BankTransaction> transactions =
        new List<BankTransaction>();

    // =========================================================
    // PUBLIC ACCOUNT DATA
    // =========================================================

    public BankAccount SavingsAccount =>
        savingsAccount;

    public BankAccount CurrentAccount =>
        currentAccount;

    // =========================================================
    // SAVINGS ACCOUNT DETAILS
    // =========================================================

    public string SavingsAccountNumber =>
        savingsAccount != null
            ? savingsAccount.AccountNumber
            : string.Empty;

    public string SavingsAccountName =>
        savingsAccount != null
            ? savingsAccount.AccountName
            : string.Empty;

    public int SavingsBalance =>
        savingsAccount != null
            ? savingsAccount.Balance
            : 0;

    // =========================================================
    // CURRENT ACCOUNT DETAILS
    // =========================================================

    public string CurrentAccountNumber =>
        currentAccount != null
            ? currentAccount.AccountNumber
            : string.Empty;

    public string CurrentAccountName =>
        currentAccount != null
            ? currentAccount.AccountName
            : string.Empty;

    public int CurrentBalance =>
        currentAccount != null
            ? currentAccount.Balance
            : 0;

    // =========================================================
    // GET BALANCE
    // =========================================================

    public int GetBalance(
        FinanceAccountType accountType)
    {
        switch (accountType)
        {
            case FinanceAccountType.Savings:
                return SavingsBalance;

            case FinanceAccountType.Current:
                return CurrentBalance;

            default:
                return 0;
        }
    }

    // =========================================================
    // EMI INFORMATION
    // =========================================================

    public int MonthlyEmiAmount =>
        monthlyEmiAmount;

    public int EmiDueDay =>
        emiDueDay;

    public int BounceCharge =>
        bounceCharge;

    // =========================================================
    // BANK TRANSACTIONS
    // =========================================================

    public IReadOnlyList<BankTransaction> Transactions =>
        transactions;

    // =========================================================
    // OVERDUE EMI COUNT
    // =========================================================

    public int OverdueEmiCount =>
        overdueEmiMonths.Count;

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

        InitializeAccounts();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        ProcessMonthlyEMI();
    }

    // =========================================================
    // INITIALIZE ACCOUNTS
    // =========================================================

    private void InitializeAccounts()
    {
        savingsStartingBalance =
            Mathf.Max(0, savingsStartingBalance);

        currentStartingBalance =
            Mathf.Max(0, currentStartingBalance);

        monthlyEmiAmount =
            Mathf.Max(0, monthlyEmiAmount);

        if (emiDueDay < 1)
        {
            emiDueDay = 3;
        }

        bounceCharge =
            Mathf.Max(0, bounceCharge);

        // =====================================================
        // SAVINGS
        // =====================================================

        savingsAccount =
            new BankAccount(
                savingsAccountNumber,
                savingsAccountName,
                FinanceAccountType.Savings,
                savingsStartingBalance
            );

        // =====================================================
        // CURRENT
        // =====================================================

        currentAccount =
            new BankAccount(
                currentAccountNumber,
                currentAccountName,
                FinanceAccountType.Current,
                currentStartingBalance
            );

        overdueEmiMonths.Clear();

        transactions.Clear();

        lastEmiProcessedMonth = -1;

        Debug.Log(
            $"Bank initialized | " +
            $"Savings: {SavingsAccountNumber} | " +
            $"Balance: Rs. {SavingsBalance:N0} | " +
            $"Current: {CurrentAccountNumber} | " +
            $"Balance: Rs. {CurrentBalance:N0}"
        );
    }

    // =========================================================
    // GET ACCOUNT
    // =========================================================

    private BankAccount GetAccount(
        FinanceAccountType accountType)
    {
        switch (accountType)
        {
            case FinanceAccountType.Savings:
                return savingsAccount;

            case FinanceAccountType.Current:
                return currentAccount;

            default:
                return null;
        }
    }

    // =========================================================
    // MONTHLY EMI PROCESSING
    // =========================================================

    private void ProcessMonthlyEMI()
    {
        if (monthlyEmiAmount <= 0)
        {
            return;
        }

        if (GameTimeManager.Instance == null)
        {
            return;
        }

        int currentMonth =
            GameTimeManager.Instance.CurrentMonth;

        int currentDay =
            GameTimeManager.Instance.CurrentDay;

        if (currentDay != emiDueDay)
        {
            return;
        }

        if (lastEmiProcessedMonth ==
            currentMonth)
        {
            return;
        }

        ProcessEMIPayments(currentMonth);
    }

    // =========================================================
    // PROCESS EMI PAYMENTS
    // =========================================================

    private void ProcessEMIPayments(
        int currentMonth)
    {
        if (savingsAccount == null)
        {
            Debug.LogError(
                "EMI processing failed: " +
                "Savings account is not initialized."
            );

            return;
        }

        int overdueCount =
            overdueEmiMonths.Count;

        int overdueEmiAmount =
            overdueCount *
            monthlyEmiAmount;

        int overdueBounceAmount =
            overdueCount *
            bounceCharge;

        int currentEmiAmount =
            monthlyEmiAmount;

        int totalRequired =
            overdueEmiAmount +
            overdueBounceAmount +
            currentEmiAmount;

        // =====================================================
        // INSUFFICIENT BALANCE
        // =====================================================

        if (savingsAccount.Balance <
            totalRequired)
        {
            HandleCurrentEMIFailure(
                currentMonth
            );

            return;
        }

        // =====================================================
        // PAY OVERDUE EMI
        // =====================================================

        if (overdueCount > 0)
        {
            foreach (
                int overdueMonth
                in overdueEmiMonths)
            {
                if (!DebitAmount(
                        FinanceAccountType.Savings,
                        monthlyEmiAmount))
                {
                    return;
                }

                RecordBankTransaction(
                    FinanceAccountType.Savings,
                    BankTransaction.TransactionType.EmiDebit,
                    monthlyEmiAmount
                );

                RecordFinanceExpense(
                    FinanceAccountType.Savings,
                    monthlyEmiAmount,
                    $"EMI Debited - Month {overdueMonth}"
                );

                Debug.Log(
                    $"Overdue EMI paid | " +
                    $"Original Month: {overdueMonth} | " +
                    $"Amount: Rs. {monthlyEmiAmount:N0}"
                );

                if (bounceCharge > 0)
                {
                    if (!DebitAmount(
                            FinanceAccountType.Savings,
                            bounceCharge))
                    {
                        return;
                    }

                    RecordBankTransaction(
                        FinanceAccountType.Savings,
                        BankTransaction.TransactionType.BounceCharge,
                        bounceCharge
                    );

                    RecordFinanceExpense(
                        FinanceAccountType.Savings,
                        bounceCharge,
                        $"Bounce Charge - Month {overdueMonth}"
                    );

                    Debug.Log(
                        $"Bounce charge applied | " +
                        $"Failed Month: {overdueMonth} | " +
                        $"Amount: Rs. {bounceCharge:N0}"
                    );
                }
            }

            overdueEmiMonths.Clear();
        }

        // =====================================================
        // PAY CURRENT MONTH EMI
        // =====================================================

        if (!DebitAmount(
                FinanceAccountType.Savings,
                currentEmiAmount))
        {
            return;
        }

        RecordBankTransaction(
            FinanceAccountType.Savings,
            BankTransaction.TransactionType.EmiDebit,
            currentEmiAmount
        );

        RecordFinanceExpense(
            FinanceAccountType.Savings,
            currentEmiAmount,
            $"EMI Debited - Month {currentMonth}"
        );

        lastEmiProcessedMonth =
            currentMonth;

        Debug.Log(
            $"Current EMI paid | " +
            $"Month: {currentMonth} | " +
            $"Amount: Rs. {currentEmiAmount:N0} | " +
            $"Savings Balance: Rs. {SavingsBalance:N0}"
        );
    }

    // =========================================================
    // HANDLE EMI FAILURE
    // =========================================================

    private void HandleCurrentEMIFailure(
        int currentMonth)
    {
        if (!overdueEmiMonths.Contains(
                currentMonth))
        {
            overdueEmiMonths.Add(
                currentMonth
            );
        }

        lastEmiProcessedMonth =
            currentMonth;

        Debug.LogWarning(
            $"EMI debit FAILED | " +
            $"Month: {currentMonth} | " +
            $"Required: Rs. {monthlyEmiAmount:N0} | " +
            $"Available: Rs. {SavingsBalance:N0} | " +
            $"No expense recorded. EMI marked overdue."
        );
    }

    // =========================================================
    // DEBIT AMOUNT
    // =========================================================

    private bool DebitAmount(
        FinanceAccountType accountType,
        int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        BankAccount account =
            GetAccount(accountType);

        if (account == null)
        {
            Debug.LogError(
                $"Debit failed: " +
                $"{accountType} account not initialized."
            );

            return false;
        }

        if (account.Balance < amount)
        {
            Debug.LogWarning(
                $"Bank debit failed | " +
                $"Account: {accountType} | " +
                $"Required: Rs. {amount:N0} | " +
                $"Available: Rs. {account.Balance:N0}"
            );

            return false;
        }

        account.Balance -= amount;

        return true;
    }

    // =========================================================
    // CREDIT AMOUNT
    // =========================================================

    public bool Credit(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "Credit failed: Amount must be greater than zero."
            );

            return false;
        }

        BankAccount account =
            GetAccount(accountType);

        if (account == null)
        {
            Debug.LogError(
                $"Credit failed: " +
                $"{accountType} account not initialized."
            );

            return false;
        }

        account.Balance += amount;

        RecordBankTransaction(
            accountType,
            BankTransaction.TransactionType.Deposit,
            amount
        );

        RecordFinanceIncome(
            accountType,
            amount,
            description
        );

        Debug.Log(
            $"Bank credit successful | " +
            $"Account: {accountType} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {account.Balance:N0}"
        );

        return true;
    }

    // =========================================================
    // DEBIT
    // =========================================================

    public bool Debit(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (!DebitAmount(
                accountType,
                amount))
        {
            return false;
        }

        RecordBankTransaction(
            accountType,
            BankTransaction.TransactionType.Withdraw,
            amount
        );

        RecordFinanceExpense(
            accountType,
            amount,
            description
        );

        Debug.Log(
            $"Bank debit successful | " +
            $"Account: {accountType} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {GetBalance(accountType):N0}"
        );

        return true;
    }

    // =========================================================
    // DEPOSIT - LEGACY BANK PANEL SUPPORT
    // =========================================================

    public bool Deposit(int amount)
    {
        return Deposit(
            FinanceAccountType.Savings,
            amount
        );
    }

    // =========================================================
    // DEPOSIT TO ACCOUNT
    // =========================================================

    public bool Deposit(
        FinanceAccountType accountType,
        int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "Deposit failed: Amount must be greater than zero."
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Deposit failed: MoneyManager not found."
            );

            return false;
        }

        if (!MoneyManager.Instance.CanAfford(
                amount))
        {
            Debug.LogWarning(
                $"Deposit failed: Insufficient money. " +
                $"Required: Rs. {amount:N0}"
            );

            return false;
        }

        BankAccount account =
            GetAccount(accountType);

        if (account == null)
        {
            Debug.LogError(
                $"Deposit failed: " +
                $"{accountType} account not initialized."
            );

            return false;
        }

        MoneyManager.Instance.RemoveMoney(
            amount
        );

        account.Balance += amount;

        RecordBankTransaction(
            accountType,
            BankTransaction.TransactionType.Deposit,
            amount
        );

        Debug.Log(
            $"Deposit successful | " +
            $"Account: {accountType} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {account.Balance:N0}"
        );

        return true;
    }

    // =========================================================
    // WITHDRAW - LEGACY BANK PANEL SUPPORT
    // =========================================================

    public bool Withdraw(int amount)
    {
        return Withdraw(
            FinanceAccountType.Savings,
            amount
        );
    }

    // =========================================================
    // WITHDRAW FROM ACCOUNT
    // =========================================================

    public bool Withdraw(
        FinanceAccountType accountType,
        int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "Withdraw failed: Amount must be greater than zero."
            );

            return false;
        }

        BankAccount account =
            GetAccount(accountType);

        if (account == null)
        {
            Debug.LogError(
                $"Withdraw failed: " +
                $"{accountType} account not initialized."
            );

            return false;
        }

        if (account.Balance < amount)
        {
            Debug.LogWarning(
                $"Withdraw failed | " +
                $"Account: {accountType} | " +
                $"Available: Rs. {account.Balance:N0}"
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Withdraw failed: MoneyManager not found."
            );

            return false;
        }

        account.Balance -= amount;

        MoneyManager.Instance.AddMoney(
            amount
        );

        RecordBankTransaction(
            accountType,
            BankTransaction.TransactionType.Withdraw,
            amount
        );

        Debug.Log(
            $"Withdraw successful | " +
            $"Account: {accountType} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {account.Balance:N0}"
        );

        return true;
    }

    // =========================================================
    // RECORD FINANCE INCOME
    // =========================================================

    private void RecordFinanceIncome(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "BankManager: FinanceTransactionManager " +
                "not found. Income ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordIncome(
            accountType,
            amount,
            description
        );

        Debug.Log(
            $"Finance income recorded | " +
            $"Account: {accountType} | " +
            $"Description: {description} | " +
            $"Amount: Rs. {amount:N0}"
        );
    }

    // =========================================================
    // RECORD FINANCE EXPENSE
    // =========================================================

    private void RecordFinanceExpense(
        FinanceAccountType accountType,
        int amount,
        string description)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "BankManager: FinanceTransactionManager " +
                "not found. Expense ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordExpense(
            accountType,
            amount,
            description
        );

        Debug.Log(
            $"Finance expense recorded | " +
            $"Account: {accountType} | " +
            $"Description: {description} | " +
            $"Amount: Rs. {amount:N0}"
        );
    }

    // =========================================================
    // RECORD BANK TRANSACTION
    // =========================================================

    private void RecordBankTransaction(
        FinanceAccountType accountType,
        BankTransaction.TransactionType type,
        int amount)
    {
        BankTransaction transaction =
            new BankTransaction(
                type,
                amount,
                GetBalance(accountType)
            );

        transactions.Add(
            transaction
        );

        Debug.Log(
            $"Bank transaction recorded | " +
            $"Account: {accountType} | " +
            $"Type: {type} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {GetBalance(accountType):N0}"
        );
    }

    // =========================================================
    // GET OVERDUE EMI MONTHS
    // =========================================================

    public IReadOnlyList<int>
        GetOverdueEMIMonths()
    {
        return overdueEmiMonths;
    }

    // =========================================================
    // GET TOTAL OVERDUE EMI
    // =========================================================

    public int GetTotalOverdueEMIAmount()
    {
        return
            overdueEmiMonths.Count *
            monthlyEmiAmount;
    }

    // =========================================================
    // GET TOTAL BOUNCE CHARGES
    // =========================================================

    public int GetTotalBounceCharges()
    {
        return
            overdueEmiMonths.Count *
            bounceCharge;
    }
}