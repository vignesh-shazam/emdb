using UnityEngine;

public class FinanceUI : MonoBehaviour
{
    public static FinanceUI Instance { get; private set; }

    [Header("Finance UI")]
    [SerializeField]
    private GameObject financePanel;

    [Header("Input")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.Q;

    public bool IsOpen =>
        financePanel != null &&
        financePanel.activeSelf;

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

        InitializeUI();
    }

    // =========================
    // INITIALIZE
    // =========================

    private void InitializeUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        financePanel.SetActive(false);
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFinanceUI();
        }
    }

    // =========================
    // TOGGLE
    // =========================

    public void ToggleFinanceUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        if (IsOpen)
        {
            CloseFinanceUI();
        }
        else
        {
            OpenFinanceUI();
        }
    }

    // =========================
    // OPEN
    // =========================

    public void OpenFinanceUI()
    {
        if (financePanel == null)
        {
            Debug.LogWarning(
                "FinanceUI: Finance Panel is not assigned."
            );

            return;
        }

        financePanel.SetActive(true);

        Debug.Log(
            "Finance UI opened."
        );
    }

    // =========================
    // CLOSE
    // =========================

    public void CloseFinanceUI()
    {
        if (financePanel == null)
        {
            return;
        }

        financePanel.SetActive(false);

        Debug.Log(
            "Finance UI closed."
        );
    }

    // =========================
    // SET VISIBILITY
    // =========================

    public void SetFinanceUIVisible(bool visible)
    {
        if (financePanel == null)
        {
            return;
        }

        financePanel.SetActive(visible);
    }
}