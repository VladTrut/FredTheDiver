using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public enum PlayerState {  LandIdle, UnderwaterIdle, UnderwaterSwim }

    private Rigidbody m_body;
    private Vector3 m_force;
    private Animator m_animator;

    private string m_animUnderwaterIdle = "Underwater-Idle";
    private string m_animLandIdle = "Land-Idle";
    private string m_animUnderwaterSwim = "Underwater-Swim";
    private PlayerState m_state = PlayerState.LandIdle;

    private void Start()
    {
        m_body = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        SwitchState(PlayerState.UnderwaterIdle);
    }

    private void FixedUpdate()
    {
        Vector3 f = Vector3.zero;
        float step = 10.0f;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            f += step * Vector3.up * Time.deltaTime;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            f += step * Vector3.left * Time.deltaTime;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            f -= step * Vector3.up * Time.deltaTime;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            f -= step * Vector3.left * Time.deltaTime;
            isMoving = true;            
        }

        if(isMoving)
        {
            SwitchState(PlayerState.UnderwaterSwim);
        }
        else
        {
            SwitchState(PlayerState.UnderwaterIdle);
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
        Vector3 direction = m_body.transform.position - transform.position;
        m_body.AddForceAtPosition(f.normalized, head);
        //m_body.AddForce(m_force, ForceMode.Impulse);

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

    public void SwitchState(PlayerState state)
    {
        if (m_state == state)
            return;
        if (state == PlayerState.LandIdle)
        {
            m_animator.Play(m_animLandIdle);
        }
        else if (state == PlayerState.UnderwaterIdle)
        {
            m_animator.Play(m_animUnderwaterIdle);
        }
        else if (state == PlayerState.UnderwaterSwim)
        {
            m_animator.Play(m_animUnderwaterSwim);
        }
        m_state = state;
    }

}


