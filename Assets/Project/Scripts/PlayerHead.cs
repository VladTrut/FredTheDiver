using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public Player m_player;
    private AudioManager m_audioManager;

    void Start()
    {
        m_audioManager = FindObjectOfType<AudioManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Rock":
                m_audioManager.PlaySound("PlayerHeadHitting");
                break;
            case "Fish":
                m_audioManager.PlaySound("PlayerPain");
                break;
            default:
                break;
        }
    }
}
