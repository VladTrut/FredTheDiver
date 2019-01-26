using UnityEngine;

public class ArrowMgt : MonoBehaviour
{
    private GameObject m_Boat;

    // Start is called before the first frame update
    void Start()
    {
        m_Boat = GameObject.FindGameObjectWithTag("Boat");
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 newPos = new Vector3(m_Boat.transform.position.x, m_Boat.transform.position.y, m_Boat.transform.position.z);
        transform.position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void OnWillRenderObject()
    {

    }

    
}
