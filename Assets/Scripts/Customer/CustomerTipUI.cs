using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerTipUI : MonoBehaviour
{
    [Header("Tips")]
    [SerializeField]
    private TMP_Text unclaimedTipsText;

    [SerializeField]
    private Button claimTipsButton;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerTipManager.OnTipsChanged +=
            RefreshUI;

        RefreshUI();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerTipManager.OnTipsChanged -=
            RefreshUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        InitializeUI();

        RefreshUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (claimTipsButton == null)
        {
            Debug.LogWarning(
                "CustomerTipUI: " +
                "Claim Tips button is not assigned."
            );

            return;
        }

        claimTipsButton.onClick.RemoveAllListeners();

        claimTipsButton.onClick.AddListener(
            ClaimTips
        );
    }

    // =========================
    // REFRESH UI
    // =========================

    public void RefreshUI()
    {
        if (CustomerTipManager.Instance == null)
        {
            if (unclaimedTipsText != null)
            {
                unclaimedTipsText.text =
                    "Unclaimed: Rs. 0";
            }

            if (claimTipsButton != null)
            {
                claimTipsButton.interactable =
                    false;
            }

            return;
        }

        int tips =
            CustomerTipManager.Instance
                .GetUnclaimedTips();

        // =========================
        // UPDATE TEXT
        // =========================

        if (unclaimedTipsText != null)
        {
            unclaimedTipsText.text =
                $"Unclaimed: Rs. {tips:N0}";
        }

        // =========================
        // UPDATE BUTTON
        // =========================

        if (claimTipsButton != null)
        {
            claimTipsButton.interactable =
                tips > 0;
        }
    }

    // =========================
    // CLAIM TIPS
    // =========================

    private void ClaimTips()
    {
        if (CustomerTipManager.Instance == null)
        {
            Debug.LogError(
                "Claim Tips failed: " +
                "CustomerTipManager not found."
            );

            return;
        }

        bool success =
            CustomerTipManager.Instance
                .ClaimTips();

        if (!success)
        {
            RefreshUI();

            return;
        }

        RefreshUI();

        Debug.Log(
            "CustomerTipUI: " +
            "Tips claimed successfully."
        );
    }
}