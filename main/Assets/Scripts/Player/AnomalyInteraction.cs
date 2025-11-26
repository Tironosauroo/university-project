using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnomalyInteraction : MonoBehaviour
{
    [Header("Settings")]
    public ItemType requiredItem;
    public GameObject normalState;
    public GameObject anomalyState;

    [Header("Audio")]
    [Tooltip("Звук, який грає, поки аномалія активна (може бути пустим)")]
    public AudioClip activeLoopSound;

    [Tooltip("Звук успішного виправлення (один раз)")]
    public AudioClip fixSuccessSound;

    private bool isFixed = false;
    public bool IsFixed => isFixed;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;

        SetAnomalyState(true);
    }

    public void SetAnomalyState(bool isActive)
    {
        isFixed = !isActive;

        if (normalState) normalState.SetActive(!isActive);
        if (anomalyState) anomalyState.SetActive(isActive);

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
            audioSource.Stop();
        }
    }

    public void TryFix()
    {
        if (isFixed) return;

        if (Inventory.Instance == null) return;

        var items = Inventory.Instance.IQtoArray();

        if (items.Length > 0 && items[0] != null)
        {
            PickableItem pickable = items[0].prefab.GetComponent<PickableItem>();

            if (pickable != null && (pickable.itemType == requiredItem || requiredItem == ItemType.Any))
            {
                Debug.Log("ANOMALY FIXED!");

                if (fixSuccessSound != null)
                {
                    AudioSource.PlayClipAtPoint(fixSuccessSound, transform.position);
                }

                SetAnomalyState(false);
                Inventory.Instance.NextItem();
            }
        }
    }
}