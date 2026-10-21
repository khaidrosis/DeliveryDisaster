using UnityEngine;

public class PlaneHazard : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        Collider col = GetComponent<Collider>();

        if (col == null)
            Debug.LogError("PlaneHazard: no Collider on THIS object. The script must be on the same object as the collider.");
        else if (!col.isTrigger)
            Debug.LogWarning("PlaneHazard: collider is not set to Is Trigger.");

        if (gameManager == null)
            Debug.LogError("PlaneHazard: Game Manager slot is empty.");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlaneHazard trigger touched by: " + other.name + " (tag: " + other.tag + ")");

        // Finds the player even if the collider is on a child object with the wrong tag
        if (other.GetComponentInParent<PlayerController>() == null) return;

        Debug.Log("Stepped on a hazard!");
        gameManager.LoseAttempt();
    }

    // Backup in case the collider is not a trigger
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PlaneHazard solid-collided with: " + collision.gameObject.name);

        if (collision.collider.GetComponentInParent<PlayerController>() == null) return;

        Debug.Log("Stepped on a hazard! (solid collision)");
        gameManager.LoseAttempt();
    }
}