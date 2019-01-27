using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuSelection : MonoBehaviour
{
    [SerializeField] private GameObject m_Cursor;
    [SerializeField] private GameObject m_Start;
    [SerializeField] private GameObject m_Quit;
    [SerializeField] private float m_Offs;
    [SerializeField] private MenuMgt m_MenuMgt;
    private GameObject m_Selected;
    // Start is called before the first frame update
    void Start()
    {
        m_Selected = m_Start;
        m_Cursor.transform.position = m_Selected.transform.position - new Vector3(m_Offs, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (m_Selected == m_Start)
            {
                m_Selected = m_Quit;


            }
            else
            {
                m_Selected = m_Start;

            }
            m_Cursor.transform.position = m_Selected.transform.position - new Vector3(m_Offs, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (m_Selected == m_Start)
            {
                m_MenuMgt.StartGame();
            }
            else
            {
                m_MenuMgt.Quit();
            }
        }
    }
}
