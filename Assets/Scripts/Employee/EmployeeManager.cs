using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager Instance { get; private set; }

    [Header("Employee Settings")]
    [SerializeField] private int maximumEmployees = 10;

    [Header("Employee ID")]
    [SerializeField] private int nextEmployeeNumber = 1;

    [Header("Employees")]
    [SerializeField]
    private List<Employee> employees =
        new List<Employee>();

    [Header("Employee UI")]
    [SerializeField] private Transform employeeList;
    [SerializeField] private EmployeeCardUI employeeCardPrefab;

    public IReadOnlyList<Employee> Employees =>
        employees;

    // Total employee records, including fired employees.
    public int EmployeeCount =>
        employees.Count;

    // Currently hired employees only.
    public int ActiveEmployeeCount
    {
        get
        {
            int count = 0;

            foreach (Employee employee in employees)
            {
                if (employee != null &&
                    employee.IsHired)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int MaximumEmployees =>
        maximumEmployees;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Duplicate EmployeeManager found. " +
                "Destroying duplicate."
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;

        UpdateNextEmployeeNumber();

        Debug.Log(
            "EmployeeManager initialized successfully."
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
    // HIRE EMPLOYEE
    // =========================================================

    public bool HireEmployee(
        string employeeId,
        string employeeName,
        EmployeeRole role,
        int monthlySalary)
    {
        // Maximum limit applies only to currently hired employees.
        if (ActiveEmployeeCount >= maximumEmployees)
        {
            Debug.LogWarning(
                "Cannot hire employee. " +
                "Maximum active employee limit reached."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            Debug.LogWarning(
                "Cannot hire employee. Employee ID is empty."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning(
                "Cannot hire employee. Employee name is empty."
            );

            return false;
        }

        if (EmployeeIdExists(employeeId))
        {
            Debug.LogWarning(
                $"Employee ID already exists: {employeeId}"
            );

            return false;
        }

        Employee newEmployee =
            new Employee(
                employeeId,
                employeeName,
                role,
                monthlySalary
            );

        employees.Add(newEmployee);

        Debug.Log(
            $"Employee hired: {employeeName} | " +
            $"ID: {employeeId} | " +
            $"Role: {role} | " +
            $"Salary: ₹{monthlySalary:N0}"
        );

        CreatePayrollRecord(newEmployee);

        RefreshEmployeeUI();

        return true;
    }

    // =========================================================
    // CREATE PAYROLL RECORD
    // =========================================================

    private void CreatePayrollRecord(
        Employee employee)
    {
        if (EmployeePayrollManager.Instance == null)
        {
            Debug.LogWarning(
                "EmployeePayrollManager instance not found. " +
                "Payroll record was not created."
            );

            return;
        }

        bool created =
            EmployeePayrollManager.Instance
                .CreatePayrollRecord(employee);

        if (created)
        {
            Debug.Log(
                $"Payroll record created for employee: " +
                $"{employee.EmployeeName}"
            );
        }
        else
        {
            Debug.LogWarning(
                $"Failed to create payroll record for: " +
                $"{employee.EmployeeName}"
            );
        }
    }

    // =========================================================
    // FIRE EMPLOYEE
    // =========================================================

    public bool FireEmployee(
        string employeeId)
    {
        Employee employee =
            FindEmployee(employeeId);

        if (employee == null)
        {
            Debug.LogWarning(
                $"Employee not found: {employeeId}"
            );

            return false;
        }

        if (!employee.IsHired)
        {
            Debug.LogWarning(
                $"Employee is already fired: " +
                $"{employee.EmployeeName}"
            );

            return false;
        }

        // Record leaving date BEFORE changing
        // the employee status.
        if (EmployeePayrollManager.Instance != null)
        {
            bool leavingDateRecorded =
                EmployeePayrollManager.Instance
                    .RecordEmployeeLeavingDate(
                        employee.EmployeeId
                    );

            if (!leavingDateRecorded)
            {
                Debug.LogWarning(
                    $"Employee leaving date could not be recorded: " +
                    $"{employee.EmployeeId}"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "EmployeePayrollManager instance not found. " +
                "Leaving date was not recorded."
            );
        }

        // IMPORTANT:
        // Do NOT remove the employee.
        // Fired employees remain in employee history.
        employee.SetHiringStatus(false);

        Debug.Log(
            $"Employee fired: " +
            $"{employee.EmployeeName} | " +
            $"ID: {employee.EmployeeId}"
        );

        RefreshEmployeeUI();

        return true;
    }

    // =========================================================
    // FIND EMPLOYEE
    // =========================================================

    public Employee FindEmployee(
        string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        return employees.Find(
            employee =>
                employee != null &&
                employee.EmployeeId == employeeId
        );
    }

    // =========================================================
    // EMPLOYEE STATUS
    // =========================================================

    public bool IsEmployeeHired(
        string employeeId)
    {
        Employee employee =
            FindEmployee(employeeId);

        return employee != null &&
               employee.IsHired;
    }

    public bool EmployeeIdExists(
        string employeeId)
    {
        return FindEmployee(employeeId) != null;
    }

    // =========================================================
    // EMPLOYEE DISPLAY
    // =========================================================

    public void DisplayAllEmployees()
    {
        if (employees.Count == 0)
        {
            Debug.Log(
                "No employees have been added."
            );

            return;
        }

        Debug.Log(
            $"Total Employee Records: " +
            $"{employees.Count}"
        );

        Debug.Log(
            $"Active Employees: " +
            $"{ActiveEmployeeCount}"
        );

        foreach (Employee employee in employees)
        {
            if (employee == null)
            {
                continue;
            }

            Debug.Log(
                $"ID: {employee.EmployeeId} | " +
                $"Name: {employee.EmployeeName} | " +
                $"Role: {employee.Role} | " +
                $"Salary: ₹{employee.MonthlySalary:N0} | " +
                $"Status: " +
                $"{(employee.IsHired ? "Hired" : "Fired")}"
            );
        }
    }

    // =========================================================
    // EMPLOYEE UI
    // =========================================================

    public void RefreshEmployeeUI()
    {
        if (employeeList == null)
        {
            Debug.LogWarning(
                "EmployeeManager: Employee List reference " +
                "is not assigned."
            );

            return;
        }

        if (employeeCardPrefab == null)
        {
            Debug.LogWarning(
                "EmployeeManager: Employee Card Prefab reference " +
                "is not assigned."
            );

            return;
        }

        ClearEmployeeCards();

        foreach (Employee employee in employees)
        {
            if (employee == null)
            {
                continue;
            }

            EmployeeCardUI employeeCard =
                Instantiate(
                    employeeCardPrefab,
                    employeeList
                );

            employeeCard.SetEmployee(
                employee
            );
        }

        Debug.Log(
            $"Employee UI refreshed. " +
            $"Cards displayed: {employees.Count}"
        );
    }

    private void ClearEmployeeCards()
    {
        if (employeeList == null)
        {
            return;
        }

        for (
            int i = employeeList.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                employeeList.GetChild(i).gameObject
            );
        }
    }

    // =========================================================
    // EMPLOYEE ID GENERATION
    // =========================================================

    private string GenerateEmployeeId()
    {
        string employeeId =
            $"EMP{nextEmployeeNumber:000}";

        while (EmployeeIdExists(employeeId))
        {
            nextEmployeeNumber++;

            employeeId =
                $"EMP{nextEmployeeNumber:000}";
        }

        return employeeId;
    }

    private void UpdateNextEmployeeNumber()
    {
        int highestNumber = 0;

        foreach (Employee employee in employees)
        {
            if (employee == null ||
                string.IsNullOrWhiteSpace(
                    employee.EmployeeId))
            {
                continue;
            }

            if (!employee.EmployeeId.StartsWith("EMP"))
            {
                continue;
            }

            string numberText =
                employee.EmployeeId.Substring(3);

            if (int.TryParse(
                numberText,
                out int number))
            {
                if (number > highestNumber)
                {
                    highestNumber = number;
                }
            }
        }

        nextEmployeeNumber =
            Mathf.Max(
                nextEmployeeNumber,
                highestNumber + 1
            );
    }

    public string GetNextEmployeeId()
    {
        return GenerateEmployeeId();
    }

    // =========================================================
    // TEST METHODS
    // =========================================================

    [ContextMenu("TEST - Hire Sample Employee 1")]
    private void TestHireSampleEmployee1()
    {
        HireEmployee(
            GetNextEmployeeId(),
            "Vignesh",
            EmployeeRole.Cashier,
            15000
        );
    }

    [ContextMenu("TEST - Hire Sample Employee 2")]
    private void TestHireSampleEmployee2()
    {
        HireEmployee(
            GetNextEmployeeId(),
            "Arun",
            EmployeeRole.ShopAssistant,
            12000
        );
    }

    [ContextMenu("TEST - Hire Sample Employee 3")]
    private void TestHireSampleEmployee3()
    {
        HireEmployee(
            GetNextEmployeeId(),
            "Ravi",
            EmployeeRole.SecurityGuard,
            13000
        );
    }

    [ContextMenu("TEST - Display All Employees")]
    private void TestDisplayAllEmployees()
    {
        DisplayAllEmployees();
    }

    [ContextMenu("TEST - Fire EMP001")]
    private void TestFireEmployee()
    {
        FireEmployee("EMP001");
    }

    [ContextMenu("TEST - Refresh Employee UI")]
    private void TestRefreshEmployeeUI()
    {
        RefreshEmployeeUI();
    }
}