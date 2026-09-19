using TMPro;
using UnityEngine;

public class CustomersatisfactionUI : MonoBehaviour
{
    [Header("Satisfaction UI")]
    [SerializeField]
    private TMP_Text satisfactionText;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomersatisfactionManager.OnSatisfactionChanged +=
            RefreshUI;

        RefreshUI();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomersatisfactionManager.OnSatisfactionChanged -=
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
        if (satisfactionText == null)
        {
            return;
        }

        if (CustomersatisfactionManager.Instance == null)
        {
            satisfactionText.text =
                "Customer Satisfaction: 0 / 100";

            return;
        }

        int satisfaction =
            CustomersatisfactionManager.Instance
                .Satisfaction;

        int maximumSatisfaction =
            CustomersatisfactionManager.Instance
                .MaximumSatisfaction;

        satisfactionText.text =
            $"Customer Satisfaction: " +
            $"{satisfaction} / {maximumSatisfaction}";
    }
}