using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float RotateSpeed = 20f;
    public float ZoomSpeed = 10f;
    public float FarthestDistance = 30f;
    public float NearestDistance = 3f;

    private Transform _cameraPivot;

    private float verticalRotation;
    private float horizontalRotation;

    [SerializeField]
    private InputAction press, touch0Pos, touch0Delta, touch1Contact, touch1Pos, touch1Delta;
    private Coroutine rotateCoroutine, zoomCoroutine;

    private void Awake()
    {
        _cameraPivot = gameObject.transform.parent;
    }

    private void OnEnable()
    {
        press.Enable();
        touch0Pos.Enable();
        touch1Pos.Enable();
        touch1Contact.Enable();
        touch0Delta.Enable();
        touch1Delta.Enable();
    }
    private void OnDisable()
    {
        press.Disable();
        touch0Pos.Disable();
        touch1Pos.Disable();
        touch1Contact.Disable();
        touch0Delta.Disable();
        touch1Delta.Disable();
    }

    private void Start()
    {
        // Rotate?
        press.performed += _ =>
        {
            // Only rotate when pressed at right side of the screen.
            if (touch0Pos.ReadValue<Vector2>().x > Screen.width * 0.4) StartRotate();
        };
        press.canceled += _ => { StopRotate(); };
        // Zoom?
        touch1Contact.performed += _ =>
        {
            // Only zoom when first pressed is available.
            if (touch0Pos.ReadValue<Vector2>().x > Screen.width * 0.4) StartZoom();
        };
        touch1Contact.canceled += _ => { StopZoom(); };
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Follow Player
        _cameraPivot.position = Target.position;
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
            Vector2 touchDelta = touch0Delta.ReadValue<Vector2>();
            if (touch1Contact.IsPressed())
            {
                touchDelta = (touchDelta + touch1Delta.ReadValue<Vector2>()) / 2;
            }
            
            // Calculate New Rotation
            verticalRotation -= touchDelta.y * Time.deltaTime * RotateSpeed;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += touchDelta.x * Time.deltaTime * RotateSpeed;

            _cameraPivot.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            yield return null;
        }
    }
    
    private float PinchDist()
    {
        Vector2 pos0 = touch0Pos.ReadValue<Vector2>();
        Vector2 pos1 = touch1Pos.ReadValue<Vector2>();
        return (pos0 - pos1).magnitude;
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
            
            // New Camera Position
            Vector3 newCamPos = transform.localPosition;
            newCamPos -= distDelta * Time.deltaTime * ZoomSpeed * newCamPos.normalized;

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

            transform.localPosition = newCamPos;
            prePinchDist = curPinchDist;
            yield return null;
        }
    }
}