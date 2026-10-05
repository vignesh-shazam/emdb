using System;
using System.Collections.Generic;
using UnityEngine;

public class EmployeePayrollManager : MonoBehaviour
{
    public static EmployeePayrollManager Instance { get; private set; }

    [Header("Payroll Records")]
    [SerializeField]
    private List<EmployeePayroll> payrollRecords =
        new List<EmployeePayroll>();

    public IReadOnlyList<EmployeePayroll> PayrollRecords =>
        payrollRecords;

    public int PayrollRecordCount =>
        payrollRecords.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Duplicate EmployeePayrollManager found. " +
                "Destroying duplicate."
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log(
            "EmployeePayrollManager initialized successfully."
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // =========================================================
    // CREATE PAYROLL RECORD
    // =========================================================

    public bool CreatePayrollRecord(Employee employee)
    {
        if (employee == null)
        {
            Debug.LogWarning(
                "Cannot create payroll record. Employee is null."
            );

            return false;
        }

        if (HasPayrollRecord(employee.EmployeeId))
        {
            Debug.LogWarning(
                $"Payroll record already exists: " +
                $"{employee.EmployeeId}"
            );

            return false;
        }

        string employmentStartDate =
            DateTime.Now.ToString("dd-MM-yyyy");

        EmployeePayroll payroll =
            new EmployeePayroll(
                employee.EmployeeId,
                employee.EmployeeName,
                employee.MonthlySalary,
                employmentStartDate
            );

        payrollRecords.Add(payroll);

        Debug.Log(
            $"Payroll record created: " +
            $"{employee.EmployeeName} | " +
            $"ID: {employee.EmployeeId} | " +
            $"Salary: ₹{employee.MonthlySalary:N0} | " +
            $"Start Date: {employmentStartDate}"
        );

        return true;
    }

    // =========================================================
    // GET PAYROLL RECORD
    // =========================================================

    public EmployeePayroll GetPayrollRecord(
        string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        return payrollRecords.Find(
            payroll =>
                payroll != null &&
                payroll.EmployeeId == employeeId
        );
    }

    public bool HasPayrollRecord(
        string employeeId)
    {
        return GetPayrollRecord(employeeId) != null;
    }

    // =========================================================
    // RECORD EMPLOYEE LEAVING DATE
    // =========================================================

    public bool RecordEmployeeLeavingDate(
        string employeeId)
    {
        EmployeePayroll payroll =
            GetPayrollRecord(employeeId);

        if (payroll == null)
        {
            Debug.LogWarning(
                $"Payroll record not found for employee: " +
                $"{employeeId}"
            );

            return false;
        }

        if (!string.IsNullOrWhiteSpace(
            payroll.EmploymentEndDate))
        {
            Debug.LogWarning(
                $"Employment end date already recorded: " +
                $"{employeeId}"
            );

            return false;
        }

        DateTime leavingDate =
            DateTime.Now;

        string leavingDateText =
            leavingDate.ToString("dd-MM-yyyy");

        payroll.SetEmploymentEndDate(
            leavingDateText
        );

        CalculateProratedSalary(
            payroll,
            leavingDate
        );

        Debug.Log(
            $"Employee leaving date recorded: " +
            $"{payroll.EmployeeName} | " +
            $"ID: {payroll.EmployeeId} | " +
            $"End Date: {leavingDateText}"
        );

        Debug.Log(
            $"Final salary calculated: " +
            $"₹{payroll.CalculatedSalary:N2}"
        );

        return true;
    }

    // =========================================================
    // PRORATED SALARY FOR FIRED EMPLOYEE
    // =========================================================

    private void CalculateProratedSalary(
        EmployeePayroll payroll,
        DateTime endDate)
    {
        if (payroll == null)
        {
            return;
        }

        DateTime startDate;

        bool validStartDate =
            DateTime.TryParseExact(
                payroll.EmploymentStartDate,
                "dd-MM-yyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out startDate
            );

        if (!validStartDate)
        {
            Debug.LogWarning(
                $"Invalid employment start date: " +
                $"{payroll.EmploymentStartDate}"
            );

            return;
        }

        if (endDate.Date < startDate.Date)
        {
            Debug.LogWarning(
                $"Employee end date cannot be before " +
                $"start date: {payroll.EmployeeName}"
            );

            return;
        }

        // Payroll is calculated for the month in which
        // the employee leaves.
        DateTime monthStart =
            new DateTime(
                endDate.Year,
                endDate.Month,
                1
            );

        int daysInMonth =
            DateTime.DaysInMonth(
                endDate.Year,
                endDate.Month
            );

        // If the employee joined before this month,
        // salary starts from the first day of this month.
        DateTime effectiveStart =
            startDate > monthStart
                ? startDate
                : monthStart;

        int daysWorked =
            (endDate.Date -
             effectiveStart.Date).Days + 1;

        daysWorked =
            Mathf.Clamp(
                daysWorked,
                1,
                daysInMonth
            );

        payroll.SetDaysWorked(
            daysWorked
        );

        payroll.CalculateSalary(
            daysInMonth
        );

        Debug.Log(
            $"Prorated Payroll | " +
            $"Employee: {payroll.EmployeeName} | " +
            $"Month: {endDate:MM-yyyy} | " +
            $"Days Worked: {daysWorked}/{daysInMonth} | " +
            $"Monthly Salary: ₹{payroll.MonthlySalary:N2} | " +
            $"Daily Salary: ₹{payroll.DailySalary:N2} | " +
            $"Final Salary: ₹{payroll.CalculatedSalary:N2}"
        );
    }

    // =========================================================
    // PAYROLL DATE
    // =========================================================

    public DateTime GetCurrentMonthPayrollDate()
    {
        DateTime today =
            DateTime.Now;

        int lastDay =
            DateTime.DaysInMonth(
                today.Year,
                today.Month
            );

        return new DateTime(
            today.Year,
            today.Month,
            lastDay
        );
    }

    public bool IsTodayPayrollDay()
    {
        DateTime today =
            DateTime.Now;

        int lastDay =
            DateTime.DaysInMonth(
                today.Year,
                today.Month
            );

        return today.Day == lastDay;
    }

    // =========================================================
    // SALARY CALCULATION
    // =========================================================

    public int GetTotalMonthlySalary()
    {
        int totalSalary = 0;

        if (EmployeeManager.Instance == null)
        {
            return totalSalary;
        }

        foreach (EmployeePayroll payroll in payrollRecords)
        {
            if (payroll == null)
            {
                continue;
            }

            Employee employee =
                EmployeeManager.Instance.FindEmployee(
                    payroll.EmployeeId
                );

            if (employee != null &&
                employee.IsHired)
            {
                totalSalary +=
                    payroll.MonthlySalary;
            }
        }

        return totalSalary;
    }

    public float GetUnpaidSalaryTotal()
    {
        float unpaidSalary = 0f;

        foreach (EmployeePayroll payroll in payrollRecords)
        {
            if (payroll == null ||
                payroll.SalaryPaid)
            {
                continue;
            }

            unpaidSalary +=
                GetSalaryAmountForPayroll(
                    payroll
                );
        }

        return unpaidSalary;
    }

    private float GetSalaryAmountForPayroll(
        EmployeePayroll payroll)
    {
        if (payroll == null)
        {
            return 0f;
        }

        // Fired employee.
        if (!string.IsNullOrWhiteSpace(
            payroll.EmploymentEndDate))
        {
            return payroll.CalculatedSalary;
        }

        // Currently hired employee.
        return payroll.MonthlySalary;
    }

    // =========================================================
    // PROCESS MONTHLY PAYROLL
    // =========================================================

    public bool ProcessMonthlyPayroll()
    {
        DateTime today =
            DateTime.Now;

        int lastDay =
            DateTime.DaysInMonth(
                today.Year,
                today.Month
            );

        // Salary can only be paid on the last
        // calendar day of the month.
        if (today.Day != lastDay)
        {
            Debug.LogWarning(
                $"Payroll cannot be processed today. " +
                $"Salary payment date is " +
                $"{lastDay:00}-{today.Month:00}-{today.Year}."
            );

            return false;
        }

        if (EmployeeManager.Instance == null)
        {
            Debug.LogError(
                "EmployeeManager instance not found. " +
                "Cannot process payroll."
            );

            return false;
        }

        if (payrollRecords.Count == 0)
        {
            Debug.LogWarning(
                "No payroll records available."
            );

            return false;
        }

        DateTime paymentDate =
            new DateTime(
                today.Year,
                today.Month,
                lastDay
            );

        string paymentDateText =
            paymentDate.ToString("dd-MM-yyyy");

        int daysInMonth =
            DateTime.DaysInMonth(
                today.Year,
                today.Month
            );

        float totalSalaryPaid = 0f;
        int employeesPaid = 0;

        foreach (EmployeePayroll payroll in payrollRecords)
        {
            if (payroll == null ||
                payroll.SalaryPaid)
            {
                continue;
            }

            Employee employee =
                EmployeeManager.Instance.FindEmployee(
                    payroll.EmployeeId
                );

            if (employee == null)
            {
                continue;
            }

            float salaryAmount = 0f;

            if (employee.IsHired)
            {
                // -------------------------------------------------
                // CURRENTLY HIRED EMPLOYEE
                // -------------------------------------------------

                DateTime startDate;

                bool validStartDate =
                    DateTime.TryParseExact(
                        payroll.EmploymentStartDate,
                        "dd-MM-yyyy",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out startDate
                    );

                if (!validStartDate)
                {
                    Debug.LogWarning(
                        $"Invalid start date for: " +
                        $"{payroll.EmployeeName}"
                    );

                    continue;
                }

                DateTime monthStart =
                    new DateTime(
                        today.Year,
                        today.Month,
                        1
                    );

                DateTime effectiveStart =
                    startDate > monthStart
                        ? startDate
                        : monthStart;

                int daysWorked =
                    (paymentDate.Date -
                     effectiveStart.Date).Days + 1;

                daysWorked =
                    Mathf.Clamp(
                        daysWorked,
                        1,
                        daysInMonth
                    );

                payroll.SetDaysWorked(
                    daysWorked
                );

                payroll.CalculateSalary(
                    daysInMonth
                );

                salaryAmount =
                    payroll.CalculatedSalary;
            }
            else
            {
                // -------------------------------------------------
                // FIRED EMPLOYEE
                // -------------------------------------------------

                salaryAmount =
                    payroll.CalculatedSalary;
            }

            if (salaryAmount <= 0f)
            {
                continue;
            }

            payroll.MarkSalaryPaid(
                paymentDateText
            );

            totalSalaryPaid +=
                salaryAmount;

            employeesPaid++;

            Debug.Log(
                $"Salary paid | " +
                $"Employee: {payroll.EmployeeName} | " +
                $"Days: {payroll.DaysWorked} | " +
                $"Amount: ₹{salaryAmount:N2}"
            );
        }

        if (employeesPaid == 0)
        {
            Debug.LogWarning(
                "No unpaid employee salaries found."
            );

            return false;
        }

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "MONTHLY PAYROLL PROCESSED"
        );

        Debug.Log(
            $"Employees Paid: {employeesPaid}"
        );

        Debug.Log(
            $"Total Salary: ₹{totalSalaryPaid:N2}"
        );

        Debug.Log(
            $"Payment Date: {paymentDateText}"
        );

        Debug.Log(
            "========================================"
        );

        return true;
    }

