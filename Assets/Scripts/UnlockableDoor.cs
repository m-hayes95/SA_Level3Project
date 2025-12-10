using System;
using Interfaces;
using UnityEngine;

public class UnlockableDoor : MonoBehaviour, IInteractable
{
    public Key.KeyType keyType;
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
        gameObject.SetActive(false);
    }
}
