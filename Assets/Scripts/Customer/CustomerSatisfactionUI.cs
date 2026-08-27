using TMPro;
using UnityEngine;

public class CustomerSatisfactionUI : MonoBehaviour
{
    [Header("Satisfaction")]
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
                "Customer Satisfaction: 0";

            return;
        }

        int satisfaction =
            CustomerSatisfactionManager.Instance
                .GetSatisfaction();

        satisfactionText.text =
            $"Customer Satisfaction: {satisfaction}";
    }
}