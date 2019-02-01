using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryMgt : MonoBehaviour
{
    public static InventoryMgt instance;

    [System.Serializable]
    public class Item
    {
        public Sprite m_FrameImage;
        public Vector3 m_Scale;

    }
    public enum ItemType { COIN, BAR, CHEST, ITEM_NB };
    [SerializeField] private int m_SlotNbMax;
    [SerializeField] private int m_TakenSlot;
    [SerializeField] private Item[] m_Items;
    private GameObject[] m_ObjectArray;
    private int[] m_ObjectQuantity;
    [SerializeField] private GameObject m_ItemPrefab;
    public float screenoffset;
    public float coeff;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectArray = new GameObject[(int)ItemType.ITEM_NB];
        m_ObjectQuantity = new int[(int)ItemType.ITEM_NB];


        for (int i = 0; i < m_ObjectQuantity.Length; i++)
        {
            m_ObjectQuantity[i] = 0;
        }
        for (int i = 0; i < (int)ItemType.ITEM_NB; i++)
        {
            m_ObjectArray[i] = Instantiate(m_ItemPrefab, transform);

            m_ObjectArray[i].transform.SetParent(this.transform);
            m_ObjectArray[i].transform.position = new Vector3(GetComponent<RectTransform>().rect.xMax - i * coeff + Screen.width - screenoffset, transform.position.y, 0);

            m_ObjectArray[i].GetComponentInChildren<Image>().sprite = m_Items[i].m_FrameImage;
            m_ObjectArray[i].transform.GetComponentInChildren<Image>().gameObject.transform.localScale = m_Items[i].m_Scale;

        }

        for (int i = 0; i < m_Items.Length; i++)
        {

        }
    }

    private void Awake()
    {
        instance = this;
    }


    public void IncreaseItemType(InventoryMgt.ItemType type, int weight)
    {
        m_ObjectQuantity[(int)type] += 1;
        m_ObjectArray[(int)type].GetComponentInChildren<Text>().text = m_ObjectQuantity[(int)type].ToString();
        WeightCounter.instance.IncreaseWeight(weight);
    }

    public void DecreaseItemType(InventoryMgt.ItemType type, int weight)
    {
        if (m_ObjectQuantity[(int)type] > 0)
        {
            m_ObjectQuantity[(int)type] -= 1;
            m_ObjectArray[(int)type].GetComponentInChildren<Text>().text = m_ObjectQuantity[(int)type].ToString();
            WeightCounter.instance.DecreaseWeight(weight);
        }
    }


    private void InstantiateItemType(int type)
    {

        if (m_TakenSlot < m_SlotNbMax)
        {
            m_ObjectArray[m_TakenSlot].transform.position = new Vector3(GetComponent<RectTransform>().rect.xMax - m_TakenSlot * coeff + Screen.width - screenoffset, transform.position.y, 0);
            m_ObjectArray[m_TakenSlot].GetComponent<Image>().enabled = true;
            m_ObjectArray[m_TakenSlot].GetComponent<Image>().sprite = m_Items[type].m_FrameImage;
            m_ObjectArray[m_TakenSlot].transform.localScale = m_Items[type].m_Scale;
            m_TakenSlot++;
        }

    }

    public void GiveAllBack(int value)
    {
        for (int i = 0; i < m_ObjectArray.Length; i++)
        {
            m_ObjectQuantity[i] = 0;
            m_ObjectArray[(int)i].GetComponentInChildren<Text>().text = m_ObjectQuantity[i].ToString();

        }

        ScoreCounter.instance.IncreaseScore(value);
        WeightCounter.instance.ResetWeight();
    }

    public void Reset()
    {
        if (m_ObjectArray == null)
            return;

        ScoreCounter.instance.ResetScore();
        WeightCounter.instance.ResetWeight();
        for (int i = 0; i < m_ObjectArray.Length; i++)
        {
            m_ObjectQuantity[i] = 0;
            m_ObjectArray[(int)i].GetComponentInChildren<Text>().text = m_ObjectQuantity[i].ToString();

        }
    }


}
