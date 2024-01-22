using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float RotateSpeed = 0.6f;
    public float ZoomSpeed = 0.1f;
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
    private Dictionary<int, Touch> _touchData;

    private void Awake()
    {
        _cameraPivot = gameObject.transform.parent;
        _touchData = new Dictionary<int, Touch>();
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
        float signedAngle = Vector2.SignedAngle(new Vector2(curVec.x, curVec.z), new Vector2(nextVec.x, nextVec.z));
        float autoRotateAngle = -signedAngle * Time.deltaTime * AutoRotateSpeed;
        _cameraPivot.position = newPivotPosition;
        Rotate(new Vector2(autoRotateAngle, 0));
    }

    IEnumerator TouchDetection()
    {
        _touchData.Clear();
        List<int> rightID = new List<int>();
        int preValidCount = 0;
        while (true)
        {
            // Update Current Touches that Rotates/Zooms Camera.
            for (int i = 0; i < Touch.activeTouches.Count; i++)
            {
                Touch touch = Touch.activeTouches[i];
                if (touch.began)
                {
                    // Prevent from quickly switching fingers.
                    if (_touchData.ContainsKey(touch.touchId))
                    {
                        _touchData.Remove(touch.touchId);
                    }
                    if (rightID.Contains(touch.touchId))
                    {
                        rightID.Remove(touch.touchId);
                    }
                    // Check if on the right side of screen.
                    if (touch.screenPosition.x > Screen.width * ScreenMiddle)
                    {
                        rightID.Add(touch.touchId);
                    }
                }
                // Make it valid if it's not a press AND on the right side of screen.
                if (touch.time > DefaultTapTime && rightID.Contains(touch.touchId) && !_touchData.ContainsKey(touch.touchId))
                {
                    _touchData.Add(touch.touchId, touch);
                }
                if (_touchData.ContainsKey(touch.touchId))
                {
                    if (touch.ended)
                    {
                        // Remove valid touch if ended.
                        _touchData.Remove(touch.touchId);
                        if (rightID.Contains(touch.touchId))
                        {
                            rightID.Remove(touch.touchId);
                        }
                    }
                    else
                    {
                        // Update touch data if still in progress.
                        _touchData[touch.touchId] = touch;
                    }
                }
            }

            // Update Rotate/Zoom Coroutine Status
            if (preValidCount < 1 && _touchData.Count >= 1)
            {
                StartRotate();
            }
            else if (preValidCount >= 1 && _touchData.Count < 1)
            {
                StopRotate();
            }
            if (preValidCount != 2 && _touchData.Count == 2)
            {
                StartZoom();
            }
            else if (preValidCount == 2 && _touchData.Count != 2)
            {
                StopZoom();
            }
            preValidCount = _touchData.Count;
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
        Vector2 angle_total = Vector2.zero;
        int cnt = 0;
        while (true)
        {
            Vector2 touchDelta = Vector2.zero;
            foreach (var item in _touchData)
            {
                Touch touch = item.Value;
                touchDelta += touch.delta;
            }
            touchDelta = ScreenScale(touchDelta);
            angle_total += touchDelta * RotateSpeed;
            cnt += 1;
            Rotate(touchDelta * RotateSpeed);
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
        Vector2 pos0 = _touchData[_touchData.Keys.First()].screenPosition;
        Vector2 pos1 = _touchData[_touchData.Keys.Last()].screenPosition;
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
            Zoom(distDelta * ZoomSpeed);
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