    // =========================================================
    // RESET PAYROLL
    // =========================================================

    public void ResetAllSalaryStatuses()
    {
        foreach (EmployeePayroll payroll in payrollRecords)
        {
            if (payroll == null)
            {
                continue;
            }

            payroll.ResetSalaryStatus();
        }

        Debug.Log(
            "All employee salary statuses have been reset."
        );
    }

    // =========================================================
    // DEBUG / TEST
    // =========================================================

    [ContextMenu("TEST - Display Payroll")]
    private void TestDisplayPayroll()
    {
        if (payrollRecords.Count == 0)
        {
            Debug.Log(
                "No payroll records available."
            );

            return;
        }

        Debug.Log(
            $"Total Payroll Records: " +
            $"{payrollRecords.Count}"
        );

        Debug.Log(
            $"Current Hired Employee Salary: " +
            $"₹{GetTotalMonthlySalary():N0}"
        );

        Debug.Log(
            $"Current Unpaid Salary: " +
            $"₹{GetUnpaidSalaryTotal():N2}"
        );

        foreach (EmployeePayroll payroll in payrollRecords)
        {
            if (payroll == null)
            {
                continue;
            }

            Debug.Log(
                $"Employee: {payroll.EmployeeName} | " +
                $"ID: {payroll.EmployeeId} | " +
                $"Salary: ₹{payroll.MonthlySalary:N0} | " +
                $"Start: {payroll.EmploymentStartDate} | " +
                $"End: {payroll.EmploymentEndDate} | " +
                $"Days: {payroll.DaysWorked} | " +
                $"Daily: ₹{payroll.DailySalary:N2} | " +
                $"Calculated: ₹{payroll.CalculatedSalary:N2} | " +
                $"Paid: {payroll.SalaryPaid} | " +
                $"Payment: {payroll.PaymentDate}"
            );
        }
    }

    [ContextMenu("TEST - Process Monthly Payroll")]
    private void TestProcessMonthlyPayroll()
    {
        ProcessMonthlyPayroll();
    }

    [ContextMenu("TEST - Check Payroll Day")]
    private void TestCheckPayrollDay()
    {
        DateTime payrollDate =
            GetCurrentMonthPayrollDate();

        Debug.Log(
            $"Today: {DateTime.Now:dd-MM-yyyy}"
        );

        Debug.Log(
            $"Payroll Date: {payrollDate:dd-MM-yyyy}"
        );

        Debug.Log(
            $"Is Today Payroll Day: {IsTodayPayrollDay()}"
        );
    }

    [ContextMenu("TEST - Reset Salary Status")]
    private void TestResetSalaryStatus()
    {
        ResetAllSalaryStatuses();
    }
}