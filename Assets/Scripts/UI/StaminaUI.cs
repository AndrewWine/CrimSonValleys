using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CC RID: 204
public class StaminaUI : MonoBehaviour
{
    // Khi UI được bật, đăng ký lắng nghe sự kiện thay đổi stamina
    private void OnEnable()
    {
        EventBus.Subscribe<StaminaChangedEvent>(OnStaminaChanged);
    }

    // Khi UI bị tắt, huỷ đăng ký sự kiện
    private void OnDisable()
    {
        EventBus.Unsubscribe<StaminaChangedEvent>(OnStaminaChanged);
    }

    // Gọi khi bắt đầu, cập nhật UI với stamina hiện tại
    private void Start()
    {
        UpdateStaminaUI(blackBoard.stamina);
    }

    // Gọi khi có sự kiện stamina thay đổi
    private void OnStaminaChanged(StaminaChangedEvent e)
    {
        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
        }
        staminaCoroutine = StartCoroutine(UpdateStaminaUIOverTime(e.NewStamina));
    }

    // Cập nhật UI stamina một cách mượt mà theo thời gian
    private IEnumerator UpdateStaminaUIOverTime(float targetStamina)
    {
        float startStamina = blackBoard.stamina;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float stamina = Mathf.Lerp(startStamina, targetStamina, elapsedTime / duration);
            UpdateStaminaUI(stamina);
            yield return null;
        }

        UpdateStaminaUI(targetStamina);
        blackBoard.stamina = targetStamina;
    }

    // Cập nhật UI trực tiếp
    private void UpdateStaminaUI(float stamina)
    {
        float normalized = Mathf.Clamp01(stamina / 100f);
        staminaIcon.color = sliderColorGradient.Evaluate(normalized);
        staminaText.text = $"{stamina:F0}";
    }

    [Header("UI Components")]
    [SerializeField] private Image staminaIcon;
    [SerializeField] private TextMeshProUGUI staminaText;

    [Header("Gradient for Stamina Bar")]
    [SerializeField] private Gradient sliderColorGradient;

    [Header("Player Blackboard Reference")]
    [SerializeField] private PlayerBlackBoard blackBoard;

    private Coroutine staminaCoroutine;
}
