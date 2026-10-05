using TMPro;
using UnityEngine;

public class PerformanceCardUI : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeIdText;
    [SerializeField] private TMP_Text employeeNameText;
    [SerializeField] private TMP_Text employeeRoleText;

    [Header("Performance Information")]
    [SerializeField] private TMP_Text performanceText;
    [SerializeField] private TMP_Text productivityText;
    [SerializeField] private TMP_Text tasksText;
    [SerializeField] private TMP_Text daysWorkedText;
    [SerializeField] private TMP_Text ratingText;

    public void SetPerformance(
        EmployeePerformance performance)
    {
        if (performance == null)
        {
            Debug.LogWarning(
                "PerformanceCardUI: Performance data is null."
            );

            return;
        }

        // Employee information
        if (employeeIdText != null)
        {
            employeeIdText.text =
                performance.EmployeeId;
        }

        if (employeeNameText != null)
        {
            employeeNameText.text =
                performance.EmployeeName;
        }

        if (employeeRoleText != null)
        {
            employeeRoleText.text =
                GetEmployeeRole(
                    performance.EmployeeId
                );
        }

        // Performance
        if (performanceText != null)
        {
            performanceText.text =
                $"{performance.PerformanceScore:F0}%";
        }

        if (productivityText != null)
        {
            productivityText.text =
                $"{performance.ProductivityScore:F0}%";
        }

        // Tasks
        if (tasksText != null)
        {
            tasksText.text =
                $"{performance.TasksCompleted}/" +
                $"{performance.TasksAssigned}";
        }

        // Days worked
        if (daysWorkedText != null)
        {
            daysWorkedText.text =
                performance.DaysWorked.ToString();
        }

        // Rating
        if (ratingText != null)
        {
            ratingText.text =
                performance.Rating.ToString();
        }
    }

    private string GetEmployeeRole(
        string employeeId)
    {
        if (EmployeeManager.Instance == null)
        {
            return "--";
        }

        Employee employee =
            EmployeeManager.Instance
                .FindEmployee(employeeId);

        if (employee == null)
        {
            return "--";
        }

        return employee.Role.ToString();
    }
}