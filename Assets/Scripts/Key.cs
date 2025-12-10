using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public KeyType type;

    public enum KeyType
    {
        Red,
        Blue,
        Yellow
    }

    public List<Material> materials;

    private void Awake()
    {
        materials = new List<Material>();
    }

    private void Start()
    {
        switch (type)
        {
            case KeyType.Red: // set visuals for this key type
                break;
            case KeyType.Blue:
                break;
            case KeyType.Yellow:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        KeyStorage keyStorage = other.gameObject.GetComponent<KeyStorage>();
        if (keyStorage != null)
        {
            keyStorage.AddKey(type);
            gameObject.SetActive(false);
        }
    }
}
