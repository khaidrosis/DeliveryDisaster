using UnityEngine;

public class DeliveryObject : MonoBehaviour
{
    public Transform resetPoint;
    public GameObject hand;

    private Rigidbody rb;
    private bool isHeld = false;

    // Goal/Hazard zones check this so one delivery can only count once
    public bool IsHeld => isHeld;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetDelivery()
    {
        // Let go of the hand first
        transform.SetParent(null, true);
        isHeld = false;

        // Restore normal physics
        rb.isKinematic = false;
        rb.useGravity = true;

        // Zero velocity only works on a non-kinematic body, so do it after the lines above
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Teleport (set transform too so it applies instantly)
        transform.position = resetPoint.position;
        transform.rotation = resetPoint.rotation;
        rb.position = resetPoint.position;
        rb.rotation = resetPoint.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHeld) return;

        if (other.CompareTag("Player"))
        {
            isHeld = true;
            transform.SetParent(hand.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}