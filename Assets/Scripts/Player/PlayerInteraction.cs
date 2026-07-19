using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Pickup Settings")]
    [SerializeField] private Transform holdPosition;

    private PickupObject currentObject;

    void Update()
    {
        DetectObject();        
    }

    private void DetectObject()
    {
        Ray ray = new(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();

            if (pickup != null) 
            {
                Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);
                Debug.Log("Looking at: " + hit.collider.name);
            }
        }
    }

    public void OnIteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentObject != null) return;

        Ray ray = new(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();
            if (pickup != null)
            {
                currentObject = pickup;
                currentObject.Pickup(holdPosition);
                Debug.Log("Picked up: " + hit.collider.name);
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentObject == null) return;
        
        currentObject.Drop();
        Debug.Log("Dropped: " + currentObject.name);
        
        currentObject = null;
    }
}
