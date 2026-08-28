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
            UpdateUI;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerReputationManager.OnReputationChanged -=
            UpdateUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        UpdateUI();
    }

    // =========================
    // UPDATE UI
    // =========================

    private void UpdateUI()
    {
        if (CustomerReputationManager.Instance == null)
        {
            return;
        }

        int level =
            CustomerReputationManager.Instance
                .GetReputationLevel();

        string reputation =
            CustomerReputationManager.Instance
                .GetReputationName();

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