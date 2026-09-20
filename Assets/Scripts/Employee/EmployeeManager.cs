using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager Instance { get; private set; }

    [Header("Employee Settings")]
    [SerializeField] private int maximumEmployees = 10;

    [Header("Employee Data")]
    [SerializeField]
    private List<Employee> employees =
        new List<Employee>();

    [Header("Employee UI")]
    [SerializeField] private Transform employeeList;
    [SerializeField] private GameObject employeeCardPrefab;

    private int nextEmployeeNumber = 1;

    // ==================================================
    // PUBLIC PROPERTIES
    // ==================================================

    public List<Employee> Employees =>
        employees;

    public int EmployeeCount =>
        employees.Count;

    public int ActiveEmployeeCount
    {
        get
        {
            int count = 0;

            foreach (Employee employee in employees)
            {
                if (employee != null && employee.IsHired)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int MaximumEmployees =>
        maximumEmployees;

    // ==================================================
    // UNITY
    // ==================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ==================================================
    // HIRE EMPLOYEE
    // ==================================================

    public bool HireEmployee(
        string employeeId,
        string employeeName,
        EmployeeRole role,
        int salary)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            Debug.LogWarning(
                "EmployeeManager: Employee ID is empty."
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning(
                "EmployeeManager: Employee name is empty."
            );

            return false;
        }

        if (ActiveEmployeeCount >= maximumEmployees)
        {
            Debug.LogWarning(
                "EmployeeManager: Maximum employee limit reached."
            );

            return false;
        }

        if (EmployeeIdExists(employeeId))
        {
            Debug.LogWarning(
                $"EmployeeManager: Employee ID {employeeId} already exists."
            );

            return false;
        }

        Employee employee =
            new Employee(
                employeeId,
                employeeName,
                role,
                salary
            );

        employees.Add(employee);

        // ==================================================
        // CREATE PAYROLL RECORD
        // ==================================================

        if (EmployeePayrollManager.Instance != null)
        {
            EmployeePayrollManager.Instance
                .CreatePayrollRecord(employee);
        }

        // ==================================================
        // CREATE PERFORMANCE RECORD
        // ==================================================

        if (EmployeePerformanceManager.Instance != null)
        {
            EmployeePerformanceManager.Instance
                .CreatePerformanceRecord(employee);
        }

        Debug.Log(
            $"Employee hired: {employee.EmployeeId} - " +
            $"{employee.EmployeeName} - " +
            $"{employee.Role} - " +
            $"Salary: ₹{employee.MonthlySalary}"
        );

        RefreshEmployeeUI();

        return true;
    }

    // ==================================================
    // OPTIONAL HIRE OVERLOAD
    // ==================================================
    // Allows other systems to hire an employee without
    // manually generating an ID.

    public Employee HireEmployee(
        string employeeName,
        EmployeeRole role)
    {
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning(
                "EmployeeManager: Employee name is empty."
            );

            return null;
        }

        if (ActiveEmployeeCount >= maximumEmployees)
        {
            Debug.LogWarning(
                "EmployeeManager: Maximum employee limit reached."
            );

            return null;
        }

        string employeeId =
            GetNextEmployeeId();

        int salary =
            GetSalaryForRole(role);

        bool hired =
            HireEmployee(
                employeeId,
                employeeName,
                role,
                salary
            );

        if (!hired)
        {
            return null;
        }

        return FindEmployee(employeeId);
    }

    // ==================================================
    // FIRE EMPLOYEE
    // ==================================================

    public bool FireEmployee(
        string employeeId)
    {
        Employee employee =
            FindEmployee(employeeId);

        if (employee == null)
        {
            Debug.LogWarning(
                $"EmployeeManager: Employee {employeeId} not found."
            );

            return false;
        }

        if (!employee.IsHired)
        {
            Debug.LogWarning(
                $"Employee {employeeId} is already inactive."
            );

            return false;
        }

        // ==================================================
        // RECORD PAYROLL END DATE
        // ==================================================

        if (EmployeePayrollManager.Instance != null)
        {
            EmployeePayrollManager.Instance
                .RecordEmployeeLeavingDate(employeeId);
        }

        // ==================================================
        // UPDATE EMPLOYMENT STATUS
        // ==================================================

        employee.SetHiringStatus(false);

        Debug.Log(
            $"Employee fired: {employeeId} - " +
            $"{employee.EmployeeName}"
        );

        RefreshEmployeeUI();

        return true;
    }

    // ==================================================
    // FIND EMPLOYEE
    // ==================================================

    public Employee FindEmployee(
        string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        foreach (Employee employee in employees)
        {
            if (employee != null &&
                employee.EmployeeId == employeeId)
            {
                return employee;
            }
        }

        return null;
    }

    // ==================================================
    // CHECK EMPLOYEE STATUS
    // ==================================================

    public bool IsEmployeeHired(
        string employeeId)
    {
        Employee employee =
            FindEmployee(employeeId);

        return employee != null &&
               employee.IsHired;
    }

    // ==================================================
    // CHECK EMPLOYEE ID
    // ==================================================

    public bool EmployeeIdExists(
        string employeeId)
    {
        return FindEmployee(employeeId) != null;
    }

    // ==================================================
    // GET NEXT EMPLOYEE ID
    // ==================================================

    public string GetNextEmployeeId()
    {
        string employeeId;

        do
        {
            employeeId =
                $"EMP{nextEmployeeNumber:000}";

            nextEmployeeNumber++;

        } while (EmployeeIdExists(employeeId));

        return employeeId;
    }

    // ==================================================
    // DISPLAY ALL EMPLOYEES
    // ==================================================

    [ContextMenu("Display All Employees")]
    public void DisplayAllEmployees()
    {
        if (employees.Count == 0)
        {
            Debug.Log(
                "EmployeeManager: No employees found."
            );

            return;
        }

        Debug.Log(
            $"========== EMPLOYEES ({employees.Count}) =========="
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
                $"Salary: ₹{employee.MonthlySalary} | " +
                $"Status: {(employee.IsHired ? "Hired" : "Inactive")} | " +
                $"Productivity: {employee.Productivity:P0}"
            );
        }

        Debug.Log(
            "=============================================="
        );
    }

    // ==================================================
    // REFRESH EMPLOYEE UI
    // ==================================================

    public void RefreshEmployeeUI()
    {
        if (employeeList == null ||
            employeeCardPrefab == null)
        {
            return;
        }

        // Remove existing cards
        for (int i = employeeList.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                employeeList.GetChild(i).gameObject
            );
        }

        // Create employee cards
        foreach (Employee employee in employees)
        {
            if (employee == null)
            {
                continue;
            }

            GameObject card =
                Instantiate(
                    employeeCardPrefab,
                    employeeList
                );

            EmployeeCardUI cardUI =
                card.GetComponent<EmployeeCardUI>();

            if (cardUI != null)
            {
                cardUI.SetEmployee(employee);
            }
        }
    }

    // ==================================================
    // GET SALARY BY ROLE
    // ==================================================

    private int GetSalaryForRole(
        EmployeeRole role)
    {
        switch (role)
        {
            case EmployeeRole.Cashier:
                return 15000;

            case EmployeeRole.ShopAssistant:
                return 12000;

            case EmployeeRole.StockManager:
                return 20000;

            case EmployeeRole.SecurityGuard:
                return 13000;

            case EmployeeRole.Cleaner:
                return 10000;

            default:
                return 0;
        }
    }

    // ==================================================
    // TEST - HIRE CASHIER
    // ==================================================

    [ContextMenu("Test Hire Cashier")]
    private void TestHireCashier()
    {
        string employeeId =
            GetNextEmployeeId();

        HireEmployee(
            employeeId,
            "Test Cashier",
            EmployeeRole.Cashier,
            15000
        );
    }

    // ==================================================
    // TEST - HIRE SHOP ASSISTANT
    // ==================================================

    [ContextMenu("Test Hire Shop Assistant")]
    private void TestHireShopAssistant()
    {
        string employeeId =
            GetNextEmployeeId();

        HireEmployee(
            employeeId,
            "Test Assistant",
            EmployeeRole.ShopAssistant,
            12000
        );
    }

    // ==================================================
    // TEST - HIRE STOCK MANAGER
    // ==================================================

    [ContextMenu("Test Hire Stock Manager")]
    private void TestHireStockManager()
    {
        string employeeId =
            GetNextEmployeeId();

        HireEmployee(
            employeeId,
            "Test Stock",
            EmployeeRole.StockManager,
            20000
        );
    }

    // ==================================================
    // TEST - FIRE EMP001
    // ==================================================

    [ContextMenu("Test Fire EMP001")]
    private void TestFireEMP001()
    {
        FireEmployee("EMP001");
    }

    // ==================================================
    // TEST - REFRESH UI
    // ==================================================

    [ContextMenu("Refresh Employee UI")]
    private void TestRefreshEmployeeUI()
    {
        RefreshEmployeeUI();
    }
}