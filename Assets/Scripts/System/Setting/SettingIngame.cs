using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020000CA RID: 202
public class SettingIngame : MonoBehaviour
{
    // Token: 0x04000452 RID: 1106
    [Header("Elements")]
    [SerializeField] private GameObject SettingWindow;
    [SerializeField] private GameObject SettingRectTransform;
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject soundEditor;
    [SerializeField] private Image fadeScreen;

    // Token: 0x04000458 RID: 1112
    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    // Token: 0x0400045A RID: 1114
    [SerializeField] private float moveSpeed = 2500f;
    [SerializeField] private float moveDistance = 200f;

    private bool check;

    private void Start()
    {
        InitializeSettings();
        StartCoroutine(FadeFromBlack(1f));
    }

    private void LateUpdate()
    {
        HandleEscapeKeyPress();
    }

    private void InitializeSettings()
    {
        if (SettingRectTransform == null)
            return;

        hiddenPosition = SettingRectTransform.transform.localPosition;
        visiblePosition = hiddenPosition + new Vector3(moveDistance, 0f, 0f);
        openButton.SetActive(true);
        closeButton.SetActive(false);
        soundEditor.SetActive(false);
    }

    private void HandleEscapeKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (check)
            {
                OnOpenWindowPressed();
            }
            else
            {
                OnCloseWindowPressed();
            }

            check = !check;
        }
    }

    public void OnOpenWindowPressed()
    {
        SettingWindow.SetActive(true);
        StartCoroutine(MoveIn(visiblePosition));
        openButton.SetActive(false);
        closeButton.SetActive(true);
    }

    public void OnCloseWindowPressed()
    {
        TooltipManager.Instance.HideTooltip();
        StartCoroutine(MoveOut(hiddenPosition));
        openButton.SetActive(true);
        closeButton.SetActive(false);
    }

    private IEnumerator MoveIn(Vector3 targetPosition)
    {
        while (Vector3.Distance(SettingRectTransform.transform.localPosition, targetPosition) > 1f)
        {
            SettingRectTransform.transform.localPosition = Vector3.MoveTowards(
                SettingRectTransform.transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        SettingRectTransform.transform.localPosition = targetPosition;
    }

    private IEnumerator MoveOut(Vector3 targetPosition)
    {
        while (Vector3.Distance(SettingRectTransform.transform.localPosition, targetPosition) > 1f)
        {
            SettingRectTransform.transform.localPosition = Vector3.MoveTowards(
                SettingRectTransform.transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        SettingRectTransform.transform.localPosition = targetPosition;
    }

    public void OnSaveGamePressed()
    {
        StartCoroutine(FadeSaveEffect());
    }

    private IEnumerator FadeSaveEffect()
    {
        yield return StartCoroutine(FadeToBlack(0.3f));
        WorldManager.instance.SaveWorld();
        InventoryManager.Instance.SaveInventory();
        Debug.Log("Game Saved");
        yield return StartCoroutine(FadeFromBlack(0.3f));
    }

    public void ExitToMainMenu()
    {
        StartCoroutine(FadeAndGoToMainMenu());
    }

    private IEnumerator FadeAndGoToMainMenu()
    {
        yield return StartCoroutine(FadeToBlack(1f));
        WorldManager.instance.SaveWorld();
        SceneManager.LoadScene("MainMenu");
    }

    public void EnableSoundEditor()
    {
        soundEditor.SetActive(true);
    }

    public void DisableSoundEditor()
    {
        soundEditor.SetActive(false);
    }

    private IEnumerator FadeToBlack(float duration)
    {
        fadeScreen.color = new Color(0f, 0f, 0f, 0f);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer / duration);
            fadeScreen.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, timer / duration);
            fadeScreen.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }
}
