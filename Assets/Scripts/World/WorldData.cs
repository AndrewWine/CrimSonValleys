using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldData
{
    public float _timeOfDay;
    public int _dayNumber;
    public int _yearNumber;
    public int _yearLength;
    public string gameDateTime;
    public Vector3 playerPosition;
    public int coins;
    public float stamina;

    public List<PlacedItemData> placedItems = new List<PlacedItemData>();
    public List<PlacedBuildingData> placedBuildings = new List<PlacedBuildingData>();
    public List<DestroyedTreeData> destroyedTrees = new List<DestroyedTreeData>();
    public List<SavedQuestData> activeQuests = new List<SavedQuestData>();
    public List<string> completedQuestNames = new List<string>();
    public List<Vector3Int> tileGridPositions;
}
