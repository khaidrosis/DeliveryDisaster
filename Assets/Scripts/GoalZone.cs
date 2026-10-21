using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject snapPoint1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DeliveryObject")) return;

        // Find the DeliveryObject even if the collider is on a child mesh
        DeliveryObject delivery = other.GetComponentInParent<DeliveryObject>();
        if (delivery == null || !delivery.IsHeld) return;   // only count a carried burger, once

        Debug.Log("Successful Delivery!");
        gameManager.AddScore();
        delivery.ResetDelivery();   // sets IsHeld = false, so re-entry can't double count
    }
}