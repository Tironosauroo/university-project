using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject prefab;

    public InventoryItem(string name, Sprite sprite, GameObject prefabObject)
    {
        itemName = name;
        itemSprite = sprite;
        prefab = prefabObject;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("HUD Слоти")]
    [SerializeField] private UnityEngine.UI.Image mainSlotImage;
    [SerializeField] private UnityEngine.UI.Image subSlotImage;

    [Header("Hand Onject")]
    [SerializeField] private Transform hand;
    private GameObject currentItemInHand;

    private Queue<InventoryItem> itemQueue = new Queue<InventoryItem>();

    private void Start()
    {
        UpdateHUD();
        UpdateHand();
    }

    public void AddItem(InventoryItem newItem)
    {
        itemQueue.Enqueue(newItem);
        UpdateHUD();
        UpdateHand();
    }

    public void NextItem()
    {
        if (itemQueue.Count > 1)
        {
            InventoryItem first = itemQueue.Dequeue();
            itemQueue.Enqueue(first);
            UpdateHUD();
            UpdateHand();
        }
    }

    private void UpdateHUD()
    {
        InventoryItem[] items = itemQueue.ToArray();

        // MainSlot
        if (items.Length > 0)
        {
            mainSlotImage.sprite = items[0].itemSprite;
            mainSlotImage.enabled = true;
        }
        else
        {
            mainSlotImage.sprite = null;
            mainSlotImage.enabled = false;
        }

        // SubSlot
        if (items.Length > 1)
        {
            subSlotImage.sprite = items[1].itemSprite;
            subSlotImage.enabled = true;
        }
        else
        {
            subSlotImage.sprite = null;
            subSlotImage.enabled = false;
        }
    }

    private void UpdateHand()
    {
        // removing last item
        if (currentItemInHand != null)
            Destroy(currentItemInHand);

        InventoryItem[] items = itemQueue.ToArray();

        // if there is/are items in queue - adding it/them
        if (items.Length > 0 && items[0].prefab != null)
        {
            currentItemInHand = Instantiate(items[0].prefab, hand);
            currentItemInHand.transform.localPosition = Vector3.zero;
            PickableItem pickable = items[0].prefab.GetComponent<PickableItem>();
            if (pickable != null)
                currentItemInHand.transform.localRotation = Quaternion.Euler(pickable.handRotation);
            else
                currentItemInHand.transform.localRotation = Quaternion.identity;
        }
    }
}
