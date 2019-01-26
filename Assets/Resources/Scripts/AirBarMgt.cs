using System;
using UnityEngine;
using UnityEngine.UI;

public class AirBarMgt : MonoBehaviour {

    [SerializeField] private RectTransform m_OxygenBarRect;
    [SerializeField] private int m_OxygenCur;
    [SerializeField] private int m_OxygenMax;
    [SerializeField] private OxygenMgt m_OxMgt;


    //[SerializeField] private Sprite m_BarImageArray;
    //[SerializeField] private Image m_BarImageSlot;
    //[SerializeField] private Canvas m_BarCanvas;

    private void Start()
    {
        if (m_OxygenBarRect == null)
            Debug.LogError(this.name + " : m_OxygenBarRect no found");
        m_OxMgt = FindObjectOfType<OxygenMgt>();

    }

    private void Update()
    {
        m_OxMgt = FindObjectOfType<OxygenMgt>();
        if (m_OxMgt == null)
        {

        }

        if (m_OxMgt != null)
        {
            m_OxygenCur = m_OxMgt.CurrentOxygen;
            m_OxygenMax = m_OxMgt.MaxOxygen;
        }
        else
            Debug.Log(name + " : No OxMgt found");
        SetOxygen(m_OxygenCur, m_OxygenMax);
        
    }




    public void SetOxygen(int _cur, int _max)
    {
        float _value = ((float)_cur / (float)_max);
        m_OxygenBarRect.GetComponent<Image>().fillAmount = _value;
        //m_OxygenBarRect.rect.Set(m_OxygenBarRect.rect.x, m_OxygenBarRect.rect.y, m_OxygenBarRect.rect.width, m_OxygenBarRect.rect.height * m_Init);
        //m_OxygenBarRect.localScale = new Vector3(m_OxygenBarRect.localScale.x, _value, m_OxygenBarRect.localScale.z);

    }




}
