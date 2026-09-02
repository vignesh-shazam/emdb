using System.Text;
using TMPro;
using UnityEngine;

public class BankUI : MonoBehaviour
{
    // =========================================================
    // UI REFERENCES
    // =========================================================

    [Header("UI References")]
    [SerializeField]
    private TMP_Text accountNumberText;

    [SerializeField]
    private TMP_Text balanceText;

    [SerializeField]
    private TMP_InputField amountInput;

    [SerializeField]
    private TMP_Text transactionHistoryText;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        RefreshUI();
    }

    // =========================================================
    // DEPOSIT
    // =========================================================
    // Uses Savings Account by default.
    // BankManager.Deposit(int) defaults to Savings.

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
            BankManager.Instance.Deposit(
                amount
            );

        if (success)
        {
            ClearAmountInput();

            RefreshUI();
        }
    }

    // =========================================================
    // WITHDRAW
    // =========================================================
    // Uses Savings Account by default.
    // BankManager.Withdraw(int) defaults to Savings.

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
            BankManager.Instance.Withdraw(
                amount
            );

        if (success)
        {
            ClearAmountInput();

            RefreshUI();
        }
    }

    // =========================================================
    // REFRESH UI
    // =========================================================

    public void RefreshUI()
    {
        if (BankManager.Instance == null)
        {
            Debug.LogError(
                "BankUI: BankManager not found."
            );

            return;
        }

        // =====================================================
        // SAVINGS ACCOUNT NUMBER
        // =====================================================

        if (accountNumberText != null)
        {
            accountNumberText.text =
                $"Account: " +
                $"{BankManager.Instance.SavingsAccountNumber}";
        }

        // =====================================================
        // SAVINGS BALANCE
        // =====================================================

        if (balanceText != null)
        {
            balanceText.text =
                $"Bank Balance: Rs. " +
                $"{BankManager.Instance.SavingsBalance:N0}";
        }

        // =====================================================
        // TRANSACTION HISTORY
        // =====================================================

        RefreshTransactionHistory();
    }

    // =========================================================
    // TRANSACTION HISTORY
    // =========================================================

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

        for (
            int i = transactions.Count - 1;
            i >= 0;
            i--)
        {
            BankTransaction transaction =
                transactions[i];

            string transactionType;

            string sign;

            // =================================================
            // DEPOSIT
            // =================================================

            if (
                transaction.Type ==
                BankTransaction.TransactionType.Deposit)
            {
                transactionType =
                    "Deposit";

                sign = "+";
            }

            // =================================================
            // EMI
            // =================================================

            else if (
                transaction.Type ==
                BankTransaction.TransactionType.EmiDebit)
            {
                transactionType =
                    "EMI";

                sign = "-";
            }

            // =================================================
            // BOUNCE CHARGE
            // =================================================

            else if (
                transaction.Type ==
                BankTransaction.TransactionType.BounceCharge)
            {
                transactionType =
                    "Bounce Charge";

                sign = "-";
            }

            // =================================================
            // WITHDRAW
            // =================================================

            else
            {
                transactionType =
                    "Withdraw";

                sign = "-";
            }

            history.AppendLine(
                $"{transactionType}  " +
                $"{sign}Rs. {transaction.Amount:N0}"
            );
        }

        transactionHistoryText.text =
            history.ToString();
    }

    // =========================================================
    // TRY GET AMOUNT
    // =========================================================

    private bool TryGetAmount(
        out int amount)
    {
        amount = 0;

        if (amountInput == null)
        {
            Debug.LogError(
                "BankUI: Amount input is not assigned."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                amountInput.text))
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

    // =========================================================
    // CLEAR AMOUNT INPUT
    // =========================================================

    private void ClearAmountInput()
    {
        if (amountInput != null)
        {
            amountInput.text =
                string.Empty;
        }
    }
}