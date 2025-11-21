using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Обов'язково для роботи з текстом
using UnityEngine.UI;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    [Header("Game Logic")]
    public int targetLevel = 8; // Цільовий рівень
    public float anomalyChance = 0.5f; // 50% шанс появи аномалії на кожному об'єкті

    [Header("References")]
    public Transform player;
    public Transform startPoint; // Точка початку (куди телепортуємо)
    public List<AnomalyInteraction> allAnomalies; // Список всіх об'єктів, що можуть ламатися

    [Header("UI")]
    public GameObject blackScreen; // Панель чорного екрану (Image)
    public TextMeshProUGUI levelText; // Текст "Level: 0 / 8"
    public GameObject winScreen; // Екран перемоги

    private int currentLevel = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // На старті гри скидаємо все
        currentLevel = 0;
        UpdateUI();
        RandomizeAnomalies(); // Генеруємо перший рівень

        // Робимо плавне зникнення чорного екрану на старті
        StartCoroutine(FadeScreen(true));
    }

    // Цей метод викликає чорний вихід (ExitTrigger)
    public void TriggerExit()
    {
        if (isTransitioning) return;
        StartCoroutine(ProcessLevelChange());
    }

    private IEnumerator ProcessLevelChange()
    {
        isTransitioning = true;

        // 1. Затемнення екрану
        yield return StartCoroutine(FadeScreen(false)); // Fade Out (стає чорним)

        // 2. Перевірка умов
        if (CheckAllFixed())
        {
            // Успіх
            currentLevel++;
            Debug.Log("Level Passed! New Level: " + currentLevel);
        }
        else
        {
            // Провал - скидання
            currentLevel = 0;
            Debug.Log("Failed! Loop Reset.");
        }

        UpdateUI();

        // 3. Фінал або Продовження
        if (currentLevel >= targetLevel)
        {
            ShowWinScreen();
        }
        else
        {
            // Телепорт на початок
            TeleportPlayer();

            // Генеруємо нові аномалії для наступного проходу
            RandomizeAnomalies();

            // Чекаємо трохи в темряві
            yield return new WaitForSeconds(0.5f);

            // Висвітлення
            yield return StartCoroutine(FadeScreen(true)); // Fade In (стає прозорим)
            isTransitioning = false;
        }
    }

    // Логіка перевірки: чи всі активні аномалії виправлені?
    private bool CheckAllFixed()
    {
        foreach (var anomaly in allAnomalies)
        {
            // Якщо об'єкт "не виправлений" (тобто він в стані аномалії), то ми програли
            // AnomalyInteraction сам керує полем IsFixed:
            // Якщо це нормальний об'єкт -> IsFixed = true
            // Якщо аномалія і не полагоджена -> IsFixed = false
            // Якщо аномалія і полагоджена -> IsFixed = true
            if (!anomaly.IsFixed) return false;
        }
        return true;
    }

    private void RandomizeAnomalies()
    {
        foreach (var anomaly in allAnomalies)
        {
            // Рандомно вирішуємо, чи буде цей об'єкт зламаний у цьому циклі
            bool shouldBeAnomalous = Random.value < anomalyChance;
            anomaly.SetAnomalyState(shouldBeAnomalous);
        }
    }

    private void TeleportPlayer()
    {
        // Обов'язково вимикаємо контролер перед телепортом!
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

        // Вимикаємо управління
        var movement = player.GetComponent<PlayerMovement>();
        if (movement) movement.enabled = false;

        var look = player.GetComponent<PlayerLook>();
        if (look) look.enabled = false;

        // Вмикаємо курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // --- [НОВЕ] Запускаємо таймер на вихід ---
        StartCoroutine(QuitAfterDelay());
    }

    // Корутина для плавного затемнення (проста альфа)
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

    // --- [НОВЕ] Додай цей метод в кінець класу LoopManager ---
    private IEnumerator QuitAfterDelay()
    {
        // Чекаємо 5 секунд
        yield return new WaitForSeconds(5f);

        Debug.Log("Game Over. Quitting...");

        // Ця команда закриває скомпільовану гру (.exe)
        Application.Quit();

        // А цей шматок коду зупинить гру, якщо ти тестуєш її в редакторі Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}