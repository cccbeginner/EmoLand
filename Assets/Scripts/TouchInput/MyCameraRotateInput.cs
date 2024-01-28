using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MyCameraRotateInput : OnScreenControl
{
    private void OnRightScreenTouch(Touch[] touches)
    {
        if (touches.Length == 0)
        {
            NoControl();
        }
        else
        {
            UpdateControl(touches);
        }
    }

    void Start()
    {
        var touchManager = GetComponent<TouchManager>();
        var action = new UnityAction<Touch[]>(OnRightScreenTouch);
        touchManager.OnTouchScreenRight.AddListener(action);

        SendValueToControl(Vector2.zero);
    }

    void NoControl()
    {
        SendValueToControl(Vector2.zero);
    }
    void UpdateControl(Touch[] touches)
    {
        Vector2 resultDelta = Vector2.zero;
        foreach (var touch in touches)
        {
            resultDelta += touch.delta;
        }
        SendValueToControl(resultDelta);
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