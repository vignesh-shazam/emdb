using TMPro;
using UnityEngine;

public class CustomerSatisfactionUI : MonoBehaviour
{
    [Header("Satisfaction UI")]
    [SerializeField]
    private TMP_Text satisfactionText;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged +=
            RefreshUI;

        RefreshUI();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerSatisfactionManager.OnSatisfactionChanged -=
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

        if (CustomerSatisfactionManager.Instance == null)
        {
            satisfactionText.text =
                "Customer Satisfaction: 0 / 100";

            return;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .Satisfaction;

        int maximumSatisfaction =
            CustomerSatisfactionManager.Instance
                .MaximumSatisfaction;

        satisfactionText.text =
            $"Customer Satisfaction: " +
            $"{satisfaction} / {maximumSatisfaction}";
    }
}