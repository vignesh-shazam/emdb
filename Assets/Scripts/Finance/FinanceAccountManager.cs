using UnityEngine;

public class FinanceAccountManager : MonoBehaviour
{
    public static FinanceAccountManager Instance { get; private set; }

    [Header("Account State")]
    [SerializeField]
    private FinanceAccountType accountType =
        FinanceAccountType.Employee;

    // =========================
    // ACCOUNT STATE
    // =========================

    public FinanceAccountType AccountType =>
        accountType;

    public bool IsEmployeeAccount =>
        accountType == FinanceAccountType.Employee;

    public bool IsShopAccount =>
        accountType == FinanceAccountType.Shop;

    public bool IsMergedAccount =>
        accountType == FinanceAccountType.Merged;

    // =========================================================
    // CASH BALANCE
    // =========================================================

    public int CashBalance
    {
        get
        {
            if (MoneyManager.Instance == null)
            {
                return 0;
            }

            return MoneyManager.Instance.CurrentMoney;
        }
    }

    // =========================================================
    // BANK BALANCE
    // =========================================================

    public int BankBalance
    {
        get
        {
            if (BankManager.Instance == null)
            {
                return 0;
            }

            return BankManager.Instance.Balance;
        }
    }

    // =========================================================
    // EMPLOYEE TOTAL BALANCE
    // =========================================================

    public int EmployeeBalance =>
        CashBalance +
        BankBalance;

    // =========================================================
    // SHOP INCOME
    // =========================================================

    public int ShopIncome
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalIncome(
                    FinanceAccountType.Shop
                );
        }
    }

    // =========================================================
    // SHOP EXPENSE
    // =========================================================

    public int ShopExpense
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalExpense(
                    FinanceAccountType.Shop
                );
        }
    }

    // =========================================================
    // SHOP NET
    // =========================================================

    public int ShopNet =>
        ShopIncome -
        ShopExpense;

    // =========================================================
    // EMPLOYEE INCOME
    // =========================================================

    public int EmployeeIncome
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalIncome(
                    FinanceAccountType.Employee
                );
        }
    }

    // =========================================================
    // EMPLOYEE EXPENSE
    // =========================================================

    public int EmployeeExpense
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalExpense(
                    FinanceAccountType.Employee
                );
        }
    }

    // =========================================================
    // EMPLOYEE NET
    // =========================================================

    public int EmployeeNet =>
        EmployeeIncome -
        EmployeeExpense;

    // =========================================================
    // MERGED TOTAL BALANCE
    // =========================================================

    public int MergedBalance =>
        CashBalance +
        BankBalance;

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

        ValidateState();
    }

    // =========================================================
    // VALIDATE
    // =========================================================

    private void ValidateState()
    {
        if (accountType != FinanceAccountType.Employee &&
            accountType != FinanceAccountType.Shop &&
            accountType != FinanceAccountType.Merged)
        {
            accountType =
                FinanceAccountType.Employee;
        }

        Debug.Log(
            $"Finance Account Manager initialized | " +
            $"Account: {accountType}"
        );
    }

    // =========================================================
    // SWITCH TO SHOP
    // =========================================================

    public void SwitchToShopAccount()
    {
        if (IsMergedAccount)
        {
            Debug.LogWarning(
                "Finance account is already merged."
            );

            return;
        }

        accountType =
            FinanceAccountType.Shop;

        Debug.Log(
            "Finance account changed to Shop."
        );
    }

    // =========================================================
    // MERGE ACCOUNTS
    // =========================================================

    public bool MergeAccounts()
    {
        if (IsMergedAccount)
        {
            Debug.LogWarning(
                "Finance accounts are already merged."
            );

            return false;
        }

        int cash =
            CashBalance;

        int bank =
            BankBalance;

        int total =
            cash + bank;

        accountType =
            FinanceAccountType.Merged;

        Debug.Log(
            $"Finance account merged | " +
            $"Cash: Rs. {cash:N0} | " +
            $"Bank: Rs. {bank:N0} | " +
            $"Total: Rs. {total:N0}"
        );

        return true;
    }

    // =========================================================
    // SET ACCOUNT TYPE
    // =========================================================

    public void SetAccountType(
        FinanceAccountType type)
    {
        accountType = type;

        Debug.Log(
            $"Finance account type changed to: {type}"
        );
    }

    // =========================================================
    // SHOP RESULT
    // =========================================================

    public int GetShopNet()
    {
        return ShopNet;
    }

    // =========================================================
    // EMPLOYEE TOTAL BALANCE
    // =========================================================

    public int GetEmployeeBalance()
    {
        return EmployeeBalance;
    }

    // =========================================================
    // MERGED BALANCE
    // =========================================================

    public int GetMergedBalance()
    {
        return MergedBalance;
    }
}