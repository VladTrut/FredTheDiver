using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public InventoryMgt.ItemType m_type;
    public float m_weight = 1.0f;
    public float m_value = 1.0f;
    public float m_spawnProbability = 1.0f;

    public float Weight { get => m_weight; }
    public float Value { get => m_value; }
    public InventoryMgt.ItemType Type { get => m_type; }
    public float SpawnProbability { get => m_spawnProbability; }

    private bool m_isCollected = false;
    private bool m_isCollecting = false;

    private void OnTriggerStay(Collider other)
    {
        if (m_isCollecting)
            return;

        if(other.gameObject.tag == "Player")
        {
            m_isCollecting = true;
            StartCoroutine(WaitAndCollect());
        }
    }

    IEnumerator WaitAndCollect()
    {
        //yield return new WaitForSeconds(0.25f);

        if (!Player.instance.Collect(this))
        {
            m_isCollecting = false;
            yield break;
        }

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
