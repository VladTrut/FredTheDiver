using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody m_body;
    private Vector3 m_force;

    private void Start()
    {
        m_body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 f = Vector3.zero;
        float step = 10.0f;
        if(Input.GetKey(KeyCode.W))
        {
            f += step * Vector3.up * Time.deltaTime;            
        }
        if (Input.GetKey(KeyCode.A))
        {
            f += step * Vector3.left * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            f -= step * Vector3.up * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            f -= step * Vector3.left * Time.deltaTime;
        }
        
        if(f.y > 0f)
            f.y = Mathf.Min(f.y, 0.05f);
        if(f.x > 0f)
            f.x = Mathf.Min(f.x, 0.025f);
        else if (f.x < 0f)
            f.x = Mathf.Max(f.x, -0.025f);
        //Debug.Log(m_body.velocity.magnitude);

        Vector3 head = m_body.position + m_body.transform.up;
        if (head.y >= 0)
        {
            if (f.y > 0f)
                f.y *= -1.0f;
        }

        m_body.AddForce(f, ForceMode.Impulse);
        //m_body.AddForceAtPosition(f, head, ForceMode.Impulse);
        m_body.AddForce(m_force, ForceMode.Impulse);

        float speed = m_body.velocity.magnitude;
        float speed2 = speed * speed;
        m_body.AddForce(-m_body.velocity.normalized * speed2 * 0.5f * Time.fixedDeltaTime, ForceMode.Impulse);

      
            

        m_force -= Vector3.one * Time.fixedDeltaTime; 
        m_force.x = Mathf.Max(0f, m_force.x);
        m_force.y = Mathf.Max(0f, m_force.y);
        m_force.z = Mathf.Max(0f, m_force.z);
    }

    public void ApplyForce(Vector3 f)
    {
        m_force = f;
    }
}


