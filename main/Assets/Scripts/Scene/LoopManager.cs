using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    [Header("Game Logic")]
    public int targetLevel = 8;
    public float anomalyChance = 0.5f;

    [Header("References")]
    public Transform player;
    public Transform startPoint;
    public List<AnomalyInteraction> allAnomalies;

    [Header("UI")]
    public GameObject blackScreen;
    public TextMeshProUGUI levelText;
    public GameObject winScreen;

    private int currentLevel = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLevel = 0;
        UpdateUI();
        RandomizeAnomalies();

        StartCoroutine(FadeScreen(true));
    }

    public void TriggerExit()
    {
        if (isTransitioning) return;
        StartCoroutine(ProcessLevelChange());
    }

    private IEnumerator ProcessLevelChange()
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeScreen(false));

        if (CheckAllFixed())
        {
            currentLevel++;
            Debug.Log("Level Passed! New Level: " + currentLevel);
        }
        else
        {
            currentLevel = 0;
            Debug.Log("Failed! Loop Reset.");
        }

        UpdateUI();

        if (currentLevel >= targetLevel)
        {
            ShowWinScreen();
        }
        else
        {
            TeleportPlayer();

            RandomizeAnomalies();

            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(FadeScreen(true));
            isTransitioning = false;
        }
    }

    private bool CheckAllFixed()
    {
        foreach (var anomaly in allAnomalies)
        {
            if (!anomaly.IsFixed) return false;
        }
        return true;
    }

    private void RandomizeAnomalies()
    {
        foreach (var anomaly in allAnomalies)
        {
            bool shouldBeAnomalous = Random.value < anomalyChance;
            anomaly.SetAnomalyState(shouldBeAnomalous);
        }
    }

    private void TeleportPlayer()
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.position = startPoint.position;
        player.rotation = startPoint.rotation;

        if (cc) cc.enabled = true;
    }

    private void UpdateUI()
    {
        if (levelText) levelText.text = $"Level: {currentLevel} / {targetLevel}";
    }

    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        blackScreen.SetActive(true);

        var movement = player.GetComponent<PlayerMovement>();
        if (movement) movement.enabled = false;

        var look = player.GetComponent<PlayerLook>();
        if (look) look.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(QuitAfterDelay());
    }

    private IEnumerator FadeScreen(bool fadeIn)
    {
        if (!blackScreen) yield break;

        blackScreen.SetActive(true);
        Image img = blackScreen.GetComponent<Image>();
        if (!img) yield break;

        float startAlpha = fadeIn ? 1f : 0f;
        float endAlpha = fadeIn ? 0f : 1f;
        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        img.color = new Color(0, 0, 0, endAlpha);
        if (fadeIn) blackScreen.SetActive(false);
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("Game Over. Quitting...");

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}