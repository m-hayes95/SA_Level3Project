using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interact : MonoBehaviour
{
    public LayerMask interactableLayer;
    public float maxDistance;
    private Collider collider;
    
    // Gizmos for testing
    private bool gizmoHasHit;
    private Vector3 gizmoHitPos;
    private Vector3 gizmoHalfExtents;
    private Vector3 gizmoStartCenter;
    private Vector3 gizmoEndCenter;
    private Quaternion  gizmoRotation;
    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
    
    public void TryInteract()
    {
        GameObject interacted = TryFindInteractableObject();
        if (interacted == null) return;
        
        IInteractable canInteract = interacted.GetComponent<IInteractable>();
        canInteract?.Interact(gameObject);
    }

    private GameObject TryFindInteractableObject()
    {
        GameObject objectFound = null;

        Vector3 center = collider.bounds.center;
        Vector3 halfExtents = collider.bounds.extents;
        Vector3 direction = transform.forward;
        Quaternion rotation = transform.rotation;
        
        gizmoStartCenter = center;
        gizmoHalfExtents = halfExtents;
        gizmoRotation = rotation;
        
        if (Physics.BoxCast(center, halfExtents,direction, out var hit, rotation , maxDistance,  interactableLayer))
        {
            gizmoHasHit = true;
            
            gizmoEndCenter = center + direction.normalized * hit.distance;
            
            objectFound = hit.transform.gameObject;
        }
        else
        {
            gizmoHasHit = false;
            
            gizmoEndCenter = center + direction.normalized * maxDistance;
        }
        
        return objectFound;
    }

    private void OnDrawGizmos()
    {
        /**
        if (collider == null) collider = GetComponent<Collider>();
        if (collider == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(gizmoStartCenter, gizmoRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, gizmoHalfExtents * 2f);
        
        Gizmos.color = gizmoHasHit ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(gizmoEndCenter, gizmoRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, gizmoHalfExtents * 2f);

        // Reset matrix
        Gizmos.matrix = Matrix4x4.identity;
        **/
    }
}
