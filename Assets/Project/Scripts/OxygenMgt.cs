using System.Collections.Generic;
using UnityEngine;

public class OxygenMgt : MonoBehaviour
{


    [SerializeField] private AirBarMgt m_AirBarUI;
    private bool m_SwimMode = false;
    private bool m_Swimming = false;
    [SerializeField] private int m_CurrentOxygen;
    [SerializeField] private int m_MaxOxygen;

    public List<ParticleCollisionEvent> collisionEvents;
    //[SerializeField] private float m_OxygenDecreaseRate;
    [SerializeField] private float m_OxygenIncreaseRate;
    [SerializeField] private float m_PassiveOxygenLossRate;

    [SerializeField] private float m_SurfaceY = -1f;

    public delegate void OnOxygenExhaustedDel();
    public static event OnOxygenExhaustedDel OnOxygenExhausted;

    public delegate void OnOxygenChangeDel(int value);
    public static event OnOxygenChangeDel OnOxygenChange;

    public int CurrentOxygen
    {
        get
        {
            return m_CurrentOxygen;
        }
        set
        {
            if (m_CurrentOxygen == value) return;
            m_CurrentOxygen = value;
            if (OnOxygenChange != null)
                OnOxygenChange(m_CurrentOxygen);
        }
    }

    public int MaxOxygen { get => m_MaxOxygen; set => m_MaxOxygen = value; }





    // Use this for initialization
    void Start()
    {


        CurrentOxygen = MaxOxygen;

        if (m_AirBarUI == null)
        {
            AirBarMgt airBarMgt = FindObjectOfType<AirBarMgt>();
            if (airBarMgt != null)
                m_AirBarUI = airBarMgt;
        }

        m_AirBarUI.SetOxygen( CurrentOxygen, MaxOxygen);


    }


    // Update is called once per frame
    void Update()
    {
    
        m_AirBarUI.SetOxygen( CurrentOxygen, MaxOxygen);
        SurfaceCheck();

    }

    private void SurfaceCheck()
    {
        if (transform.position.y + transform.up.y < m_SurfaceY)
        {
            CancelInvoke("IncreaseOxygen");
            if (!IsInvoking("PassiveOxygenLoss"))
                InvokeRepeating("PassiveOxygenLoss", 1f / m_PassiveOxygenLossRate, 1f / m_PassiveOxygenLossRate);

        }
        else
        {

            CancelInvoke("PassiveOxygenLoss");
            InvokeRepeating("IncreaseOxygen", 1f / m_OxygenIncreaseRate, 1f / m_OxygenIncreaseRate);
        }

        if (CurrentOxygen <= 0)
        {
            CancelInvoke("PassiveOxygenLoss");
            CurrentOxygen = 0;
            if (OnOxygenExhausted != null)
                OnOxygenExhausted();
        }
        else if (CurrentOxygen >= MaxOxygen)
        {
            CancelInvoke("IncreaseOxygen");
            CurrentOxygen = MaxOxygen;
        }
    }

    public void DecreaseOxygen(int value)
    {
         CurrentOxygen -= value;
    }

    void IncreaseOxygen()
    {
         CurrentOxygen += 1;
    }

    void PassiveOxygenLoss()
    {
         CurrentOxygen -= 1;
    }


}
