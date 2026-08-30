using System.Collections.Generic;
using UnityEngine;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance { get; private set; }

    // =========================
    // BANK ACCOUNT
    // =========================

    [Header("Bank Account")]
    [SerializeField]
    private string accountNumber = "EMDB001";

    [SerializeField]
    private int startingBalance = 0;

    // =========================
    // MONTHLY EMI
    // =========================

    [Header("Monthly EMI")]
    [SerializeField]
    private int monthlyEmiAmount = 3000;

    [SerializeField]
    private int emiDueDay = 3;

    [SerializeField]
    private int bounceCharge = 500;

    // =========================
    // EMI STATE
    // =========================

    private readonly List<int> overdueEmiMonths =
        new List<int>();

    private int lastEmiProcessedMonth = -1;

    // =========================
    // BANK DATA
    // =========================

    private BankAccount bankAccount;

    private readonly List<BankTransaction> transactions =
        new List<BankTransaction>();

    // =========================
    // PUBLIC PROPERTIES
    // =========================

    public string AccountNumber =>
        bankAccount != null
            ? bankAccount.AccountNumber
            : string.Empty;

    public int Balance =>
        bankAccount != null
            ? bankAccount.Balance
            : 0;

    public int MonthlyEmiAmount =>
        monthlyEmiAmount;

    public int EmiDueDay =>
        emiDueDay;

    public int BounceCharge =>
        bounceCharge;

    public IReadOnlyList<BankTransaction> Transactions =>
        transactions;

    // =========================
    // OVERDUE EMI COUNT
    // =========================

    public int OverdueEmiCount =>
        overdueEmiMonths.Count;

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

        InitializeAccount();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        ProcessMonthlyEMI();
    }

    // =========================
    // INITIALIZE ACCOUNT
    // =========================

    private void InitializeAccount()
    {
        if (startingBalance < 0)
        {
            startingBalance = 0;
        }

        if (monthlyEmiAmount < 0)
        {
            monthlyEmiAmount = 0;
        }

        if (emiDueDay < 1)
        {
            emiDueDay = 3;
        }

        if (bounceCharge < 0)
        {
            bounceCharge = 0;
        }

        bankAccount = new BankAccount(
            accountNumber,
            startingBalance
        );

        overdueEmiMonths.Clear();

        lastEmiProcessedMonth = -1;

        Debug.Log(
            $"Bank initialized | " +
            $"Account: {bankAccount.AccountNumber} | " +
            $"Balance: Rs. {bankAccount.Balance:N0} | " +
            $"Monthly EMI: Rs. {monthlyEmiAmount:N0} | " +
            $"EMI Day: {emiDueDay} | " +
            $"Bounce Charge: Rs. {bounceCharge:N0}"
        );
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

        ProcessEMIPayments(
            currentMonth
        );
    }

    // =========================================================
    // PROCESS ALL EMI PAYMENTS
    // =========================================================

    private void ProcessEMIPayments(
        int currentMonth)
    {
        if (bankAccount == null)
        {
            Debug.LogError(
                "EMI processing failed: " +
                "Bank account not initialized."
            );

            return;
        }

        // =========================
        // BUILD REQUIRED PAYMENT
        // =========================

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

        // =========================
        // CHECK FULL PAYMENT
        // =========================

        if (bankAccount.Balance <
            totalRequired)
        {
            HandleCurrentEMIFailure(
                currentMonth
            );

            return;
        }

        // =========================
        // PAY OVERDUE EMI
        // =========================

        if (overdueCount > 0)
        {
            foreach (
                int overdueMonth
                in overdueEmiMonths)
            {
                if (!DebitAmount(
                        monthlyEmiAmount))
                {
                    return;
                }

                RecordTransaction(
                    BankTransaction.TransactionType.EmiDebit,
                    monthlyEmiAmount
                );

                RecordFinanceTransaction(
                    monthlyEmiAmount,
                    "EMI Debited"
                );

                Debug.Log(
                    $"Overdue EMI paid | " +
                    $"Original Month: {overdueMonth} | " +
                    $"Amount: Rs. {monthlyEmiAmount:N0}"
                );

                if (bounceCharge > 0)
                {
                    if (!DebitAmount(
                            bounceCharge))
                    {
                        return;
                    }

                    RecordTransaction(
                        BankTransaction.TransactionType.BounceCharge,
                        bounceCharge
                    );

                    RecordFinanceTransaction(
                        bounceCharge,
                        "Bounce Charge"
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

        // =========================
        // PAY CURRENT EMI
        // =========================

        if (!DebitAmount(
                currentEmiAmount))
        {
            return;
        }

        RecordTransaction(
            BankTransaction.TransactionType.EmiDebit,
            currentEmiAmount
        );

        RecordFinanceTransaction(
            currentEmiAmount,
            "EMI Debited"
        );

        lastEmiProcessedMonth =
            currentMonth;

        Debug.Log(
            $"Current EMI paid | " +
            $"Month: {currentMonth} | " +
            $"Day: {GameTimeManager.Instance.CurrentDay} | " +
            $"Amount: Rs. {currentEmiAmount:N0} | " +
            $"Remaining Bank Balance: Rs. {bankAccount.Balance:N0}"
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
            $"EMI debit failed | " +
            $"Month: {currentMonth} | " +
            $"Required Current EMI: Rs. {monthlyEmiAmount:N0} | " +
            $"Available Bank Balance: Rs. {bankAccount.Balance:N0} | " +
            $"Current EMI marked overdue."
        );
    }

    // =========================================================
    // DEBIT AMOUNT
    // =========================================================

    private bool DebitAmount(
        int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (bankAccount.Balance <
            amount)
        {
            Debug.LogWarning(
                $"Bank debit failed | " +
                $"Required: Rs. {amount:N0} | " +
                $"Available: Rs. {bankAccount.Balance:N0}"
            );

            return false;
        }

        bankAccount.Balance -=
            amount;

        return true;
    }

    // =========================================================
    // RECORD FINANCE TRANSACTION
    // =========================================================

    private void RecordFinanceTransaction(
        int amount,
        string description)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "BankManager: " +
                "FinanceTransactionManager not found. " +
                $"{description} ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordExpense(
            FinanceAccountType.Employee,
            amount,
            description
        );

        Debug.Log(
            $"Employee finance transaction recorded | " +
            $"Description: {description} | " +
            $"Amount: Rs. {amount:N0}"
        );
    }

    // =========================================================
    // DEPOSIT
    // =========================================================

    public bool Deposit(int amount)
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
                $"Deposit failed: Insufficient cash. " +
                $"Required: Rs. {amount:N0}"
            );

            return false;
        }

        MoneyManager.Instance.RemoveMoney(
            amount
        );

        bankAccount.Balance +=
            amount;

        RecordTransaction(
            BankTransaction.TransactionType.Deposit,
            amount
        );

        Debug.Log(
            $"Deposit successful | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Bank Balance: Rs. {bankAccount.Balance:N0}"
        );

        return true;
    }

    // =========================================================
    // WITHDRAW
    // =========================================================

    public bool Withdraw(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "Withdraw failed: Amount must be greater than zero."
            );

            return false;
        }

        if (bankAccount.Balance <
            amount)
        {
            Debug.LogWarning(
                $"Withdraw failed: Insufficient bank balance. " +
                $"Available: Rs. {bankAccount.Balance:N0}"
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

        bankAccount.Balance -=
            amount;

        MoneyManager.Instance.AddMoney(
            amount
        );

        RecordTransaction(
            BankTransaction.TransactionType.Withdraw,
            amount
        );

        Debug.Log(
            $"Withdraw successful | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Bank Balance: Rs. {bankAccount.Balance:N0}"
        );

        return true;
    }

    // =========================================================
    // RECORD BANK TRANSACTION
    // =========================================================

    private void RecordTransaction(
        BankTransaction.TransactionType type,
        int amount)
    {
        BankTransaction transaction =
            new BankTransaction(
                type,
                amount,
                bankAccount.Balance
            );

        transactions.Add(
            transaction
        );

        Debug.Log(
            $"Bank transaction recorded | " +
            $"Type: {type} | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Balance: Rs. {bankAccount.Balance:N0}"
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