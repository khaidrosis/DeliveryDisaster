using UnityEngine;

public class DeliveryObject : MonoBehaviour
{
    public Transform resetPoint;
    public GameObject hand;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetDelivery()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = resetPoint.position;
        rb.rotation = resetPoint.rotation;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.SetParent(hand.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}