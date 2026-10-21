using UnityEngine;
using UnityEngine.XR;

public class GoalZone : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject snapPoint1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeliveryObject"))
        {
            Debug.Log("Successful Delivery!");

            gameManager.AddScore();

            DeliveryObject delivery =
                 other.GetComponent<DeliveryObject>();

            if (delivery != null)
            {
                delivery.ResetDelivery();
            }

        }
    }
}