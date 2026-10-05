using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeCardUI : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeIdText;
    [SerializeField] private TMP_Text employeeNameText;
    [SerializeField] private TMP_Text employeeRoleText;
    [SerializeField] private TMP_Text employeeSalaryText;
    [SerializeField] private TMP_Text employeeProductivityText;
    [SerializeField] private TMP_Text employeeStatusText;

    [Header("Fire Button")]
    [SerializeField] private Button fireButton;
    [SerializeField] private TMP_Text fireButtonText;

    private Employee currentEmployee;

    public void SetEmployee(Employee employee)
    {
        if (employee == null)
        {
            Debug.LogWarning(
                "EmployeeCardUI: Employee data is null."
            );

            return;
        }

        currentEmployee = employee;

        // Employee ID
        if (employeeIdText != null)
        {
            employeeIdText.text =
                employee.EmployeeId;
        }

        // Employee Name
        if (employeeNameText != null)
        {
            employeeNameText.text =
                GetDisplayName(employee.EmployeeName);
        }

        // Employee Role
        if (employeeRoleText != null)
        {
            employeeRoleText.text =
                employee.Role.ToString();
        }

        // Salary
        if (employeeSalaryText != null)
        {
            employeeSalaryText.text =
                $"RS.{employee.MonthlySalary:N0}";
        }

        // Productivity
        if (employeeProductivityText != null)
        {
            employeeProductivityText.text =
                $"{employee.Productivity * 100f:0}%";
        }

        // Status
        if (employeeStatusText != null)
        {
            employeeStatusText.text =
                employee.IsHired
                    ? "Hired"
                    : "Fired";
        }

        SetupFireButton();
    }

    private string GetDisplayName(string employeeName)
    {
        if (string.IsNullOrEmpty(employeeName))
        {
            return string.Empty;
        }

        const int maximumCharacters = 15;

        if (employeeName.Length <= maximumCharacters)
        {
            return employeeName;
        }

        return employeeName.Substring(
            0,
            maximumCharacters
        ) + "...";
    }

    private void SetupFireButton()
    {
        if (fireButton == null)
        {
            return;
        }

        fireButton.onClick.RemoveListener(
            OnFireButtonClicked
        );

        fireButton.onClick.AddListener(
            OnFireButtonClicked
        );

        if (currentEmployee == null)
        {
            fireButton.interactable = false;

            if (fireButtonText != null)
            {
                fireButtonText.text = "Fire";
            }

            return;
        }

        if (currentEmployee.IsHired)
        {
            fireButton.interactable = true;

            if (fireButtonText != null)
            {
                fireButtonText.text = "Fire";
            }
        }
        else
        {
            fireButton.interactable = false;

            if (fireButtonText != null)
            {
                fireButtonText.text = "Fired";
            }
        }
    }

    private void OnFireButtonClicked()
    {
        if (currentEmployee == null)
        {
            return;
        }

        if (!currentEmployee.IsHired)
        {
            return;
        }

        if (EmployeeManager.Instance == null)
        {
            Debug.LogWarning(
                "EmployeeCardUI: EmployeeManager instance not found."
            );

            return;
        }

        bool fired =
            EmployeeManager.Instance.FireEmployee(
                currentEmployee.EmployeeId
            );

        if (fired)
        {
            Debug.Log(
                $"Employee fired from card: " +
                $"{currentEmployee.EmployeeName}"
            );
        }
    }
}