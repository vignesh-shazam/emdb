using TMPro;
using UnityEngine;

public class CustomerReputationUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text reputationLevelText;

    [SerializeField]
    private TMP_Text reputationNameText;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerReputationManager.OnReputationChanged +=
            RefreshUI;

        RefreshUI();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerReputationManager.OnReputationChanged -=
            RefreshUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        RefreshUI();
    }

    // =========================
    // REFRESH UI
    // =========================

    public void RefreshUI()
    {
        if (reputationLevelText == null &&
            reputationNameText == null)
        {
            return;
        }

        if (CustomerReputationManager.Instance == null)
        {
            if (reputationLevelText != null)
            {
                reputationLevelText.text =
                    "Level 1";
            }

            if (reputationNameText != null)
            {
                reputationNameText.text =
                    "Very Poor";
            }

            return;
        }

        int level =
            CustomerReputationManager.Instance
                .ReputationLevel;

        string reputation =
            CustomerReputationManager.Instance
                .ReputationName;

        if (reputationLevelText != null)
        {
            reputationLevelText.text =
                $"Level {level}";
        }

        if (reputationNameText != null)
        {
            reputationNameText.text =
                reputation;
        }
    }
}