using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
