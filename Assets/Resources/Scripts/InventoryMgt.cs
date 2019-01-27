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
            //[i].AddComponent<Image>();
            m_ObjectArray[i].transform.SetParent(this.transform);
            m_ObjectArray[i].transform.position = new Vector3(GetComponent<RectTransform>().rect.xMax - i * coeff + Screen.width - screenoffset, transform.position.y, 0);
            // = m_ObjectArray[m_TakenSlot].AddComponent<Image>();

            //img.sprite = m_FrameImage[(int) type];

            m_ObjectArray[i].GetComponentInChildren<Image>().sprite = m_Items[i].m_FrameImage;
            m_ObjectArray[i].transform.GetComponentInChildren<Image>().gameObject.transform.localScale = m_Items[i].m_Scale;
            //m_ObjectArray[i].GetComponentInChildren<Text>().gameObject.transform.localScale = new Vector3(1/ m_Items[i].m_Scale.x, 1/ m_Items[i].m_Scale.y, 1/ m_Items[i].m_Scale.z) ;
            //Image img = m_ObjectArray[i].AddComponent<Image>();


        }

        for (int i = 0; i < m_Items.Length; i++)
        {

        }
        //m_ObjectArray = new GameObject[m_SlotNbMax];

        //m_FrameImage = new Sprite[(int)ItemType.ITEM_NB];
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyUp(KeyCode.U))
        {

            IncreaseItemType(ItemType.COIN);
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            IncreaseItemType(ItemType.BAR);
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            IncreaseItemType(ItemType.CHEST);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            GiveAllBack();
        }*/

    }

    public void IncreaseItemType(InventoryMgt.ItemType type)
    {
        m_ObjectQuantity[(int)type] += 1;
        m_ObjectArray[(int)type].GetComponentInChildren<Text>().text = m_ObjectQuantity[(int)type].ToString();

    }




    private void InstantiateItemType(int type)
    {

        if (m_TakenSlot < m_SlotNbMax)
        {


            //Image img = new GameObject().AddComponent<Image>();


            //m_ObjectArray[m_TakenSlot].transform.parent = this.transform;
            m_ObjectArray[m_TakenSlot].transform.position = new Vector3(GetComponent<RectTransform>().rect.xMax - m_TakenSlot * coeff + Screen.width - screenoffset, transform.position.y, 0);
            // = m_ObjectArray[m_TakenSlot].AddComponent<Image>();

            //img.sprite = m_FrameImage[(int) type];
            m_ObjectArray[m_TakenSlot].GetComponent<Image>().enabled = true;
            m_ObjectArray[m_TakenSlot].GetComponent<Image>().sprite = m_Items[type].m_FrameImage;
            m_ObjectArray[m_TakenSlot].transform.localScale = m_Items[type].m_Scale;
            m_TakenSlot++;
        }

    }

    void GiveAllBack()
    {
        for (int i = 0; i < m_ObjectArray.Length; i++)
        {
            m_ObjectQuantity[i] = 0;
            m_ObjectArray[(int)i].GetComponentInChildren<Text>().text = m_ObjectQuantity[i].ToString();

        }
    }


}
