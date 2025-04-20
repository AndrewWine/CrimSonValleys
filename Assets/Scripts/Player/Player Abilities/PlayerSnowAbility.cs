using System;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class PlayerSnowAbility : MonoBehaviour
{
    [Header(" Elements ")]
    private PlayerToolSelector playerToolSelector;

    [SerializeField]
    private GameObject SeedUI;

    private PlayerBlackBoard blackBoard;

    [Header(" Actions ")]
    public static Action<ItemData> SownNotify;

    public static Action generateSeedUIButton;

    private bool unlockCropField;
    private void Start()
    {
        this.playerToolSelector = base.transform.parent.GetComponentInChildren<PlayerToolSelector>();
        this.blackBoard = base.GetComponentInParent<PlayerBlackBoard>();
        PlayerToolSelector playerToolSelector = this.playerToolSelector;
        playerToolSelector.onToolSelected = (Action<PlayerToolSelector.Tool>)Delegate.Combine(playerToolSelector.onToolSelected, new Action<PlayerToolSelector.Tool>(this.ToolSelectedCallBack));
        this.SeedUI.SetActive(false);
        CheckGameObject.UnlockCropField = (Action<bool>)Delegate.Combine(CheckGameObject.UnlockCropField, new Action<bool>(this.UnlockCropFieldHandler));
        UISelectButton.seedButtonPressed = (Action<ItemData>)Delegate.Combine(UISelectButton.seedButtonPressed, new Action<ItemData>(this.SelectSeeds));
    }

    private void OnDisable()
    {
        PlayerToolSelector playerToolSelector = this.playerToolSelector;
        playerToolSelector.onToolSelected = (Action<PlayerToolSelector.Tool>)Delegate.Remove(playerToolSelector.onToolSelected, new Action<PlayerToolSelector.Tool>(this.ToolSelectedCallBack));
        CheckGameObject.UnlockCropField = (Action<bool>)Delegate.Remove(CheckGameObject.UnlockCropField, new Action<bool>(this.UnlockCropFieldHandler));
        UISelectButton.seedButtonPressed = (Action<ItemData>)Delegate.Remove(UISelectButton.seedButtonPressed, new Action<ItemData>(this.SelectSeeds));
    }

    private void ToolSelectedCallBack(PlayerToolSelector.Tool selectedTool)
    {
        if (!this.playerToolSelector.CanSow())
        {
            this.SeedUI.SetActive(false);
            return;
        }
        this.SeedUI.SetActive(true);
        Action action = PlayerSnowAbility.generateSeedUIButton;
        if (action == null)
        {
            return;
        }
        action();
    }

    // Token: 0x060005B1 RID: 1457 RVA: 0x0001B9BB File Offset: 0x00019BBB
    private void UnlockCropFieldHandler(bool unlock)
    {
        this.unlockCropField = unlock;
    }

    // Token: 0x060005B2 RID: 1458 RVA: 0x0001B9C4 File Offset: 0x00019BC4
    private void SelectSeeds(ItemData seed)
    {
        this.blackBoard.seed = seed;
        Debug.Log(" Chọn hạt giống: " + seed.itemName);
    }

   
}
