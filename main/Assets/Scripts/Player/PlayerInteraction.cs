using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("HUD for Pickable Items")]
    [SerializeField] private GameObject hud;  // HUD inside Canvas

    [Header("Anomaly Interaction")] // [НОВЕ] Налаштування для лагодження
    [SerializeField] private Transform cameraTransform; // Сюди перетягни Main Camera
    [SerializeField] private float interactionDistance = 3f; // Як далеко дістає гравець

    private PlayerControls controls;
    private GameObject currentPickable;
    private Inventory inventory;

    private void Awake()
    {
        controls = new PlayerControls();

        // 1. Підбір предметів (Твій старий код)
        controls.Player.Interact.started += ctx => Interact();

        // 2. Лагодження аномалій (Ліва кнопка миші) - [НОВЕ]
        // Переконайся, що в Input Actions дія називається "Attack"
        controls.Player.Attack.started += ctx => TryFixAnomaly();

        inventory = GetComponent<Inventory>();
        controls.Player.NextItemQueue.started += ctx => inventory?.NextItem();
    }

    private void Start()
    {
        // [НОВЕ] Автоматично знаходимо камеру, якщо ти забув перетягнути її в інспекторі
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
        if (hud != null)
            hud.SetActive(false);
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // --- ТВОЯ ЛОГІКА ПІДБОРУ (НЕ ЗМІНЮВАЛАСЬ) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            currentPickable = other.gameObject;
            if (hud != null)
                hud.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable") && other.gameObject == currentPickable)
        {
            currentPickable = null;
            if (hud != null)
                hud.SetActive(false);
        }
    }

    private void Interact()
    {
        if (currentPickable != null)
        {
            Sprite itemSprite = currentPickable.GetComponent<PickableItem>()?.itemSprite;

            if (inventory != null && itemSprite != null)
            {
                GameObject clone = Instantiate(currentPickable);
                clone.SetActive(true);
                clone.transform.SetParent(null);
                clone.transform.position = Vector3.zero;
                clone.transform.rotation = Quaternion.identity;
                clone.tag = "Untagged";

                inventory.AddItem(new InventoryItem(currentPickable.name, itemSprite, clone));
            }

            currentPickable.SetActive(false);
            currentPickable = null;

            if (hud != null)
                hud.SetActive(false);
        }
    }

    // --- НОВА ЛОГІКА ДЛЯ АНОМАЛІЙ ---

    private void TryFixAnomaly()
    {
        // Створюємо невидимий промінь з центру камери вперед
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Якщо промінь влучив у щось на відстані 3 метри
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Перевіряємо, чи є на цьому об'єкті скрипт AnomalyInteraction
            AnomalyInteraction anomaly = hit.collider.GetComponent<AnomalyInteraction>();

            if (anomaly != null)
            {
                // Якщо так - намагаємось полагодити
                anomaly.TryFix();
            }
        }
    }
}