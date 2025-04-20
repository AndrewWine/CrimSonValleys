using System;
using UnityEngine;

public enum NPCStateType
{
    Idle,
    Move,
    Chop,
    PickAxe,
    PlantTree
}

[CreateAssetMenu(fileName = "NPCScheduleData", menuName = "NPC/Schedule Data")]
public class NPCScheduleData : ScriptableObject
{
    [Header("Danh sách lịch biểu (sắp xếp theo thời gian tăng dần)")]
    public NPCScheduleData.ScheduleEntry[] scheduleEntries;
    [Serializable]
    public class ScheduleEntry
    {
        [Tooltip("Time begin")]
        public float startTime;

        public float endTime;

        [Tooltip("State NPC switch")]
        public NPCStateType desiredState;

        [Tooltip("If MoveState, use target name to find transform in scene")]
        public string targetName;
    }
}
