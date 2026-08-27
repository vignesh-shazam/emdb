using System.Collections;
using TMPro;
using UnityEngine;

public class SatisfactionMilestoneUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField]
    private GameObject milestonePanel;

    [Header("Text")]
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text messageText;

    [Header("Display")]
    [SerializeField]
    private float displayDuration = 3f;

    private Coroutine hideCoroutine;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        CustomerSatisfactionManager.OnMaximumSatisfactionReached +=
            ShowMilestone;

        HideMilestone();
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        CustomerSatisfactionManager.OnMaximumSatisfactionReached -=
            ShowMilestone;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        SetupText();

        HideMilestone();
    }

    // =========================
    // SETUP TEXT
    // =========================

    private void SetupText()
    {
        if (titleText != null)
        {
            titleText.text =
                "⭐ EXCELLENT REPUTATION!";
        }

        if (messageText != null)
        {
            messageText.text =
                "Customer satisfaction reached 100!";
        }
    }

    // =========================
    // SHOW MILESTONE
    // =========================

    private void ShowMilestone()
    {
        if (milestonePanel == null)
        {
            Debug.LogWarning(
                "Satisfaction milestone UI failed: " +
                "Milestone panel is not assigned."
            );

            return;
        }

        if (titleText != null)
        {
            titleText.text =
                "⭐ EXCELLENT REPUTATION!";
        }

        if (messageText != null)
        {
            messageText.text =
                "Customer satisfaction reached 100!";
        }

        milestonePanel.SetActive(true);

        Debug.Log(
            "Satisfaction milestone UI displayed."
        );

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine =
            StartCoroutine(
                HideAfterDelay()
            );
    }

    // =========================
    // HIDE MILESTONE
    // =========================

    private void HideMilestone()
    {
        if (milestonePanel != null)
        {
            milestonePanel.SetActive(false);
        }
    }

    // =========================
    // HIDE AFTER DELAY
    // =========================

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(
            displayDuration
        );

        HideMilestone();

        hideCoroutine = null;

        Debug.Log(
            "Satisfaction milestone UI hidden."
        );
    }
}