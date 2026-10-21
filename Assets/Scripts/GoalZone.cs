using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public GameManager gameManager;
    public Transform[] snapPoints;   // one empty object per item on the table

    private int nextSnap = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DeliveryObject")) return;

        DeliveryObject delivery = other.GetComponentInParent<DeliveryObject>();
        if (delivery == null || !delivery.IsHeld) return;

        if (snapPoints == null || snapPoints.Length == 0)
        {
            Debug.LogWarning("GoalZone has no snap points assigned!");
            return;
        }

        Debug.Log("Successful Delivery!");
        gameManager.AddScore();

        // Next free spot (reuses the last one if you run out)
        Transform snap = snapPoints[Mathf.Min(nextSnap, snapPoints.Length - 1)];
        nextSnap++;

        delivery.Deliver(snap);
    }
}