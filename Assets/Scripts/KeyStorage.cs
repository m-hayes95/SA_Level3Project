using System.Collections.Generic;
using UnityEngine;

public class KeyStorage : MonoBehaviour
{
    [SerializeField] private List<Key.KeyType> heldKeys;

    private void Awake()
    {
        heldKeys = new List<Key.KeyType>();
    }
    public void AddKey(Key.KeyType keyType)
    {
        heldKeys.Add(keyType);
    }

    public void RemoveKey(Key.KeyType keyType)
    {
        heldKeys.Remove(keyType);
    }

    public bool CheckHasKey(Key.KeyType keyType)
    {
        return heldKeys.Contains(keyType);
    }
}
