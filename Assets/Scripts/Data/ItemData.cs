using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData", order = 0)]
public class ItemData : ScriptableObject
{
    [Header("Settings")]
    public string itemName;
    public ItemType itemType;
    public EquipType equipType;
    public Item itemPrefab;
    public Sprite icon;
    public int price;
    public Sprite[] requiredItemsIcon;
    public string[] requiredItems;
    public int[] requiredAmounts;
    public int damage;
    public int durability;
    public float plantHealth = 100f;
    public float timeToGrowUp;
    public bool FullyGown;
}
