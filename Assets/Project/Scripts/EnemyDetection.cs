using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyAI m_EnemyAI;

    private void Awake()
    {
        if (m_EnemyAI == null)
            m_EnemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_EnemyAI.State != EnemyAI.EnemyState.TARGET && m_EnemyAI.State != EnemyAI.EnemyState.GIVEUP)
        {
            if (m_EnemyAI.m_TargetsArray.Length >= 1)
            {
                for (int i = 0; i < m_EnemyAI.m_TargetsArray.Length; i++)
                {
                    if (other.tag == m_EnemyAI.m_TargetsArray[i].targettag)
                    {
                        bool hit = Physics.Raycast(this.transform.position, other.gameObject.transform.position - this.transform.position, 50f, m_EnemyAI.m_WhatIsTarget);

                        if (m_EnemyAI.GetTarget != null)
                        {
                                m_EnemyAI.SetTargetState(other.transform, m_EnemyAI.m_TargetsArray[i].priority, EnemyAI.EnemyState.TARGET);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_EnemyAI.State != EnemyAI.EnemyState.TARGET && m_EnemyAI.State != EnemyAI.EnemyState.GIVEUP)
        {
            if (m_EnemyAI.m_TargetsArray.Length >= 1)
            {
                for (int i = 0; i < m_EnemyAI.m_TargetsArray.Length; i++)
                {
                    if (other.tag == m_EnemyAI.m_TargetsArray[i].targettag)
                    {
                        bool hit = Physics.Raycast(this.transform.position, other.gameObject.transform.position - this.transform.position, Mathf.Infinity, m_EnemyAI.m_WhatIsTarget);

                        if (hit)
                        {

                            if (m_EnemyAI.GetTarget != null)
                            {
                                m_EnemyAI.SetTargetState(other.transform, m_EnemyAI.m_TargetsArray[i].priority, EnemyAI.EnemyState.TARGET);
                            }
                        }
                    }
                }
            }
        }
    }


}
