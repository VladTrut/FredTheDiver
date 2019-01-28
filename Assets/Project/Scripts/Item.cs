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

    private bool m_isCollected = false;

    private void FixedUpdate()
    {
        if (Player.instance == null || m_isCollected)
            return;

        float d = Vector3.Distance(transform.position, Player.instance.transform.position);
        if(d < 2.0f)
        {
            StartCoroutine(WaitAndCollect());
        }
    }

    IEnumerator WaitAndCollect()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (!Player.instance.Collect(this))
            yield break;

        m_isCollected = true;
        gameObject.SetActive(false);

        if (m_type == InventoryMgt.ItemType.COIN)
            AudioManager.instance.PlaySound("ItemCoin");
        else if (m_type == InventoryMgt.ItemType.BAR)
            AudioManager.instance.PlaySoundAt("ItemIngot", 1.5f);
        else if (m_type == InventoryMgt.ItemType.CHEST)
            AudioManager.instance.PlaySound("ItemChest");
        // TODO animation
    }
}
