﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public float m_maxSafeDistance = 100.0f;
    public float m_speed = 1.0f;
    public float m_range = 0.15f;

    private float m_step = -1.0f;
    private float m_value = 0.0f;
    private Rigidbody m_body;

    private void Awake()
    {
        //m_body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var pos = transform.position;
        float ds = m_step * Time.deltaTime * 0.1f;
        m_value += ds;
        pos.y += Mathf.Sin(m_value) * m_range * 0.1f;
        transform.position = pos;

   
        if (m_value > m_range && m_step > 0.0f)
        {
            m_value -= ds;
            m_step *= -1.0f;
        }
        else if(m_value < -m_range && m_step < 0.0f)
        {
            m_value -= ds;
            m_step *= -1.0f;
        }
    }

    private void FixedUpdate()
    {
        //m_body.AddForce(transform.forward * m_speed * Time.fixedDeltaTime, ForceMode.Impulse);
        var pos = transform.position;
        pos += Vector3.right * m_speed * Time.fixedDeltaTime;
        transform.position = pos;
    }

}
