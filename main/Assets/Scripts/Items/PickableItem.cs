using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("HUD Sprite")] // for inventory slots
    public Sprite itemSprite;

    [Header("InHand Rotation")] // for custom rotation in hand
    public Vector3 handRotation = Vector3.zero;
}

