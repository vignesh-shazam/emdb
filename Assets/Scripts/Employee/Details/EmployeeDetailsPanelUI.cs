using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeDetailsPanelUI : MonoBehaviour
{
    public static EmployeeDetailsPanelUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Employee List")]
    [SerializeField] private Transform employeeList;
    [SerializeField] private GameObject employeeListCardPrefab;

    [Header("Selected Employee")]
    [SerializeField] private TMP_Text selectedEmployeeIdText;
    [SerializeField] private TMP_Text selectedEmployeeNameText;
    [SerializeField] private TMP_Text selectedEmployeeRoleText;
    [SerializeField] private TMP_Text selectedEmployeeStatusText;
    [SerializeField] private TMP_Text selectedEmployeeHireDateText;

    [Header("Employment Information")]
    [SerializeField] private TMP_Text employmentStatusText;
    [SerializeField] private TMP_Text employmentRoleText;
    [SerializeField] private TMP_Text monthlySalaryText;
    [SerializeField] private TMP_Text startDateText;

    [Header("Attendance")]
    [SerializeField] private TMP_Text presentDaysText;
    [SerializeField] private TMP_Text absentDaysText;
    [SerializeField] private TMP_Text todayStatusText;
    [SerializeField] private TMP_Text attendanceRateText;

    [Header("Performance")]
    [SerializeField] private TMP_Text performanceText;
    [SerializeField] private TMP_Text productivityText;
    [SerializeField] private TMP_Text tasksCompletedText;
    [SerializeField] private TMP_Text daysWorkedText;
    [SerializeField] private TMP_Text ratingText;

    [Header("Performance Bars")]
    [SerializeField] private Slider performanceSlider;
    [SerializeField] private Slider productivitySlider;

    [Header("Payroll")]
    [SerializeField] private TMP_Text payrollSalaryText;
    [SerializeField] private TMP_Text payrollPaidDateText;
    [SerializeField] private TMP_Text payrollPaidStatusText;
    [SerializeField] private TMP_Text payrollNextPaymentText;

    [Header("Action Buttons")]
    [SerializeField] private Button editButton;
    [SerializeField] private Button fireButton;

    private string selectedEmployeeId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (panel == null)
        {
            panel = gameObject;
        }

        ClosePanel();
    }

    // ==================================================
    // OPEN PANEL
    // ==================================================

    public void OpenPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        RefreshEmployeeList();

        if (!string.IsNullOrWhiteSpace(selectedEmployeeId))
        {
            SelectEmployee(selectedEmployeeId);
        }
        else
        {
            SelectFirstEmployee();
        }
    }

    // ==================================================
    // CLOSE PANEL
    // ==================================================

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    // ==================================================
    // REFRESH EMPLOYEE LIST
    // ==================================================

    public void RefreshEmployeeList()
    {
        if (employeeList == null)
        {
            Debug.LogWarning(
                "EmployeeDetailsPanelUI: Employee List is not assigned."
            );

            return;
        }

        if (employeeListCardPrefab == null)
        {
            Debug.LogWarning(
                "EmployeeDetailsPanelUI: Employee List Card Prefab is not assigned."
            );

            return;
        }

        for (int i = employeeList.childCount - 1; i >= 0; i--)
        {
            Destroy(employeeList.GetChild(i).gameObject);
        }

        if (EmployeeManager.Instance == null)
        {
            return;
        }

        foreach (Employee employee in EmployeeManager.Instance.Employees)
        {
            if (employee == null)
            {
                continue;
            }

            GameObject card =
                Instantiate(
                    employeeListCardPrefab,
                    employeeList
                );

            EmployeeDetailsListCardUI cardUI =
                card.GetComponent<EmployeeDetailsListCardUI>();

            if (cardUI != null)
            {
                cardUI.Setup(
                    employee,
                    this
                );
            }
        }
    }

    // ==================================================
    // SELECT EMPLOYEE
    // ==================================================

    public void SelectEmployee(string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return;
        }

        if (EmployeeManager.Instance == null)
        {
            return;
        }

        Employee employee =
            EmployeeManager.Instance.FindEmployee(
                employeeId
            );

        if (employee == null)
        {
            Debug.LogWarning(
                $"EmployeeDetailsPanelUI: Employee {employeeId} not found."
            );

            return;
        }

        selectedEmployeeId = employeeId;

        DisplayEmployee(employee);
    }

    // ==================================================
    // SELECT FIRST EMPLOYEE
    // ==================================================

    private void SelectFirstEmployee()
    {
        if (EmployeeManager.Instance == null)
        {
            return;
        }

        if (EmployeeManager.Instance.Employees.Count == 0)
        {
            ClearDetails();
            return;
        }

        foreach (Employee employee
                 in EmployeeManager.Instance.Employees)
        {
            if (employee != null)
            {
                SelectEmployee(employee.EmployeeId);
                return;
            }
        }

        ClearDetails();
    }

    // ==================================================
    // DISPLAY EMPLOYEE
    // ==================================================

    private void DisplayEmployee(
        Employee employee)
    {
        // --------------------------------------------------
        // BASIC INFORMATION
        // --------------------------------------------------

        SetText(
            selectedEmployeeIdText,
            employee.EmployeeId
        );

        SetText(
            selectedEmployeeNameText,
            employee.EmployeeName
        );

        SetText(
            selectedEmployeeRoleText,
            GetRoleName(employee.Role)
        );

        SetText(
            selectedEmployeeStatusText,
            employee.IsHired ? "Active" : "Inactive"
        );

        // --------------------------------------------------
        // EMPLOYMENT
        // --------------------------------------------------

        SetText(
            employmentStatusText,
            employee.IsHired ? "Hired" : "Inactive"
        );

        SetText(
            employmentRoleText,
            GetRoleName(employee.Role)
        );

        SetText(
            monthlySalaryText,
            FormatCurrency(employee.MonthlySalary)
        );

        string startDate = GetEmploymentStartDate(
            employee.EmployeeId
        );

        SetText(
            startDateText,
            startDate
        );

        SetText(
            selectedEmployeeHireDateText,
            startDate
        );

        // --------------------------------------------------
        // ATTENDANCE
        // --------------------------------------------------

        int presentDays =
            GetPresentDays(employee.EmployeeId);

        int absentDays =
            GetAbsentDays(employee.EmployeeId);

        int totalAttendanceDays =
            presentDays + absentDays;

        float attendanceRate = 0f;

        if (totalAttendanceDays > 0)
        {
            attendanceRate =
                ((float)presentDays /
                 totalAttendanceDays) * 100f;
        }

        SetText(
            presentDaysText,
            presentDays.ToString()
        );

        SetText(
            absentDaysText,
            absentDays.ToString()
        );

        SetText(
            todayStatusText,
            GetTodayAttendanceStatus(
                employee.EmployeeId
            )
        );

        SetText(
            attendanceRateText,
            $"{attendanceRate:F0}%"
        );

        // --------------------------------------------------
        // PERFORMANCE
        // --------------------------------------------------

        EmployeePerformance performance =
            GetPerformance(
                employee.EmployeeId
            );

        if (performance != null)
        {
            SetText(
                performanceText,
                $"{performance.PerformanceScore:F0}%"
            );

            SetText(
                productivityText,
                $"{performance.ProductivityScore:F0}%"
            );

            SetText(
                tasksCompletedText,
                $"{performance.TasksCompleted} / " +
                $"{performance.TasksAssigned}"
            );

            SetText(
                daysWorkedText,
                performance.DaysWorked.ToString()
            );

            SetText(
                ratingText,
                GetRatingName(performance.Rating)
            );

            if (performanceSlider != null)
            {
                performanceSlider.value =
                    performance.PerformanceScore / 100f;
            }

            if (productivitySlider != null)
            {
                productivitySlider.value =
                    performance.ProductivityScore / 100f;
            }
        }
        else
        {
            ClearPerformance();
        }

        // --------------------------------------------------
        // PAYROLL
        // --------------------------------------------------

        DisplayPayroll(
            employee
        );
    }

    // ==================================================
    // PERFORMANCE
    // ==================================================

    private EmployeePerformance GetPerformance(
        string employeeId)
    {
        if (EmployeePerformanceManager.Instance == null)
        {
            return null;
        }

        return EmployeePerformanceManager.Instance
            .GetPerformanceRecord(employeeId);
    }

    private void ClearPerformance()
    {
        SetText(performanceText, "--");
        SetText(productivityText, "--");
        SetText(tasksCompletedText, "--");
        SetText(daysWorkedText, "--");
        SetText(ratingText, "--");

        if (performanceSlider != null)
        {
            performanceSlider.value = 0f;
        }

        if (productivitySlider != null)
        {
            productivitySlider.value = 0f;
        }
    }

    // ==================================================
    // ATTENDANCE
    // ==================================================

    private int GetPresentDays(
        string employeeId)
    {
        if (EmployeeAttendanceManager.Instance == null)
        {
            return 0;
        }

        return EmployeeAttendanceManager.Instance
            .GetPresentDays(employeeId);
    }

    private int GetAbsentDays(
        string employeeId)
    {
        if (EmployeeAttendanceManager.Instance == null)
        {
            return 0;
        }

        return EmployeeAttendanceManager.Instance
            .GetAbsentDays(employeeId);
    }

    private string GetTodayAttendanceStatus(
        string employeeId)
    {
        if (EmployeeAttendanceManager.Instance == null)
        {
            return "--";
        }

        EmployeeAttendance attendance =
     EmployeeAttendanceManager.Instance
         .GetAttendanceRecord(
             employeeId,
             DateTime.Now.ToString("yyyy-MM-dd")
         );

        if (attendance == null)
        {
            return "Not Marked";
        }

        return attendance.IsPresent
            ? "Present"
            : "Absent";
    }

    // ==================================================
    // PAYROLL
    // ==================================================

    private void DisplayPayroll(
        Employee employee)
    {
        if (EmployeePayrollManager.Instance == null)
        {
            ClearPayroll();
            return;
        }

        EmployeePayroll payroll =
            EmployeePayrollManager.Instance
                .GetPayrollRecord(
                    employee.EmployeeId
                );

        if (payroll == null)
        {
            ClearPayroll();
            return;
        }

        SetText(
            payrollSalaryText,
            FormatCurrency(
                payroll.MonthlySalary
            )
        );

        SetText(
            payrollPaidDateText,
            string.IsNullOrWhiteSpace(
                payroll.PaymentDate
            )
                ? "--"
                : payroll.PaymentDate
        );

        SetText(
            payrollPaidStatusText,
            payroll.SalaryPaid
                ? "Paid"
                : "Pending"
        );

        SetText(
            payrollNextPaymentText,
            GetNextPaymentDate()
        );
    }

    private void ClearPayroll()
    {
        SetText(payrollSalaryText, "--");
        SetText(payrollPaidDateText, "--");
        SetText(payrollPaidStatusText, "--");
        SetText(payrollNextPaymentText, "--");
    }

    // ==================================================
    // EMPLOYMENT DATE
    // ==================================================

    private string GetEmploymentStartDate(
        string employeeId)
    {
        if (EmployeePayrollManager.Instance == null)
        {
            return "--";
        }

        EmployeePayroll payroll =
            EmployeePayrollManager.Instance
                .GetPayrollRecord(
                    employeeId
                );

        if (payroll == null)
        {
            return "--";
        }

        return payroll.EmploymentStartDate;
    }

    private string GetNextPaymentDate()
    {
        DateTime today = DateTime.Now;

        DateTime nextPayment =
            new DateTime(
                today.Year,
                today.Month,
                1
            ).AddMonths(1);

        return nextPayment.ToString(
            "dd MMM yyyy"
        );
    }

    // ==================================================
    // FIRE EMPLOYEE
    // ==================================================

    public void FireSelectedEmployee()
    {
        if (string.IsNullOrWhiteSpace(
                selectedEmployeeId))
        {
            return;
        }

        if (EmployeeManager.Instance == null)
        {
            return;
        }

        bool result =
            EmployeeManager.Instance.FireEmployee(
                selectedEmployeeId
            );

        if (result)
        {
            RefreshEmployeeList();

            Employee employee =
                EmployeeManager.Instance.FindEmployee(
                    selectedEmployeeId
                );

            if (employee != null)
            {
                DisplayEmployee(employee);
            }
        }
    }

    // ==================================================
    // EDIT EMPLOYEE
    // ==================================================

    public void EditSelectedEmployee()
    {
        Debug.Log(
            $"Edit employee requested: {selectedEmployeeId}"
        );

        // We will connect this to the existing
        // Employee Edit/Hiring UI later.
    }

    // ==================================================
    // HELPERS
    // ==================================================

    private void ClearDetails()
    {
        SetText(selectedEmployeeIdText, "--");
        SetText(selectedEmployeeNameText, "--");
        SetText(selectedEmployeeRoleText, "--");
        SetText(selectedEmployeeStatusText, "--");
        SetText(selectedEmployeeHireDateText, "--");

        SetText(employmentStatusText, "--");
        SetText(employmentRoleText, "--");
        SetText(monthlySalaryText, "--");
        SetText(startDateText, "--");

        SetText(presentDaysText, "--");
        SetText(absentDaysText, "--");
        SetText(todayStatusText, "--");
        SetText(attendanceRateText, "--");

        ClearPerformance();
        ClearPayroll();
    }

    private void SetText(
        TMP_Text target,
        string value)
    {
        if (target != null)
        {
            target.text = value;
        }
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

    private string GetRatingName(
        EmployeePerformanceRating rating)
    {
        switch (rating)
        {
            case EmployeePerformanceRating.Excellent:
                return "Excellent";

            case EmployeePerformanceRating.Good:
                return "Good";

            case EmployeePerformanceRating.Average:
                return "Average";

            case EmployeePerformanceRating.Poor:
                return "Poor";

            case EmployeePerformanceRating.Critical:
                return "Critical";

            default:
                return rating.ToString();
        }
    }

    private string FormatCurrency(
        int amount)
    {
        return $"₹ {amount:N0}";
    }
}