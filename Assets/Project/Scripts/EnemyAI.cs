﻿using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    private Animator m_Anim;
    public bool DebugMode;

    /* ---- DRAGON BEHAVIOR ---- */

    public enum EnemyState { PATROL, TARGET, SLEEP, SURPRISED, GIVEUP };
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
        public string targetname;
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
    [SerializeField] private bool m_CanMoveInTargetMode = true;
    [SerializeField] private float m_TargetSpeed;

    // Detection
    [SerializeField] private GameObject m_Detection;
    private Transform m_DetectionTransform;
    [HideInInspector] public bool m_IsPlayerSwimming;
    public bool m_UndetectIfPlayerSwimIs;
    [Range(0f, 1f)] [SerializeField] private float m_SleepScaleTrigger = 1f;

    // orientation
    private bool m_FacingRight = true;

    /* ----------------------------- */

    /* ---- MODE PATROL ---- */

    public Transform[] m_PatrolPoints;
    [SerializeField] private int m_LastPatrolIndex = 0;
    [SerializeField] private int m_PatrolPointIndex = 0;
    public bool m_Randomize;
    private int[] m_validChoices;
    [SerializeField] private float m_PatrolSpeed;
    [SerializeField] private float m_PointReachDist;
    /* ----------------------------- */

    /* ---- MODE SLEEP ---- */

    private Transform m_StartPosition;
    public bool m_SleepRight;
    [SerializeField] private float m_MaxSpeed = 1f;
    [SerializeField] private float m_MinSpeed = 1f;


    /* ----------------------------- */

    /* ---- MODE SURPRISED ---- */

    [SerializeField] private float m_SurprisedDelay = 3f;
    private bool m_IsSurprisedDelayInit;
    private float m_InitSurprisedTime;

    /* ----------------------------- */
    private bool m_ForceApplied = false;
    [SerializeField] private float m_ApplyForceDelay = 1f;



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
                    //m_AiLerpScript.canMove = true;
                    //m_AiLerpScript.speed = m_PatrolSpeed;
                    m_Anim.SetBool("Attack", false);
                    break;

                case EnemyState.TARGET:
                    //m_AiLerpScript.canMove = m_CanMoveInTargetMode;
                    //m_AiLerpScript.speed = m_TargetSpeed;

                    m_DetectionTransform.position = transform.position;
                    m_Anim.SetBool("Attack", true);
                    break;

                case EnemyState.SLEEP:
                    //if (m_LastState != EnemyState.SLEEP)
                        //m_AiLerpScript.canMove = true;
        
                    //m_Anim.SetBool("Sleep", true);
                    break;

                case EnemyState.SURPRISED:
                    //m_AiLerpScript.canMove = false;
             
                    break;

                case EnemyState.GIVEUP:
                    //m_AiLerpScript.canMove = true;
                    m_Anim.SetBool("Attack", false);
                    break;

                default:
                    if (DebugMode)
                        Debug.Log("Unknown state in " + this.name);
                    break;
            }



            m_LastState = m_State;
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

       // if (m_AiLerpScript == null)
        //    m_AiLerpScript = GetComponent<AILerp>();
      //  if (m_AISetter == null)
           // m_AISetter = GetComponent<AIDestinationSetter>();

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

        if (m_PatrolPoints.Length <= 1)
        {
            m_ModeEnabled[(int)EnemyState.PATROL] = true;
            m_ModeEnabled[(int)EnemyState.SLEEP] = false;
            m_LastState = EnemyState.SLEEP;

            if (m_PatrolPoints.Length < 1)
                m_StartPosition = transform.parent;
            else
                m_StartPosition = m_PatrolPoints[0];

        }
        else if (m_PatrolPoints.Length >= 2)
        {
            m_ModeEnabled[(int)EnemyState.PATROL] = true;
            m_ModeEnabled[(int)EnemyState.SLEEP] = false;
            m_LastState = EnemyState.PATROL;
            m_StartPosition = m_PatrolPoints[0];
        }

        State = (m_ModeEnabled[(int)EnemyState.PATROL] == true) ? EnemyState.PATROL : EnemyState.SLEEP;
        m_DetectionTransform = new GameObject().transform; // empty gameobject

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
            case EnemyState.SLEEP:
                SleepMode();
                break;
            case EnemyState.SURPRISED:
                //SurprisedMode();
                break;
            case EnemyState.GIVEUP:
                GiveUpMode();
                break;
            default:
                if (DebugMode)
                    Debug.Log("Unknown state in " + this.name);
                break;
        }

        /*if (m_Target.targettransform != null && State != EnemyState.SLEEP && State != EnemyState.SURPRISED)
        {
            if (!m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x > 0)
            {
                FlipRotate();
            }
            else if (m_FacingRight && m_Target.targettransform.position.x - this.transform.position.x < 0)
            {
                FlipRotate();
            }
        }*/

    }

    private void FixedUpdate()
    {
        if (!m_ForceApplied)
            StartCoroutine(ApplyForce());
        //Debug.Log(GetComponent<Rigidbody>().velocity);
        /*Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.velocity.x > m_MaxSpeed)
            rb.velocity = new Vector3(m_MaxSpeed, rb.velocity.y, rb.velocity.z);
        if (rb.velocity.y > m_MaxSpeed)
            rb.velocity = new Vector3(rb.velocity.x, m_MaxSpeed, rb.velocity.z);
        if (rb.velocity.z > m_MaxSpeed)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, m_MaxSpeed);
        if (rb.velocity.x < -m_MinSpeed)
            rb.velocity = new Vector3(-m_MinSpeed, rb.velocity.y, rb.velocity.z);
        if (rb.velocity.y < -m_MinSpeed)
            rb.velocity = new Vector3(rb.velocity.x, -m_MinSpeed, rb.velocity.z);
        if (rb.velocity.z < -m_MinSpeed)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -m_MinSpeed);*/



    }

    private IEnumerator ApplyForce()
    {
        m_ForceApplied = true;
        if (m_Target != null && m_Target.targettransform != null)
        {
            Vector3 vec = m_Target.targettransform.position - transform.position;
            vec.Normalize();
            //GetComponent<Rigidbody>().AddForce(vec * Time.fixedDeltaTime * m_PatrolSpeed, ForceMode.Force);
            GetComponent<Rigidbody>().velocity = vec * m_PatrolSpeed;

            Vector3 difference = m_Target.targettransform.position - transform.position;
            difference.Normalize();
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            float rotX = Mathf.Atan2(difference.y, difference.z) * Mathf.Rad2Deg;
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
            //m_AISetter.target = m_PatrolPoints[m_PatrolPointIndex];
            m_Target.targettransform = m_PatrolPoints[m_PatrolPointIndex];
            m_Target.targetpriority = 0;

        }
        //Debug.Log(Vector3.Distance(m_PatrolPoints[m_PatrolPointIndex].position, this.transform.position));
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
                Debug.Log("length : " + m_PatrolPoints.Length);
                if (m_PatrolPointIndex >= m_PatrolPoints.Length - 1)
                {
                    m_PatrolPointIndex = 0;
                    m_LastPatrolIndex = m_PatrolPoints.Length - 1;
                }
                else
                {

                    m_LastPatrolIndex = m_PatrolPointIndex;
                    ++m_PatrolPointIndex;
                    Debug.Log("m_PatrolPointIndex : " + m_PatrolPointIndex);
                }
            }

        }

    }

    void TargetMode()
    {
    
        m_IsSurprisedDelayInit = false;




        if (m_Target.targettransform != null)
        {
            //Debug.Log(Vector3.Distance(m_Target.targettransform.position, m_DetectionTransform.position));
            if (Vector3.Distance(m_Target.targettransform.position, m_DetectionTransform.position) >= m_GiveUpDistance)
            {
                SetTargetState(m_DetectionTransform, 0, EnemyState.GIVEUP);
            }
        }





    }

    void SleepMode()
    {

        m_Detection.transform.localScale = new Vector3(m_SleepScaleTrigger, m_SleepScaleTrigger, m_SleepScaleTrigger);
      

        //m_AISetter.target = m_StartPosition;
        m_Target.targettransform = m_StartPosition;

        if (m_SleepRight != m_FacingRight)
            FlipRotate();

    }

    void SurprisedMode()
    {
  

        if (!m_IsSurprisedDelayInit)
        {
            m_InitSurprisedTime = Time.time;
            m_IsSurprisedDelayInit = true;
        }


        if (Time.time - m_InitSurprisedTime > m_SurprisedDelay)
        {
            State = m_LastState;
            m_IsSurprisedDelayInit = false;
        }

    }

    void GiveUpMode()
    {

        m_ModeEnabled[(int)EnemyState.TARGET] = false;

        if (Vector3.Distance(transform.position, m_DetectionTransform.position) <= m_PointReachDist)
        {
            m_ModeEnabled[(int)EnemyState.TARGET] = true;
            if (m_ModeEnabled[(int)EnemyState.SLEEP] == true)
                SetTargetState(transform.parent.transform, 0, EnemyState.SLEEP);
            else
                SetTargetState(transform.parent.transform, 0, EnemyState.PATROL);
        }

    }

    private void FlipScale()
    {
        // unparent expressionmanager then reparent it after scaling
        //m_ExpressionManager.transform.parent = null;

        m_FacingRight = !m_FacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        //m_ExpressionManager.transform.parent = transform;
    }

    private void FlipRotate()
    {

        transform.Rotate(new Vector3(0, 180, 0));

        //   transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
        //transform.Rotate Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        /*Vector3 scale = m_ExpressionManager.transform.localScale;
        scale.x *= -1;
        m_ExpressionManager.transform.localScale = scale;*/

    }

    public void SetTargetState(Transform targettransform, int priority, EnemyState state)
    {
        if (State != state)
        {
           //m_AISetter.target = targettransform.transform;
            m_Target.targettransform = targettransform.transform;

            m_Target.targetpriority = priority;

            State = state;
        }
    }


    /* public void SetTarget(GameObject targetobj, int priority)
     {
         if (State != EnemyState.TARGET)
         {
             m_AISetter.target = targetobj.transform;
             m_Target.targettransform = targetobj.transform;

             m_Target.targetdata.priority = priority;

             State = EnemyState.TARGET;
         }

     }

     public void SetPreTarget(GameObject targetobj, int priority)
     {
         m_AISetter.target = targetobj.transform;
         m_Target.targettransform = targetobj.transform;

         m_Target.targetdata.priority = priority;

         State = EnemyState.SURPRISED;
     }

     private void UnsetTarget(GameObject targetobj, float detectiondistance, float giveupdistance, int priority)
     {
         m_Target.targettransform = null;
         State = EnemyState.TARGET;
     }*/



    /* private void DetectTarget(string targetname, string targettag, float detectiondistance, float giveupdistance, int priority)
     {
         GameObject[] potentialtargets = GameObject.FindGameObjectsWithTag(targettag);
         GameObject target = null;

         if (potentialtargets.Length >= 1)
         {
             for (int i = 0; i < potentialtargets.Length; i++)
             {
                 if (potentialtargets[i].name == targetname)
                 {
                     if (target != null)
                         Debug.Log("More than one target found with name : " + targetname + " in tags : " + targettag);
                     target = potentialtargets[i];
                 }

             }
             if (target == null)
             {
                 Debug.Log("No target found with name : " + targetname + " in tags : " + targettag);
                 return;
             }
         }
         else
         {
             Debug.Log("No target found with tag : " + targettag);
             return;
         }

         if (Vector3.Distance(transform.position, target.transform.position) <= detectiondistance )
         {
             RaycastHit2D hit = Physics2D.Raycast(this.transform.position, target.transform.position - this.transform.position, detectiondistance, m_WhatIsTarget);
             //if (hit)
               //  Debug.Log("hit : " + hit.transform.gameObject.name + " " + hit.transform.gameObject.tag);
             //Debug.DrawLine(this.transform.position, target.transform.position - this.transform.position, Color.red);

             if (m_Target.targettransform != null)
             {
                 if (priority >= m_Target.targetdata.priority && hit.collider != null && hit.collider.gameObject.name == targetname)
                     SetTarget(target, detectiondistance, giveupdistance, priority);
             }
             else
                 SetTarget(target, detectiondistance, giveupdistance, priority);
         }
     }*/

    /*private IEnumerator ShutOffDetection()
    {
        m_ShutOffDetect = true;
        yield return new WaitForSeconds(3f);
        m_ShutOffDetect = false;
    }*/

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

    private void OnPlayerSwim(bool state)
    {
        m_IsPlayerSwimming = state;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State == EnemyState.SLEEP && collision.gameObject.tag == "Player")
            FlipRotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        OxygenMgt playerox = collision.gameObject.GetComponent<OxygenMgt>();
        if(playerox != null)
        {
            playerox.DecreaseOxygen(m_Damage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        OxygenMgt playerox = collision.gameObject.GetComponent<OxygenMgt>();
        if (playerox != null)
        {
            playerox.DecreaseOxygen(m_Damage);
        }
    }
}
