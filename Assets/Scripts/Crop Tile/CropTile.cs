using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Quản lý 1 ô đất trong hệ thống trồng trọt.
/// </summary>
public class CropTile : MonoBehaviour
{
    public TileFieldState state;

    [Header("Elements")]
    [SerializeField] private MeshRenderer tileRenderer;
    [SerializeField] private Material infestedMaterial;

    private Material originalMaterial;
    private Coroutine infestationCoroutine;
    private Transform cropParent;
    private Crop crop;

    private int dropAmount;
    private int DebuffAmount;
    private int BuffAmount;

    private string cropFieldID;
    public CropField parentField;
    public Vector3Int tileGridPosition;

    public PlacedItemData placedItemData;
    public ItemData cropData;
    public bool hasCrop;

    [Header("Actions")]
    public static Action<string, int> onCropHarvested;

    private void Awake()
    {
        parentField = GetComponentInParent<CropField>();
        if (parentField != null)
            cropFieldID = parentField.fieldID;
    }

    private void Start()
    {
        cropParent = transform;
        state = TileFieldState.Empty;
        InitializeSettings();
    }

    private void OnEnable()
    {
        RemoveWormsButton.removeWorms += CureInfestation;
    }

    private void OnDisable()
    {
        RemoveWormsButton.removeWorms -= CureInfestation;
    }

    private void InitializeSettings()
    {
        dropAmount = UnityEngine.Random.Range(3, 10);
        DebuffAmount = UnityEngine.Random.Range(1, 3);
        BuffAmount = UnityEngine.Random.Range(1, 3);
    }

    public bool HasCrop() => crop != null || hasCrop;

    public bool IsEmpty() => state == TileFieldState.Empty;

    public bool IsSown() => state == TileFieldState.Sown;

    public bool IsReadyToHarvest() => crop != null && crop.IsFullyGrown();

    public void Sow(ItemData data)
    {
        if (data == null || data.itemPrefab == null)
        {
            Debug.LogError("CropData hoặc prefab null!");
            return;
        }

        state = TileFieldState.Sown;
        cropData = data;

        crop = Instantiate(data.itemPrefab, transform) as Crop;
        crop.transform.localPosition = Vector3.zero;
        crop.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 10f, transform.localScale.z);
        crop.itemData = data;
        crop.isFullyGrown = false;

        if (string.IsNullOrEmpty(name))
            name = $"CropTile_{transform.position.x}_{transform.position.z}";

        placedItemData = new PlacedItemData("CropField", data.itemName, transform.position, transform.rotation, name, crop.isFullyGrown, crop.timeToGrowUp, state);
    }

    public void Water()
    {
        crop = GetComponentInChildren<Crop>();
        state = TileFieldState.Watered;

        if (crop != null)
            crop.ScaleUp();

        tileRenderer.gameObject.LeanColor(Color.white * 0.3f, 1f).setEase(LeanTweenType.easeOutBack);
    }

    public void Harvest()
    {
        if (crop == null || !crop.IsFullyGrown())
        {
            Debug.Log("Chưa thể thu hoạch");
            return;
        }

        InitializeSettings();
        state = TileFieldState.Empty;

        crop.ScaleDown();
        tileRenderer.gameObject.LeanColor(Color.white, 1f).setEase(LeanTweenType.easeOutBack);

        EventBus.Publish(new ItemPickedUp(cropData.itemName, dropAmount));
        Debug.Log("Thu hoạch thành công");

        onCropHarvested?.Invoke(cropData.itemName, dropAmount);
    }

    public void StartInfestation()
    {
        if (cropData == null) return;

        if (infestationCoroutine != null)
            StopCoroutine(infestationCoroutine);

        crop = GetComponentInChildren<Crop>();

        if (crop != null)
        {
            Renderer renderer = crop.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                originalMaterial = renderer.material;
                renderer.material = infestedMaterial;
            }
        }

        infestationCoroutine = StartCoroutine(DamagePlantOverTime());
    }

    private IEnumerator DamagePlantOverTime()
    {
        while (cropData.plantHealth > 0f)
        {
            cropData.plantHealth -= 1f;
            Debug.Log($"{gameObject.name} - plantHealth: {cropData.plantHealth}");

            if (cropData.plantHealth <= 0f)
            {
                Debug.Log($"{gameObject.name} - Cây đã chết vì sâu bệnh!");

                if (crop != null)
                {
                    Destroy(crop.gameObject);
                    crop = null;
                }

                state = TileFieldState.Empty;
                cropData = null;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void CureInfestation()
    {
        if (infestationCoroutine != null)
            StopCoroutine(infestationCoroutine);

        crop = GetComponentInChildren<Crop>();

        if (crop != null && originalMaterial != null)
        {
            Renderer renderer = crop.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material = originalMaterial;
        }

        state = TileFieldState.Watered;
        Debug.Log($"{gameObject.name} đã được chữa khỏi bệnh!");
    }

    public void UpdatePlacedItemData()
    {
        placedItemData = new PlacedItemData(
            "CropField",
            cropData.itemName,
            transform.position,
            transform.rotation,
            name,
            crop != null && crop.isFullyGrown,
            crop != null ? crop.timeToGrowUp : 0f,
            state
        );
    }

    public PlacedItemData GetPlacedItemData() => placedItemData;

    public void UpdateGrowthState()
    {
        if (crop == null || cropData == null) return;

        crop.isFullyGrown = true;
        state = TileFieldState.Ripened;
        UpdatePlacedItemData();
    }

    [Serializable]
    public class CropTileSaveData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string itemName;
        public float timeToGrowUp;
        public bool isFullyGrown;
        public TileFieldState state;
    }
}
