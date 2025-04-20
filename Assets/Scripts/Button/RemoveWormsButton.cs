using System;
using UnityEngine;

public class RemoveWormsButton : MonoBehaviour
{
    public void PressRemoveWormsButton()
    {
        Action action = RemoveWormsButton.removeWorms;
        if (action == null)
        {
            return;
        }
        action();
    }

    public static Action removeWorms;
}
