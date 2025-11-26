using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LoopManager.Instance != null)
            {
                LoopManager.Instance.TriggerExit();
            }
            else
            {
                Debug.LogError("LoopManager not found!");
            }
        }
    }
}