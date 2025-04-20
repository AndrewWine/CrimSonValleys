using System;
using UnityEngine;
using UnityEngine.UI;

public class CheckUICageStatus : MonoBehaviour
{
    // UI Elements
    [Header("UI Elements")]
    [SerializeField]
    private Button feedButton;

    [SerializeField]
    private Button harvestButton;

    public Cage activeCage;
    public static event Action<int> OnFeed;

    protected virtual void Start()
    {
        GameObject gameObject = GameObject.Find("CageUI");
        if (gameObject == null)
        {
            Debug.LogWarning("CageUI not found!");
            return;
        }
        Transform transform = gameObject.transform.Find("FeedButton");
        this.feedButton = ((transform != null) ? transform.GetComponent<Button>() : null);
        Transform transform2 = gameObject.transform.Find("HarvestButton");
        this.harvestButton = ((transform2 != null) ? transform2.GetComponent<Button>() : null);
        if (!this.feedButton)
        {
            Debug.LogWarning("FeedButton not found in CageUI!");
        }
        if (!this.harvestButton)
        {
            Debug.LogWarning("HarvestButton not found in CageUI!");
        }
        this.InitializeUI();
    }

    private void InitializeUI()
    {
        Button button = this.feedButton;
        if (button != null)
        {
            button.gameObject.SetActive(false);
        }
        Button button2 = this.harvestButton;
        if (button2 == null)
        {
            return;
        }
        button2.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Cage"))
        {
            return;
        }
        Cage component = other.GetComponent<Cage>();
        if (component == null)
        {
            Debug.LogWarning("Cage component missing!");
            return;
        }
        this.activeCage = component;
        this.UpdateCageUI(component);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cage") && this.activeCage != null)
        {
            this.activeCage = null;
            this.ResetUI();
        }
    }

    private void UpdateCageUI(Cage cage)
    {
        bool active = cage.state == CageState.Hungry;
        bool active2 = cage.state == CageState.TakeProduce;
        Button button = this.feedButton;
        if (button != null)
        {
            button.gameObject.SetActive(active);
        }
        Button button2 = this.harvestButton;
        if (button2 == null)
        {
            return;
        }
        button2.gameObject.SetActive(active2);
    }

    private void ResetUI()
    {
        Button button = this.feedButton;
        if (button != null)
        {
            button.gameObject.SetActive(false);
        }
        Button button2 = this.harvestButton;
        if (button2 == null)
        {
            return;
        }
        button2.gameObject.SetActive(false);
    }

    public void FeedAnimal()
    {
        if (this.activeCage == null)
        {
            return;
        }
        int num = 50;
        Action<int> onFeed = CheckUICageStatus.OnFeed;
        if (onFeed != null)
        {
            onFeed(num);
        }
        this.activeCage.HandleFeeding(num);
        this.activeCage.state = CageState.Empty;
        this.UpdateCageUI(this.activeCage);
    }

    public void TakeProduce()
    {
        if (this.activeCage == null || this.activeCage.state != CageState.TakeProduce)
        {
            return;
        }
        this.activeCage.HarvestItem();
        this.activeCage.ResetHarvestTimer();
        this.activeCage.state = CageState.Empty;
        this.UpdateCageUI(this.activeCage);
    }

 
}
