using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.UI;

public class MyScreenStick : OnScreenControl
{
    public float StickRaduis = 50f;

    [SerializeField]
    private GameObject JoyStickFront;
    [SerializeField]
    private GameObject JoyStickBack;
    [SerializeField]
    private Canvas canvas;
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
    public void OnStickTouch(Touch[] touches)
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
        m_StickFrontTransform = JoyStickFront.GetComponent<RectTransform>();
        m_StickFrontInitPos = m_StickFrontTransform.anchoredPosition;
        m_StickBackTransform = JoyStickBack.GetComponent<RectTransform>();
        m_StickBackInitPos = m_StickBackTransform.anchoredPosition;
        JoyStickBack.SetActive(false);
        SendValueToControl(Vector2.zero);

        m_State = StickState.Stay;
    }

    void StartControl()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            m_StartPos,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        m_StickFrontTransform.position = canvas.transform.TransformPoint(localPoint);
        m_StickBackTransform.position = canvas.transform.TransformPoint(localPoint);
        JoyStickBack.SetActive(true);
    }
    void UpdateControl()
    {
        float RealStickRadius = StickRaduis * canvas.scaleFactor;

        // Update control value.
        Vector2 resultVec = (m_CurrentPos - m_StartPos) / RealStickRadius;
        if (resultVec.sqrMagnitude > 1)
        {
            resultVec = resultVec.normalized;
        }
        SendValueToControl(resultVec);

        // Update front stick position.
        Vector2 targetPos = m_StartPos + resultVec * RealStickRadius;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            targetPos,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        m_StickFrontTransform.position = canvas.transform.TransformPoint(localPoint); // scaler;
    }
    void EndControl()
    {
        m_StickFrontTransform.anchoredPosition = m_StickFrontInitPos;
        m_StickBackTransform.anchoredPosition = m_StickBackInitPos;
        SendValueToControl(Vector2.zero);

        JoyStickBack.SetActive(false);
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