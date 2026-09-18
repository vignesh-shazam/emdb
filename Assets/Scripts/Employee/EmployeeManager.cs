using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager Instance { get; private set; }

    [Header("Employee Settings")]
    [SerializeField] private int maximumEmployees = 10;

    [Header("Hired Employees")]
    [SerializeField] private List<Employee> employees = new List<Employee>();

    [Header("Employee UI")]
    [SerializeField] private Transform employeeList;
    [SerializeField] private EmployeeCardUI employeeCardPrefab;

    public IReadOnlyList<Employee> Employees => employees;
    public int EmployeeCount => employees.Count;
    public int MaximumEmployees => maximumEmployees;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate EmployeeManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("EmployeeManager initialized successfully.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool HireEmployee(
        string employeeId,
        string employeeName,
        EmployeeRole role,
        int monthlySalary)
    {
        if (employees.Count >= maximumEmployees)
        {
            Debug.LogWarning("Cannot hire employee. Maximum employee limit reached.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            Debug.LogWarning("Cannot hire employee. Employee ID is empty.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning("Cannot hire employee. Employee name is empty.");
            return false;
        }

        if (IsEmployeeHired(employeeId))
        {
            Debug.LogWarning($"Employee ID already exists: {employeeId}");
            return false;
        }

        Employee newEmployee = new Employee(
            employeeId,
            employeeName,
            role,
            monthlySalary
        );

        employees.Add(newEmployee);

        Debug.Log(
            $"Employee hired: {employeeName} | Role: {role} | Salary: ₹{monthlySalary}"
        );

        RefreshEmployeeUI();

        return true;
    }

    public bool FireEmployee(string employeeId)
    {
        Employee employee = FindEmployee(employeeId);

        if (employee == null)
        {
            Debug.LogWarning($"Employee not found: {employeeId}");
            return false;
        }

        employee.SetHiringStatus(false);
        employees.Remove(employee);

        Debug.Log($"Employee fired: {employee.EmployeeName}");

        RefreshEmployeeUI();

        return true;
    }

    public Employee FindEmployee(string employeeId)
    {
        return employees.Find(employee => employee.EmployeeId == employeeId);
    }

    public bool IsEmployeeHired(string employeeId)
    {
        return FindEmployee(employeeId) != null;
    }

    public void DisplayAllEmployees()
    {
        if (employees.Count == 0)
        {
            Debug.Log("No employees have been hired.");
            return;
        }

        Debug.Log($"Total hired employees: {employees.Count}");

        foreach (Employee employee in employees)
        {
            Debug.Log(
                $"ID: {employee.EmployeeId} | " +
                $"Name: {employee.EmployeeName} | " +
                $"Role: {employee.Role} | " +
                $"Salary: ₹{employee.MonthlySalary} | " +
                $"Productivity: {employee.Productivity}"
            );
        }
    }

    private void RefreshEmployeeUI()
    {
        if (employeeList == null)
        {
            Debug.LogWarning(
                "EmployeeManager: Employee List reference is not assigned."
            );

            return;
        }

        if (employeeCardPrefab == null)
        {
            Debug.LogWarning(
                "EmployeeManager: Employee Card Prefab reference is not assigned."
            );

            return;
        }

        ClearEmployeeCards();

        foreach (Employee employee in employees)
        {
            EmployeeCardUI employeeCard =
                Instantiate(employeeCardPrefab, employeeList);

            employeeCard.SetEmployee(employee);
        }

        Debug.Log(
            $"Employee UI refreshed. Cards displayed: {employees.Count}"
        );
    }

    private void ClearEmployeeCards()
    {
        for (int i = employeeList.childCount - 1; i >= 0; i--)
        {
            Destroy(employeeList.GetChild(i).gameObject);
        }
    }

    [ContextMenu("TEST - Hire Sample Employee 1")]
    private void TestHireSampleEmployee1()
    {
        HireEmployee(
            "EMP_001",
            "Arun",
            EmployeeRole.Cashier,
            15000
        );
    }

    [ContextMenu("TEST - Hire Sample Employee 2")]
    private void TestHireSampleEmployee2()
    {
        HireEmployee(
            "EMP_002",
            "Priya",
            EmployeeRole.ShopAssistant,
            12000
        );
    }

    [ContextMenu("TEST - Hire Sample Employee 3")]
    private void TestHireSampleEmployee3()
    {
        HireEmployee(
            "EMP_003",
            "Kumar",
            EmployeeRole.StockManager,
            13000
        );
    }

    [ContextMenu("TEST - Display All Employees")]
    private void TestDisplayAllEmployees()
    {
        DisplayAllEmployees();
    }

    [ContextMenu("TEST - Fire Sample Employee")]
    private void TestFireSampleEmployee()
    {
        FireEmployee("EMP_001");
    }

    [ContextMenu("TEST - Refresh Employee UI")]
    private void TestRefreshEmployeeUI()
    {
        RefreshEmployeeUI();
    }
}