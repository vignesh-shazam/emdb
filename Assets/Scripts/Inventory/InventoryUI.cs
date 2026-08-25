using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Bread")]
    [SerializeField] private TMP_Text breadNameText;
    [SerializeField] private TMP_Text breadQuantityText;
    [SerializeField] private Button breadUseButton;

    [Header("Milk")]
    [SerializeField] private TMP_Text milkNameText;
    [SerializeField] private TMP_Text milkQuantityText;
    [SerializeField] private Button milkUseButton;

    [Header("Tool Kit")]
    [SerializeField] private TMP_Text toolKitNameText;
    [SerializeField] private TMP_Text toolKitQuantityText;

    [Header("Panel")]
    [SerializeField] private Button closeButton;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Start()
    {
        InitializeUI();
        RefreshUI();
    }

    private void InitializeUI()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(
                CloseInventory
            );
        }

        if (breadUseButton != null)
        {
            breadUseButton.onClick.RemoveAllListeners();
            breadUseButton.onClick.AddListener(
                UseBread
            );
        }

        if (milkUseButton != null)
        {
            milkUseButton.onClick.RemoveAllListeners();
            milkUseButton.onClick.AddListener(
                UseMilk
            );
        }
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "InventoryUI: InventoryManager not found."
            );

            return;
        }

        UpdateItemUI(
            "food_001",
            "Bread",
            breadNameText,
            breadQuantityText,
            breadUseButton
        );

        UpdateItemUI(
            "food_002",
            "Milk",
            milkNameText,
            milkQuantityText,
            milkUseButton
        );

        UpdateItemUI(
            "tool_001",
            "Tool Kit",
            toolKitNameText,
            toolKitQuantityText,
            null
        );
    }

    private void UpdateItemUI(
        string itemId,
        string defaultItemName,
        TMP_Text nameText,
        TMP_Text quantityText,
        Button useButton)
    {
        if (nameText != null)
        {
            nameText.text = defaultItemName;
        }

        if (quantityText == null)
        {
            return;
        }

        int quantity =
            InventoryManager.Instance.GetQuantity(
                itemId
            );

        quantityText.text =
            $"Qty: {quantity}";

        if (useButton != null)
        {
            useButton.interactable =
                quantity > 0;
        }
    }

    private void UseBread()
    {
        UseItem("food_001");
    }

    private void UseMilk()
    {
        UseItem("food_002");
    }

    private void UseItem(string itemId)
    {
        if (ItemUsageManager.Instance == null)
        {
            Debug.LogError(
                "InventoryUI: ItemUsageManager not found."
            );

            return;
        }

        bool success =
            ItemUsageManager.Instance.UseItem(
                itemId
            );

        if (!success)
        {
            return;
        }

        RefreshUI();
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);

        Debug.Log(
            "Inventory UI closed."
        );
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);

        RefreshUI();

        Debug.Log(
            "Inventory UI opened."
        );
    }
}