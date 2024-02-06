using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MyCameraZoomInput : OnScreenControl
{

    float prePinchDist = -1f;

    public void OnRightScreenTouch(Touch[] touches)
    {
        if (touches.Length < 2)
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
        SendValueToControl(0f);
    }

    void NoControl()
    {
        SendValueToControl(0f);
        prePinchDist = -1f;
    }
    void UpdateControl(Touch[] touches)
    {
        if (prePinchDist < 0)
        {
            prePinchDist = PinchDist(touches);
            SendValueToControl(0f);
        }
        else
        {
            float curPinchDist = PinchDist(touches);
            SendValueToControl(curPinchDist - prePinchDist);
            prePinchDist = curPinchDist;
        }
    }
    private float PinchDist(Touch[] touches)
    {

        Vector2 pos0 = touches[0].screenPosition;
        Vector2 pos1 = touches[1].screenPosition;
        return (pos0 - pos1).magnitude;
    }

    [InputControl(layout = "Axis")]
    [SerializeField]
    private string m_ControlPath;
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}