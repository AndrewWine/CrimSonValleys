using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackSmithWindow : UIRequirementDisplay
{
    [Header("Elements")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject enterQuantity;
    [SerializeField] private InputField quantityInputField;
    [SerializeField] private Button finishButton;
    [SerializeField] private UISelectButton uiSelectButton;
    [SerializeField] private Image itemIconImage;

    [Header("Data")]
    [SerializeField] private ItemData[] items;

    [Header("Actions")]
    public static Action<Dictionary<ItemData, int>> SellItem;
    public static Action<Dictionary<ItemData, int>> BuyItem;

    private Dictionary<ItemData, int> listItemChosen = new Dictionary<ItemData, int>();
    private HashSet<ItemData> selectedItems = new HashSet<ItemData>();
    private ItemData currentItemSelected;
    private ItemData currentItem;

    private void Start()
    {
        enterQuantity.gameObject.SetActive(false);
        finishButton.onClick.RemoveAllListeners();
        finishButton.onClick.AddListener(OnFinishButtonPressed);
    }

    private void OnEnable()
    {
        UISelectButton.craftButtonPressed += SelectCraftItem;
        UISelectButton.tradeShopCraftButtonPressed += OnItemClicked;
    }

    private void OnDisable()
    {
        UISelectButton.craftButtonPressed -= SelectCraftItem;
        UISelectButton.tradeShopCraftButtonPressed -= OnItemClicked;
    }

    public void OnItemClicked(ItemData clickedItem)
    {
        if (currentItem != null && currentItem != clickedItem)
        {
            selectedItems.Remove(currentItem);
            TooltipManager.Instance.ShowToolTipOnTradeWindow(currentItem);
        }

        if (selectedItems.Contains(clickedItem))
        {
            selectedItems.Remove(clickedItem);
            enterQuantity.SetActive(false);
            currentItem = null;
            TooltipManager.Instance.HideTooltip();
            return;
        }

        currentItem = clickedItem;
        selectedItems.Add(clickedItem);
        enterQuantity.SetActive(true);
        quantityInputField.text = listItemChosen.ContainsKey(currentItem) ? listItemChosen[currentItem].ToString() : "";
        TooltipManager.Instance.ShowToolTipOnTradeWindow(currentItem);
    }

    public void OnFinishButtonPressed()
    {
        if (currentItem == null) return;

        if (int.TryParse(quantityInputField.text, out int num) && num > 0)
        {
            listItemChosen[currentItem] = num;
            Debug.Log($" {currentItem.itemName} x{num} đã được thêm vào danh sách.");
            enterQuantity.SetActive(false);
        }
        else
        {
            Debug.LogError(" Số lượng nhập không hợp lệ!");
        }
    }

    private void ResetTradeSelection()
    {
        selectedItems.Clear();
        enterQuantity.SetActive(false);
    }

    public void OnSellButtonPressed()
    {
        if (listItemChosen.Count > 0)
        {
            Debug.Log(" Danh sách vật phẩm bán:");
            foreach (var kvp in listItemChosen)
            {
                Debug.Log($"{kvp.Key.itemName} x{kvp.Value}");
            }

            SellItem?.Invoke(new Dictionary<ItemData, int>(listItemChosen));
            listItemChosen.Clear();
            ResetTradeSelection();
        }
        else
        {
            Debug.Log(" Không có vật phẩm nào để bán!");
        }
    }

    public void OnBuyButtonPressed()
    {
        if (listItemChosen.Count > 0)
        {
            BuyItem?.Invoke(new Dictionary<ItemData, int>(listItemChosen));
            ResetTradeSelection();
            listItemChosen.Clear();
        }
        else
        {
            Debug.Log("Không có vật phẩm nào để mua!");
        }
    }

    public void SelectCraftItem(ItemData itemData)
    {
        currentItemSelected = itemData;

        if (itemIconImage != null && itemData.icon != null)
        {
            itemIconImage.sprite = itemData.icon;
            itemIconImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(" ItemData hoặc Image chưa được gán đúng!");
        }

        ShowRequiredItems(itemData.requiredItems, itemData.requiredAmounts, DataManagers.instance.GetItemSpriteFromName);
        Debug.Log(" Đã chọn " + itemData.itemName);
    }

    public void CreateItem()
    {
        int num = 1;
        bool hasAllItems = true;

        for (int i = 0; i < currentItemSelected.requiredItems.Length; i++)
        {
            string itemName = currentItemSelected.requiredItems[i];
            if (DataManagers.instance.GetItemDataByName(itemName) == null)
            {
                Debug.LogError("Không tìm thấy dữ liệu cho vật phẩm: " + itemName);
                hasAllItems = false;
                break;
            }

            int requiredAmount = currentItemSelected.requiredAmounts[i];
            int inventoryAmount = InventoryManager.Instance.GetInventory().GetItemCountByName(itemName);

            if (inventoryAmount < requiredAmount * num)
            {
                hasAllItems = false;
                Debug.LogError($"Thiếu {itemName} để tạo {currentItemSelected.itemName}. Cần {requiredAmount * num} nhưng chỉ có {inventoryAmount}");
                break;
            }
        }

        if (hasAllItems)
        {
            EventBus.Publish(new ItemPickedUp(currentItemSelected.itemName, 1));
            Debug.Log($"Đã tạo {currentItemSelected.itemName} x1");

            for (int j = 0; j < currentItemSelected.requiredItems.Length; j++)
            {
                string itemName = currentItemSelected.requiredItems[j];
                int amountToRemove = currentItemSelected.requiredAmounts[j] * num;
                InventoryManager.Instance.GetInventory().RemoveItemByName(itemName, amountToRemove);
                Debug.Log($"Đã trừ {itemName} x{amountToRemove} khỏi kho");
            }

            listItemChosen[currentItemSelected] = num;
            ResetTradeSelection();
        }
        else
        {
            Debug.LogError("Không đủ nguyên liệu để tạo vật phẩm!");
        }
    }
}
