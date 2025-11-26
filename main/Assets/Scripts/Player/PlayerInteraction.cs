using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("HUD for Pickable Items")]
    [SerializeField] private GameObject hud;

    [Header("Anomaly Interaction")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionDistance = 3f;

    private PlayerControls controls;
    private GameObject currentPickable;
    private Inventory inventory;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Interact.started += ctx => Interact();

        controls.Player.Attack.started += ctx => TryFixAnomaly();

        inventory = GetComponent<Inventory>();
        controls.Player.NextItemQueue.started += ctx => inventory?.NextItem();
    }

    private void Start()
    {
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

    private void TryFixAnomaly()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            AnomalyInteraction anomaly = hit.collider.GetComponent<AnomalyInteraction>();

            if (anomaly != null)
            {
                anomaly.TryFix();
            }
        }
    }
}