using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;

public class UnlockableDoor : MonoBehaviour, IInteractable
{
    public Key.KeyType keyType;
    public UnityEvent OnOpen;
    public float openAmount = 5;
    public float openTime = 1.5f;
    [Header("Door Materials")]
    public List<Material> doorMaterials;
    public Renderer doorRenderer;

    private void Start()
    {
        switch (keyType)
        {
            case Key.KeyType.Red:
                doorRenderer.material = doorMaterials[0];
                break;
            case Key.KeyType.Blue:
                doorRenderer.material = doorMaterials[1];
                break;
            case Key.KeyType.Yellow:
                doorRenderer.material = doorMaterials[2];
                break;
            default:
                doorRenderer.material = doorMaterials[0];
                break;
        }
    }
    public void Interact(GameObject instigator)
    {
        TryOpenDoor(instigator);
    }

    private void TryOpenDoor(GameObject instigator)
    {
        KeyStorage keys = instigator.GetComponent<KeyStorage>();
        if (keys.CheckHasKey(keyType))
        {
            keys.RemoveKey(keyType); // Remove if you want to have perm keys
            StartCoroutine(Open());
        }
    }

    private IEnumerator Open()
    {
        OnOpen?.Invoke();
        
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.down * openAmount;
        
        while (elapsedTime < openTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / openTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        //gameObject.SetActive(false);
    }
}
