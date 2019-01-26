using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public GameObject m_segmentPrefab;
    public Camera m_cam;
    public Player m_player;

    private List<GameObject> m_world = new List<GameObject>();
    private GameObject m_currentSegment;
    private float m_segmentSize = 100.0f;

    private void Start()
    {
        GenerateFirstSegment();
    }

    private void FixedUpdate()
    {
        m_player.ApplyForce(Vector3.up * 9.0f * Time.fixedDeltaTime);

        var dx = m_currentSegment.transform.position.x + m_segmentSize * 0.5f - m_player.transform.position.x;
        if(dx < m_segmentSize * 0.4f)
        {
            if(m_world[m_world.Count-1] == m_currentSegment)
                GenerateNextSegment();
        }
    }

    void GenerateFirstSegment()
    {
        GameObject b = Instantiate(m_segmentPrefab);

        // TODO 

        m_world.Add(b);
        m_currentSegment = b;
    }

    void GenerateNextSegment()
    {
        
        GameObject b = Instantiate(m_segmentPrefab, Vector3.zero, Quaternion.identity);
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
