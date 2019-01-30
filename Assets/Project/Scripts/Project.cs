using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    public GameObject [] m_segmentPrefab;
    public Item[] m_itemPrefab;
    public float m_itemSpawnProbability = 0.8f;

    public Camera m_cam;
    public Player m_player;
    public GameObject m_boat;
    public float m_camDistance = 10.0f;
    public float m_segmentSize = 50.0f;
    public GameObject m_spotLight;

    private List<GameObject> m_world = new List<GameObject>();
    private GameObject m_currentSegment;
    private Vector3 m_playerPos;
    private Quaternion m_playerRot;
    private Vector3 m_boatPos;
    private Quaternion m_boatRotation;
    private List<GameObject> m_items = new List<GameObject>();

    private void Awake()
    {
        m_playerPos = m_player.transform.position;
        m_playerRot = m_player.transform.rotation;
        m_boatPos = m_boat.transform.position;
        m_boatRotation = m_boat.transform.rotation;
        enabled = false;
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        if (m_currentSegment == null)
            return;

        if(m_player.transform.position.y < -1.0f)
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
        Vector3 camPos = m_player.transform.position - Vector3.forward * m_camDistance;
        camPos.z = Mathf.Max(camPos.z, -m_segmentSize - 2.0f);
        m_cam.transform.position = camPos;

        var pos = m_player.transform.position;
        pos.y = 30;
        m_spotLight.transform.position = pos;
    }

    public void GenerateFirstSegment()
    {
        if (m_segmentPrefab.Length == 0)
            return;

        GameObject b = Instantiate(m_segmentPrefab[0]);

        GenerateItems(b);

        m_world.Add(b);
        m_currentSegment = b;
    }

    void GenerateNextSegment()
    {
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

        float maxP = 0.0f;
        foreach (var i in m_itemPrefab)
            maxP += i.SpawnProbability;

        var points = level.GetComponentsInChildren<ItemSpawnPoint>();
        if(points != null)
        {
            for(int i=0; i<points.Length; i++)
            {
                float p = Random.value;
                if (p >= m_itemSpawnProbability)
                    continue;
                
                points[i].transform.localRotation = Quaternion.identity;
                points[i].transform.rotation = Quaternion.identity;
                
                m_items.Add(SpawnItem(points[i].transform, p * maxP, maxP));
            }
        }
    }

    GameObject SpawnItem(Transform parent, float spawnP, float maxP)
    {
        float p = maxP;
        foreach(var item in m_itemPrefab)
        {
            p -= item.SpawnProbability;
            if(spawnP >= p)
                return Instantiate(item.gameObject, parent);
        }

        return null;
    }

    public void Init()
    {
        m_player.transform.position = m_playerPos;
        m_player.transform.rotation = m_playerRot;
        m_boat.transform.position = m_boatPos;
        m_boat.transform.rotation = m_boatRotation;
        m_player.Reset();
        GenerateFirstSegment();
        enabled = true;
        AudioManager.instance.PlaySound("PlayerHeartBeat");
    }

    public void GameOver()
    {
        enabled = false;
        m_player.transform.position = m_playerPos;
        m_player.transform.rotation = m_playerRot;
        m_boat.transform.position = m_boatPos;
        m_boat.transform.rotation = m_boatRotation;
        foreach (var l in m_world)
            DestroyImmediate(l);
        m_world.Clear();
        m_currentSegment = null;
        foreach (var i in m_items)
            DestroyImmediate(i);
        m_items.Clear();

        AudioManager.instance.StopSound("EnvironmentUnderWaterNoise");
        AudioManager.instance.StopSound("PlayerBreathUnderWater");
        AudioManager.instance.StopSound("PlayerHeartBeat");
    }
}
