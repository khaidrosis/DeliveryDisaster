using UnityEngine;

public class Target : MonoBehaviour
{
    public void HitTarget()
    {
        Debug.Log("Target Hit!");

        gameObject.SetActive(false);
    }
}