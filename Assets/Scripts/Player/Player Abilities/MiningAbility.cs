using System;
using UnityEngine;

public class MiningAbility : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField]
    public Transform checkGameObject;
    [Header(" Settings ")]
    private bool canCut;
    private PlayerBlackBoard blackBoard;
    [Header("Action")]
    public static Action doDamageToOre;
    private void OnEnable()
    {
        MiningState.NotifyMining = (Action)Delegate.Combine(MiningState.NotifyMining, new Action(this.OnMiningButtonPressed));
    }

    private void OnDisable()
    {
        MiningState.NotifyMining = (Action)Delegate.Remove(MiningState.NotifyMining, new Action(this.OnMiningButtonPressed));
    }

    private void Start()
    {
        this.blackBoard = base.GetComponentInParent<PlayerBlackBoard>();
    }

    public void OnMiningButtonPressed()
    {
        if (!this.blackBoard.isOre)
        {
            return;
        }
        Action action = MiningAbility.doDamageToOre;
        if (action == null)
        {
            return;
        }
        action();
    }

 
}
