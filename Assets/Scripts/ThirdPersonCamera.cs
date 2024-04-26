using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public float HorizontalRotateSpeed = 0.6f;
    public float VerticalRotateSpeed = 0.3f;
    public float ZoomSpeed = 0.1f;
    public float LerpSpeed = 10f;
    public float AutoRotateSpeed = 0.6f;
    public float FarthestDistance = 30f;
    public float NearestDistance = 3f;
    public float ScreenMiddle = 0.5f;
    public float DefaultTapTime = 0.2f;
    public Transform m_CameraPivot;

    // Maintain current rotation.
    private float m_VerticalRotation;
    private float m_HorizontalRotation;
    private float m_PinchCameraDistance;

    [SerializeField]
    private InputAction m_Rotate, m_Zoom;

    private Vector3 m_PivotOffset = Vector3.zero;

    private bool m_IsDragging = false;

    public static ThirdPersonCamera main { get; private set; }

    private void Awake()
    {
        m_PivotOffset = m_CameraPivot.localPosition;
        transform.parent = m_CameraPivot;
        m_PinchCameraDistance = transform.localPosition.magnitude;
        main = this;
    }

    private void OnEnable()
    {
        m_Rotate.Enable();
        m_Zoom.Enable();
        m_IsDragging = false;
    }

    private void OnDisable()
    {
        m_Rotate.Disable();
        m_Zoom.Disable();
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        if (Player.main == null)
        {
            return;
        }

        // Follow Player
        if (!m_IsDragging)
        {
            Vector3 newPivotPosition = Vector3.Lerp(m_CameraPivot.position, Player.main.transform.position, LerpSpeed * Time.deltaTime) + m_PivotOffset;
            Vector3 curVec = m_CameraPivot.forward;
            Vector3 nextVec = curVec + (newPivotPosition - m_CameraPivot.position);
            float signedAngle = Vector2.SignedAngle(new Vector2(curVec.x, curVec.z), new Vector2(nextVec.x, nextVec.z));
            float autoRotateAngle = -signedAngle * AutoRotateSpeed;
            m_CameraPivot.position = newPivotPosition;
            Rotate(new Vector2(autoRotateAngle, 0));
        }
    }

    private void Update()
    {
        if (!m_IsDragging)
        {
            UpdateRotate();
            UpdateZoom();
        }
    }

    public void DragCameraToTransform(Transform targetTransform, float timeStay)
    {
        // The condition prevents 2 drags of camera
        if (!m_IsDragging) {
            StartCoroutine(DragCameraAction(targetTransform, timeStay));
        }
    }

    IEnumerator DragCameraAction(Transform targetTransform, float timeStay)
    {
        m_IsDragging = true;
        float timeNow = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // Drag the camera to the target transform
        while (timeNow <= timeStay)
        {
            timeNow += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, LerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, LerpSpeed * Time.deltaTime);
            yield return null;
        }

        // Drag camera to the original transform
        // (Put away the camera)
        Vector3 speed = Vector3.one;
        while (speed.sqrMagnitude >= 0.1f)
        {
            Vector3 nextPosition = Vector3.Lerp(transform.position, startPos, LerpSpeed * Time.deltaTime);
            speed = (nextPosition - transform.position) / Time.deltaTime;
            transform.position = nextPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, startRot, LerpSpeed * Time.deltaTime);
            yield return null;
        }
        m_IsDragging = false;
    }

    private void UpdateRotate()
    {
        Vector2 touchDelta = m_Rotate.ReadValue<Vector2>();
        Vector2 angle_rotate = ScreenScale(touchDelta);
        angle_rotate.x *= HorizontalRotateSpeed;
        angle_rotate.y *= VerticalRotateSpeed;
        Rotate(angle_rotate);
    }

    private void Rotate(Vector2 angle)
    {
        // Calculate New Rotation
        m_VerticalRotation -= angle.y;
        m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, -80f, 80f);
        m_HorizontalRotation += angle.x;
        m_CameraPivot.rotation = Quaternion.Euler(m_VerticalRotation, m_HorizontalRotation, 0);
    }
    
    private void UpdateZoom()
    {

        float pinchDelta = m_Zoom.ReadValue<float>();
        m_PinchCameraDistance -= pinchDelta * ZoomSpeed;
        m_PinchCameraDistance = Mathf.Clamp(m_PinchCameraDistance, NearestDistance, FarthestDistance);

        // Camera collider ray
        int layerMask = LayerMask.GetMask("Water", "Static Object");
        bool isRayHit = Physics.Raycast(
            m_CameraPivot.position,
            (transform.position - m_CameraPivot.position).normalized, 
            out RaycastHit hit,
            m_PinchCameraDistance,
            layerMask
        );

        float curDist = transform.localPosition.magnitude;
        float targetDist = m_PinchCameraDistance;
        if (isRayHit)
        {
            // Override pinch distance
            targetDist = hit.distance;
        }
        float smoothDist = Mathf.Lerp(curDist, targetDist, LerpSpeed * Time.deltaTime);
        SetCameraDist(smoothDist);
    }

    // Return result distance;
    private float SetCameraDist(float newDist)
    {
        // Clamp Magnitude
        newDist = Mathf.Clamp(newDist, NearestDistance, FarthestDistance);

        // Update sprimg arm (by change camera position)
        Vector3 newCamPos = newDist * transform.localPosition.normalized;
        transform.localPosition = newCamPos;
        return newDist;
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