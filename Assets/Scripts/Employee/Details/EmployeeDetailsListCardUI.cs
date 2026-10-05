using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeDetailsListCardUI : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeIdText;
    [SerializeField] private TMP_Text employeeNameText;
    [SerializeField] private TMP_Text employeeRoleText;
    [SerializeField] private TMP_Text statusText;

    [Header("Selection")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;

    private string employeeId;
    private EmployeeDetailsPanelUI detailsPanel;

    public void Setup(
        Employee employee,
        EmployeeDetailsPanelUI panel)
    {
        if (employee == null)
        {
            return;
        }

        employeeId =
            employee.EmployeeId;

        detailsPanel =
            panel;

        if (employeeIdText != null)
        {
            employeeIdText.text =
                employee.EmployeeId;
        }

        if (employeeNameText != null)
        {
            employeeNameText.text =
                employee.EmployeeName;
        }

        if (employeeRoleText != null)
        {
            employeeRoleText.text =
                GetRoleName(employee.Role);
        }

        if (statusText != null)
        {
            statusText.text =
                employee.IsHired
                    ? "● Active"
                    : "● Inactive";
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();

            selectButton.onClick.AddListener(
                SelectEmployee
            );
        }
    }

    private void SelectEmployee()
    {
        if (detailsPanel == null)
        {
            return;
        }

        detailsPanel.SelectEmployee(
            employeeId
        );
    }

    private string GetRoleName(
        EmployeeRole role)
    {
        switch (role)
        {
            case EmployeeRole.Cashier:
                return "Cashier";

            case EmployeeRole.ShopAssistant:
                return "Assistant";

            case EmployeeRole.StockManager:
                return "Stock Manager";

            case EmployeeRole.SecurityGuard:
                return "Security";

            case EmployeeRole.Cleaner:
                return "Cleaner";

            default:
                return role.ToString();
        }
    }
}