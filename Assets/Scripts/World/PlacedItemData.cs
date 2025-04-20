using System;
using UnityEngine;

[Serializable]
public class PlacedItemData
{
    public string itemName;

    public string plantedItemName;

    public Vector3 position;

    public Quaternion rotation;

    public string fieldID;

    public bool isFullyGrown;

    public float timeToGrowUp;

    public TileFieldState tileFieldState;
    public PlacedItemData(string name, Vector3 pos, Quaternion rot)
    {
        this.itemName = name;
        this.position = pos;
        this.rotation = rot;
    }

    public PlacedItemData(string itemName, string plantedItemName, Vector3 position, Quaternion rotation, string fieldID, bool isFullyGrown, float timeToGrowUp, TileFieldState tileFieldState)
    {
        this.itemName = itemName;
        this.plantedItemName = plantedItemName;
        this.position = position;
        this.rotation = rotation;
        this.fieldID = fieldID;
        this.isFullyGrown = isFullyGrown;
        this.timeToGrowUp = timeToGrowUp;
        this.tileFieldState = tileFieldState;
    }


}
