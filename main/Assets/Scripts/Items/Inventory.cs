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

public class Inventory : MonoBehaviour, IInventoryQueue<InventoryItem> //double inheritance hierarchy
{
    [Header("HUD Slots")]
    [SerializeField] private UnityEngine.UI.Image mainSlotImage;
    [SerializeField] private UnityEngine.UI.Image subSlotImage;

    [Header("Hand Onject")]
    [SerializeField] private Transform hand;
    private GameObject currentItemInHand;

    private InventoryQueue<InventoryItem> itemQueue = new InventoryQueue<InventoryItem>();
    
    //inventory interface Count delegation
    public int Count => itemQueue.Count;

    //inv. if. Enqueue() delegation
    public void Enqueue(InventoryItem item)
    {
        itemQueue.Enqueue(item);
        UpdateHUD();
        UpdateHand();
    }

    //inv. if. Dequeue() delegation
    public InventoryItem Dequeue()
    {
        InventoryItem item = itemQueue.Dequeue();
        UpdateHUD();
        UpdateHand();
        return item;
    }

    //inv. if. ToArray() delegation
    public InventoryItem[] IQtoArray()
    {
        return itemQueue.IQtoArray();
    }

    private void Start()
    {
        UpdateHUD();
        UpdateHand();
    }

    public void AddItem(InventoryItem newItem)
    {
        Enqueue(newItem);
    }

    public void NextItem()
    {
        if (Count > 1)
        {
            InventoryItem first = Dequeue();
            Enqueue(first);
        }
    }

    private void UpdateHUD()
    {
        InventoryItem[] items = IQtoArray();

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

        InventoryItem[] items = IQtoArray();

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
