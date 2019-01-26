using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float m_strength = 1.0f;
    //public float m_minDepth = 4.0f;
    //public float m_maxDepth = 17.0f;
    public float m_minSpawnTime = 3.0f;
    public float m_maxSpawnTime = 7.0f;
    public float m_spawnProbability = 0.75f;
    public GameObject m_fish;

    private void Awake()
    {
        m_fish.SetActive(false);
    }

    private void Start()
    {
        if(Random.value < m_spawnProbability)
            Spawn(m_minSpawnTime + Random.value * (m_maxSpawnTime - m_minSpawnTime));
    }

    public void Spawn(float waitUntil)
    {
        StartCoroutine(spawnTimer(waitUntil));
    }

    IEnumerator spawnTimer(float waitUntil = 5.0f)
    {
        while(waitUntil > 0)
        {
            waitUntil -= Time.deltaTime;
            yield return null;
        }

        m_fish.SetActive(true);
    }

    public float Strength { get => m_strength; }

    
}
