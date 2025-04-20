using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropField : Item
{
    // Variables
    public static List<CropField> allCropFields = new List<CropField>();
    public static Action<CropField> onFullyWatered;
    public static Action<CropField> onFullyHarvested;
    public string fieldID;
    public TileFieldState state;
    public ItemData cropData;
    public List<CropTile> cropTiles = new List<CropTile>();
    public List<GameObject> itemPlaced = new List<GameObject>();
    public Transform tilesParent;
    private int tilesHarvested;
    private void Awake()
    {
        if (string.IsNullOrEmpty(this.fieldID))
        {
            this.fieldID = Guid.NewGuid().ToString();
        }
    }

    private void Start()
    {
        this.state = TileFieldState.Empty;
        this.StoreTiles();
        HarvestAbillity.Harvesting = (Action<Transform>)Delegate.Combine(HarvestAbillity.Harvesting, new Action<Transform>(this.Harvest));
        Crop.realdyToHarvest = (Action<CropTile>)Delegate.Combine(Crop.realdyToHarvest, new Action<CropTile>(this.OnCropTileRipened));
        AnimationTriggerCrop.onHarvestTriggered = (Action)Delegate.Combine(AnimationTriggerCrop.onHarvestTriggered, new Action(this.OnHarvestTriggered));
    }

    private void OnEnable()
    {
        if (!CropField.allCropFields.Contains(this))
        {
            CropField.allCropFields.Add(this);
        }
    }

    private void OnDestroy()
    {
        HarvestAbillity.Harvesting = (Action<Transform>)Delegate.Remove(HarvestAbillity.Harvesting, new Action<Transform>(this.Harvest));
        Crop.realdyToHarvest = (Action<CropTile>)Delegate.Remove(Crop.realdyToHarvest, new Action<CropTile>(this.OnCropTileRipened));
        AnimationTriggerCrop.onHarvestTriggered = (Action)Delegate.Remove(AnimationTriggerCrop.onHarvestTriggered, new Action(this.OnHarvestTriggered));
        if (CropField.allCropFields.Contains(this))
        {
            CropField.allCropFields.Remove(this);
        }
    }

    public List<CropTile> GetTiles()
    {
        return this.cropTiles;
    }

    private void StoreTiles()
    {
        for (int i = 0; i < this.tilesParent.childCount; i++)
        {
            CropTile component = this.tilesParent.GetChild(i).GetComponent<CropTile>();
            component.parentField = this;
            this.cropTiles.Add(component);
        }
    }

    public void Sow(ItemData cropData)
    {
        PlayerStatusManager.Instance.UseStamina(1f);
        InventoryManager.Instance.GetInventory().RemoveItemByName(cropData.itemName, 1);
        this.cropData = cropData;
        bool flag = false;
        foreach (CropTile cropTile in this.cropTiles)
        {
            if (cropTile.IsEmpty())
            {
                cropTile.Sow(cropData);
                cropTile.hasCrop = true;
                flag = true;
            }
        }
        if (flag)
        {
            Debug.Log("At least one tile was sown!");
            this.CheckFullySown();
        }
    }

    private void CheckFullySown()
    {
        foreach (CropTile cropTile in this.cropTiles)
        {
            if (cropTile.IsEmpty())
            {
                return;
            }
        }
        this.state = TileFieldState.Sown;
        Debug.Log("All tiles are now sown!");
    }

    public void FieldFullySown()
    {
        this.state = TileFieldState.Sown;
    }

    public void Harvest(Transform harvestSphere)
    {
        if (this.state != TileFieldState.Ripened)
        {
            Debug.LogWarning(string.Format("[{0}] Không thể thu hoạch, cây chưa chín! State hiện tại: {1}", base.gameObject.name, this.state));
            return;
        }
        Debug.Log(string.Format("[{0}] Đã vào function HARVEST, harvestSphere position: {1}, scale: {2}", base.gameObject.name, harvestSphere.position, harvestSphere.localScale));
        float x = harvestSphere.localScale.x;
        bool flag = false;
        for (int i = 0; i < this.cropTiles.Count; i++)
        {
            if (this.cropTiles[i].IsEmpty())
            {
                Debug.Log(string.Format("Tile {0} ({1}) is empty, bỏ qua.", i, this.cropTiles[i].gameObject.name));
            }
            else
            {
                float num = Vector3.Distance(harvestSphere.position, this.cropTiles[i].transform.position);
                Debug.Log(string.Format("Tile {0}: Distance = {1}, Sphere Radius = {2}", i, num, x));
                Debug.Log(string.Format("Tile {0} ({1}) sẵn sàng thu hoạch!", i, this.cropTiles[i].gameObject.name));
                this.HarvestTile(this.cropTiles[i]);
                flag = true;
            }
        }
        if (!flag)
        {
            Debug.LogWarning("[" + base.gameObject.name + "] Không có ô nào được thu hoạch! Kiểm tra lại khoảng cách hoặc trạng thái cây trồng.");
        }
    }

    public bool IsEmpty()
    {
        return this.state == TileFieldState.Empty;
    }

    private void HarvestTile(CropTile cropTile)
    {
        cropTile.Harvest();
        this.tilesHarvested++;
        this.CheckFullyHarvested();
    }

    private void CheckFullyHarvested()
    {
        foreach (CropTile cropTile in this.cropTiles)
        {
            if (!cropTile.IsEmpty())
            {
                return;
            }
        }
        this.FieldFullyHarvested();
    }

    public void FieldFullyHarvested()
    {
        this.state = TileFieldState.Empty;
        Action<CropField> action = CropField.onFullyHarvested;
        if (action != null)
        {
            action(this);
        }
    }

    public void FieldFullyWatered()
    {
        this.state = TileFieldState.Watered;
        Action<CropField> action = CropField.onFullyWatered;
        if (action != null)
        {
            action(this);
        }
        foreach (CropTile cropTile in this.cropTiles)
        {
            if (cropTile.HasCrop())
            {
                cropTile.Water();
            }
        }
        base.StartCoroutine(this.CheckForInfestation());
        this.CheckNearbyCropFieldsAndWater(5f);
    }

    private IEnumerator CheckForInfestation()
    {
        yield return new WaitForSeconds(300f);
        if (UnityEngine.Random.value <= 0.4f)
        {
            this.state = TileFieldState.Infested;
            Debug.Log(base.gameObject.name + " has become infested after watering!");
        }
        yield break;
    }

    private void OnHarvestTriggered()
    {
        this.Harvest(UnityEngine.Object.FindObjectOfType<AnimationTriggerCrop>().transform);
    }

    public List<GameObject> GetItemPlaced()
    {
        return this.itemPlaced;
    }

    public void ClearPlacedItem()
    {
        Debug.Log(string.Format("Xóa {0} item trong CropField...", this.itemPlaced.Count));
        foreach (GameObject go in this.itemPlaced)
        {
            UnityEngine.Object.Destroy(go);
        }
        this.itemPlaced.Clear();
        Debug.Log(string.Format("Sau khi xóa: {0} item", this.itemPlaced.Count));
    }

    public void LoadCropField(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return;
        }
        GameObject item = UnityEngine.Object.Instantiate(prefab, position, rotation);
        this.itemPlaced.Add(item);
        Debug.Log(string.Format(" Đã tải công trình '{0}' tại {1}!", prefab.name, position));
    }

    private void OnCropTileRipened(CropTile ripenedTile)
    {
        if (!this.cropTiles.Contains(ripenedTile))
        {
            return;
        }
        foreach (CropTile cropTile in this.cropTiles)
        {
            if (!cropTile.IsReadyToHarvest())
            {
                return;
            }
        }
        this.state = TileFieldState.Ripened;
        Debug.Log(string.Format("[{0}] Tất cả ô trong CropField đã chín!", base.gameObject.name));
    }

    private void CheckNearbyCropFieldsAndWater(float radius = 5f)
    {
        foreach (CropField cropField in CropField.allCropFields)
        {
            if (!(cropField == this))
            {
                float num = Vector3.Distance(base.transform.position, cropField.transform.position);
                if (num <= radius && cropField.state == TileFieldState.Sown)
                {
                    Debug.Log(string.Format("Tưới tự động: {0} trong bán kính {1} (khoảng cách: {2})", cropField.name, radius, num));
                    cropField.FieldFullyWatered();
                }
            }
        }
    }
}
