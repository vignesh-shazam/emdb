using TMPro;
using UnityEngine;

public class EmployeeCardUI : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeNameText;
    [SerializeField] private TMP_Text employeeRoleText;
    [SerializeField] private TMP_Text employeeSalaryText;
    [SerializeField] private TMP_Text employeeProductivityText;
    [SerializeField] private TMP_Text employeeStatusText;

    public void SetEmployee(Employee employee)
    {
        if (employee == null)
        {
            Debug.LogWarning("EmployeeCardUI: Employee data is null.");
            return;
        }

        employeeNameText.text = employee.EmployeeName;
        employeeRoleText.text = employee.Role.ToString();
        employeeSalaryText.text = $"rs. {employee.MonthlySalary:N0}";
        employeeProductivityText.text = $"{employee.Productivity * 100f:0}%";
        employeeStatusText.text = employee.IsHired ? "Hired" : "Not Hired";
    }
}