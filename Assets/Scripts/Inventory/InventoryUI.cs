using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Bread")]
    [SerializeField] private TMP_Text breadNameText;
    [SerializeField] private TMP_Text breadQuantityText;

    [Header("Milk")]
    [SerializeField] private TMP_Text milkNameText;
    [SerializeField] private TMP_Text milkQuantityText;

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
            closeButton.onClick.AddListener(CloseInventory);
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
            breadQuantityText
        );

        UpdateItemUI(
            "food_002",
            "Milk",
            milkNameText,
            milkQuantityText
        );

        UpdateItemUI(
            "tool_001",
            "Tool Kit",
            toolKitNameText,
            toolKitQuantityText
        );
    }

    private void UpdateItemUI(
        string itemId,
        string defaultItemName,
        TMP_Text nameText,
        TMP_Text quantityText)
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