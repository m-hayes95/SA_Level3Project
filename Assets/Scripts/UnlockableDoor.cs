using System;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;

public class UnlockableDoor : MonoBehaviour, IInteractable
{
    public Key.KeyType keyType;
    public UnityEvent OnOpen;
    
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
            Open();
        }
    }

    private void Open()
    {
        OnOpen?.Invoke();
        gameObject.SetActive(false);
    }
}
