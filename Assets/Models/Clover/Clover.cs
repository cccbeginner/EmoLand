using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clover : MonoBehaviour
{
    public float PushForce = 1.0f;
    public float PopForce = 0.5f;
    public UnityEvent OnFirstPush;
    public bool HasGrown { get; private set; }
    Vector3 m_InitPos;
    Coroutine m_PushCoroutine;
    private bool m_HasPushed = false;
    void Start()
    {
        m_InitPos = transform.localPosition;
        m_PushCoroutine = null;
        DontGrow();
        //Grow();
    }

    public void Push()
    {
        if (m_PushCoroutine == null)
            StartPush();
    }

    private void OnDisable()
    {
        CheckAndStopPush();
    }

    private void CheckAndStopPush()
    {
        if (m_PushCoroutine != null)
        {
            StopCoroutine(m_PushCoroutine);
            m_PushCoroutine = null;
        }
        transform.localPosition = m_InitPos;
    }

    private void StartPush()
    {
        m_PushCoroutine = StartCoroutine(PushAnimation());
        if (!m_HasPushed)
        {
            m_HasPushed = true;
            OnFirstPush.Invoke();
        }
    }

    public void Grow()
    {
        if (!HasGrown)
        {
            GetComponent<Animator>().SetTrigger("Grow");
            HasGrown = true;
        }
    }
    public void DontGrow()
    {
        HasGrown = false;
        GetComponent<Animator>().SetTrigger("DontGrow");
    }

    IEnumerator PushAnimation()
    {
        float speed = -PushForce;
        while (transform.localPosition.y <= m_InitPos.y)
        {
            transform.localPosition += speed * Vector3.up * Time.deltaTime;
            speed += PopForce * Time.deltaTime * (m_InitPos.y - transform.localPosition.y);
            yield return null;
        }
        transform.localPosition = m_InitPos;
        m_PushCoroutine = null;
    }
}
