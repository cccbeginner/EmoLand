using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WithMainPlayer : MonoBehaviour
{
    Vector3 localPos;
    Quaternion localRot;
    public UnityEvent OnMainPlayerJoin;
    public UnityEvent OnMainPlayerTouch;
    public bool InitEnableFollow = false;
    private bool _enableFollow = true;
    public bool EnableFollow { get
        {
            return _enableFollow;
        } set {
            _enableFollow = value;
            if (value) StartCoroutine(FollowMainPlayer());
        }
    }
    public bool FollowPosition = true;
    public bool FollowRotation = false;

    private void OnEnable()
    {
        localPos = transform.localPosition;
        localRot = transform.localRotation;
        StartCoroutine(WaitMainPlayer());
        EnableFollow = InitEnableFollow;
    }

    IEnumerator WaitMainPlayer()
    {
        while (Player.main == null)
        {
            yield return null;
        }
        OnMainPlayerJoin.Invoke();
    }

    IEnumerator FollowMainPlayer()
    {
        while (_enableFollow)
        {
            if (Player.main != null)
            {
                if (FollowPosition) transform.position = Player.main.transform.position + localPos;
                if (FollowRotation) transform.rotation = Player.main.transform.rotation * localRot;
            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && ReferenceEquals(player, Player.main))
        {
            OnMainPlayerTouch.Invoke();
        }
    }
}
