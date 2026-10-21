using UnityEngine;

public class DeliveryObject : MonoBehaviour
{
    public Transform resetPoint;
    public GameObject hand;

    private Rigidbody rb;
    private bool isHeld = false;
    private bool isDelivered = false;

    // Only one item can be carried at a time
    private static DeliveryObject currentlyHeld;

    public bool IsHeld => isHeld;
    public bool IsDelivered => isDelivered;

    // Clears the static between play sessions (in case Domain Reload is off)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        currentlyHeld = null;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called by GoalZone: item stays on the table
    public void Deliver(Transform snapPoint)
    {
        if (currentlyHeld == this) currentlyHeld = null;

        isHeld = false;
        isDelivered = true;

        transform.SetParent(null, true);

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;
    }

    public void ResetDelivery()
    {
        if (isDelivered) return;   // delivered items stay on the table

        if (currentlyHeld == this) currentlyHeld = null;

        transform.SetParent(null, true);
        isHeld = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = resetPoint.position;
        transform.rotation = resetPoint.rotation;
        rb.position = resetPoint.position;
        rb.rotation = resetPoint.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHeld || isDelivered) return;
        if (currentlyHeld != null) return;   // already carrying something else

        if (other.CompareTag("Player"))
        {
            isHeld = true;
            currentlyHeld = this;

            transform.SetParent(hand.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}