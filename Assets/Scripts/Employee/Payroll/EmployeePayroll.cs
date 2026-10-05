using System;
using UnityEngine;

[Serializable]
public class EmployeePayroll
{
    [Header("Employee")]
    [SerializeField] private string employeeId;
    [SerializeField] private string employeeName;

    [Header("Salary")]
    [SerializeField] private int monthlySalary;

    [Header("Employment Period")]
    [SerializeField] private string employmentStartDate;
    [SerializeField] private string employmentEndDate;

    [Header("Payroll Calculation")]
    [SerializeField] private int daysWorked;
    [SerializeField] private float dailySalary;
    [SerializeField] private float calculatedSalary;

    [Header("Payment Status")]
    [SerializeField] private bool salaryPaid;
    [SerializeField] private string paymentDate;

    public string EmployeeId => employeeId;
    public string EmployeeName => employeeName;
    public int MonthlySalary => monthlySalary;

    public string EmploymentStartDate =>
        employmentStartDate;

    public string EmploymentEndDate =>
        employmentEndDate;

    public int DaysWorked =>
        daysWorked;

    public float DailySalary =>
        dailySalary;

    public float CalculatedSalary =>
        calculatedSalary;

    public bool SalaryPaid =>
        salaryPaid;

    public string PaymentDate =>
        paymentDate;

    public EmployeePayroll(
        string id,
        string name,
        int salary,
        string startDate)
    {
        employeeId = id;
        employeeName = name;
        monthlySalary = Mathf.Max(0, salary);

        employmentStartDate = startDate;
        employmentEndDate = string.Empty;

        daysWorked = 0;
        dailySalary = 0f;
        calculatedSalary = 0f;

        salaryPaid = false;
        paymentDate = string.Empty;
    }

    public void SetEmploymentEndDate(
        string endDate)
    {
        employmentEndDate = endDate;
    }

    public void CalculateSalary(
        int totalDaysInMonth)
    {
        if (totalDaysInMonth <= 0)
        {
            daysWorked = 0;
            dailySalary = 0f;
            calculatedSalary = 0f;

            return;
        }

        dailySalary =
            (float)monthlySalary /
            totalDaysInMonth;

        daysWorked =
            Mathf.Clamp(
                daysWorked,
                0,
                totalDaysInMonth
            );

        calculatedSalary =
            dailySalary * daysWorked;
    }

    public void SetDaysWorked(
        int value)
    {
        daysWorked =
            Mathf.Max(0, value);
    }

    public void MarkSalaryPaid(
        string date)
    {
        salaryPaid = true;
        paymentDate = date;
    }

    public void ResetSalaryStatus()
    {
        salaryPaid = false;
        paymentDate = string.Empty;
        calculatedSalary = 0f;
        daysWorked = 0;
    }

    public void UpdateSalary(
        int salary)
    {
        monthlySalary =
            Mathf.Max(0, salary);
    }
}