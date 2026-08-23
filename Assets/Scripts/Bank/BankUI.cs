using System.Text;
using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text accountNumberText;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_Text transactionHistoryText;

    private void Start()
    {
        RefreshUI();
    }

    public void Deposit()
    {
        if (!TryGetAmount(out int amount))
        {
            return;
        }

        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "BankUI: BankManager not found."
            );

            return;
        }

        bool success =
            BankManager.Instance.Deposit(amount);

        if (success)
        {
            ClearAmountInput();
            RefreshUI();
        }
    }

    public void Withdraw()
    {
        if (!TryGetAmount(out int amount))
        {
            return;
        }

        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "BankUI: BankManager not found."
            );

            return;
        }

        bool success =
            BankManager.Instance.Withdraw(amount);

        if (success)
        {
            ClearAmountInput();
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "BankUI: BankManager not found."
            );

            return;
        }

        if (accountNumberText != null)
        {
            accountNumberText.text =
                $"Account: {BankManager.Instance.AccountNumber}";
        }

        if (balanceText != null)
        {
            balanceText.text =
                $"Bank Balance: Rs. {BankManager.Instance.Balance:N0}";
        }

        RefreshTransactionHistory();
    }

    private void RefreshTransactionHistory()
    {
        if (transactionHistoryText == null)
        {
            return;
        }

        if (BankManager.Instance == null)
        {
            return;
        }

        var transactions =
            BankManager.Instance.Transactions;

        if (transactions == null ||
            transactions.Count == 0)
        {
            transactionHistoryText.text =
                "No transactions yet.";

            return;
        }

        StringBuilder history =
            new StringBuilder();

        for (int i = transactions.Count - 1;
             i >= 0;
             i--)
        {
            BankTransaction transaction =
                transactions[i];

            string transactionType =
                transaction.Type ==
                BankTransaction.TransactionType.Deposit
                    ? "Deposit"
                    : "Withdraw";

            string sign =
                transaction.Type ==
                BankTransaction.TransactionType.Deposit
                    ? "+"
                    : "-";

            history.AppendLine(
                $"{transactionType}  " +
                $"{sign}Rs. {transaction.Amount:N0}"
            );
        }

        transactionHistoryText.text =
            history.ToString();
    }

    private bool TryGetAmount(out int amount)
    {
        amount = 0;

        if (amountInput == null)
        {
            Debug.LogError(
                "BankUI: Amount input is not assigned."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(amountInput.text))
        {
            Debug.LogWarning(
                "BankUI: Please enter an amount."
            );

            return false;
        }

        if (!int.TryParse(
                amountInput.text,
                out amount))
        {
            Debug.LogWarning(
                "BankUI: Please enter a valid number."
            );

            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning(
                "BankUI: Amount must be greater than zero."
            );

            return false;
        }

        return true;
    }

    private void ClearAmountInput()
    {
        if (amountInput != null)
        {
            amountInput.text = string.Empty;
        }
    }
}