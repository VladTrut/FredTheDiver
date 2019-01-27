using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetScore : MonoBehaviour
{

    [SerializeField] private ScoreCounter m_ScoreCounter;
    [SerializeField] private Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        OxygenMgt.OnOxygenExhausted += FinalScore;
    }

    private void OnDisable()
    {
        OxygenMgt.OnOxygenExhausted -= FinalScore;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void FinalScore()
    {
        m_Text.text = m_ScoreCounter.m_Score.ToString();
    }

}
