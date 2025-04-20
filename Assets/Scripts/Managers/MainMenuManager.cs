using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField]
    private Button NewGameButton;
    [SerializeField]
    private Button LoadGameButton;
    [SerializeField]
    private Button ExitGameButton;
    [SerializeField]
    private Image fadeScreen;

    private string worldSavePath;
    private string inventorySavePath;
    private void Start()
    {
        this.worldSavePath = Path.Combine(Application.persistentDataPath, "worlddata.json");
        this.inventorySavePath = Path.Combine(Application.persistentDataPath, "inventoryData.txt");
        bool active = File.Exists(this.worldSavePath) || File.Exists(this.inventorySavePath);
        this.LoadGameButton.gameObject.SetActive(active);
        this.NewGameButton.onClick.AddListener(delegate ()
        {
            base.StartCoroutine(this.StartNewGame());
        });
        this.LoadGameButton.onClick.AddListener(delegate ()
        {
            base.StartCoroutine(this.LoadGame());
        });
        this.ExitGameButton.onClick.AddListener(delegate ()
        {
            base.StartCoroutine(this.ExitGame());
        });
        base.StartCoroutine(this.FadeFromBlack(5f));
    }

    private IEnumerator StartNewGame()
    {
        yield return base.StartCoroutine(this.FadeToBlack(1f));
        if (File.Exists(this.worldSavePath))
        {
            File.Delete(this.worldSavePath);
        }
        if (File.Exists(this.inventorySavePath))
        {
            File.Delete(this.inventorySavePath);
        }
        Debug.Log("Bắt đầu trò chơi mới, tất cả dữ liệu đã bị xóa!");
        SceneManager.LoadScene("Main");
        yield break;
    }

    private IEnumerator LoadGame()
    {
        yield return base.StartCoroutine(this.FadeToBlack(1f));
        Debug.Log("Đang tải trò chơi từ file save...");
        SceneManager.LoadScene("Main");
        yield break;
    }

    private IEnumerator ExitGame()
    {
        yield return base.StartCoroutine(this.FadeToBlack(1f));
        Debug.Log("Thoát game!");
        Application.Quit();
        yield break;
    }

    private IEnumerator FadeToBlack(float duration)
    {
        this.fadeScreen.color = new Color(0f, 0f, 0f, 0f);
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer / duration);
            this.fadeScreen.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        this.fadeScreen.color = new Color(0f, 0f, 0f, 1f);
        yield break;
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        this.fadeScreen.color = new Color(0f, 0f, 0f, 1f);
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, timer / duration);
            this.fadeScreen.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        this.fadeScreen.color = new Color(0f, 0f, 0f, 0f);
        yield break;
    }

   
}
