﻿using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListEquipment: MonoBehaviour
{
    [SerializeField] private List<ItemData> equippedItems = new List<ItemData>();

    public static ListEquipment Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is null! Cannot add item.");
            return;
        }

        // Kiểm tra nếu item đã được trang bị
        if (!equippedItems.Contains(itemData))
        {
            equippedItems.Add(itemData);
            Debug.Log($"Equipped: {itemData.itemName} (Type: {itemData.equipType})");
        }
        else
        {
            Debug.Log($"{itemData.itemName} is already equipped (Type: {itemData.equipType}).");
        }
    }



    public void RemoveItem(ItemData itemData)
    {
        if (equippedItems.Contains(itemData))
        {
            equippedItems.Remove(itemData);
            Debug.Log($"Unequipped: {itemData.itemName}");
        }
        else
        {
            Debug.LogWarning($"{itemData.itemName} is not equipped.");
        }
    }

    public bool IsEquipped(ItemData itemData)
    {
        return equippedItems.Contains(itemData);
    }

    public List<ItemData> GetEquippedItems()
    {
        if (equippedItems == null)
        {
            Debug.LogError("Danh sách trang bị chưa được khởi tạo.");
            return new List<ItemData>(); // Trả về một danh sách trống nếu chưa khởi tạo.
        }
        return equippedItems;
    }


    // Tìm item có equipType trùng
    public ItemData GetEquippedItemByEquipType(EquipType equipType)
    {
        return equippedItems.Find(item => item.equipType == equipType);
    }

    public void Clear()
    {
        equippedItems.Clear();
    }

    public int GetDurabilityIfEquipped(ItemData itemData)
    {
        if (itemData != null && equippedItems.Contains(itemData))
        {
            return itemData.durability;
        }

        Debug.LogWarning($"{itemData?.itemName} không được trang bị hoặc không hợp lệ.");
        return 0; // Trả về -1 nếu không tìm thấy hoặc không hợp lệ
    }

}
