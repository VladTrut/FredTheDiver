using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Renderer>().enabled = false;
        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;
    }
}
