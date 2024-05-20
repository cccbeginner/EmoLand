using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButtonHere : MonoBehaviour
{
    [SerializeField]
    Canvas m_UICanvas;
    [SerializeField]
    GameObject m_WorldButtonPrefab;
    [SerializeField]
    Sprite m_Icon;
    [SerializeField]
    Color m_IconColor = Color.white;
    [SerializeField]
    float m_AppearDistance = 5f;
    public UnityEvent OnButtonClick;

    GameObject m_ExistedWorldButton = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.main)
        {
            Vector3 dist = transform.position - Player.main.transform.position;
            if (dist.sqrMagnitude <= m_AppearDistance)
            {
                if (m_ExistedWorldButton == null)
                {
                    InitWorldButton();
                    //Debug.Log(gameObject.name);
                }
                else
                {
                    UpdateWorldButton();
                }
            }
            else
            {
                if (m_ExistedWorldButton != null)
                {
                    DiscardWorldButton();
                }
            }
        }
    }

    void UpdateWorldButtonUIPosition()
    {
        RectTransform rectTransform = m_ExistedWorldButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(transform.position) / m_UICanvas.scaleFactor;
    }

    void InitWorldButton()
    {
        m_ExistedWorldButton = Instantiate(m_WorldButtonPrefab, m_UICanvas.transform);
        WorldButton btnComponent = m_ExistedWorldButton.GetComponent<WorldButton>();
        btnComponent.SetIconSprite(m_Icon);
        btnComponent.SetColor(m_IconColor);
        btnComponent.button.onClick.AddListener(OnButtonClick.Invoke);
        UpdateWorldButtonUIPosition();
    }

    void UpdateWorldButton()
    {
        UpdateWorldButtonUIPosition();
    }

    void DiscardWorldButton()
    {
        WorldButton btnComponent = m_ExistedWorldButton.GetComponent<WorldButton>();
        btnComponent.Discard();
        m_ExistedWorldButton = null;
    }
}
