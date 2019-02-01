﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public enum PlayerState { LandIdle, UnderwaterIdle, UnderwaterSwim }
    public static Player instance;

    public float m_speed = 10.0f;
    public float m_zOrigin = -43.0f;

    private Rigidbody m_body;
    private Vector3 m_force;
    private Animator m_animator;

    private string m_animUnderwaterIdle = "Underwater-Idle";
    private string m_animLandIdle = "Land-Idle";
    private string m_animUnderwaterSwim = "Underwater-Swim";
    private PlayerState m_state = PlayerState.LandIdle;
    private int m_direction = 1;

    private List<Item> m_items = new List<Item>();
    private float m_itemsWeight = 0;
    private float m_itemsMaxWeight = 100;
    private float m_itemsValue = 0;
    private OxygenMgt m_oxygenManager;
    public Vector3 BodyPosition { get => m_body.transform.position; }
    public Vector3 HeadPosition { get => m_body.transform.position + m_body.transform.up; }
    private bool m_IsDropping;
    [SerializeField] private float m_DropDelay;
    [Range (0.6f, 1f)][SerializeField] private float m_FullChargeSpeedCoeff = 0.25f;
    private string m_DroppedItemsStr = "DroppedItems";

    private void Awake()
    {
        instance = this;
        m_oxygenManager = GetComponent<OxygenMgt>();
    }

    private void Start()
    {
        m_body = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        SwitchState(PlayerState.UnderwaterIdle);
    }

    private void FixedUpdate()
    {
        Vector3 f = Vector3.zero;
        float step = m_speed;
        bool isMoving = false;
        bool isPlayerDead = OxygenMgt.instance.CurrentOxygen <= 0;

        if (!isPlayerDead)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
            {
                f += step * Vector3.up * Time.fixedDeltaTime;
                isMoving = true;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
            {
                f += step * Vector3.left * Time.fixedDeltaTime;
                isMoving = true;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                f -= step * Vector3.up * Time.fixedDeltaTime;
                isMoving = true;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                f -= step * Vector3.left * Time.fixedDeltaTime;
                isMoving = true;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                // TODO Boost
            }
            if (Input.GetKey(KeyCode.Space))
            {
                if (!m_IsDropping && m_items.Count > 0)
                    StartCoroutine(Drop());

                // TODO UI so that the players knows that space = drop
            }
        }

        // TODO Smooth transition between states
        if (isMoving)
        {
            SwitchState(PlayerState.UnderwaterSwim);
        }
        else
        {
            SwitchState(PlayerState.UnderwaterIdle);
        }

        if (f.y > 0f)
            f.y = Mathf.Min(f.y, 0.05f);
        if (f.x > 0f)
            f.x = Mathf.Min(f.x, 0.025f);
        else if (f.x < 0f)
            f.x = Mathf.Max(f.x, -0.025f);

        Vector3 head = m_body.position + m_body.transform.up;
        if (head.y >= 0)
        {
            if (f.y > 0f)
                f.y *= -1.0f;
        }

        float weightCoeff = (m_FullChargeSpeedCoeff - 1f) / m_itemsMaxWeight * m_itemsWeight + 1f;

        m_body.AddForceAtPosition(f, head, ForceMode.Impulse);
        m_body.AddForce(m_force * weightCoeff, ForceMode.Impulse);

        // keep player on path
        float offset = m_body.transform.position.z - m_zOrigin;
        m_body.AddForce(-Vector3.forward * offset, ForceMode.Impulse);

        float targetDir = Vector3.Dot(Vector3.right, f.normalized);

        if (!isMoving)
        {
            float alpha2 = Vector3.Angle(m_body.transform.up, Vector3.up);
            bool isUp = Vector3.Dot(-m_body.transform.forward, Vector3.up) >= 0.0f;
            if (isUp)
            {
                if (m_direction == 1)
                    m_body.AddTorque(Vector3.forward * alpha2 * 1f * Time.fixedDeltaTime * (isUp ? 1f : -1f), ForceMode.Force);
                else if (m_direction == -1)
                    m_body.AddTorque(Vector3.forward * alpha2 * 1f * Time.fixedDeltaTime * (isUp ? -1f : 1f), ForceMode.Force);
            }
        }
        // right
        else if (targetDir > 1e-2f || (targetDir >= -1e-2f && m_direction == 1))
        {
            m_direction = 1;
            float alpha2 = Vector3.Angle(Vector3.right, m_body.transform.forward);
            bool isRight = Vector3.Dot(Vector3.right, m_body.transform.right) >= 0.0f;
            Vector3 r = Vector3.up * alpha2 * 0.75f * Time.fixedDeltaTime * (isRight ? 1f : -1f);
            m_body.AddTorque(r, ForceMode.Force);

            alpha2 = Vector3.Angle(m_body.transform.up, f);
            bool isUp = Vector3.Dot(-m_body.transform.forward, f) >= 0.0f;
            m_body.AddTorque(Vector3.forward * alpha2 * 0.75f * Time.fixedDeltaTime * (isUp ? 1f : -1f), ForceMode.Force);
        }
        // left
        else if (targetDir < -1e-2f || (targetDir <= 1e-2f && m_direction == -1))
        {
            m_direction = -1;
            float alpha2 = Vector3.Angle(Vector3.left, m_body.transform.forward);
            bool isLeft = Vector3.Dot(Vector3.left, m_body.transform.right) >= 0.0f;
            Vector3 r = Vector3.up * alpha2 * 0.75f * Time.fixedDeltaTime * (isLeft ? 1f : -1f);
            m_body.AddTorque(r, ForceMode.Force);

            alpha2 = Vector3.Angle(m_body.transform.up, f);
            bool isUp = Vector3.Dot(-m_body.transform.forward, f) >= 0.0f;
            m_body.AddTorque(Vector3.forward * alpha2 * 0.75f * Time.fixedDeltaTime * (isUp ? -1f : 1f), ForceMode.Force);
        }

        float speed = m_body.velocity.magnitude;
        float speed2 = speed * speed;
        m_body.AddForce(-m_body.velocity.normalized * speed2 * 0.25f * Time.fixedDeltaTime, ForceMode.Impulse);

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

    private void OnParticleCollision(GameObject other)
    {
        if (m_oxygenManager.CurrentOxygen < m_oxygenManager.MaxOxygen)
        {
            m_oxygenManager.CurrentOxygen = Mathf.Min(m_oxygenManager.CurrentOxygen + 10, m_oxygenManager.MaxOxygen);
            AudioManager.instance.PlaySound("PlayerBreathAboveWater");
        }
    }

    public bool Collect(Item item)
    {
        float w = m_itemsWeight + item.Weight;
        if (w > m_itemsMaxWeight || m_IsDropping)
            return false;

        m_items.Add(item);
        m_itemsWeight = w;
        m_itemsValue += item.Value;
        InventoryMgt.instance.IncreaseItemType(item.Type, (int)item.Weight);

        return true;
    }

    public void GiveAllBack()
    {
        if (m_items.Count == 0)
            return;

        InventoryMgt.instance.GiveAllBack((int)m_itemsValue);
        m_items.Clear();
        m_itemsWeight = 0.0f;
        m_itemsValue = 0.0f;
        AudioManager.instance.PlaySound("BoatTakeAllItems");
    }

    public void Reset()
    {
        InventoryMgt.instance.Reset();
        OxygenMgt.instance.Reset();
        m_items.Clear();
        m_itemsWeight = 0.0f;
        m_itemsValue = 0.0f;
    }

    private IEnumerator Drop()
    {
        m_IsDropping = true;

        int itemNumber = m_items.Count;
        if (itemNumber > 0)
        {
            Item lastItem = m_items[itemNumber - 1];
            Project project = FindObjectOfType<Project>();
            if (project != null)
            {
                GameObject droppedItem = project.GetComponent<Project>().InstantiateItem(lastItem.Type, transform.position);
                droppedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                if (droppedItem != null)
                {
                    GameObject droppeditems = GameObject.Find(m_DroppedItemsStr);
                    if (droppeditems != null)
                        droppedItem.transform.parent = droppeditems.transform;
                }

                m_itemsWeight -= lastItem.Weight;
                m_itemsValue -= lastItem.Value;
                InventoryMgt.instance.DecreaseItemType(lastItem.Type, (int)lastItem.Weight);
                m_items.RemoveAt(itemNumber - 1);
            }
        }

        yield return new WaitForSeconds(m_DropDelay);

        m_IsDropping = false;
    }
}


