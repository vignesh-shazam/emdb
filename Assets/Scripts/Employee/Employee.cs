using System;
using UnityEngine;

[Serializable]
public class Employee
{
    [Header("Employee Information")]
    [SerializeField] private string employeeId;
    [SerializeField] private string employeeName;
    [SerializeField] private EmployeeRole role;

    [Header("Employment Details")]
    [SerializeField] private int monthlySalary;
    [SerializeField] private bool isHired;
    [SerializeField] private float productivity = 1f;

    public string EmployeeId => employeeId;
    public string EmployeeName => employeeName;
    public EmployeeRole Role => role;
    public int MonthlySalary => monthlySalary;
    public bool IsHired => isHired;
    public float Productivity => productivity;

    public Employee(
        string id,
        string name,
        EmployeeRole employeeRole,
        int salary)
    {
        employeeId = id;
        employeeName = name;
        role = employeeRole;
        monthlySalary = Mathf.Max(0, salary);
        isHired = true;
        productivity = 1f;
    }

    public void SetHiringStatus(bool hired)
    {
        isHired = hired;
    }

    public void SetProductivity(float value)
    {
        productivity = Mathf.Clamp(value, 0f, 1f);
    }

    public void SetSalary(int salary)
    {
        monthlySalary = Mathf.Max(0, salary);
    }
}

public enum EmployeeRole
{
    Cashier,
    ShopAssistant,
    StockManager,
    SecurityGuard,
    Cleaner
}