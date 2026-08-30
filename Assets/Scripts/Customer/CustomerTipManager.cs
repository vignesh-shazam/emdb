using System;
using UnityEngine;

public class CustomerTipManager : MonoBehaviour
{
    public static CustomerTipManager Instance { get; private set; }

    public static event Action OnTipsChanged;

    [Header("Tips")]
    [SerializeField]
    private int unclaimedTips = 0;

    [Header("Tip Settings")]
    [SerializeField]
    private int minimumTip = 5;

    [SerializeField]
    private int maximumTip = 20;

    public int UnclaimedTips =>
        unclaimedTips;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeTips();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeTips()
    {
        if (minimumTip < 0)
        {
            minimumTip = 0;
        }

        if (maximumTip < minimumTip)
        {
            maximumTip = minimumTip;
        }

        if (unclaimedTips < 0)
        {
            unclaimedTips = 0;
        }

        Debug.Log(
            $"Customer Tip Manager initialized | " +
            $"Unclaimed Tips: Rs. {unclaimedTips:N0}"
        );

        OnTipsChanged?.Invoke();
    }

    // =========================
    // ADD TIP
    // =========================

    public void AddTip(Customer customer)
    {
        if (customer == null)
        {
            Debug.LogWarning(
                "Tip failed: Customer is null."
            );

            return;
        }

        if (!customer.WasServed())
        {
            Debug.LogWarning(
                $"Tip failed: Customer was not served. " +
                $"Customer: {customer.CustomerName}"
            );

            return;
        }

        int tipAmount =
            GenerateTipAmount();

        if (tipAmount <= 0)
        {
            Debug.Log(
                $"No tip generated | " +
                $"Customer: {customer.CustomerName}"
            );

            return;
        }

        unclaimedTips += tipAmount;

        Debug.Log(
            $"Tip generated | " +
            $"Customer: {customer.CustomerName} | " +
            $"Tip: Rs. {tipAmount:N0} | " +
            $"Unclaimed Tips: Rs. {unclaimedTips:N0}"
        );

        OnTipsChanged?.Invoke();
    }

    // =========================
    // GENERATE TIP
    // =========================

    private int GenerateTipAmount()
    {
        if (maximumTip <= minimumTip)
        {
            return minimumTip;
        }

        return UnityEngine.Random.Range(
            minimumTip,
            maximumTip + 1
        );
    }

    // =========================
    // GET TIPS
    // =========================

    public int GetUnclaimedTips()
    {
        return unclaimedTips;
    }

    // =========================
    // CLAIM TIPS
    // =========================

    public bool ClaimTips()
    {
        if (unclaimedTips <= 0)
        {
            Debug.LogWarning(
                "Claim tips failed: " +
                "No unclaimed tips available."
            );

            return false;
        }

        if (MoneyManager.Instance == null)
        {
            Debug.LogError(
                "Claim tips failed: " +
                "MoneyManager not found."
            );

            return false;
        }

        int claimedAmount =
            unclaimedTips;

        // =========================
        // ADD MONEY
        // =========================

        MoneyManager.Instance.AddMoney(
            claimedAmount
        );

        // =========================
        // RECORD SHOP INCOME
        // =========================

        RecordClaimedTip(
            claimedAmount
        );

        // =========================
        // CLEAR UNCLAIMED TIPS
        // =========================

        unclaimedTips = 0;

        Debug.Log(
            $"Tips claimed successfully | " +
            $"Amount: Rs. {claimedAmount:N0} | " +
            $"Unclaimed Tips: Rs. 0"
        );

        OnTipsChanged?.Invoke();

        return true;
    }

    // =========================
    // RECORD CLAIMED TIP
    // =========================

    private void RecordClaimedTip(
        int amount)
    {
        if (FinanceTransactionManager.Instance == null)
        {
            Debug.LogWarning(
                "CustomerTipManager: " +
                "FinanceTransactionManager not found. " +
                "Tip ledger entry skipped."
            );

            return;
        }

        FinanceTransactionManager.Instance.RecordIncome(
            FinanceAccountType.Shop,
            amount,
            "Tip Claimed"
        );

        Debug.Log(
            $"Tip claimed recorded | " +
            $"Account: Shop | " +
            $"Amount: Rs. {amount:N0}"
        );
    }

    // =========================
    // RESET TIPS
    // =========================

    public void ResetTips()
    {
        unclaimedTips = 0;

        Debug.Log(
            "Unclaimed tips reset | " +
            "Tips: Rs. 0"
        );

        OnTipsChanged?.Invoke();
    }
}