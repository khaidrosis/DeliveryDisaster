using UnityEngine;
using UnityEngine.InputSystem;

public class DragDeliveryObject : MonoBehaviour
{
    [Header("Drag Settings")]
    [Tooltip("Max distance from the player within which objects can be grabbed.")]
    public float maxGrabDistance = 6f;

    [Tooltip("Max distance from the player the object can be dragged.")]
    public float maxDragDistance = 8f;

    [Tooltip("How quickly the held object follows the cursor.")]
    public float followSpeed = 20f;

    [Tooltip("Layers the drag raycast can hit (ground, counters, etc.).")]
    public LayerMask dragSurfaceMask = ~0;

    [Tooltip("Height the object floats above the surface while held.")]
    public float holdHeight = 0.5f;

    [Tooltip("Aim from the center of the screen (crosshair) instead of the mouse position. Auto-enabled when the cursor is locked.")]
    public bool useScreenCenter = true;

    [Header("Delivery")]
    [Tooltip("If true, the object snaps to the center of the drop zone when delivered.")]
    public bool snapToDropZone = true;

    [Tooltip("Extra vertical forgiveness (in units) when checking if an object is inside a drop zone.")]
    public float dropZoneVerticalPadding = 1f;

    [Tooltip("Destroy the object after it's delivered.")]
    public bool destroyOnDelivery = false;

    /// <summary>Fires when an object is delivered: (the object, the zone it landed in).</summary>
    public System.Action<GameObject, GameObject> OnDelivered;

    private Camera cam;
    private Transform player;
    private Transform held;
    private Rigidbody heldRb;
    private bool heldWasKinematic;
    private bool heldHadGravity;
    private Vector3 grabOffset;
    private Plane dragPlane;

    void Start()
    {
        cam = Camera.main;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : transform;

        if (cam == null)
            Debug.LogWarning("DragDeliveryObject: No camera tagged MainCamera found.");
    }

    void Update()
    {
        if (cam == null || Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPickUp();

        if (held != null)
        {
            if (Mouse.current.leftButton.isPressed)
                DragHeld();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                Drop();
        }
    }

    Ray MouseRay()
    {
        // First-person with a locked cursor: aim from the center of the screen (the crosshair).
        if (useScreenCenter || Cursor.lockState == CursorLockMode.Locked)
            return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        return cam.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    void TryPickUp()
    {
        Ray ray = MouseRay();

        // Find the first thing the ray hits, ignoring the player's own colliders
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        RaycastHit hit = default;
        bool found = false;
        foreach (RaycastHit h in hits)
        {
            if (h.collider.CompareTag("Player") || h.transform.IsChildOf(player)) continue;
            hit = h;
            found = true;
            break;
        }

        if (!found) return;

        // Accept the tag on the hit collider or on a parent (handles tagged parent + child mesh)
        Transform tagged = hit.collider.transform;
        while (tagged != null && !tagged.CompareTag("DeliveryObject"))
            tagged = tagged.parent;
        if (tagged == null) return;

        if (Vector3.Distance(player.position, tagged.position) > maxGrabDistance)
            return;

        held = tagged;
        heldRb = held.GetComponent<Rigidbody>();

        if (heldRb != null)
        {
            heldWasKinematic = heldRb.isKinematic;
            heldHadGravity = heldRb.useGravity;
            heldRb.isKinematic = true;
            heldRb.useGravity = false;
        }

        dragPlane = new Plane(Vector3.up, new Vector3(0, held.position.y, 0));

        if (dragPlane.Raycast(ray, out float enter))
            grabOffset = held.position - ray.GetPoint(enter);
        else
            grabOffset = Vector3.zero;
    }

    void DragHeld()
    {
        Ray ray = MouseRay();
        Vector3 target = held.position;

        // Prefer the actual surface under the cursor (floor, counter, etc.)
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f, dragSurfaceMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool foundSurface = false;
        foreach (RaycastHit h in hits)
        {
            // Skip the held object itself, other deliverables, and the player
            if (h.transform == held || h.transform.IsChildOf(held)) continue;
            if (h.collider.CompareTag("DeliveryObject")) continue;
            if (h.collider.CompareTag("Player")) continue;

            target = h.point + Vector3.up * holdHeight;
            foundSurface = true;
            break;
        }

        if (!foundSurface && dragPlane.Raycast(ray, out float enter))
            target = ray.GetPoint(enter) + grabOffset;

        // Keep within reach of the player
        Vector3 fromPlayer = target - player.position;
        if (fromPlayer.magnitude > maxDragDistance)
            target = player.position + fromPlayer.normalized * maxDragDistance;

        held.position = Vector3.Lerp(held.position, target, followSpeed * Time.deltaTime);
    }

    void Drop()
    {
        GameObject obj = held.gameObject;
        GameObject zone = FindDropZoneAt(held.position);

        if (heldRb != null)
        {
            heldRb.isKinematic = heldWasKinematic;
            heldRb.useGravity = heldHadGravity;
            heldRb.linearVelocity = Vector3.zero;   // Unity 6. On 2022 or earlier use .velocity
            heldRb.angularVelocity = Vector3.zero;
        }

        if (zone != null)
        {
            if (snapToDropZone)
            {
                Collider zc = zone.GetComponent<Collider>();
                Vector3 center = zc != null ? zc.bounds.center : zone.transform.position;
                held.position = center + Vector3.up * holdHeight;
            }

            Debug.Log($"{obj.name} delivered to {zone.name}!");
            OnDelivered?.Invoke(obj, zone);

            if (destroyOnDelivery)
                Destroy(obj);
        }

        held = null;
        heldRb = null;
    }

    /// <summary>Returns the first DropZone-tagged object whose collider contains the point, or null.</summary>
    GameObject FindDropZoneAt(Vector3 point)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag("DropZone");
        foreach (GameObject zone in zones)
        {
            Collider[] cols = zone.GetComponentsInChildren<Collider>();
            foreach (Collider c in cols)
            {
                Bounds b = c.bounds;
                b.Expand(new Vector3(0f, dropZoneVerticalPadding * 2f, 0f));
                if (b.Contains(point))
                    return zone;
            }
        }
        return null;
    }
}