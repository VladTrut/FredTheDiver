using System.Collections;
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
    public float m_boostAmount = 150.0f;
    public int m_boostOxygen = 10;
    public float m_dropBoostAmount = 100.0f;    

    private Rigidbody m_body;
    private Vector3 m_force;
    private Vector3 m_boost;
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
    [SerializeField] private int m_DropAmount;
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
        Vector3 head = m_body.position + m_body.transform.up;


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
                if (head.y < -1f)
                {
                    if (m_oxygenManager.CurrentOxygen > m_boostOxygen && m_boost.sqrMagnitude <= 1e-4)
                    {
                        AddBoost(m_boostAmount);
                        m_oxygenManager.DecreaseOxygen(m_boostOxygen);
                    }
                }
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

        if (head.y >= 0)
        {
            if (f.y > 0f)
                f.y *= -1.0f;
        }

        m_body.AddForceAtPosition(f, head, ForceMode.Impulse);

        float weightCoeff = (m_FullChargeSpeedCoeff - 1f) / m_itemsMaxWeight * m_itemsWeight + 1f;        
        if (head.y < 0)
            m_body.AddForce(m_force * weightCoeff, ForceMode.Impulse);

        // Boost
        if (m_body.position.y < 0)
            m_body.AddForce(m_boost, ForceMode.Impulse);
        m_boost *= 0.9f;

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
        // right turn
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
        // left turn
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

        // resistance force
        float speed = m_body.velocity.magnitude;
        float speed2 = speed * speed;
        m_body.AddForce(-m_body.velocity.normalized * speed2 * 0.25f * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    public void ApplyForce(Vector3 f)
    {
        m_force = f;
    }

    public void AddBoost(float amount)
    {
        Vector3 boost = m_body.transform.up;
        boost.z = 0.0f;
        boost.Normalize();
        m_boost += boost * Time.fixedDeltaTime * amount;
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
        m_oxygenManager.Reset();
        m_items.Clear();
        m_itemsWeight = 0.0f;
        m_itemsValue = 0.0f;
    }

    private IEnumerator Drop()
    {
        m_IsDropping = true;

        int itemsToDrop = m_DropAmount;
        while(itemsToDrop > 0 && m_items.Count > 0)
        {
            itemsToDrop--;
            Item lastItem = m_items[m_items.Count - 1];            

            m_itemsWeight -= lastItem.Weight;
            m_itemsValue -= lastItem.Value;
            InventoryMgt.instance.DecreaseItemType(lastItem.Type, (int)lastItem.Weight);
            m_items.RemoveAt(m_items.Count - 1);

            lastItem.gameObject.SetActive(true);
            // TODO randomize position, so that the item doesn't collide with the player?
            lastItem.transform.position = transform.position;
            GameObject droppeditems = GameObject.Find(m_DroppedItemsStr);
            if (droppeditems != null)
                lastItem.transform.parent = droppeditems.transform;

            AddBoost(m_dropBoostAmount);

            yield return new WaitForSeconds(0.15f);             
        }
        /*
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
        */
        yield return new WaitForSeconds(m_DropDelay);

        m_IsDropping = false;
    }
}


