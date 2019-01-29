using System;
using UnityEngine;
using UnityEngine.UI;
public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_UIOverlay;
    private Camera mainCamera;
    private RectTransform m_icon;
    private Text m_ArrowText;
    private Image m_iconImage;
    private Canvas mainCanvas;
    [SerializeField] private Sprite m_targetIcon;
    [SerializeField] private Vector3 m_targetIconScale;
    [SerializeField] private float m_ArrowOffs;
    [SerializeField] private Font m_DistFont;
    [SerializeField] private int m_DistTextSize;
    [SerializeField] private float m_TxtOffs;

    void Start()
    {
        mainCamera = Camera.main;
        mainCanvas = m_UIOverlay.GetComponent<Canvas>();
        Debug.Assert((mainCanvas != null), "There needs to be a Canvas object in the scene for the OTI to display");
        InstantiateTargetIcon();
    }

    void Update()
    {
        UpdateTargetIconPosition();
    }

    private void UpdateTargetIconPosition()
    {
        Vector3 newPos;
        Vector3 screencoordinate = Camera.main.WorldToScreenPoint(transform.position);

        m_icon.gameObject.SetActive(true);

        if (m_ArrowText == null)
        {
            m_ArrowText = new GameObject().AddComponent<Text>();
            m_ArrowText.transform.SetParent(m_icon.transform);

            m_ArrowText.font = m_DistFont;
            m_ArrowText.fontSize = m_DistTextSize;
            m_ArrowText.alignment = TextAnchor.LowerCenter;
            m_ArrowText.color = Color.white;
            m_ArrowText.verticalOverflow = VerticalWrapMode.Overflow;
            m_ArrowText.horizontalOverflow = HorizontalWrapMode.Overflow;

        }
        m_ArrowText.transform.position = new Vector3(m_icon.transform.position.x, m_icon.transform.position.y - m_TxtOffs, m_icon.transform.position.z);

        int boatdist = (int)Vector3.Distance(transform.position, m_player.transform.position);
        m_ArrowText.text = boatdist.ToString() + " m";



        m_icon.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        Vector3 difference = screencoordinate - m_icon.transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        m_icon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        m_ArrowText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        m_iconImage.sprite = m_targetIcon;


        if (screencoordinate.x < 0 && screencoordinate.y < 0)
            newPos = new Vector3(0 + m_ArrowOffs, 0 + m_ArrowOffs, m_icon.transform.position.z);
        else if (screencoordinate.x < 0 && screencoordinate.y > mainCamera.pixelHeight)
            newPos = new Vector3(0 + m_ArrowOffs, mainCamera.pixelHeight - m_ArrowOffs, m_icon.transform.position.z);
        else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y > mainCamera.pixelHeight)
            newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, mainCamera.pixelHeight - m_ArrowOffs, m_icon.transform.position.z);
        else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y < 0)
            newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, 0 + m_ArrowOffs, m_icon.transform.position.z);
        else if (screencoordinate.x > mainCamera.pixelWidth)
            newPos = new Vector3(mainCamera.pixelWidth - m_ArrowOffs, screencoordinate.y, m_icon.transform.position.z);
        else if (screencoordinate.x < 0)
            newPos = new Vector3(0 + m_ArrowOffs, screencoordinate.y, m_icon.transform.position.z);
        else if (screencoordinate.y > mainCamera.pixelHeight)
            newPos = new Vector3(screencoordinate.x, mainCamera.pixelHeight - m_ArrowOffs, m_icon.transform.position.z);
        else if (screencoordinate.y < 0)
            newPos = new Vector3(screencoordinate.x, 0 + m_ArrowOffs, m_icon.transform.position.z);
        else
            newPos = new Vector3(screencoordinate.x, screencoordinate.y, m_icon.transform.position.z);

        m_icon.transform.position = newPos;


    }

    private void InstantiateTargetIcon()
    {
        m_icon = new GameObject().AddComponent<RectTransform>();
        m_icon.transform.SetParent(mainCanvas.transform);
        m_icon.localScale = m_targetIconScale;
        m_icon.name = name + ": OTI icon";
        m_iconImage = m_icon.gameObject.AddComponent<Image>();
        m_iconImage.sprite = m_targetIcon;
    }


}