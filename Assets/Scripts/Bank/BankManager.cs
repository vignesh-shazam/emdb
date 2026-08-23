using UnityEngine;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance { get; private set; }

    [Header("Bank Account")]
    [SerializeField] private string accountNumber = "EMDB001";
    [SerializeField] private int startingBalance = 0;

    private BankAccount bankAccount;

    public string AccountNumber =>
        bankAccount.AccountNumber;

    public int Balance =>
        bankAccount.Balance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeAccount();
    }

    private void InitializeAccount()
    {
        bankAccount = new BankAccount(
            accountNumber,
            startingBalance
        );

        Debug.Log(
            $"Bank initialized | " +
            $"Account: {bankAccount.AccountNumber} | " +
            $"Balance: Rs. {bankAccount.Balance:N0}"
        );
    }

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

        if (!MoneyManager.Instance.CanAfford(amount))
        {
            Debug.LogWarning(
                $"Deposit failed: Insufficient cash. " +
                $"Required: Rs. {amount:N0}"
            );

            return false;
        }

        MoneyManager.Instance.RemoveMoney(amount);

        bankAccount.Balance += amount;

        Debug.Log(
            $"Deposit successful | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Bank Balance: Rs. {bankAccount.Balance:N0}"
        );

        return true;
    }

    public bool Withdraw(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning(
                "Withdraw failed: Amount must be greater than zero."
            );

            return false;
        }

        if (bankAccount.Balance < amount)
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

        bankAccount.Balance -= amount;

        MoneyManager.Instance.AddMoney(amount);

        Debug.Log(
            $"Withdraw successful | " +
            $"Amount: Rs. {amount:N0} | " +
            $"Bank Balance: Rs. {bankAccount.Balance:N0}"
        );

        return true;
    }
}