using System.Collections;
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
    public Renderer keyRenderer;

    private void Start()
    {
        StartCoroutine(UpdateKeyType());
    }
    private IEnumerator UpdateKeyType()
    {
        yield return new WaitForEndOfFrame();
        if (materials.Count == 0)
        {
            Debug.LogWarning("No materials found");
            yield break;
        }
        switch (type)
        {
            case KeyType.Red: // set visuals for this key type
                keyRenderer.material = materials[0];
                break;
            case KeyType.Blue:
                keyRenderer.material = materials[1];
                break;
            case KeyType.Yellow:
                keyRenderer.material = materials[2];
                break;
            default:
                keyRenderer.material = materials[0];
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
