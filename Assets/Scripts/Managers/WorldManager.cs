using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using System.Collections;



public class WorldManager : MonoBehaviour
{
    public static WorldManager instance; // Thêm biến instance
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private QuestManager questManager;
    private string dataPath;
    public WorldData worldData;
    [SerializeField] private Transform playerObject;
    [SerializeField] private PlayerBlackBoard blackBoard;
    public List<CropTile> cropTiles;  // Khai báo danh sách CropTile

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Gán instance cho đối tượng đầu tiên
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy object này để tránh trùng lặp
            return;
        }
        dataPath = Path.Combine(Application.persistentDataPath, "worlddata.json");
        worldData = new WorldData();
    }

    private IEnumerator Start()
    {

        // Chờ DataManagers khởi tạo
        while (DataManagers.instance == null)
        {
            Debug.LogWarning(" Đợi DataManagers.instance...");
            yield return null;
        }

        Debug.Log(" DataManagers.instance đã có, tiếp tục LoadWorld()");
        StartCoroutine(LoadWorldCoroutine());
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ItemPlacedEvent>(OnItemPlaced);
        EventBus.Subscribe<ItemDestroyedEvent>(OnItemDestroyed);
        EventBus.Subscribe<BuildingPlacedEvent>(OnBuildingPlaced);
        EventBus.Subscribe<TreeDestroyedEvent>(OnTreeDestroyed);
        EventBus.Subscribe<BuildingDestroyedEvent>(OnBuildingDestroyed);


    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemPlacedEvent>(OnItemPlaced);
        EventBus.Unsubscribe<ItemDestroyedEvent>(OnItemDestroyed);
        EventBus.Unsubscribe<BuildingPlacedEvent>(OnBuildingPlaced);
        EventBus.Unsubscribe<TreeDestroyedEvent>(OnTreeDestroyed);
        EventBus.Unsubscribe<BuildingDestroyedEvent>(OnBuildingDestroyed);

    }



    public void SaveWorld()
    {
        if (worldData == null)
            worldData = new WorldData();

        // ————————— Thời gian + player —————————
        worldData._timeOfDay = dayNightCycle.timeOfDay;
        worldData._dayNumber = dayNightCycle.dayNumber;
        worldData._yearNumber = dayNightCycle.yearNumber;
        worldData._yearLength = dayNightCycle.yearLength;
        worldData.gameDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        worldData.playerPosition = playerObject.position;
        worldData.coins = CashManager.instance.GetCoins();
        worldData.stamina = blackBoard.stamina;
        worldData.tileGridPositions = new List<Vector3Int>();

        // —————— Reset danh sách rồi fill lại ——————
        var newItems = new List<PlacedItemData>();
        var newBuildings = new List<PlacedBuildingData>();

        worldData.placedItems.Clear();
        worldData.placedBuildings.Clear();

        // 1) Lưu tất cả cây qua CropTile
        // Duyệt qua tất cả các CropTile trong game và lưu vị trí của chúng
        foreach (var cropTile in cropTiles) // cropTiles là danh sách các CropTile bạn đang sử dụng
        {
            if (cropTile != null)
            {
                // Lưu vị trí tileGridPosition của CropTile
                worldData.tileGridPositions.Add(cropTile.tileGridPosition);
            }
        }

        // 2) Lưu items khác (không phải crop)
        foreach (var item in FindObjectsOfType<Item>())
        {
            if (item is Crop)
                continue;

            if (item.itemData == null || string.IsNullOrEmpty(item.itemData.itemName))
                continue;

            newItems.Add(new PlacedItemData(
                item.itemData.itemName,
                item.transform.position,
                item.transform.rotation
            ));
        }

        // 3) Lưu buildings như cũ
        var bs = FindObjectOfType<BuildingSystem>();
        if (bs != null)
        {
            foreach (var go in bs.GetPlacedBuildings())
            {
                var pb = go.GetComponent<PlacedBuilding>();
                if (pb == null || pb.buildingData == null) continue;
                if (pb.buildingData.buildingName == "Player") continue;

                newBuildings.Add(new PlacedBuildingData(
                    pb.buildingData.buildingName,
                    go.transform.position,
                    go.transform.rotation
                ));
            }
        }

        worldData.placedItems = newItems;
        worldData.placedBuildings = newBuildings;

        File.WriteAllText(dataPath, JsonUtility.ToJson(worldData, true));
        questManager.SaveQuests();
        Debug.Log("World Saved!");
    }

    public IEnumerator LoadWorldCoroutine()
    {
        // 1) Đảm bảo worldData hợp lệ
        if (!File.Exists(dataPath))
        {
            worldData = new WorldData();
            CashManager.instance.SetCoins(20);
            SaveWorld();
            yield break;
        }

        var text = File.ReadAllText(dataPath);
        worldData = string.IsNullOrEmpty(text)
            ? new WorldData()
            : JsonUtility.FromJson<WorldData>(text) ?? new WorldData();

        // 2) Xóa toàn bộ Item & Building hiện có
        foreach (var it in FindObjectsOfType<Item>())
            Destroy(it.gameObject);

        var bs = FindObjectOfType<BuildingSystem>();
        bs?.ClearPlacedBuildings();

        yield return new WaitForEndOfFrame();

        // 3) Load lại cây vào đúng Croptile
        var allTiles = FindObjectsOfType<CropTile>();
        foreach (var pd in worldData.placedItems)
        {
            // tìm Tile gần đúng vị trí
            var tile = allTiles.FirstOrDefault(t =>
                Vector3.Distance(t.transform.position, pd.position) < 0.5f);

            if (tile != null)
            {
                // sow và restore state
                var itemData = DataManagers.instance.GetItemDataByName(pd.itemName);
                tile.Sow(itemData);
                tile.state = pd.tileFieldState;

                var c = tile.GetComponentInChildren<Crop>();
                c.isFullyGrown = pd.isFullyGrown;
                c.timeToGrowUp = pd.timeToGrowUp;
            }
            else
            {
                // non-crop: instantiate bình thường
                var itemData = DataManagers.instance.GetItemDataByName(pd.itemName);
                if (itemData?.itemPrefab != null)
                    Instantiate(itemData.itemPrefab.gameObject, pd.position, pd.rotation);
            }
        }

        // 4) Load buildings
        if (bs != null)
        {
            foreach (var bd in worldData.placedBuildings)
            {
                if (bd.buildingName == "Player") continue;
                var bData = DataManagers.instance.GetBuildingDataByName(bd.buildingName);
                if (bData?.buildingPrefab != null)
                    bs.LoadBuilding(bData.buildingPrefab, bd.position, bd.rotation);
            }
        }

        // 5) Xóa cây đã chết, restore time & player
        foreach (var tree in FindObjectsOfType<Tree>())
            if (worldData.destroyedTrees.Any(d =>
                Vector3.Distance(d.position, tree.transform.position) < 0.1f))
                Destroy(tree.gameObject);

        dayNightCycle.timeOfDay = worldData._timeOfDay;
        dayNightCycle.dayNumber = worldData._dayNumber;
        dayNightCycle.yearNumber = worldData._yearNumber;
        dayNightCycle.yearLength = worldData._yearLength;
        dayNightCycle.AdjustSun();
        dayNightCycle.AdjustSkybox();

        playerObject.position = worldData.playerPosition;
        CashManager.instance.SetCoins(worldData.coins);
        blackBoard.stamina = worldData.stamina;

        Debug.Log("LoadWorld hoàn tất!");
    }



    private void OnBuildingDestroyed(BuildingDestroyedEvent evt)
    {
        if (worldData == null)
            worldData = new WorldData();

        // Kiểm tra xem evt.building có buildingData hay không
        if (evt.building == null || evt.building.buildingData == null)
        {
            Debug.LogWarning(" Không tìm thấy buildingData, bỏ qua xóa.");
            return;
        }

        string buildingName = evt.building.buildingData.buildingName;
        Vector3 buildingPosition = evt.building.transform.position;

        // Xóa công trình khỏi danh sách lưu trữ
        bool removed = worldData.placedBuildings.RemoveAll(building =>
            building.buildingName == buildingName &&
            Vector3.Distance(building.position, buildingPosition) < 0.01f) > 0;

        if (removed)
        {
            Debug.Log($" Đã xóa {buildingName} khỏi dữ liệu save!");
            SaveWorld(); // Cập nhật file save ngay lập tức
        }
    }

    private void OnItemPlaced(ItemPlacedEvent evt)
    {
        worldData.placedItems.Add(new PlacedItemData(
            evt.item.itemData.itemName,
            evt.item.transform.position,
            evt.item.transform.rotation
        ));
    }
    private void OnTreeDestroyed(TreeDestroyedEvent evt)
    {
        if (worldData == null)
            worldData = new WorldData();

        // Lưu vị trí cây vào danh sách đã bị phá hủy
        worldData.destroyedTrees.Add(new DestroyedTreeData(evt.position));
        SaveWorld();
    }


    private void OnItemDestroyed(ItemDestroyedEvent evt)
    {
        worldData.placedItems.RemoveAll(i =>
            i.itemName == evt.item.itemData.itemName && Vector3.Distance(i.position, evt.item.transform.position) < 0.01f);
        SaveWorld();
    }
    private void OnBuildingPlaced(BuildingPlacedEvent eventData)
    {
        if (eventData.placedBuilding.buildingName == "Player") return;

        worldData.placedBuildings.Add(eventData.placedBuilding);
        Debug.Log($" Công trình {eventData.placedBuilding.buildingName} đã được đặt tại {eventData.placedBuilding.position}");

        SaveWorld(); // GỌI NGAY KHI ĐẶT BUILDING!
    }



    private void OnApplicationQuit()
    {
        SaveWorld();
        Debug.Log("ĐÃ LƯU");
    }
}