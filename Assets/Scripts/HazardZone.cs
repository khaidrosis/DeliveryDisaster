using UnityEngine;

public class HazardZone : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DeliveryObject")) return;

        DeliveryObject delivery = other.GetComponentInParent<DeliveryObject>();
        if (delivery == null) return;

        Debug.Log("Delivery Failed!");
        gameManager.LoseAttempt();

        if (gameManager.isPlaying)
        {
            delivery.ResetDelivery();
        }
    }
}