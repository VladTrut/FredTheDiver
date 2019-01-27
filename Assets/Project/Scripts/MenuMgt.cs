using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMgt : MonoBehaviour
{

    [SerializeField] private Project m_Project;
    [SerializeField] private GameObject m_UIOverlay;
    [SerializeField] private GameObject m_MenuCanvas;
    [SerializeField] private GameObject m_EndCanvas;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private GameObject m_Boat;
    private bool m_gameover;

    private void OnEnable()
    {
        OxygenMgt.OnOxygenExhausted += GameOver;

    }

    private void OnDisable()
    {
        OxygenMgt.OnOxygenExhausted -= GameOver;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_UIOverlay.SetActive(false);
        m_MenuCanvas.SetActive(true);
        m_Player.SetActive(false);
        m_Boat.SetActive(false);
        m_Project.GameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateGame(false);
            m_EndCanvas.SetActive(false);
        }

        if (m_gameover && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)))
        {
            m_gameover = false;
            ActivateGame(false);
            m_EndCanvas.SetActive(false);
        }

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        m_Project.Init();
        ActivateGame(true);



    }

    public void Credits()
    {

    }

    void ActivateGame(bool state)
    {
        m_UIOverlay.SetActive(state);

        m_MenuCanvas.SetActive(!state);
        m_Player.SetActive(state);
        m_Boat.SetActive(state);
    }



    void GameOver()
    {
  
        m_gameover = true;
        m_Project.GameOver();
        ActivateGame(false);
        m_MenuCanvas.SetActive(false);
        m_EndCanvas.SetActive(true);

    }


}
