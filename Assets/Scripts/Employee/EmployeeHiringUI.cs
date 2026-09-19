using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeHiringUI : MonoBehaviour
{
    [Header("Hiring Panel")]
    [SerializeField] private GameObject hireEmployeePanel;
    [SerializeField] private Button hireEmployeeButton;

    [Header("Employee Inputs")]
    [SerializeField] private TMP_InputField employeeNameInput;
    [SerializeField] private TMP_Dropdown employeeRoleDropdown;
    [SerializeField] private TMP_Text employeeSalaryText;

    [Header("Confirm Button")]
    [SerializeField] private Button confirmHireButton;

    [Header("Salary Settings")]
    [SerializeField] private int cashierSalary = 15000;
    [SerializeField] private int shopAssistantSalary = 12000;
    [SerializeField] private int stockManagerSalary = 13000;
    [SerializeField] private int securityGuardSalary = 11000;
    [SerializeField] private int cleanerSalary = 10000;

    private void Start()
    {
        if (hireEmployeePanel != null)
        {
            hireEmployeePanel.SetActive(false);
        }

        if (hireEmployeeButton != null)
        {
            hireEmployeeButton.gameObject.SetActive(true);
        }

        SetupRoleDropdown();
        SetupNameInput();
        UpdateSalaryDisplay();
        UpdateConfirmButton();
    }

    private void SetupRoleDropdown()
    {
        if (employeeRoleDropdown == null)
        {
            Debug.LogWarning(
                "EmployeeHiringUI: Role Dropdown is not assigned."
            );

            return;
        }

        employeeRoleDropdown.onValueChanged.RemoveListener(OnRoleChanged);
        employeeRoleDropdown.onValueChanged.AddListener(OnRoleChanged);
    }

    private void SetupNameInput()
    {
        if (employeeNameInput == null)
        {
            Debug.LogWarning(
                "EmployeeHiringUI: Employee Name Input is not assigned."
            );

            return;
        }

        employeeNameInput.onValueChanged.RemoveListener(OnNameChanged);
        employeeNameInput.onValueChanged.AddListener(OnNameChanged);

        employeeNameInput.onValueChanged.RemoveListener(ValidateEmployeeName);
        employeeNameInput.onValueChanged.AddListener(ValidateEmployeeName);
    }

    private void OnNameChanged(string value)
    {
        UpdateConfirmButton();
    }

    private void ValidateEmployeeName(string value)
    {
        if (employeeNameInput == null)
        {
            return;
        }

        string validName = "";

        foreach (char character in value)
        {
            // Allow letters
            if (char.IsLetter(character))
            {
                validName += character;
            }
            // Allow spaces
            else if (character == ' ')
            {
                if (validName.Length > 0)
                {
                    validName += character;
                }
            }
            // Allow dot
            else if (character == '.')
            {
                // Dot cannot be the first character
                if (validName.Length == 0)
                {
                    continue;
                }

                // Do not allow consecutive dots
                if (validName.EndsWith("."))
                {
                    continue;
                }

                validName += character;
            }
        }

        if (employeeNameInput.text != validName)
        {
            int caretPosition =
                employeeNameInput.caretPosition;

            employeeNameInput.SetTextWithoutNotify(
                validName
            );

            employeeNameInput.caretPosition =
                Mathf.Clamp(
                    caretPosition,
                    0,
                    validName.Length
                );
        }

        UpdateConfirmButton();
    }

    private void OnRoleChanged(int selectedIndex)
    {
        UpdateSalaryDisplay();
        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        if (confirmHireButton == null)
        {
            return;
        }

        bool hasName =
            employeeNameInput != null &&
            !string.IsNullOrWhiteSpace(
                employeeNameInput.text
            );

        bool hasRole =
            employeeRoleDropdown != null &&
            employeeRoleDropdown.value > 0;

        confirmHireButton.interactable =
            hasName && hasRole;
    }

    private void UpdateSalaryDisplay()
    {
        if (employeeRoleDropdown == null ||
            employeeSalaryText == null)
        {
            return;
        }

        if (employeeRoleDropdown.options.Count == 0)
        {
            employeeSalaryText.text = "Select a role";
            return;
        }

        string selectedRole =
            employeeRoleDropdown.options[
                employeeRoleDropdown.value
            ].text;

        int salary =
            GetSalaryForRole(selectedRole);

        if (selectedRole == "Select Role")
        {
            employeeSalaryText.text = "Select a role";
            return;
        }

        employeeSalaryText.text =
            $"RS.{salary:N0} / Month";
    }

    private int GetSalaryForRole(string role)
    {
        switch (role)
        {
            case "Cashier":
                return cashierSalary;

            case "Shop Assistant":
                return shopAssistantSalary;

            case "Stock Manager":
                return stockManagerSalary;

            case "Security Guard":
                return securityGuardSalary;

            case "Cleaner":
                return cleanerSalary;

            default:
                return 0;
        }
    }

    public void OpenHirePanel()
    {
        if (hireEmployeePanel == null)
        {
            Debug.LogWarning(
                "EmployeeHiringUI: Hire Employee Panel is not assigned."
            );

            return;
        }

        hireEmployeePanel.SetActive(true);

        if (hireEmployeeButton != null)
        {
            hireEmployeeButton.gameObject.SetActive(false);
        }

        ClearForm();

        Debug.Log(
            "Employee hiring panel opened."
        );
    }

    public void CloseHirePanel()
    {
        if (hireEmployeePanel == null)
        {
            return;
        }

        hireEmployeePanel.SetActive(false);

        if (hireEmployeeButton != null)
        {
            hireEmployeeButton.gameObject.SetActive(true);
        }

        Debug.Log(
            "Employee hiring panel closed."
        );
    }

    public void ConfirmHire()
    {
        if (EmployeeManager.Instance == null)
        {
            Debug.LogError(
                "EmployeeHiringUI: EmployeeManager instance not found."
            );

            return;
        }

        if (employeeNameInput == null ||
            employeeRoleDropdown == null)
        {
            Debug.LogError(
                "EmployeeHiringUI: Required UI references are missing."
            );

            return;
        }

        string employeeName =
            employeeNameInput.text.Trim();

        // Basic name validation
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning(
                "Please enter an employee name."
            );

            UpdateConfirmButton();
            return;
        }

        // Name cannot start with a dot
        if (employeeName.StartsWith("."))
        {
            Debug.LogWarning(
                "Employee name cannot start with a dot."
            );

            return;
        }

        // Name cannot end with a dot
        if (employeeName.EndsWith("."))
        {
            Debug.LogWarning(
                "Employee name cannot end with a dot."
            );

            return;
        }

        // Name cannot contain consecutive dots
        if (employeeName.Contains(".."))
        {
            Debug.LogWarning(
                "Employee name cannot contain consecutive dots."
            );

            return;
        }

        // Role validation
        if (employeeRoleDropdown.value == 0)
        {
            Debug.LogWarning(
                "Please select an employee role."
            );

            UpdateConfirmButton();
            return;
        }

        string selectedRole =
            employeeRoleDropdown.options[
                employeeRoleDropdown.value
            ].text;

        EmployeeRole role =
            ConvertToEmployeeRole(selectedRole);

        int salary =
            GetSalaryForRole(selectedRole);

        if (salary <= 0)
        {
            Debug.LogWarning(
                "Invalid employee salary."
            );

            return;
        }

        string employeeId =
            GenerateEmployeeId();

        bool hired =
            EmployeeManager.Instance.HireEmployee(
                employeeId,
                employeeName,
                role,
                salary
            );

        if (!hired)
        {
            Debug.LogWarning(
                "Employee hiring failed."
            );

            return;
        }

        Debug.Log(
            $"Employee hiring completed: {employeeName} | " +
            $"Role: {role} | Salary: RS.{salary}"
        );

        ClearForm();
        CloseHirePanel();
    }

    private EmployeeRole ConvertToEmployeeRole(
        string role)
    {
        switch (role)
        {
            case "Cashier":
                return EmployeeRole.Cashier;

            case "Shop Assistant":
                return EmployeeRole.ShopAssistant;

            case "Stock Manager":
                return EmployeeRole.StockManager;

            case "Security Guard":
                return EmployeeRole.SecurityGuard;

            case "Cleaner":
                return EmployeeRole.Cleaner;

            default:
                return EmployeeRole.Cashier;
        }
    }

    private string GenerateEmployeeId()
    {
        int employeeNumber =
            EmployeeManager.Instance.EmployeeCount + 1;

        string employeeId =
            $"EMP_{employeeNumber:000}";

        while (
            EmployeeManager.Instance.IsEmployeeHired(
                employeeId))
        {
            employeeNumber++;

            employeeId =
                $"EMP_{employeeNumber:000}";
        }

        return employeeId;
    }

    private void ClearForm()
    {
        if (employeeNameInput != null)
        {
            employeeNameInput.SetTextWithoutNotify("");
        }

        if (employeeRoleDropdown != null)
        {
            employeeRoleDropdown.value = 0;
            employeeRoleDropdown.RefreshShownValue();
        }

        UpdateSalaryDisplay();
        UpdateConfirmButton();
    }
}