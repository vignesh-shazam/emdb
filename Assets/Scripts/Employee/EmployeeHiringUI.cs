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
    [SerializeField] private int securityGuardSalary = 13000;
    [SerializeField] private int cleanerSalary = 10000;

    [Header("Employee Name Settings")]
    [SerializeField] private int maximumNameCharacters = 15;

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

        employeeRoleDropdown.onValueChanged.RemoveListener(
            OnRoleChanged
        );

        employeeRoleDropdown.onValueChanged.AddListener(
            OnRoleChanged
        );
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

        // Limit employee name to 15 characters.
        employeeNameInput.characterLimit =
            maximumNameCharacters;

        employeeNameInput.onValueChanged.RemoveListener(
            OnNameChanged
        );

        employeeNameInput.onValueChanged.AddListener(
            OnNameChanged
        );

        employeeNameInput.onValueChanged.RemoveListener(
            ValidateEmployeeName
        );

        employeeNameInput.onValueChanged.AddListener(
            ValidateEmployeeName
        );
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
            // Allow letters.
            if (char.IsLetter(character))
            {
                validName += character;
            }

            // Allow spaces.
            else if (character == ' ')
            {
                if (validName.Length > 0)
                {
                    validName += character;
                }
            }

            // Allow dots.
            else if (character == '.')
            {
                // Dot cannot be the first character.
                if (validName.Length == 0)
                {
                    continue;
                }

                // Do not allow consecutive dots.
                if (validName.EndsWith("."))
                {
                    continue;
                }

                validName += character;
            }
        }

        // Extra safety in case the text is changed programmatically.
        if (validName.Length > maximumNameCharacters)
        {
            validName =
                validName.Substring(
                    0,
                    maximumNameCharacters
                );
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
            employeeSalaryText.text =
                "Select a role";

            return;
        }

        string selectedRole =
            employeeRoleDropdown.options[
                employeeRoleDropdown.value
            ].text;

        if (selectedRole == "Select Role")
        {
            employeeSalaryText.text =
                "Select a role";

            return;
        }

        int salary =
            GetSalaryForRole(selectedRole);

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

        // Basic name validation.
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            Debug.LogWarning(
                "Please enter an employee name."
            );

            UpdateConfirmButton();
            return;
        }

        // Maximum length validation.
        if (employeeName.Length >
            maximumNameCharacters)
        {
            Debug.LogWarning(
                $"Employee name cannot exceed " +
                $"{maximumNameCharacters} characters."
            );

            return;
        }

        // Name cannot start with a dot.
        if (employeeName.StartsWith("."))
        {
            Debug.LogWarning(
                "Employee name cannot start with a dot."
            );

            return;
        }

        // Name cannot end with a dot.
        if (employeeName.EndsWith("."))
        {
            Debug.LogWarning(
                "Employee name cannot end with a dot."
            );

            return;
        }

        // Name cannot contain consecutive dots.
        if (employeeName.Contains(".."))
        {
            Debug.LogWarning(
                "Employee name cannot contain consecutive dots."
            );

            return;
        }

        // Role validation.
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

        if (string.IsNullOrEmpty(employeeId))
        {
            Debug.LogError(
                "Employee ID generation failed."
            );

            return;
        }

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
            $"Employee hiring completed: " +
            $"{employeeName} | " +
            $"ID: {employeeId} | " +
            $"Role: {role} | " +
            $"Salary: RS.{salary:N0}"
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
        if (EmployeeManager.Instance == null)
        {
            Debug.LogError(
                "EmployeeHiringUI: EmployeeManager instance not found."
            );

            return string.Empty;
        }

        return EmployeeManager.Instance.GetNextEmployeeId();
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