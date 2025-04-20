using System;
using System.Collections.Generic;
using UnityEngine;

public class TraderInteraction : MonoBehaviour
{
    // Elements
    [Header("Elements")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [SerializeField]
    private GameObject WindowPanel;

    // Actions
    [Header("Actions")]
    public static Action EnableMarketWindow;
    public static Action<bool> OpenedMarketWindow;

    private void Start()
    {
        // Subscribe to trade events
        TradeWindow.SellItem += OnSellItem;
        TradeWindow.BuyItem += OnBuyItem;

        InitializeSettings();
    }

    private void InitializeSettings()
    {
        WindowPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe from trade events
        TradeWindow.SellItem -= OnSellItem;
        TradeWindow.BuyItem -= OnBuyItem;
    }

    private void OnSellItem(Dictionary<ItemData, int> soldItems)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned!");
            return;
        }

        Inventory inventory = inventoryManager.GetInventory();
        int totalEarned = 0;

        foreach (var item in soldItems)
        {
            ItemData itemData = item.Key;
            int quantity = item.Value;

            InventoryItem[] inventoryItems = inventory.GetInventoryItems();
            bool itemSold = false;

            foreach (var inventoryItem in inventoryItems)
            {
                if (inventoryItem.itemName == itemData.itemName)
                {
                    int itemPrice = DataManagers.instance.GetItemPriceFromItemName(inventoryItem.itemName);

                    if (inventoryItem.amount >= quantity)
                    {
                        totalEarned += itemPrice * quantity;
                        inventory.RemoveItemByName(inventoryItem.itemName, quantity);
                        itemSold = true;
                        break;
                    }

                    Debug.LogError($"Not enough {itemData.itemName} to sell! (Required: {quantity}, Available: {inventoryItem.amount})");
                    break;
                }
            }

            if (!itemSold)
                break;
        }

        Debug.Log($"Earned {totalEarned} coins.");
        CashManager.instance.AddCoins(totalEarned);
        inventoryManager.GetInventoryDisplay().UpdateDisplay(inventory);
    }

    private void OnBuyItem(Dictionary<ItemData, int> boughtItems)
    {
        Debug.Log("Buy item trade");

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned!");
            return;
        }

        Inventory inventory = inventoryManager.GetInventory();
        int totalSpent = 0;

        foreach (var item in boughtItems)
        {
            ItemData itemData = item.Key;
            int quantity = item.Value;
            int totalPrice = DataManagers.instance.GetItemPriceFromItemName(itemData.itemName) * quantity;

            if (CashManager.instance.GetCoins() >= totalPrice)
            {
                totalSpent += totalPrice;
                CashManager.instance.SpendCoins(totalPrice);
                inventory.AddItemByName(itemData.itemName, quantity);
                Debug.Log($"Bought {itemData.itemName} x{quantity} for {totalPrice} coins.");
            }
            else
            {
                Debug.LogError($"Not enough coins to buy {itemData.itemName} x{quantity}!");
            }
        }

        Debug.Log($"Spent {totalSpent} coins to buy items.");
        inventoryManager.GetInventoryDisplay().UpdateDisplay(inventory);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected - Opening trade window!");

            if (WindowPanel == null)
            {
                Debug.LogError("WindowPanel is not assigned! Cannot open window.");
                return;
            }

            EnableMarketWindow?.Invoke();
            OpenedMarketWindow?.Invoke(true);

            WindowPanel.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Left - Closing trade window!");
            WindowPanel.gameObject.SetActive(false);
        }
    }

    public void CloseMarketWindow()
    {
        TooltipManager.Instance.HideTooltip();
        WindowPanel.gameObject.SetActive(false);
        OpenedMarketWindow?.Invoke(false);
    }
}
