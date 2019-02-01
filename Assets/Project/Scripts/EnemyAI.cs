using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    private Animator m_Anim;
    public bool DebugMode;

    /* ---- DRAGON BEHAVIOR ---- */

    public enum EnemyState { PATROL, TARGET, GIVEUP };
    [SerializeField] private EnemyState m_State;
    private EnemyState m_LastState;
   // [SerializeField] private AILerp m_AiLerpScript;
   // [SerializeField] private AIDestinationSetter m_AISetter;
    public bool[] m_ModeEnabled;
    private Enemy m_Enemy;
    private const int m_ModeNb = 5;

    [SerializeField] private int m_Damage;

    /* ----------------------------- */

    /* ---- MODE TARGET ---- */

    // target related
    [System.Serializable]
    public class TargetData
    {
        public string targettag;
        public int priority;
    };

    [System.Serializable]
    public class Target
    {
        public Transform targettransform;
        public int targetpriority;

    };

    [SerializeField] private Target m_Target;
    public TargetData[] m_TargetsArray;
    public LayerMask m_WhatIsTarget;
    [SerializeField] private float m_GiveUpDistance = 15f;

    // Detection
    [SerializeField] private GameObject m_Detection;
    private Transform m_DetectionTransform;

    // orientation
    private bool m_FacingRight = true;

    /* ----------------------------- */

    /* ---- MODE PATROL ---- */

    public Transform[] m_PatrolPoints;
    private int m_LastPatrolIndex = 0;
    private int m_PatrolPointIndex = 0;
    public bool m_Randomize;
    private int[] m_validChoices;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_PointReachDist;
    /* ----------------------------- */

    /* ---- MODE SLEEP ---- */

    private Transform m_StartPosition;

    /* ----------------------------- */
    private bool m_ForceApplied = false;
    [SerializeField] private float m_ApplyForceDelay = 1f;
    [SerializeField] private float m_DamageDelay = 1f;
    private bool m_DamageTriggered;



    public EnemyState State
    {
        get
        {
            return m_State;
        }

        set
        {
            if (m_State == value) return;
            if (m_ModeEnabled[(int)value] == false) return;
            switch (value)
            {
                case EnemyState.PATROL:
                    m_Anim.SetBool("Attack", false);
                    break;

                case EnemyState.TARGET:
                    m_DetectionTransform.position = transform.position;
                    m_Anim.SetBool("Attack", true);
                    break;

                case EnemyState.GIVEUP:
                    m_Anim.SetBool("Attack", false);
                    break;

                default:
                    if (DebugMode)
                        Debug.Log("Unknown state in " + this.name);
                    break;
            }
            m_State = value;

        }
    }


    public Target GetTarget
    {
        get
        {
            return m_Target;
        }
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }



    private void Awake()
    {
        if (m_Enemy == null)
            m_Enemy = GetComponent<Enemy>();
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_Detection == null)
            m_Detection = GetComponentInChildren<EnemyDetection>().gameObject;



        m_ModeEnabled = new bool[m_ModeNb];



        for (int i = 0; i < m_ModeNb; i++)
        {
            m_ModeEnabled[i] = true;
        }

        if (m_TargetsArray.Length < 1)
        {
            if (DebugMode)
                Debug.Log(this.name + " will not follow any target, possible target array is empty. Mode TARGET disabled");
            m_ModeEnabled[(int)EnemyState.TARGET] = false;
        }

        if (m_Randomize && m_PatrolPoints.Length < 3)
        {
            if (DebugMode)
                Debug.Log("Not enough points to randomize path");
        }

        if (m_Randomize)
        {
            m_validChoices = new int[m_PatrolPoints.Length - 1];
            for (int i = 0; i < m_PatrolPoints.Length - 1; ++i)
            {
                m_validChoices[i] = i + 1;
            }
        }
    }

    private void Start()
    {
    
        m_ModeEnabled[(int)EnemyState.PATROL] = true;

        State = EnemyState.PATROL;
        m_DetectionTransform = new GameObject().transform;// empty gameobject
        m_DetectionTransform.SetParent(this.transform);

    }

    private void Update()
    {
        switch (State)
        {
            case EnemyState.PATROL:
                PatrolMode();
                break;
            case EnemyState.TARGET:
                TargetMode();
                break;
            case EnemyState.GIVEUP:
                GiveUpMode();
                break;
            default:
                if (DebugMode)
                    Debug.Log("Unknown state in " + this.name);
                break;
        }

    }

    private void FixedUpdate()
    {
        if (!m_ForceApplied)
            StartCoroutine(ApplyForce());
    }

    private IEnumerator ApplyForce()
    {
        m_ForceApplied = true;

        if (m_Target != null && m_Target.targettransform != null)
        {
            Vector3 vec = m_Target.targettransform.position - transform.position;
            vec.Normalize();
            GetComponent<Rigidbody>().velocity = vec * m_Speed;

            Vector3 difference = m_Target.targettransform.position - transform.position;
            difference.Normalize();
            float rotY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;

            this.transform.rotation = Quaternion.Euler(transform.rotation.x, rotY, transform.rotation.y);
        }

        yield return new WaitForSeconds(m_ApplyForceDelay);

        m_ForceApplied = false;
    }

    void PatrolMode()
    {
        m_Detection.transform.localScale = new Vector3(1, 1, 1);

        if (m_Target.targettransform != m_PatrolPoints[m_PatrolPointIndex])
        {
            m_Target.targettransform = m_PatrolPoints[m_PatrolPointIndex];
            m_Target.targetpriority = 0;

        }
        if (Vector3.Distance(m_PatrolPoints[m_PatrolPointIndex].position, this.transform.position) < m_PointReachDist)
        {
            if (m_Randomize)
            {
                m_LastPatrolIndex = m_PatrolPointIndex;
                m_PatrolPointIndex = GetRandomTagetIndex();
                m_validChoices[GetValueIndex(m_PatrolPointIndex)] = m_LastPatrolIndex;

            }
            else
            {
                if (m_PatrolPointIndex >= m_PatrolPoints.Length - 1)
                {
                    m_PatrolPointIndex = 0;
                    m_LastPatrolIndex = m_PatrolPoints.Length - 1;
                }
                else
                {

                    m_LastPatrolIndex = m_PatrolPointIndex;
                    ++m_PatrolPointIndex;
                }
            }

        }

    }

    void TargetMode()
    {
        if (m_Target.targettransform != null)
        {
            if (Vector3.Distance(m_Target.targettransform.position, m_DetectionTransform.position) >= m_GiveUpDistance)
            {
                SetTargetState(m_DetectionTransform, 0, EnemyState.GIVEUP);
            }
        }
    }


    void GiveUpMode()
    {

        m_ModeEnabled[(int)EnemyState.TARGET] = false;

        if (Vector3.Distance(transform.position, m_DetectionTransform.position) <= m_PointReachDist)
        {
            m_ModeEnabled[(int)EnemyState.TARGET] = true;
            SetTargetState(transform.parent.transform, 0, EnemyState.PATROL);
        }

    }

    private void FlipScale()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void FlipRotate()
    {

        transform.Rotate(new Vector3(0, 180, 0));
        m_FacingRight = !m_FacingRight;
    }

    public void SetTargetState(Transform targettransform, int priority, EnemyState state)
    {
        if (State != state)
        {
            m_Target.targettransform = targettransform.transform;

            m_Target.targetpriority = priority;

            State = state;
        }
    }

    private int GetRandomTagetIndex()
    {
        return m_validChoices[(Random.Range(0, m_PatrolPoints.Length - 1))];
    }

    private int GetValueIndex(int value)
    {
        for (int i = 0; i < m_validChoices.Length; i++)
        {
            if (m_validChoices[i] == value)
                return i;

        }
        return 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            OxygenMgt playerox = collision.gameObject.GetComponent<OxygenMgt>(); 
            if (playerox != null) { playerox.DecreaseOxygen(m_Damage); }
            Destroy(gameObject);
        }

    }

    private IEnumerator DecreaseOx(OxygenMgt oxmgt, int value)
    {
        m_DamageTriggered = true;
        oxmgt.DecreaseOxygen(m_Damage);
        yield return new WaitForSeconds(m_DamageDelay);
        m_DamageTriggered = false;
    }
}
