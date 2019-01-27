using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public GameObject [] m_segmentPrefab;
    public GameObject[] m_itemPrefab;
    public float m_itemSpawnProbability = 0.8f;

    public Camera m_cam;
    public Player m_player;
    public float m_camDistance = 10.0f;
    public float m_segmentSize = 50.0f;

    private List<GameObject> m_world = new List<GameObject>();
    private GameObject m_currentSegment;
    private bool m_GameStarted = false;

    public bool GameStarted { get => m_GameStarted; set => m_GameStarted = value; }

    private void Start()
    {
        //GenerateFirstSegment();
    }

    private void FixedUpdate()
    {
        if (!m_GameStarted) return;

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
        if (!m_GameStarted) return;

        m_cam.transform.position = m_player.transform.position - Vector3.forward * m_camDistance;
    }

    public void GenerateFirstSegment()
    {
        if (!m_GameStarted) return;

        if (m_segmentPrefab.Length == 0)
            return;

        GameObject b = Instantiate(m_segmentPrefab[0]);

        GenerateItems(b);

        m_world.Add(b);
        m_currentSegment = b;
    }

    void GenerateNextSegment()
    {
        if (!m_GameStarted) return;

        GameObject b = Instantiate(m_segmentPrefab[(int)Random.Range(1, m_segmentPrefab.Length)], Vector3.zero, Quaternion.identity);
        var pos = b.transform.position;
        pos.x += m_currentSegment.transform.position.x + m_segmentSize;
        b.transform.position = pos;
        GenerateItems(b);
        m_world.Add(b);
        m_currentSegment = b;        
    }

    void GenerateItems(GameObject level)
    {
        if (m_itemPrefab.Length == 0)
            return;

        var points = level.GetComponentsInChildren<ItemSpawnPoint>();
        if(points != null)
        {
            for(int i=0; i<points.Length; i++)
            {
                if (Random.value >= m_itemSpawnProbability)
                    continue;

                int index = Random.Range(0, m_itemPrefab.Length - 1);
                Instantiate(m_itemPrefab[index], points[i].transform);
            }
        }
    }

    public void Init()
    {
        m_GameStarted = true;

        // TODO
    }

    public void GameOver()
    {
        m_GameStarted = false;

        // TODO
    }
}
