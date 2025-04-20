using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreRock : MonoBehaviour, IDamageAble
{
    [Header("Ore Properties")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;
    private Vector3 originalPosition;
    private bool canDropItem;

    [Header("Actions")]
    public static Action decreasedPickAxeDurability;

    [Header("Drop Table (Editable)")]
    public List<DropItem> dropTable = new List<DropItem>();

    private void Start()
    {
        canDropItem = true;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        originalPosition = transform.position;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        PlayerStatusManager.Instance.UseStamina(1f);
        decreasedPickAxeDurability?.Invoke();

        ItemData pickaxe = ListEquipment.Instance?.GetEquippedItemByEquipType(EquipType.Pickaxe);
        if (pickaxe != null)
        {
            pickaxe.durability--;
            if (pickaxe.durability <= 0)
            {
                ListEquipment.Instance.RemoveItem(pickaxe);
                Debug.Log($"{pickaxe.itemName} đã bị hỏng và bị gỡ khỏi trang bị!");
            }
        }

        if (currentHealth <= 0f)
        {
            MineOre();
            ExecuteOre();
        }
    }

    private void MineOre()
    {
        if (canDropItem)
        {
            DropItem? randomDrop = GetRandomDrop();
            if (randomDrop.HasValue)
            {
                InventoryManager.Instance.PickUpItemCallBack(
                    randomDrop.Value.itemData.itemName,
                    randomDrop.Value.dropAmount
                );
            }

            if (ObjectPool.Instance != null)
            {
                ObjectPool.Instance.ReturnObject(gameObject);
                ObjectPool.Instance.StartCoroutine(RespawnOre());
            }
            else
            {
                Debug.LogError("ObjectPool.Instance is null! Make sure ObjectPool exists in the scene.");
            }
        }
    }

    private IEnumerator RespawnOre()
    {
        yield return new WaitForSeconds(600f);
        GameObject respawned = ObjectPool.Instance.GetObject(originalPosition);
        respawned.GetComponent<OreRock>().ResetOre();
    }

    public void ResetOre()
    {
        currentHealth = maxHealth;
        canDropItem = true;
        transform.position = originalPosition;
        gameObject.SetActive(true);
    }

    private DropItem? GetRandomDrop()
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (DropItem drop in dropTable)
        {
            cumulative += drop.dropRate;
            if (rand <= cumulative)
                return drop;
        }

        return null;
    }

    private void ExecuteOre()
    {
        if (currentHealth < 1f)
        {
            gameObject.SetActive(false);
            canDropItem = false;
        }
    }

    [Serializable]
    public struct DropItem
    {
        public ItemData itemData;
        public float dropRate;
        public int dropAmount;
    }
}
