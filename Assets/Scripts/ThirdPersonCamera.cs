using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float RotateSpeed = 20f;
    public float ZoomSpeed = 10f;
    public float LerpSpeed = 10f;
    public float AutoRotateSpeed = 10f;
    public float FarthestDistance = 30f;
    public float NearestDistance = 3f;
    public float ScreenMiddle = 0.5f;
    public float DefaultTapTime = 0.2f;

    private Transform _cameraPivot;

    private float verticalRotation;
    private float horizontalRotation;

    private Coroutine rotateCoroutine, zoomCoroutine, touchDetectCoroutine;

    // Maintain currently valid touches.
    // All valid touches must on right side of the screen.
    // All valid touches must not be a tap.
    private List<int> _validTouchID;
    private Touch[] _touchData;

    private void Awake()
    {
        _cameraPivot = gameObject.transform.parent;
        _validTouchID = new List<int>();
        _touchData = new Touch[10];
    }

    protected void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        StartCoroutine(TouchDetection());
    }

    protected void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Follow Player
        Vector3 newPivotPosition = Vector3.Lerp(_cameraPivot.position, Target.position, LerpSpeed * Time.deltaTime);
        Vector3 curVec = _cameraPivot.forward;
        Vector3 nextVec = curVec + (newPivotPosition - _cameraPivot.position);
        float autoRotateAngle = Vector3.SignedAngle(curVec, nextVec, Vector3.up) * Time.deltaTime * AutoRotateSpeed;
        _cameraPivot.position = newPivotPosition;
        Rotate(new Vector2(autoRotateAngle, 0));
    }

    IEnumerator TouchDetection()
    {
        bool[] isRight = new bool[10];
        int preValidCount = 0;
        for (int i = 0; i < 10; ++i)
        {
            isRight[i] = false;
        }
        while (true)
        {
            // Update Current Touches that Rotates/Zooms Camera.
            for (int i = 0; i < Touch.activeTouches.Count; i++)
            {
                Touch touch = Touch.activeTouches[i];
                if (touch.began)
                {
                    // Prevent from quickly switching fingers.
                    if (_validTouchID.Contains(touch.touchId))
                    {
                        _validTouchID.Remove(touch.touchId);
                    }
                    // Check if on the right side of screen.
                    if (touch.screenPosition.x > Screen.width * ScreenMiddle)
                    {
                        isRight[touch.touchId] = true;
                    }
                }
                // Make it valid if it's not a press AND on the right side of screen.
                if (touch.time > DefaultTapTime && isRight[touch.touchId] && !_validTouchID.Contains(touch.touchId))
                {
                    _validTouchID.Add(touch.touchId);
                }
                if (_validTouchID.Contains(touch.touchId))
                {
                    if (touch.ended)
                    {
                        // Remove valid touch if ended.
                        _validTouchID.Remove(touch.touchId);
                        isRight[touch.touchId] = false;
                    }
                    else
                    {
                        // Update touch data if still in progress.
                        _touchData[touch.touchId] = touch;
                    }
                }
            }

            // Update Rotate/Zoom Coroutine Status
            if (preValidCount < 1 && _validTouchID.Count >= 1)
            {
                StartRotate();
            }
            else if (preValidCount >= 1 && _validTouchID.Count < 1)
            {
                StopRotate();
            }
            if (preValidCount < 2 && _validTouchID.Count >= 2)
            {
                StartZoom();
            }
            else if (preValidCount >= 2 && _validTouchID.Count < 2)
            {
                StopZoom();
            }
            preValidCount = _validTouchID.Count;
            yield return null;
        }
    }

    private void StartRotate()
    {
        rotateCoroutine = StartCoroutine(RotateDetection());
    }
    private void StopRotate()
    {
        if (rotateCoroutine != null)
        StopCoroutine(rotateCoroutine);
    }

    IEnumerator RotateDetection()
    {
        while (true)
        {
            Vector2 touchDelta = Vector2.zero;
            for (int i = 0; i < _validTouchID.Count; i++)
            {
                int touchID = _validTouchID[i];
                Touch touch = _touchData[touchID];
                touchDelta += touch.delta;
            }
            touchDelta = ScreenScale(touchDelta);
            Rotate(touchDelta * Time.deltaTime * RotateSpeed);
            yield return null;
        }
    }

    private void Rotate(Vector2 angle)
    {
        // Calculate New Rotation
        verticalRotation -= angle.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);
        horizontalRotation += angle.x;
        _cameraPivot.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
    
    private float PinchDist()
    {
        Vector2 pos0 = _touchData[_validTouchID[0]].screenPosition;
        Vector2 pos1 = _touchData[_validTouchID[1]].screenPosition;
        return ScreenScale(pos0 - pos1).magnitude;
    }
    private void StartZoom()
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }
    private void StopZoom()
    {
        if (zoomCoroutine != null)
        StopCoroutine (zoomCoroutine);
    }
    IEnumerator ZoomDetection()
    {
        float prePinchDist = PinchDist();
        while (true)
        {
            float curPinchDist = PinchDist();
            float distDelta = curPinchDist - prePinchDist;
            Zoom(distDelta * Time.deltaTime * ZoomSpeed);
            prePinchDist = curPinchDist;
            yield return null;
        }
    }

    private void Zoom(float distDelta)
    {
        // New Camera Position
        Vector3 newCamPos = transform.localPosition;
        newCamPos -= distDelta * newCamPos.normalized;

        // Clamp Magnitude
        float dist = newCamPos.magnitude;
        if (newCamPos.z > 0) // Disable Negative
        {
            newCamPos = -newCamPos.normalized * NearestDistance;
        }
        if (dist > FarthestDistance)
        {
            newCamPos = newCamPos.normalized * FarthestDistance;
        }
        else if (dist < NearestDistance)
        {
            newCamPos = newCamPos.normalized * NearestDistance;
        }

        // Update sprimg arm (by change camera position)
        transform.localPosition = newCamPos;
    }

    private Vector2 ScreenScale(Vector2 vec)
    {
        // Scale vector like we do in Canvas Scaler.
        // Reference Resolution : (width, height) = (800, 600)
        Vector2 ret = vec;
        ret.x *= 800f / Screen.width;
        ret.y *= 600f / Screen.height;
        return ret;
    }
}