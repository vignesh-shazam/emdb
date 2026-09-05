using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LowStockUI : MonoBehaviour
{
    // --------------------------------------------------
    // ITEM ROW DATA
    // --------------------------------------------------

    [Serializable]
#pragma warning disable 0649
    private class ItemRow
    {
        public string ItemId;
        public string ItemName;

        [Header("UI References")]
        public TMP_Text ItemNameText;
        public Button MinusButton;
        public TMP_Text QuantityText;
        public Button PlusButton;

        [HideInInspector]
        public int BoxQuantity;
    }
#pragma warning restore 0649

    // --------------------------------------------------
    // SINGLETON
    // --------------------------------------------------

    public static LowStockUI Instance { get; private set; }

    // --------------------------------------------------
    // PANEL
    // --------------------------------------------------

    [Header("Panel")]
    [SerializeField] private GameObject lowStockPanel;

    // --------------------------------------------------
    // TITLE
    // --------------------------------------------------

    [Header("Title")]
    [SerializeField] private TMP_Text titleText;

    // --------------------------------------------------
    // ITEM ROWS
    // --------------------------------------------------

    [Header("Item Rows")]
    [SerializeField] private ItemRow milkRow;
    [SerializeField] private ItemRow breadRow;
    [SerializeField] private ItemRow toolKitRow;

    // --------------------------------------------------
    // BUTTONS
    // --------------------------------------------------

    [Header("Buttons")]
    [SerializeField] private Button orderButton;
    [SerializeField] private Button closeButton;

    // --------------------------------------------------
    // SETTINGS
    // --------------------------------------------------

    [Header("Selection Settings")]
    [SerializeField] private int maximumBoxQuantity = 99;

    // --------------------------------------------------
    // UNITY
    // --------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // --------------------------------------------------
        // CORRECT ITEM ID MAPPING
        // --------------------------------------------------

        // food_001 → Milk
        SetupItemRow(
            milkRow,
            "food_001",
            "Milk"
        );

        // food_002 → Bread
        SetupItemRow(
            breadRow,
            "food_002",
            "Bread"
        );

        // tool_001 → Tool Kit
        SetupItemRow(
            toolKitRow,
            "tool_001",
            "Tool Kit"
        );

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                ClosePanel
            );
        }

        if (orderButton != null)
        {
            orderButton.onClick.AddListener(
                HandleOrderButton
            );
        }

        if (LowStockManager.Instance != null)
        {
            LowStockManager.Instance.OnLowStockChanged +=
                HandleLowStockChanged;
        }
    }

    private void Start()
    {
        ResetAllQuantities();

        HidePanel();

        RefreshLowStockState();
    }

    private void OnDestroy()
    {
        if (LowStockManager.Instance != null)
        {
            LowStockManager.Instance.OnLowStockChanged -=
                HandleLowStockChanged;
        }

        RemoveItemRowListeners(milkRow);
        RemoveItemRowListeners(breadRow);
        RemoveItemRowListeners(toolKitRow);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                ClosePanel
            );
        }

        if (orderButton != null)
        {
            orderButton.onClick.RemoveListener(
                HandleOrderButton
            );
        }
    }

    // --------------------------------------------------
    // SETUP ITEM ROW
    // --------------------------------------------------

    private void SetupItemRow(
        ItemRow row,
        string itemId,
        string itemName)
    {
        if (row == null)
            return;

        row.ItemId = itemId;
        row.ItemName = itemName;
        row.BoxQuantity = 0;

        if (row.ItemNameText != null)
        {
            row.ItemNameText.text =
                itemName;
        }

        if (row.QuantityText != null)
        {
            row.QuantityText.text = "0";
        }

        if (row.MinusButton != null)
        {
            row.MinusButton.onClick.AddListener(
                () => DecreaseQuantity(row)
            );
        }

        if (row.PlusButton != null)
        {
            row.PlusButton.onClick.AddListener(
                () => IncreaseQuantity(row)
            );
        }

        RefreshItemRowButtons(row);
    }

    // --------------------------------------------------
    // REMOVE ITEM ROW LISTENERS
    // --------------------------------------------------

    private void RemoveItemRowListeners(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.MinusButton != null)
        {
            row.MinusButton.onClick.RemoveAllListeners();
        }

        if (row.PlusButton != null)
        {
            row.PlusButton.onClick.RemoveAllListeners();
        }
    }

    // --------------------------------------------------
    // INCREASE QUANTITY
    // --------------------------------------------------

    private void IncreaseQuantity(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.BoxQuantity >= maximumBoxQuantity)
        {
            return;
        }

        row.BoxQuantity++;

        RefreshQuantityText(row);
        RefreshItemRowButtons(row);
        RefreshOrderButton();
    }

    // --------------------------------------------------
    // DECREASE QUANTITY
    // --------------------------------------------------

    private void DecreaseQuantity(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.BoxQuantity <= 0)
        {
            return;
        }

        row.BoxQuantity--;

        RefreshQuantityText(row);
        RefreshItemRowButtons(row);
        RefreshOrderButton();
    }

    // --------------------------------------------------
    // REFRESH QUANTITY TEXT
    // --------------------------------------------------

    private void RefreshQuantityText(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.QuantityText == null)
            return;

        row.QuantityText.text =
            row.BoxQuantity.ToString();
    }

    // --------------------------------------------------
    // REFRESH ITEM BUTTONS
    // --------------------------------------------------

    private void RefreshItemRowButtons(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.MinusButton != null)
        {
            row.MinusButton.interactable =
                row.BoxQuantity > 0;
        }

        if (row.PlusButton != null)
        {
            row.PlusButton.interactable =
                row.BoxQuantity < maximumBoxQuantity;
        }
    }

    // --------------------------------------------------
    // REFRESH ORDER BUTTON
    // --------------------------------------------------

    private void RefreshOrderButton()
    {
        if (orderButton == null)
            return;

        orderButton.interactable =
            GetTotalSelectedBoxes() > 0;
    }

    // --------------------------------------------------
    // RESET ALL QUANTITIES
    // --------------------------------------------------

    private void ResetAllQuantities()
    {
        ResetQuantity(milkRow);
        ResetQuantity(breadRow);
        ResetQuantity(toolKitRow);

        RefreshOrderButton();
    }

    private void ResetQuantity(
        ItemRow row)
    {
        if (row == null)
            return;

        row.BoxQuantity = 0;

        RefreshQuantityText(row);
        RefreshItemRowButtons(row);
    }

    // --------------------------------------------------
    // LOW STOCK EVENT
    // --------------------------------------------------

    private void HandleLowStockChanged()
    {
        RefreshLowStockState();
    }

    // --------------------------------------------------
    // REFRESH LOW STOCK STATE
    // --------------------------------------------------

    private void RefreshLowStockState()
    {
        if (LowStockManager.Instance == null)
        {
            return;
        }

        if (!LowStockManager.Instance.HasAnyLowStock())
        {
            HidePanel();
            return;
        }

        ShowPanel();

        HighlightLowStockItems();
    }

    // --------------------------------------------------
    // HIGHLIGHT LOW STOCK ITEMS
    // --------------------------------------------------

    private void HighlightLowStockItems()
    {
        UpdateRowLowStockState(milkRow);
        UpdateRowLowStockState(breadRow);
        UpdateRowLowStockState(toolKitRow);
    }

    private void UpdateRowLowStockState(
        ItemRow row)
    {
        if (row == null)
            return;

        if (LowStockManager.Instance == null)
            return;

        bool isLowStock =
            LowStockManager.Instance.IsItemLowStock(
                row.ItemId
            );

        if (row.ItemNameText != null)
        {
            row.ItemNameText.fontStyle =
                isLowStock
                    ? FontStyles.Bold
                    : FontStyles.Normal;
        }
    }

    // --------------------------------------------------
    // SHOW PANEL
    // --------------------------------------------------

    public void ShowPanel()
    {
        if (lowStockPanel == null)
            return;

        lowStockPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = "LOW STOCK";
        }

        RefreshAllRows();
        RefreshOrderButton();
    }

    // --------------------------------------------------
    // HIDE PANEL
    // --------------------------------------------------

    public void HidePanel()
    {
        if (lowStockPanel == null)
            return;

        lowStockPanel.SetActive(false);
    }

    // --------------------------------------------------
    // REFRESH ALL ROWS
    // --------------------------------------------------

    private void RefreshAllRows()
    {
        RefreshItemRow(milkRow);
        RefreshItemRow(breadRow);
        RefreshItemRow(toolKitRow);
    }

    private void RefreshItemRow(
        ItemRow row)
    {
        if (row == null)
            return;

        RefreshQuantityText(row);
        RefreshItemRowButtons(row);
    }

    // --------------------------------------------------
    // CLOSE PANEL
    // --------------------------------------------------

    public void ClosePanel()
    {
        HidePanel();
    }

    // --------------------------------------------------
    // ORDER BUTTON
    // --------------------------------------------------

    private void HandleOrderButton()
    {
        int totalSelectedBoxes =
            GetTotalSelectedBoxes();

        if (totalSelectedBoxes <= 0)
        {
            Debug.Log(
                "LowStockUI: No boxes selected."
            );

            return;
        }

        if (PurchaseOrderManager.Instance == null)
        {
            Debug.LogError(
                "LowStockUI: " +
                "PurchaseOrderManager not found."
            );

            return;
        }

        // --------------------------------------------------
        // ADD SELECTED ITEMS
        // --------------------------------------------------

        AddSelectedItemToPurchaseOrder(
            milkRow
        );

        AddSelectedItemToPurchaseOrder(
            breadRow
        );

        AddSelectedItemToPurchaseOrder(
            toolKitRow
        );

        // --------------------------------------------------
        // SHOW RESULT
        // --------------------------------------------------

        Debug.Log(
            "========== PURCHASE ORDER CREATED =========="
        );

        PurchaseOrderManager.Instance.LogOrder();

        Debug.Log(
            "============================================="
        );

        // --------------------------------------------------
        // RESET UI
        // --------------------------------------------------

        ResetAllQuantities();

        HidePanel();
    }

    // --------------------------------------------------
    // ADD SELECTED ITEM TO PURCHASE ORDER
    // --------------------------------------------------

    private void AddSelectedItemToPurchaseOrder(
        ItemRow row)
    {
        if (row == null)
            return;

        if (row.BoxQuantity <= 0)
            return;

        PurchaseOrderManager.Instance.AddItem(
            row.ItemId,
            row.ItemName,
            row.BoxQuantity,
            10
        );

        Debug.Log(
            $"Purchase Order Added | " +
            $"{row.ItemName} | " +
            $"{row.BoxQuantity} Box(es) | " +
            $"{row.BoxQuantity * 10} Nos"
        );
    }

    // --------------------------------------------------
    // GET SELECTED QUANTITY
    // --------------------------------------------------

    public int GetSelectedBoxQuantity(
        string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        if (milkRow != null &&
            milkRow.ItemId == itemId)
        {
            return milkRow.BoxQuantity;
        }

        if (breadRow != null &&
            breadRow.ItemId == itemId)
        {
            return breadRow.BoxQuantity;
        }

        if (toolKitRow != null &&
            toolKitRow.ItemId == itemId)
        {
            return toolKitRow.BoxQuantity;
        }

        return 0;
    }

    // --------------------------------------------------
    // GET TOTAL SELECTED BOXES
    // --------------------------------------------------

    public int GetTotalSelectedBoxes()
    {
        int total = 0;

        if (milkRow != null)
        {
            total += milkRow.BoxQuantity;
        }

        if (breadRow != null)
        {
            total += breadRow.BoxQuantity;
        }

        if (toolKitRow != null)
        {
            total += toolKitRow.BoxQuantity;
        }

        return total;
    }

    // --------------------------------------------------
    // TEST - SHOW PANEL
    // --------------------------------------------------

    [ContextMenu("TEST - Show Low Stock Panel")]
    private void TestShowPanel()
    {
        ResetAllQuantities();

        ShowPanel();

        Debug.Log(
            "LowStockUI Test: Panel shown."
        );
    }

    // --------------------------------------------------
    // TEST - HIDE PANEL
    // --------------------------------------------------

    [ContextMenu("TEST - Hide Low Stock Panel")]
    private void TestHidePanel()
    {
        HidePanel();

        Debug.Log(
            "LowStockUI Test: Panel hidden."
        );
    }

    // --------------------------------------------------
    // TEST - RESET QUANTITIES
    // --------------------------------------------------

    [ContextMenu("TEST - Reset Quantities")]
    private void TestResetQuantities()
    {
        ResetAllQuantities();

        Debug.Log(
            "LowStockUI Test: " +
            "All quantities reset to 0."
        );
    }

    // --------------------------------------------------
    // TEST - SHOW SELECTED ORDER
    // --------------------------------------------------

    [ContextMenu("TEST - Show Selected Order")]
    private void TestShowSelectedOrder()
    {
        HandleOrderButton();
    }
}