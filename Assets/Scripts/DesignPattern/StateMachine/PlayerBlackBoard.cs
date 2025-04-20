using System;
using UnityEngine;

public class PlayerBlackBoard : EntityBlackboard
{
    [Header("State")]
    public IdleStatePlayer idlePlayer;
    public MoveState moveState;
    public SowState sowState;
    public HarvestState harvestState;
    public WaterState waterState;
    public HoeState hoeState;
    public CutState cutState;
    public MiningState miningState;
    public JumpState jumpState;
    public SleepState sleepState;

    [Header("Elements")]
    public PlayerToolSelector playerToolSelector;
    public Transform playerTransform;

    [Header("Attribute")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float Axedamage = 1f;
    public float Pickaxedamage = 1f;
    public float speed;
    public float runThreshold = 0.5f;
    public float moveSpeedMultiplier = 100f;
    public float JumpForce = 100f;

    [Header("Crop & seed")]
    public ItemData seed;

    [Header("Check Variable")]
    public bool isGround;
    public bool sowButtonPressed;
    public bool removeWormsButtonPressed;
    public bool harvestButtonPress;
    public bool waterButtonPressed;
    public bool hoeButtonPressed;
    public bool cutButtonPressed;
    public bool miningButtonPressed;
    public bool jumpButtonPressed;
    public bool sleepButtonPressed;
    public bool shovelButtonPressed;
    public bool timeToSleep;
    public bool isTree;
    public bool isOre;
    public bool isFarmArea;
}
