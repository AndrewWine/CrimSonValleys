using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifyMessage : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject failMessage;
    [SerializeField] private GameObject completeMessage;
    [SerializeField] private GameObject itemProcess;
    [SerializeField] private GameObject BacKGround;

    private Image itemIcon;
    private TextMeshProUGUI itemCount;
    private Queue<ShowItemPickup> itemQueue = new Queue<ShowItemPickup>();
    private bool isShowingItem;

    private void Start()
    {
        ConfigUI();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ShowItemPickup>(OnShowItemPickup);
        BuildingSystem.NotifyFailMessage += TriggerFailMessage;
        QuestManager.NotifyCompleteMessage += TriggerCompleteMessage;
        QuestManager.NotifyFailMessage += TriggerFailMessage;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ShowItemPickup>(OnShowItemPickup);
        BuildingSystem.NotifyFailMessage -= TriggerFailMessage;
        QuestManager.NotifyCompleteMessage -= TriggerCompleteMessage;
        QuestManager.NotifyFailMessage -= TriggerFailMessage;
    }

    private void ConfigUI()
    {
        failMessage.SetActive(false);
        completeMessage.SetActive(false);
        itemProcess.SetActive(false);
        BacKGround.SetActive(false);
        itemIcon = itemProcess.GetComponentInChildren<Image>();
        itemCount = itemProcess.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void TriggerFailMessage()
    {
        StartCoroutine(ShowTemporaryMessage(failMessage));
    }

    public void TriggerCompleteMessage()
    {
        StartCoroutine(ShowTemporaryMessage(completeMessage));
    }

    private void OnShowItemPickup(ShowItemPickup eventData)
    {
        itemQueue.Enqueue(eventData);
        if (!isShowingItem)
        {
            StartCoroutine(ProcessItemQueue());
        }
    }

    private IEnumerator ProcessItemQueue()
    {
        isShowingItem = true;
        while (itemQueue.Count > 0)
        {
            ShowItemPickup showItemPickup = itemQueue.Dequeue();
            ItemData itemDataByName = DataManagers.instance.GetItemDataByName(showItemPickup.itemName);
            if (itemDataByName != null)
            {
                itemIcon.sprite = itemDataByName.icon;
                itemIcon.rectTransform.sizeDelta = new Vector2(100f, 100f);
                itemCount.text = "x" + showItemPickup.itemAmount;
                itemProcess.SetActive(false);
                yield return null;
                itemProcess.SetActive(true);
                BacKGround.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                itemProcess.SetActive(false);
                BacKGround.SetActive(false);
            }
        }
        isShowingItem = false;
    }

    private IEnumerator ShowTemporaryMessage(GameObject target)
    {
        target.SetActive(true);
        yield return new WaitForSeconds(2f);
        target.SetActive(false);
    }
}
