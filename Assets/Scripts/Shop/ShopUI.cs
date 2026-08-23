using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Bread")]
    [SerializeField] private TMP_Text breadNameText;
    [SerializeField] private TMP_Text breadPriceText;
    [SerializeField] private TMP_Text breadQuantityText;
    [SerializeField] private Button breadBuyButton;
    [SerializeField] private Button breadSellButton;

    [Header("Milk")]
    [SerializeField] private TMP_Text milkNameText;
    [SerializeField] private TMP_Text milkPriceText;
    [SerializeField] private TMP_Text milkQuantityText;
    [SerializeField] private Button milkBuyButton;
    [SerializeField] private Button milkSellButton;

    [Header("Tool Kit")]
    [SerializeField] private TMP_Text toolKitNameText;
    [SerializeField] private TMP_Text toolKitPriceText;
    [SerializeField] private TMP_Text toolKitQuantityText;
    [SerializeField] private Button toolKitBuyButton;
    [SerializeField] private Button toolKitSellButton;

    private ShopItem breadItem;
    private ShopItem milkItem;
    private ShopItem toolKitItem;

    private void Start()
    {
        InitializeUI();
    }

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
            ShopManager.Instance.GetItem("food_001");

        milkItem =
            ShopManager.Instance.GetItem("food_002");

        toolKitItem =
            ShopManager.Instance.GetItem("tool_001");

        SetupItemUI(
            breadItem,
            breadNameText,
            breadPriceText,
            breadQuantityText,
            breadBuyButton,
            breadSellButton
        );

        SetupItemUI(
            milkItem,
            milkNameText,
            milkPriceText,
            milkQuantityText,
            milkBuyButton,
            milkSellButton
        );

        SetupItemUI(
            toolKitItem,
            toolKitNameText,
            toolKitPriceText,
            toolKitQuantityText,
            toolKitBuyButton,
            toolKitSellButton
        );
    }

    private void SetupItemUI(
        ShopItem item,
        TMP_Text nameText,
        TMP_Text priceText,
        TMP_Text quantityText,
        Button buyButton,
        Button sellButton)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "ShopUI: Shop item not found."
            );

            if (buyButton != null)
            {
                buyButton.interactable = false;
            }

            if (sellButton != null)
            {
                sellButton.interactable = false;
            }

            return;
        }

        if (nameText != null)
        {
            nameText.text = item.ItemName;
        }

        if (priceText != null)
        {
            priceText.text =
                $"Buy: Rs. {item.BuyPrice:N0} | " +
                $"Sell: Rs. {item.SellPrice:N0}";
        }

        UpdateQuantityText(
            item.ItemId,
            quantityText
        );

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();

            buyButton.onClick.AddListener(
                () => BuyItem(item.ItemId)
            );
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();

            sellButton.onClick.AddListener(
                () => SellItem(item.ItemId)
            );

            UpdateSellButtonState(
                item.ItemId,
                sellButton
            );
        }
    }

    private void BuyItem(string itemId)
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError(
                "ShopUI: ShopManager not found."
            );

            return;
        }

        bool success =
            ShopManager.Instance.BuyItem(itemId);

        if (!success)
        {
            return;
        }

        UpdateAllUI();

        ShopItem item =
            ShopManager.Instance.GetItem(itemId);

        if (item != null)
        {
            int quantity =
                ShopManager.Instance.GetPurchasedQuantity(
                    itemId
                );

            Debug.Log(
                $"Shop UI purchase completed | " +
                $"Item: {item.ItemName} | " +
                $"Quantity: {quantity}"
            );
        }
    }

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
            ShopManager.Instance.SellItem(itemId);

        if (!success)
        {
            return;
        }

        UpdateAllUI();

        ShopItem item =
            ShopManager.Instance.GetItem(itemId);

        if (item != null)
        {
            int quantity =
                ShopManager.Instance.GetPurchasedQuantity(
                    itemId
                );

            Debug.Log(
                $"Shop UI sale completed | " +
                $"Item: {item.ItemName} | " +
                $"Quantity: {quantity}"
            );
        }
    }

    private void UpdateAllUI()
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

        UpdateSellButtonState(
            "food_001",
            breadSellButton
        );

        UpdateSellButtonState(
            "food_002",
            milkSellButton
        );

        UpdateSellButtonState(
            "tool_001",
            toolKitSellButton
        );
    }

    private void UpdateQuantityText(
        string itemId,
        TMP_Text quantityText)
    {
        if (quantityText == null)
        {
            return;
        }

        if (ShopManager.Instance == null)
        {
            return;
        }

        int quantity =
            ShopManager.Instance.GetPurchasedQuantity(
                itemId
            );

        quantityText.text =
            $"Qty: {quantity}";
    }

    private void UpdateSellButtonState(
        string itemId,
        Button sellButton)
    {
        if (sellButton == null)
        {
            return;
        }

        if (ShopManager.Instance == null)
        {
            sellButton.interactable = false;
            return;
        }

        int quantity =
            ShopManager.Instance.GetPurchasedQuantity(
                itemId
            );

        sellButton.interactable =
            quantity > 0;
    }
}