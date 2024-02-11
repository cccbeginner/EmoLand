using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eatable : MonoBehaviour
{
    public float FollowSpeed = 5f;
    public float FinalDistance = 0.2f;
    public int SizeAddition = 1;
    public bool EnableDetect = true;
    public Collider AreaCollider;
    public UnityEvent OnEaten;
    private GameObject m_TargetPlayer;

    void Start()
    {

        if (AreaCollider == null)
        {
            Debug.LogWarning("Please use a collider that represent the detection area of being eaten.");
        }
        else
        {
            AreaCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (EnableDetect && Player.main?.gameObject == collider.gameObject)
        {
            m_TargetPlayer = collider.gameObject;
            StartCoroutine(BeingEaten());
        }
    }

    IEnumerator BeingEaten()
    {
        while (true)
        {
            Vector3 distVec = m_TargetPlayer.transform.position - transform.position;
            Vector3 moveVec = distVec.normalized * Time.deltaTime * FollowSpeed / distVec.magnitude;
            if (moveVec.sqrMagnitude <= distVec.sqrMagnitude)
            {
                transform.position += moveVec;
            }
            else
            {
                transform.position += distVec;
            }
            if (distVec.sqrMagnitude < FinalDistance * FinalDistance)
            {
                break;
            }
            yield return null;
        }
        BeEaten();
    }

    private void BeEaten()
    {
        OnEaten.Invoke();
        m_TargetPlayer.GetComponent<Player>().EatTrigger();
        m_TargetPlayer.GetComponent<Player>().Size += SizeAddition;
        Destroy(gameObject);
    }
}
