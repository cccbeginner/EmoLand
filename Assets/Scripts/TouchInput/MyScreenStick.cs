using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MyScreenStick : OnScreenControl
{
    public float StickRaduis = 50f;

    [SerializeField]
    private GameObject JoyStickFront;
    [SerializeField]
    private GameObject JoyStickBack;

    private Vector2 m_StartPos;
    private Vector2 m_CurrentPos;
    private Vector2 m_StickFrontInitPos;
    private Vector2 m_StickBackInitPos;
    private RectTransform m_StickFrontTransform;
    private RectTransform m_StickBackTransform;

    private StickState m_State;

    private enum StickState
    {
        Start, Moving, End, Stay
    }
    private void OnStickTouch(Touch[] touches)
    {
        if (touches.Length == 0)
        {
            if (m_State == StickState.Start || m_State == StickState.Moving)
            {
                m_State = StickState.End;
            }
            else
            {
                m_State = StickState.Stay;
            }
        }
        else
        {
            if (m_State == StickState.End || m_State == StickState.Stay)
            {
                m_State = StickState.Start;
            }
            else
            {
                m_State = StickState.Moving;
            }
        }

        switch (m_State)
        {
            case StickState.Start:
                m_StartPos = touches[0].screenPosition;
                StartControl();
                break;
            case StickState.Moving:
                m_CurrentPos = touches[0].screenPosition;
                UpdateControl();
                break;
            case StickState.End:
                EndControl();
                break;
        }
    }

    void Start()
    {
        var touchManager = GetComponent<TouchManager>();
        var action = new UnityAction<Touch[]>(OnStickTouch);
        touchManager.OnTouchScreenLeft.AddListener(action);

        m_StickFrontTransform = JoyStickFront.GetComponent<RectTransform>();
        m_StickFrontInitPos = m_StickFrontTransform.anchoredPosition;
        m_StickBackTransform = JoyStickBack.GetComponent<RectTransform>();
        m_StickBackInitPos = m_StickBackTransform.anchoredPosition;
        SendValueToControl(Vector2.zero);

        m_State = StickState.Stay;
    }

    void StartControl()
    {
        m_StickFrontTransform.anchoredPosition = m_StartPos;
        m_StickBackTransform.anchoredPosition = m_StartPos;
    }
    void UpdateControl()
    {
        Vector2 resultVec = (m_CurrentPos - m_StartPos) / StickRaduis;
        if (resultVec.sqrMagnitude > 1)
        {
            resultVec = resultVec.normalized;
        }
        SendValueToControl(resultVec);
        m_StickFrontTransform.anchoredPosition = m_StartPos + resultVec * StickRaduis;
    }
    void EndControl()
    {
        m_StickFrontTransform.anchoredPosition = m_StickFrontInitPos;
        m_StickBackTransform.anchoredPosition = m_StickBackInitPos;
        SendValueToControl(Vector2.zero);
    }

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}