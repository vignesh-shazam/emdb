using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Bread")]
    [SerializeField] private TMP_Text breadNameText;
    [SerializeField] private TMP_Text breadPriceText;
    [SerializeField] private TMP_Text breadQuantityText;
    [SerializeField] private Button breadMinusButton;
    [SerializeField] private Button breadPlusButton;

    [Header("Milk")]
    [SerializeField] private TMP_Text milkNameText;
    [SerializeField] private TMP_Text milkPriceText;
    [SerializeField] private TMP_Text milkQuantityText;
    [SerializeField] private Button milkMinusButton;
    [SerializeField] private Button milkPlusButton;

    [Header("Tool Kit")]
    [SerializeField] private TMP_Text toolKitNameText;
    [SerializeField] private TMP_Text toolKitPriceText;
    [SerializeField] private TMP_Text toolKitQuantityText;
    [SerializeField] private Button toolKitMinusButton;
    [SerializeField] private Button toolKitPlusButton;

    private ShopItem breadItem;
    private ShopItem milkItem;
    private ShopItem toolKitItem;

    // =========================
    // ENABLE
    // =========================

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged +=
            UpdateAllUI;

        CustomerManager.OnCustomerListChanged +=
            UpdateAllUI;
    }

    // =========================
    // DISABLE
    // =========================

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -=
            UpdateAllUI;

        CustomerManager.OnCustomerListChanged -=
            UpdateAllUI;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        InitializeUI();
    }

    // =========================
    // INITIALIZE UI
    // =========================

    private void InitializeUI()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        breadItem =
            ShopManager.Instance.GetItem(
                "food_001"
            );

        milkItem =
            ShopManager.Instance.GetItem(
                "food_002"
            );

        toolKitItem =
            ShopManager.Instance.GetItem(
                "tool_001"
            );

        SetupItemUI(
            breadItem,
            breadNameText,
            breadPriceText,
            breadQuantityText,
            breadMinusButton,
            breadPlusButton
        );

        SetupItemUI(
            milkItem,
            milkNameText,
            milkPriceText,
            milkQuantityText,
            milkMinusButton,
            milkPlusButton
        );

        SetupItemUI(
            toolKitItem,
            toolKitNameText,
            toolKitPriceText,
            toolKitQuantityText,
            toolKitMinusButton,
            toolKitPlusButton
        );

        UpdateAllUI();
    }

    // =========================
    // SETUP ITEM UI
    // =========================

    private void SetupItemUI(
        ShopItem item,
        TMP_Text nameText,
        TMP_Text priceText,
        TMP_Text quantityText,
        Button minusButton,
        Button plusButton)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "ShopUI: Shop item not found."
            );

            if (minusButton != null)
            {
                minusButton.interactable = false;
            }

            if (plusButton != null)
            {
                plusButton.interactable = false;
            }

            return;
        }

        if (nameText != null)
        {
            nameText.text =
                item.ItemName;
        }

        if (priceText != null)
        {
            priceText.text =
                $"Rs. {item.BuyPrice:N0}";
        }

        // =========================
        // MINUS BUTTON
        // =========================

        if (minusButton != null)
        {
            minusButton.onClick.RemoveAllListeners();

            minusButton.onClick.AddListener(
                () => SellItem(item.ItemId)
            );
        }

        // =========================
        // PLUS BUTTON
        // =========================

        if (plusButton != null)
        {
            plusButton.onClick.RemoveAllListeners();

            plusButton.onClick.AddListener(
                () => BuyItem(item.ItemId)
            );
        }

        UpdateQuantityText(
            item.ItemId,
            quantityText
        );

        UpdateButtonStates(
            item,
            minusButton,
            plusButton
        );
    }

    // =========================
    // BUY
    // =========================

    private void BuyItem(string itemId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        // Check request before attempting purchase.
        if (!CanBuyForCurrentCustomer(itemId))
        {
            Debug.LogWarning(
                "Shop UI: Cannot buy this item for " +
                "the current customer."
            );

            UpdateAllUI();

            return;
        }

        bool success =
            ShopManager.Instance.BuyItem(
                itemId
            );

        if (!success)
        {
            UpdateAllUI();

            return;
        }

        UpdateAllUI();

        if (InventoryManager.Instance != null)
        {
            int quantity =
                InventoryManager.Instance.GetQuantity(
                    itemId
                );

            ShopItem item =
                ShopManager.Instance.GetItem(
                    itemId
                );

            if (item != null)
            {
                Debug.Log(
                    $"Shop UI +1 | " +
                    $"Item: {item.ItemName} | " +
                    $"Quantity: {quantity}"
                );
            }
        }
    }

    // =========================
    // SELL
    // =========================

    private void SellItem(string itemId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        bool success =
            ShopManager.Instance.SellItem(
                itemId
            );

        if (!success)
        {
            UpdateAllUI();

            return;
        }

        UpdateAllUI();

        if (InventoryManager.Instance != null)
        {
            int quantity =
                InventoryManager.Instance.GetQuantity(
                    itemId
                );

            ShopItem item =
                ShopManager.Instance.GetItem(
                    itemId
                );

            if (item != null)
            {
                Debug.Log(
                    $"Shop UI -1 | " +
                    $"Item: {item.ItemName} | " +
                    $"Quantity: {quantity}"
                );
            }
        }
    }

    // =========================
    // UPDATE ALL UI
    // =========================

    public void UpdateAllUI()
    {
        UpdateQuantityText(
            "food_001",
            breadQuantityText
        );

        UpdateQuantityText(
            "food_002",
            milkQuantityText
        );

        UpdateQuantityText(
            "tool_001",
            toolKitQuantityText
        );

        UpdateButtonStates(
            breadItem,
            breadMinusButton,
            breadPlusButton
        );

        UpdateButtonStates(
            milkItem,
            milkMinusButton,
            milkPlusButton
        );

        UpdateButtonStates(
            toolKitItem,
            toolKitMinusButton,
            toolKitPlusButton
        );
    }

    // =========================
    // UPDATE QUANTITY
    // =========================

    private void UpdateQuantityText(
        string itemId,
        TMP_Text quantityText)
    {
        if (quantityText == null)
        {
            return;
        }

        if (InventoryManager.Instance == null)
        {
            quantityText.text =
                "Qty: 0";

            return;
        }

        int quantity =
            InventoryManager.Instance.GetQuantity(
                itemId
            );

        quantityText.text =
            $"Qty: {quantity}";
    }

    // =========================
    // UPDATE BUTTON STATES
    // =========================

    private void UpdateButtonStates(
        ShopItem item,
        Button minusButton,
        Button plusButton)
    {
        if (item == null)
        {
            if (minusButton != null)
            {
                minusButton.interactable = false;
            }

            if (plusButton != null)
            {
                plusButton.interactable = false;
            }

            return;
        }

        int inventoryQuantity = 0;

        if (InventoryManager.Instance != null)
        {
            inventoryQuantity =
                InventoryManager.Instance.GetQuantity(
                    item.ItemId
                );
        }

        // =========================
        // MINUS
        // =========================

        if (minusButton != null)
        {
            minusButton.interactable =
                inventoryQuantity > 0;
        }

        // =========================
        // PLUS
        // =========================

        if (plusButton != null)
        {
            plusButton.interactable =
                CanBuyForCurrentCustomer(
                    item.ItemId
                );
        }
    }

    // =========================
    // CURRENT CUSTOMER
    // =========================

    private Customer GetCurrentCustomer()
    {
        if (CustomerManager.Instance == null)
        {
            return null;
        }

        if (CustomerManager.Instance.CustomerCount <= 0)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers == null)
        {
            return null;
        }

        if (CustomerManager.Instance.ActiveCustomers.Count <= 0)
        {
            return null;
        }

        return
            CustomerManager.Instance
                .ActiveCustomers[0];
    }

    // =========================
    // CAN BUY FOR CUSTOMER
    // =========================

    private bool CanBuyForCurrentCustomer(
        string itemId)
    {
        Customer customer =
            GetCurrentCustomer();

        if (customer == null)
        {
            return false;
        }

        // Only the requested item can be purchased.
        if (customer.RequestedItemId != itemId)
        {
            return false;
        }

        int currentQuantity = 0;

        if (InventoryManager.Instance != null)
        {
            currentQuantity =
                InventoryManager.Instance.GetQuantity(
                    itemId
                );
        }

        // Stop at requested quantity.
        if (currentQuantity >=
            customer.RequestedQuantity)
        {
            return false;
        }

        // Check money.
        ShopItem item =
            ShopManager.Instance.GetItem(
                itemId
            );

        if (item == null)
        {
            return false;
        }

        if (MoneyManager.Instance == null)
        {
            return false;
        }

        if (!MoneyManager.Instance.CanAfford(
                item.BuyPrice))
        {
            return false;
        }

        return true;
    }
}