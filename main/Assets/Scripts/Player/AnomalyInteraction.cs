using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnomalyInteraction : MonoBehaviour
{
    [Header("Settings")]
    public ItemType requiredItem;
    public GameObject normalState;   // Стан "Виправлено"
    public GameObject anomalyState;  // Стан "Аномалія"

    [Header("Audio")]
    [Tooltip("Звук, який грає, поки аномалія активна (може бути пустим)")]
    public AudioClip activeLoopSound;

    [Tooltip("Звук успішного виправлення (один раз)")]
    public AudioClip fixSuccessSound;

    private bool isFixed = false;
    private AudioSource audioSource;

    private void Start()
    {
        // Налаштування звуку
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true; // Зациклюємо звук аномалії
        audioSource.spatialBlend = 1.0f; // 3D звук

        // Запуск аномалії
        SetAnomalyState(true);
    }

    // true = аномалія активна, false = виправлено
    public void SetAnomalyState(bool isActive)
    {
        isFixed = !isActive;

        if (normalState) normalState.SetActive(!isActive);
        if (anomalyState) anomalyState.SetActive(isActive);

        // Логіка звуку
        if (isActive)
        {
            if (activeLoopSound != null)
            {
                audioSource.clip = activeLoopSound;
                audioSource.Play();
            }
        }
        else
        {
            // Якщо виправили - зупиняємо шум
            audioSource.Stop();
        }
    }

    public void TryFix()
    {
        if (isFixed) return;

        if (Inventory.Instance == null) return;

        // Перевіряємо перший предмет через твій метод масиву
        var items = Inventory.Instance.IQtoArray();

        if (items.Length > 0 && items[0] != null)
        {
            PickableItem pickable = items[0].prefab.GetComponent<PickableItem>();

            if (pickable != null && (pickable.itemType == requiredItem || requiredItem == ItemType.Any))
            {
                Debug.Log("ANOMALY FIXED!");

                // 1. Граємо звук успіху (окремо, в точці, щоб не обірвався)
                if (fixSuccessSound != null)
                {
                    AudioSource.PlayClipAtPoint(fixSuccessSound, transform.position);
                }

                // 2. Вимикаємо аномалію
                SetAnomalyState(false); // Це зупинить activeLoopSound і змінить моделі

                // 3. [ЗМІНА] Замість видалення - прокручуємо інвентар
                Inventory.Instance.NextItem();
            }
        }
    }
}