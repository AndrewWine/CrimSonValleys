using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest ScriptableObject lưu trữ thông tin nhiệm vụ, yêu cầu và phần thưởng.
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Objects/Quest", order = 1)]
public class Quest : ScriptableObject
{
    [Header("Quest Information")]
    public string questName;
    public string description;
    public bool isActive;
    public bool isCompleted;
    public string questGiverID;

    [Header("Quest Requirements")]
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    [Header("Quest Rewards")]
    public List<QuestReward> rewards = new List<QuestReward>();

    /// <summary>
    /// Trả về danh sách yêu cầu nhiệm vụ.
    /// </summary>
    public List<QuestRequirement> GetRequirements()
    {
        return requirements;
    }

    /// <summary>
    /// Trả về danh sách phần thưởng.
    /// </summary>
    public List<QuestReward> GetRewards()
    {
        return rewards;
    }

    /// <summary>
    /// Kiểm tra xem tất cả yêu cầu đã hoàn thành chưa.
    /// </summary>
    public bool CheckCompletion()
    {
        if (isCompleted) return true;

        foreach (var requirement in requirements)
        {
            if (!requirement.IsCompleted())
                return false;
        }

        isCompleted = true;
        CompleteQuest();
        return true;
    }

    /// <summary>
    /// Hoàn thành nhiệm vụ và cấp phần thưởng.
    /// </summary>
    public void CompleteQuest()
    {
        if (!isCompleted) return;

        Debug.Log($"Quest '{questName}' đã hoàn thành! Đang cấp phần thưởng...");
        GiveRewards();
        EventBus.Publish(new QuestCompletedEvent(questName));
    }

    /// <summary>
    /// Cấp phần thưởng cho người chơi.
    /// </summary>
    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            EventBus.Publish<ItemPickedUp>(new ItemPickedUp(reward.itemName, reward.amount));
            Debug.Log($"Nhận được {reward.amount}x {reward.itemName}");
        }
    }

    /// <summary>
    /// Tạo bản sao nhiệm vụ, reset trạng thái và gán ID người giao nhiệm vụ.
    /// </summary>
    public Quest Clone(string giverID)
    {
        Quest clone = Instantiate(this);
        clone.requirements = new List<QuestRequirement>();

        foreach (var requirement in requirements)
        {
            clone.requirements.Add(new QuestRequirement(requirement.requiredItemName, requirement.requiredItemAmount));
        }

        clone.rewards = new List<QuestReward>(rewards);
        clone.isActive = false;
        clone.isCompleted = false;
        clone.questGiverID = giverID;

        return clone;
    }
}
