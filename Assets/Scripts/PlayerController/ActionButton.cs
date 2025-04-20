using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [Header("Elements")]
    private PlayerToolSelector playerToolSelector;

    private PlayerBlackBoard blackBoard;

    [Header("Settings")]
    [SerializeField]
    private Button actionButton;

    [SerializeField]
    private Button sowButton;

    [SerializeField]
    private Button waterButton;

    [SerializeField]
    private Button harvestButton;

    [SerializeField]
    private Button sleepButton;

    [SerializeField]
    private Button removeWormsButton;

    public CropField cropField;

    [Header("Actions")]
    public static Action Hoeing;
    public static Action Cutting;
    public static Action Building;
    public static Action Shoveling;
    private void Start()
    {
        this.sowButton.gameObject.SetActive(false);
        this.waterButton.gameObject.SetActive(false);
        this.harvestButton.gameObject.SetActive(false);
        this.sleepButton.gameObject.SetActive(false);
        this.blackBoard = base.GetComponentInParent<PlayerBlackBoard>();
        this.playerToolSelector = base.GetComponent<PlayerToolSelector>();
        this.actionButton.onClick.AddListener(new UnityAction(this.DoAction));
        this.actionButton.gameObject.SetActive(false);
        this.removeWormsButton.gameObject.SetActive(false);
        PlayerToolSelector playerToolSelector = this.playerToolSelector;
        playerToolSelector.onToolSelected = (Action<PlayerToolSelector.Tool>)Delegate.Combine(playerToolSelector.onToolSelected, new Action<PlayerToolSelector.Tool>(this.OnToolChanged));
        CheckGameObject.EnableSowBTTN = (Action<bool>)Delegate.Combine(CheckGameObject.EnableSowBTTN, new Action<bool>(this.EnableSowButton));
        CheckGameObject.EnableWaterBTTN = (Action<bool>)Delegate.Combine(CheckGameObject.EnableWaterBTTN, new Action<bool>(this.EnableWaterButton));
        CheckGameObject.EnableHarvestBTTN = (Action<bool>)Delegate.Combine(CheckGameObject.EnableHarvestBTTN, new Action<bool>(this.EnableHarvestButton));
        CheckGameObject.EnableSleepBTTN = (Action<bool>)Delegate.Combine(CheckGameObject.EnableSleepBTTN, new Action<bool>(this.EnableSleepButton));
    }

    private void OnDestroy()
    {
        CheckGameObject.EnableSowBTTN = (Action<bool>)Delegate.Remove(CheckGameObject.EnableSowBTTN, new Action<bool>(this.EnableSowButton));
        CheckGameObject.EnableWaterBTTN = (Action<bool>)Delegate.Remove(CheckGameObject.EnableWaterBTTN, new Action<bool>(this.EnableWaterButton));
        CheckGameObject.EnableHarvestBTTN = (Action<bool>)Delegate.Remove(CheckGameObject.EnableHarvestBTTN, new Action<bool>(this.EnableHarvestButton));
        CheckGameObject.EnableSleepBTTN = (Action<bool>)Delegate.Remove(CheckGameObject.EnableSleepBTTN, new Action<bool>(this.EnableSleepButton));
        PlayerToolSelector playerToolSelector = this.playerToolSelector;
        playerToolSelector.onToolSelected = (Action<PlayerToolSelector.Tool>)Delegate.Remove(playerToolSelector.onToolSelected, new Action<PlayerToolSelector.Tool>(this.OnToolChanged));
    }

    private void OnToolChanged(PlayerToolSelector.Tool selectedTool)
    {
        bool active = selectedTool == PlayerToolSelector.Tool.Hoe || selectedTool == PlayerToolSelector.Tool.Axe || selectedTool == PlayerToolSelector.Tool.Hammer || selectedTool == PlayerToolSelector.Tool.Pickaxe || selectedTool == PlayerToolSelector.Tool.Shovel;
        this.actionButton.gameObject.SetActive(active);
    }

    public void DoAction()
    {
        if (this.playerToolSelector.activeTool == PlayerToolSelector.Tool.Hoe)
        {
            this.blackBoard.hoeButtonPressed = true;
            return;
        }
        if (this.playerToolSelector.activeTool == PlayerToolSelector.Tool.Axe)
        {
            this.blackBoard.cutButtonPressed = true;
            return;
        }
        if (this.playerToolSelector.activeTool == PlayerToolSelector.Tool.Hammer)
        {
            Action building = ActionButton.Building;
            if (building == null)
            {
                return;
            }
            building();
            return;
        }
        else
        {
            if (this.playerToolSelector.activeTool == PlayerToolSelector.Tool.Pickaxe)
            {
                this.blackBoard.miningButtonPressed = true;
                return;
            }
            if (this.playerToolSelector.activeTool == PlayerToolSelector.Tool.Shovel)
            {
                Action shoveling = ActionButton.Shoveling;
                if (shoveling == null)
                {
                    return;
                }
                shoveling();
            }
            return;
        }
    }

    public void OnSleepButtonPressed()
    {
        this.blackBoard.sleepButtonPressed = true;
    }

    public void EnableSleepButton(bool enable)
    {
        this.sleepButton.gameObject.SetActive(enable);
    }

    public void EnableSowButton(bool enable)
    {
        this.sowButton.gameObject.SetActive(enable);
    }

    public void EnableWaterButton(bool enable)
    {
        this.waterButton.gameObject.SetActive(enable);
    }

    public void EnableHarvestButton(bool enable)
    {
        this.harvestButton.gameObject.SetActive(enable);
    }

    public void EnableremoveWormsButton(bool enable)
    {
        this.removeWormsButton.gameObject.SetActive(enable);
    }

    public void OnJumpButtonPressed()
    {
        this.blackBoard.jumpButtonPressed = true;
    }

    public void OnShovelButtonPressed()
    {
        this.blackBoard.shovelButtonPressed = true;
    }

    
}
