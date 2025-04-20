using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackSmithInteraction : UIRequirementDisplay
{
    [Header("Elements")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject WindowShopPanel;
    [SerializeField] private GameObject WindowCraftingPanel;
    [Header("Actions")]
    public static Action EnableSmithyWindow;
    public static Action generateItemRequire;
    public static Action<bool> OpenedSmithyWindow;
    private void Start()
    {
        this.InitializeSettings();
    }

    private void OnEnable()
    {
        BlackSmithWindow.BuyItem = (Action<Dictionary<ItemData, int>>)Delegate.Combine(BlackSmithWindow.BuyItem, new Action<Dictionary<ItemData, int>>(this.OnBuyItem));
        BlackSmithWindow.SellItem = (Action<Dictionary<ItemData, int>>)Delegate.Combine(BlackSmithWindow.SellItem, new Action<Dictionary<ItemData, int>>(this.OnSellItem));
    }

    private void InitializeSettings()
    {
        this.WindowShopPanel.gameObject.SetActive(false);
        this.WindowCraftingPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        BlackSmithWindow.BuyItem = (Action<Dictionary<ItemData, int>>)Delegate.Remove(BlackSmithWindow.BuyItem, new Action<Dictionary<ItemData, int>>(this.OnBuyItem));
        BlackSmithWindow.SellItem = (Action<Dictionary<ItemData, int>>)Delegate.Remove(BlackSmithWindow.SellItem, new Action<Dictionary<ItemData, int>>(this.OnSellItem));
    }

    private void OnSellItem(Dictionary<ItemData, int> soldItems)
    {
        if (this.inventoryManager == null)
        {
            Debug.LogError("InventoryManager chưa được gán!");
            return;
        }

        Inventory inventory = this.inventoryManager.GetInventory();
        int totalCoins = 0;

        foreach (KeyValuePair<ItemData, int> pair in soldItems)
        {
            ItemData item = pair.Key;
            int amountToSell = pair.Value;
            InventoryItem[] inventoryItems = inventory.GetInventoryItems();

            foreach (InventoryItem invItem in inventoryItems)
            {
                if (invItem.itemName == item.itemName)
                {
                    int price = DataManagers.instance.GetItemPriceFromItemName(invItem.itemName);
                    if (invItem.amount >= amountToSell)
                    {
                        totalCoins += price * amountToSell;
                        inventory.RemoveItemByName(invItem.itemName, amountToSell);
                        break;
                    }
                    else
                    {
                        Debug.LogError($"Không đủ {item.itemName} để bán! (Yêu cầu: {amountToSell}, Hiện có: {invItem.amount})");
                        break;
                    }
                }
            }
        }

        Debug.Log($"Đã kiếm được {totalCoins} coins.");
        CashManager.instance.AddCoins(totalCoins);
        this.inventoryManager.GetInventoryDisplay().UpdateDisplay(inventory);
    }

    private void OnBuyItem(Dictionary<ItemData, int> boughtItems)
    {
        Debug.Log("Buy item smithy");

        if (this.inventoryManager == null)
        {
            Debug.LogError("InventoryManager chưa được gán!");
            return;
        }

        int totalSpent = 0;

        foreach (KeyValuePair<ItemData, int> pair in boughtItems)
        {
            ItemData item = pair.Key;
            int quantity = pair.Value;

            int itemCost = DataManagers.instance.GetItemPriceFromItemName(item.itemName) * quantity;

            if (CashManager.instance.GetCoins() >= itemCost)
            {
                totalSpent += itemCost;
                CashManager.instance.SpendCoins(itemCost);
                this.inventoryManager.PickUpItemCallBack(item.itemName, quantity);
                Debug.Log($"Mua {item.itemName} x{quantity} với giá {itemCost} coins.");
            }
            else
            {
                Debug.LogError($"Không đủ tiền để mua {item.itemName} x{quantity}!");
            }
        }

        Debug.Log($"Đã tiêu {totalSpent} coins để mua vật phẩm.");
        this.inventoryManager.GetInventoryDisplay().UpdateDisplay(this.inventoryManager.GetInventory());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected - Mở bảng giao dịch!");
            if (this.WindowShopPanel == null)
            {
                Debug.LogError("WindowPanel chưa được gán! Không thể mở cửa sổ.");
                return;
            }

            EnableSmithyWindow?.Invoke();
            generateItemRequire?.Invoke();
            OpenedSmithyWindow?.Invoke(true);

            Debug.Log("Mở ShopWindow");
            this.WindowShopPanel.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(" Player Rời Khỏi - Đóng bảng giao dịch!");
            this.WindowShopPanel.gameObject.SetActive(false);
        }
    }

    public void CloseMarketWindow()
    {
        TooltipManager.Instance.HideTooltip();
        this.WindowShopPanel.gameObject.SetActive(false);
        OpenedSmithyWindow?.Invoke(false);
    }

    public void OnButtonCraftPressed()
    {
        this.WindowCraftingPanel.gameObject.SetActive(true);
        this.WindowShopPanel.gameObject.SetActive(false);
    }

    public void OnShopCraftButtonPressed()
    {
        this.WindowShopPanel.gameObject.SetActive(true);
        this.WindowCraftingPanel.gameObject.SetActive(false);
    }

  
}
