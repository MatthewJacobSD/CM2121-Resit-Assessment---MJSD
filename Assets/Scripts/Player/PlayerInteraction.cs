using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    void Update()
    {
        DetectObject();        
    }

    private void DetectObject()
    {
        Ray ray = new(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);
            Debug.Log("Looking at: " + hit.collider.name);
        }
    }
}
