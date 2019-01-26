﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public GameObject [] m_segmentPrefab;
    public GameObject m_fishPrefab;
    public Camera m_cam;
    public Player m_player;
    public float m_camDistance = 10.0f;
    public float m_segmentSize = 50.0f;
    public float m_minFishSpawnHeight = -4.0f;
    public float m_maxFishSpawnHeight = 17.0f;
    public float m_minFishCount = 0;
    public float m_maxFishCount = 2;

    private List<GameObject> m_world = new List<GameObject>();
    private GameObject m_currentSegment;
    

    private void Start()
    {
        GenerateFirstSegment();
    }

    private void FixedUpdate()
    {
        m_player.ApplyForce(Vector3.up * 9.8f * Time.fixedDeltaTime);

        var dx = m_currentSegment.transform.position.x + m_segmentSize * 0.5f - m_player.transform.position.x;
        if(dx < m_segmentSize * 0.4f)
        {
            if(m_world[m_world.Count-1] == m_currentSegment)
                GenerateNextSegment();
        }
    }

    private void Update()
    {
        m_cam.transform.position = m_player.transform.position - Vector3.forward * m_camDistance;
    }

    void GenerateFirstSegment()
    {
        if (m_segmentPrefab.Length == 0)
            return;

        GameObject b = Instantiate(m_segmentPrefab[0]);

        // TODO 

        m_world.Add(b);
        m_currentSegment = b;
    }

    void GenerateNextSegment()
    {        
        GameObject b = Instantiate(m_segmentPrefab[(int)Random.Range(1, m_segmentPrefab.Length)], Vector3.zero, Quaternion.identity);
        var pos = b.transform.position;
        pos.x += m_currentSegment.transform.position.x + m_segmentSize;
        b.transform.position = pos;
        GenerateTerrain(b);
        m_world.Add(b);
        m_currentSegment = b;


    }

    void GenerateTerrain(GameObject segment)
    {

    }
}
