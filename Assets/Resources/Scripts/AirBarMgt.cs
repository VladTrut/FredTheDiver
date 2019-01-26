using System;
using UnityEngine;
using UnityEngine.UI;

public class AirBarMgt : MonoBehaviour {

    [SerializeField] private RectTransform m_OxygenBarRect;
    [SerializeField] private int m_OxygenCur;
    [SerializeField] private int m_OxygenMax;


    //[SerializeField] private Sprite m_BarImageArray;
    //[SerializeField] private Image m_BarImageSlot;
    //[SerializeField] private Canvas m_BarCanvas;

    private void Start()
    {
        if (m_OxygenBarRect == null)
            Debug.LogError(this.name + " : m_OxygenBarRect no found");

    }

    private void Update()
    {

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
