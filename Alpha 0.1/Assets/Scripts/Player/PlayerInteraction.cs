using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("HUD for Pickable Items")]
    [SerializeField] private GameObject hud;  // HUD inside Canvas

    private PlayerControls controls;
    private GameObject currentPickable;
    private Inventory inventory;

    private void Awake()
    {
        controls = new PlayerControls();

        // subscribe on events
        controls.Player.Interact.started += ctx => Interact();

        inventory = GetComponent<Inventory>();
        controls.Player.NextItemQueue.started += ctx => inventory?.NextItem();
    }

    private void OnEnable()
    {
        controls.Enable();
        if (hud != null)
            hud.SetActive(false); // default disabled HUD cursor
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
                // clone of active item
                GameObject clone = Instantiate(currentPickable);
                clone.SetActive(true); // must be true for OK copy
                clone.transform.SetParent(null); // removing from scene
                clone.transform.position = Vector3.zero;
                clone.transform.rotation = Quaternion.identity;
                clone.tag = "Untagged"; // no infinite picking

                // adding copy to queue
                inventory.AddItem(new InventoryItem(currentPickable.name, itemSprite, clone));
            }

            currentPickable.SetActive(false);
            currentPickable = null;

            if (hud != null)
                hud.SetActive(false);
        }
    }
}
