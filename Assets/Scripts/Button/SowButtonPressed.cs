using System;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000041 RID: 65
public class SowButtonPressed : MonoBehaviour
{
    private PlayerBlackBoard blackboard;
    private void Start()
    {
        this.blackboard = base.GetComponentInParent<PlayerBlackBoard>();
    }

    public void PressSowButton()
    {
        this.blackboard.sowButtonPressed = true;
    }

    
}
