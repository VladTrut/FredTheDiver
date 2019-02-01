using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeightCounter : MonoBehaviour
{
    public static WeightCounter instance;

    [SerializeField] private Text m_WeightText;
    private int m_Weight;
    [SerializeField] private int m_WeightLimit;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        m_Weight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_WeightText.text = m_Weight.ToString() + " / " + m_WeightLimit;
    }

    public void IncreaseWeight(int value)
    {
        m_Weight += value;
    }

    public void DecreaseWeight(int value)
    {
        m_Weight -= value;
    }

    public void ResetWeight()
    {
        m_Weight = 0;
    }
}
