using UnityEngine;

public class PlaneHazard : MonoBehaviour
{
    public GameManager gameManager;
    public DeliveryObject delivery;   // optional: auto-found if left empty

    void Awake()
    {
        // Make it red
        Renderer r = GetComponent<Renderer>();
        if (r != null)
            r.material.color = Color.red;
    }

    void Start()
    {
        if (delivery == null)
            delivery = FindFirstObjectByType<DeliveryObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!gameManager.isPlaying) return;

        Debug.Log("Stepped on a hazard!");

        gameManager.LoseAttempt();

        if (delivery != null && gameManager.isPlaying)
        {
            delivery.ResetDelivery();
        }
    }
}