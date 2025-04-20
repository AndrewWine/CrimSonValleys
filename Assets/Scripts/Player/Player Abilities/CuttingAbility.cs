using System;
using UnityEngine;

public class CuttingAbility : MonoBehaviour
{
    [SerializeField]
    public Transform checkGameObject;

    private bool canCut;

    private PlayerBlackBoard blackBoard;

    public static Action<Transform, float> Cutting;

    public static Action causeDamage;
    private void Start()
    {
        this.blackBoard = base.GetComponentInParent<PlayerBlackBoard>();
        CutState.notifyCutting = (Action)Delegate.Combine(CutState.notifyCutting, new Action(this.OnCuttingButtonPressed));
    }

    private void OnDestroy()
    {
        CutState.notifyCutting = (Action)Delegate.Remove(CutState.notifyCutting, new Action(this.OnCuttingButtonPressed));
    }

    public void OnCuttingButtonPressed()
    {
        if (!this.blackBoard.isTree)
        {
            Debug.Log("No tree detected!");
            return;
        }
        PlayerStatusManager.Instance.UseStamina(1f);
        Action action = CuttingAbility.causeDamage;
        if (action == null)
        {
            return;
        }
        action();
    }

    
}
