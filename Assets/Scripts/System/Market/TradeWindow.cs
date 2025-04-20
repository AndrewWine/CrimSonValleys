using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TradeWindow : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject enterQuantity;
    [SerializeField] private InputField quantityInputField;
    [SerializeField] private Button finishButton;
    [SerializeField] private UISelectButton uiSelectButton;

    [Header("Data")]
    [SerializeField] private ItemData[] items;

    [Header("Events")]
    public static Action<Dictionary<ItemData, int>> SellItem;
    public static Action<Dictionary<ItemData, int>> BuyItem;
    public static Action<ItemData> EnableNotifyToolTipItem;
    public static Action<ItemData> DisableNotifyToolTipItem;

    private ItemData currentItem;
    private Dictionary<ItemData, int> listItemChosen = new();
    private HashSet<ItemData> selectedItems = new();

    private void Start()
    {
        enterQuantity.SetActive(false);
        finishButton.onClick.RemoveAllListeners();
        finishButton.onClick.AddListener(OnFinishButtonPressed);
    }

    private void OnEnable()
    {
        UISelectButton.tradeButtonPressed += OnItemClicked;
        UISelectButton.tradeShopCraftButtonPressed += OnItemClicked;
    }

    private void OnDisable()
    {
        UISelectButton.tradeButtonPressed -= OnItemClicked;
        UISelectButton.tradeShopCraftButtonPressed -= OnItemClicked;
    }

    /// <summary>
    /// Gọi khi người dùng click vào item trong danh sách.
    /// </summary>
    public void OnItemClicked(ItemData clickedItem)
    {
        if (currentItem != null && currentItem != clickedItem)
        {
            uiSelectButton.SetClickedBorderActive(currentItem, false);
            selectedItems.Remove(currentItem);
            TooltipManager.Instance.ShowToolTipOnTradeWindow(currentItem);
        }

        if (selectedItems.Contains(clickedItem))
        {
            selectedItems.Remove(clickedItem);
            listItemChosen.Remove(clickedItem);
            uiSelectButton.SetClickedBorderActive(clickedItem, false);
            enterQuantity.SetActive(false);
            currentItem = null;
            TooltipManager.Instance.HideTooltip();
            return;
        }

        currentItem = clickedItem;
        selectedItems.Add(clickedItem);
        enterQuantity.SetActive(true);
        quantityInputField.text = "";
        uiSelectButton.SetClickedBorderActive(clickedItem, true);
        TooltipManager.Instance.ShowToolTipOnTradeWindow(currentItem);
    }

    /// <summary>
    /// Gọi khi người dùng bấm nút "OK" sau khi nhập số lượng.
    /// </summary>
    public void OnFinishButtonPressed()
    {
        if (currentItem == null) return;

        if (int.TryParse(quantityInputField.text, out int quantity) && quantity > 0)
        {
            listItemChosen[currentItem] = quantity;
            Debug.Log($"{currentItem.itemName} x{quantity} đã được thêm vào danh sách.");
            enterQuantity.SetActive(false);
            currentItem = null;
        }
        else
        {
            Debug.LogError("Số lượng nhập không hợp lệ!");
        }
    }

    /// <summary>
    /// Gọi khi bấm nút bán.
    /// </summary>
    public void OnSellButtonPressed()
    {
        if (listItemChosen.Count > 0)
        {
            SellItem?.Invoke(new Dictionary<ItemData, int>(listItemChosen));
            ResetTradeSelection();
        }
        else
        {
            Debug.Log("Không có vật phẩm nào để bán!");
        }
    }

    /// <summary>
    /// Gọi khi bấm nút mua.
    /// </summary>
    public void OnBuyButtonPressed()
    {
        if (listItemChosen.Count > 0)
        {
            BuyItem?.Invoke(new Dictionary<ItemData, int>(listItemChosen));
            ResetTradeSelection();
        }
        else
        {
            Debug.Log("Không có vật phẩm nào để mua!");
        }
    }

    /// <summary>
    /// Đặt lại trạng thái chọn vật phẩm và UI liên quan.
    /// </summary>
    private void ResetTradeSelection()
    {
        foreach (var item in selectedItems)
        {
            uiSelectButton.SetClickedBorderActive(item, false);
        }

        selectedItems.Clear();
        listItemChosen.Clear();
        enterQuantity.SetActive(false);
        currentItem = null;
        TooltipManager.Instance.HideTooltip();
    }
}
