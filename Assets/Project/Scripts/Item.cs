using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public InventoryMgt.ItemType m_type;
    public float m_weight = 1.0f;
    public float m_value = 1.0f;
    public float m_size = 1.0f;
    public float m_minDepth = 1.0f;
    public float m_maxDepth = 100.0f;

    public float Weight { get => m_weight; }
    public float Value { get => m_value; }
    public InventoryMgt.ItemType Type { get => m_type; }

    private void FixedUpdate()
    {
        if (Player.instance == null)
            return;

        float d = Vector3.Distance(transform.position, Player.instance.transform.position);
        if(d < 2.0f)
        {
            gameObject.SetActive(false);
            Player.instance.Collect(this);
        }
    }
}
