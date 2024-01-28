using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchManager : MonoBehaviour
{
    public float ScreenMiddle = 0.5f;

    public UnityEvent<Touch[]> OnTouchScreenLeft;
    public UnityEvent<Touch[]> OnTouchScreenRight;

    // Maintain currently valid touches.
    // All valid touches must on right side of the screen.
    // All valid touches must not be a tap.
    List<Touch> m_LeftTouches = new List<Touch>();
    List<Touch> m_RightTouches = new List<Touch>();

    protected void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    protected void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void RemoveTouch(Touch touch)
    {
        m_LeftTouches.RemoveAll(_touch => _touch.touchId == touch.touchId);
        m_RightTouches.RemoveAll(_touch => _touch.touchId == touch.touchId);
    }

    private void AddTouch(Touch touch)
    {
        if (touch.screenPosition.x > Screen.width * ScreenMiddle)
        {
            m_RightTouches.Add(touch);
        }
        else
        {
            m_LeftTouches.Add(touch);
        }
    }

    private void UpdateTouch(Touch touch)
    {
        int idx = m_RightTouches.FindIndex(0, m_RightTouches.Count, _touch => _touch.touchId == touch.touchId);
        if (0 <= idx && idx < m_RightTouches.Count)
        {
            m_RightTouches[idx] = touch;
        }
        idx = m_LeftTouches.FindIndex(0, m_LeftTouches.Count, _touch => _touch.touchId == touch.touchId);
        if (0 <= idx && idx < m_LeftTouches.Count)
        {
            m_LeftTouches[idx] = touch;
        }
    }

    private void Update()
    {
        // Update Current Touches that Rotates/Zooms Camera.
        for (int i = 0; i < Touch.activeTouches.Count; i++)
        {
            Touch touch = Touch.activeTouches[i];
            if (touch.began)
            {
                // Prevent from quickly switching fingers.
                RemoveTouch(touch);
                AddTouch(touch);
            }
            else if (touch.ended)
            {
                RemoveTouch(touch);
            }
            else
            {
                UpdateTouch(touch);
            }
        }
        OnTouchScreenLeft.Invoke(m_LeftTouches.ToArray());
        OnTouchScreenRight.Invoke(m_RightTouches.ToArray());
    }
}