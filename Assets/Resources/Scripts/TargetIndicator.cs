using System;
using UnityEngine;
using UnityEngine.UI;
public class TargetIndicator : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform m_icon;
    private Text m_ArrowText;
    private Image m_iconImage;
    private Canvas mainCanvas;
    private Vector3 m_cameraOffsetUp;
    private Vector3 m_cameraOffsetRight;
    private Vector3 m_cameraOffsetForward;
    private Sprite m_targetIconOnScreen;
    public Sprite m_targetIconOffScreen;
    [Space]
    [Range(0, 100)]
    public float m_edgeBuffer;
    public Vector3 m_targetIconScale;
    [Space]
    public bool ShowDebugLines;
    public float offset;
    public Font m_DistFont;
    public int m_DistTextSize;
    public float m_ArrowOffs;

    void Start()
    {
        mainCamera = Camera.main;
        mainCanvas = FindObjectOfType<Canvas>();
        Debug.Assert((mainCanvas != null), "There needs to be a Canvas object in the scene for the OTI to display");
        InstainateTargetIcon();




    }

    void Update()
    {
        if (ShowDebugLines)
            DrawDebugLines();
        //UpdateTargetIconPosition();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        Vector3 newPos;
        Vector3 screencoordinate = Camera.main.WorldToScreenPoint(transform.position);

        if (transform.position.x > mainCamera.pixelWidth || transform.position.x < 0 || transform.position.y > mainCamera.pixelHeight || transform.position.y < 0)
        {

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
            m_ArrowText.transform.position = new Vector3(m_icon.transform.position.x, m_icon.transform.position.y - m_ArrowOffs, m_icon.transform.position.z);
           
            int boatdist = (int)Vector2.Distance(m_icon.position, screencoordinate);
            Debug.Log("dist : " + boatdist);
            m_ArrowText.text = boatdist.ToString() + " m";





            m_icon.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            Vector3 difference = screencoordinate - m_icon.transform.position;
            difference.Normalize();
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            m_icon.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
            m_ArrowText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            m_iconImage.sprite = m_targetIconOffScreen;



            if (screencoordinate.x < 0 && screencoordinate.y < 0)
                newPos = new Vector3(0 + offset, 0 + offset, m_icon.transform.position.z);
            else if (screencoordinate.x < 0  && screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(0 + offset, mainCamera.pixelHeight - offset, m_icon.transform.position.z);
            else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(mainCamera.pixelWidth - offset, mainCamera.pixelHeight - offset, m_icon.transform.position.z);
            else if (screencoordinate.x > mainCamera.pixelWidth && screencoordinate.y < 0)
                newPos = new Vector3(mainCamera.pixelWidth - offset, 0 + offset, m_icon.transform.position.z);
            else if (screencoordinate.x > mainCamera.pixelWidth)
                newPos = new Vector3(mainCamera.pixelWidth - offset, screencoordinate.y, m_icon.transform.position.z);
            else if (screencoordinate.x < 0)
                newPos = new Vector3(0 + offset, screencoordinate.y, m_icon.transform.position.z);
            else if (screencoordinate.y > mainCamera.pixelHeight)
                newPos = new Vector3(screencoordinate.x, mainCamera.pixelHeight - offset, m_icon.transform.position.z);
            else if (screencoordinate.y < 0)
                newPos = new Vector3(screencoordinate.x, 0 + offset, m_icon.transform.position.z);
                else
                newPos = new Vector3(screencoordinate.x, screencoordinate.y, m_icon.transform.position.z);

            m_icon.transform.position = newPos;
        }
        else
        {
            m_icon.gameObject.SetActive(false);


        }

    }

    private void InstainateTargetIcon()
    {
        m_icon = new GameObject().AddComponent<RectTransform>();
        m_icon.transform.SetParent(mainCanvas.transform);
        m_icon.localScale = m_targetIconScale;
        m_icon.name = name + ": OTI icon";
        m_iconImage = m_icon.gameObject.AddComponent<Image>();
        m_iconImage.sprite = m_targetIconOnScreen;
    }


    private void UpdateTargetIconPosition()
    {
        Vector3 newPos = transform.position;
        //newPos = mainCamera.p
        //newPos = mainCamera.WorldToViewportPoint(newPos);
        if (newPos.z < 0)
        {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;
            newPos = Vector3Maxamize(newPos);
        }
        newPos = mainCamera.ViewportToScreenPoint(newPos);
        newPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
        newPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);
        m_icon.transform.position = newPos;
    }


    public void DrawDebugLines()
    {
        Vector3 directionFromCamera = transform.position - mainCamera.transform.position;
        Vector3 cameraForwad = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;
        cameraForwad *= Vector3.Dot(cameraForwad, directionFromCamera);
        cameraRight *= Vector3.Dot(cameraRight, directionFromCamera);
        cameraUp *= Vector3.Dot(cameraUp, directionFromCamera);
        Debug.DrawRay(mainCamera.transform.position, directionFromCamera, Color.magenta);
        Vector3 forwardPlaneCenter = mainCamera.transform.position + cameraForwad;
        Debug.DrawLine(mainCamera.transform.position, forwardPlaneCenter, Color.blue);
        Debug.DrawLine(forwardPlaneCenter, forwardPlaneCenter + cameraUp, Color.green);
        Debug.DrawLine(forwardPlaneCenter, forwardPlaneCenter + cameraRight, Color.red);
    }


    public Vector3 Vector3Maxamize(Vector3 vector)
    {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }
}