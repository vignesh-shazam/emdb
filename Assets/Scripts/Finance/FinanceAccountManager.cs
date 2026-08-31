using UnityEngine;

public class FinanceAccountManager : MonoBehaviour
{
    public static FinanceAccountManager Instance { get; private set; }

    [Header("Selected Finance Account")]
    [SerializeField]
    private FinanceAccountType accountType =
        FinanceAccountType.Savings;

    // =========================================================
    // ACCOUNT STATE
    // =========================================================

    public FinanceAccountType AccountType =>
        accountType;

    public bool IsSavingsAccount =>
        accountType == FinanceAccountType.Savings;

    public bool IsCurrentAccount =>
        accountType == FinanceAccountType.Current;

    // =========================================================
    // ACCOUNT DISPLAY NAME
    // =========================================================

    public string AccountDisplayName
    {
        get
        {
            switch (accountType)
            {
                case FinanceAccountType.Savings:
                    return "SAVINGS";

                case FinanceAccountType.Current:
                    return "CURRENT";

                default:
                    return "SAVINGS";
            }
        }
    }

    // =========================================================
    // ACCOUNT PURPOSE
    // =========================================================

    public string AccountPurpose
    {
        get
        {
            switch (accountType)
            {
                case FinanceAccountType.Savings:
                    return "Personal";

                case FinanceAccountType.Current:
                    return "Business";

                default:
                    return "Personal";
            }
        }
    }

    // =========================================================
    // SELECTED ACCOUNT BALANCE
    // =========================================================
    // IMPORTANT:
    // BankManager will be updated next to maintain:
    //
    // Savings Balance
    // Current Balance
    //
    // No Cash In Hand.
    // =========================================================

    public int SelectedAccountBalance
    {
        get
        {
            if (BankManager.Instance == null)
            {
                return 0;
            }

            return BankManager.Instance.GetBalance(
                accountType
            );
        }
    }

    // =========================================================
    // SAVINGS BALANCE
    // =========================================================

    public int SavingsBalance
    {
        get
        {
            if (BankManager.Instance == null)
            {
                return 0;
            }

            return BankManager.Instance.GetBalance(
                FinanceAccountType.Savings
            );
        }
    }

    // =========================================================
    // CURRENT BALANCE
    // =========================================================

    public int CurrentBalance
    {
        get
        {
            if (BankManager.Instance == null)
            {
                return 0;
            }

            return BankManager.Instance.GetBalance(FinanceAccountType.Current);
        }
    }

    // =========================================================
    // SAVINGS INCOME
    // =========================================================

    public int SavingsIncome
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalIncome(
                    FinanceAccountType.Savings
                );
        }
    }

    // =========================================================
    // SAVINGS EXPENSE
    // =========================================================

    public int SavingsExpense
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalExpense(
                    FinanceAccountType.Savings
                );
        }
    }

    // =========================================================
    // SAVINGS NET
    // =========================================================

    public int SavingsNet =>
        SavingsIncome -
        SavingsExpense;

    // =========================================================
    // CURRENT INCOME
    // =========================================================

    public int CurrentIncome
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalIncome(
                    FinanceAccountType.Current
                );
        }
    }

    // =========================================================
    // CURRENT EXPENSE
    // =========================================================

    public int CurrentExpense
    {
        get
        {
            if (FinanceTransactionManager.Instance == null)
            {
                return 0;
            }

            return FinanceTransactionManager.Instance
                .GetTotalExpense(
                    FinanceAccountType.Current
                );
        }
    }

    // =========================================================
    // CURRENT NET
    // =========================================================

    public int CurrentNet =>
        CurrentIncome -
        CurrentExpense;

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

        Debug.Log(
            $"Finance Account Manager initialized | " +
            $"Selected Account: {AccountDisplayName} | " +
            $"Purpose: {AccountPurpose}"
        );
    }

    // =========================================================
    // SWITCH TO SAVINGS
    // =========================================================

    public void SwitchToSavingsAccount()
    {
        accountType =
            FinanceAccountType.Savings;

        Debug.Log(
            "Finance account changed to SAVINGS."
        );
    }

    // =========================================================
    // SWITCH TO CURRENT
    // =========================================================

    public void SwitchToCurrentAccount()
    {
        accountType =
            FinanceAccountType.Current;

        Debug.Log(
            "Finance account changed to CURRENT."
        );
    }

    // =========================================================
    // SET ACCOUNT TYPE
    // =========================================================

    public void SetAccountType(
        FinanceAccountType type)
    {
        accountType = type;

        Debug.Log(
            $"Finance account changed to: " +
            $"{AccountDisplayName}"
        );
    }

    // =========================================================
    // SELECTED ACCOUNT BALANCE
    // =========================================================

    public int GetSelectedAccountBalance()
    {
        return SelectedAccountBalance;
    }

    // =========================================================
    // SAVINGS BALANCE
    // =========================================================

    public int GetSavingsBalance()
    {
        return SavingsBalance;
    }

    // =========================================================
    // CURRENT BALANCE
    // =========================================================

    public int GetCurrentBalance()
    {
        return CurrentBalance;
    }
}