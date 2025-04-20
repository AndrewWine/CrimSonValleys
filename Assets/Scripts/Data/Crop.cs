using System;
using UnityEngine;

public class Crop : Item
{
    [Header("Elements")]
    [SerializeField] private Transform cropRenderer;
    [SerializeField] private ParticleSystem harvestedParticles;

    [Header("Crop Properties")]
    public TileFieldState tileFieldState;
    public float timeToGrowUp = 10f;
    public bool isFullyGrown;

    [Header("Actions")]
    public static Action<CropTile> realdyToHarvest;
    public static Action arealdyHarvested;

    /// <summary>
    /// Bắt đầu quá trình phát triển cây (scale lớn dần).
    /// </summary>
    public void ScaleUp()
    {
        LeanTween.scale(
            cropRenderer.gameObject,
            cropRenderer.transform.localScale * 10f,
            timeToGrowUp
        )
        .setEase(LeanTweenType.easeOutBack)
        .setOnComplete(() =>
        {
            isFullyGrown = true;

            // Gửi sự kiện khi cây trưởng thành
            realdyToHarvest?.Invoke(transform.parent.GetComponent<CropTile>());
        });
    }

    /// <summary>
    /// Kiểm tra cây đã trưởng thành chưa và cập nhật trạng thái trên tile nếu có.
    /// </summary>
    public bool IsFullyGrown()
    {
        CropTile tile = transform.parent.GetComponent<CropTile>();
        if (tile != null)
        {
            tile.UpdateGrowthState();
        }
        return isFullyGrown;
    }

    /// <summary>
    /// Trả về thời gian phát triển của cây.
    /// </summary>
    public float TimeToGrowUp()
    {
        return timeToGrowUp;
    }

    /// <summary>
    /// Thu hoạch cây, bật hiệu ứng và hủy đối tượng.
    /// </summary>
    public void ScaleDown()
    {
        if (!isFullyGrown) return;

        Debug.Log("thu hoạch");

        harvestedParticles.gameObject.SetActive(true);
        harvestedParticles.transform.parent = null;
        harvestedParticles.Play();

        Destroy(gameObject);
    }
}
