using System;
using UnityEngine;

public class CheckGameObject : MonoBehaviour
{


    public CropField GetCurrentCropField() => currentCropField;

    public OreRock GetCurrentOreRock() => ore;

    [Header("Actions")]
    public static Action<bool> EnableSowBTTN;
    public static Action<bool> EnableWaterBTTN;
    public static Action<bool> EnableHarvestBTTN;
    public static Action<bool> EnableSleepBTTN;
    public static Action<bool> removeWormsBTTN;
    public static Action<bool> UnlockCropField;
    public static Action<CropField> cropFieldDetected;
    public static Action<bool> changeCameraAngel;

    [Header("Elements")]
    public CropField currentCropField;
    private LineRenderer lineRenderer;
    private PlayerBlackBoard blackBoard;
    public bool buildingGameObject;
    public bool cropTile;
    public Transform player;
    private CharacterController controller;
    public GameObject currentGameObject;
    private Tree tree;
    private OreRock ore;
    private void Awake()
    {
        controller = player.GetComponent<CharacterController>();
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        blackBoard = GetComponentInParent<PlayerBlackBoard>();
        SetupLineRenderer();
    }

    private void OnEnable()
    {
        CuttingAbility.causeDamage += DoCuttingTree;
        MiningAbility.doDamageToOre += DoMining;
    }

    private void OnDisable()
    {
        CuttingAbility.causeDamage -= DoCuttingTree;
        MiningAbility.doDamageToOre -= DoMining;
    }

    private void LateUpdate()
    {
        if (controller != null)
        {
            Vector3 velocity = controller.velocity;
            velocity.y = 0f;
            if (velocity.magnitude > 0.1f)
            {
                Vector3 normalized = velocity.normalized;
                transform.position = player.position + normalized;
                return;
            }
            transform.position = player.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Croptile"))
        {
            CropField component = other.GetComponent<CropField>();
            if (component != null)
            {
                if (currentCropField == null)
                {
                    currentCropField = component;
                    currentGameObject = currentCropField.gameObject;
                    cropTile = true;
                    UnlockCropField?.Invoke(true);
                }
                lineRenderer.enabled = true;
                UpdateButtons(component.state, true);
            }
            cropFieldDetected?.Invoke(component);
            return;
        }

        if (other.CompareTag("Ore"))
        {
            ore = other.GetComponent<OreRock>();
            blackBoard.isOre = true;
            return;
        }

        if (other.CompareTag("Tree"))
        {
            tree = other.GetComponent<Tree>();
            blackBoard.isTree = true;
            return;
        }

        if (other.CompareTag("Indoor"))
        {
            changeCameraAngel?.Invoke(true);
            EnableSleepBTTN?.Invoke(true);
            return;
        }

        if (other.CompareTag("Cage") || other.CompareTag("Decore"))
        {
            buildingGameObject = true;
            currentGameObject = other.gameObject;
            return;
        }

        if (other.CompareTag("FarmArea"))
        {
            blackBoard.isFarmArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Croptile"))
        {
            if (other.GetComponent<CropField>() == currentCropField)
            {
                currentCropField = null;
                UpdateButtons(TileFieldState.Empty, false);
                lineRenderer.enabled = false;
            }
            cropTile = false;
            currentGameObject = null;
            return;
        }

        if (other.CompareTag("Indoor"))
        {
            changeCameraAngel?.Invoke(false);
            EnableSleepBTTN?.Invoke(false);
            return;
        }

        if (other.CompareTag("Cage") || other.CompareTag("Decore"))
        {
            buildingGameObject = false;
            currentGameObject = null;
            return;
        }

        if (other.CompareTag("FarmArea"))
        {
            blackBoard.isFarmArea = false;
            return;
        }

        if (other.CompareTag("Tree"))
        {
            other.GetComponent<Tree>();
            blackBoard.isTree = false;
            return;
        }

        if (other.CompareTag("Ore"))
        {
            other.GetComponent<OreRock>();
            blackBoard.isOre = false;
        }
    }

    private void DoCuttingTree()
    {
        tree.TakeDamage(blackBoard.Axedamage);
    }

    private void DoMining()
    {
        if (ore != null)
        {
            ore.TakeDamage(blackBoard.Pickaxedamage);
        }
    }

    private void UpdateButtons(TileFieldState state, bool enable)
    {
        EnableSowBTTN?.Invoke(state == TileFieldState.Empty && enable);
        EnableWaterBTTN?.Invoke(state == TileFieldState.Sown && enable);
        EnableHarvestBTTN?.Invoke(state == TileFieldState.Ripened && enable);
        removeWormsBTTN?.Invoke(state == TileFieldState.Infested && enable);
        lineRenderer.enabled = enable;
    }

    private void SetupLineRenderer()
    {
        lineRenderer.positionCount = 8;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.material.color = Color.green;
        lineRenderer.enabled = true;
    }

}
