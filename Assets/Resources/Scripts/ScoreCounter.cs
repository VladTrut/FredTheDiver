using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{

    [SerializeField] private Text m_CoinText;
    public int m_Score;

    // Use this for initialization
    void Start()
    {
        m_Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_CoinText.color = Color.white;
        m_CoinText.text = "Score : " + m_Score.ToString();
    }

    public void IncreaseScore(int value)
    {
        m_Score += value;
    }

}
