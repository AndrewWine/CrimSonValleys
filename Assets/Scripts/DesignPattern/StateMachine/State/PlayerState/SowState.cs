using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class SowState : PlayerState
{
    [SerializeField]
    private CheckGameObject checkGameObject;
    public override void Enter()
    {
        base.Enter();
        this.blackboard.animator.Play("Sow");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public void OnSowButtonPressed()
    {
        blackboard.sowButtonPressed = false;
        CropField currentCropField = checkGameObject.GetCurrentCropField();
        if (currentCropField == null)
        {
           return;
        }
        currentCropField.Sow(blackboard.seed);
        stateMachine.ChangeState(blackboard.idlePlayer);
        return;

    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (isAnimationFinished)
        {
            OnSowButtonPressed();
            stateMachine.ChangeState(blackboard.idlePlayer);

        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
      
    }

    
}
