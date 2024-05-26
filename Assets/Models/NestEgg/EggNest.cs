using PathCreation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EggNest : MonoBehaviour
{
    [SerializeField] GameObject Egg;
    [SerializeField] PathCreator Path;
    [SerializeField] AudioSource IdleAudio;
    [SerializeField] AudioSource TouchAudio;
    [SerializeField] AudioSource ShowAudio;
    [SerializeField] ParticleSystem HonkParticle;
    [SerializeField] AudioSource HonkAudio;
    [SerializeField] float HonkDuration;
    public bool ShowEggInit = true;
    public float ShowEggScale = 1f;
    public int Stage = 0;
    public UnityEvent RestoreBegin;
    public UnityEvent RestoreEnd;
    private bool m_IsRestored = false;

    public void InitNotDone()
    {
        Egg.transform.position = Path.path.GetPointAtDistance(0f);
        if (ShowEggInit) ShowEgg(0);
        else Egg.gameObject.SetActive(false);
        m_IsRestored = false;
        StartCoroutine(HonkRoutine());
    }
    public void InitDone()
    {
        float length = GetPathLength();
        Egg.transform.position = Path.path.GetPointAtDistance(length-0.1f);
        ShowEggScale = 1f;
        ShowEgg(0);
        m_IsRestored = true;
        StartCoroutine(HonkRoutine());
    }

    private void Update()
    {
        if (!HonkParticle.isPlaying && PlayerDataSystem.currentStage == Stage && m_IsRestored)
        {
            HonkParticle.Play();
        }
        else if (HonkParticle.isPlaying && PlayerDataSystem.currentStage != Stage)
        {
            HonkParticle.Stop();
        }
    }

    IEnumerator HonkRoutine()
    {
        bool firstHonk = true;
        while (true)
        {
            if (PlayerDataSystem.currentStage == Stage && m_IsRestored)
            {
                HonkParticle.Stop();
                HonkParticle.Play();
                float randf = UnityEngine.Random.Range(-0.1f, 0.1f);
                if (firstHonk ) HonkAudio.volume += 0.5f;
                HonkAudio.pitch += randf;
                HonkAudio.Play();
                yield return new WaitForSeconds(HonkDuration);
                HonkAudio.pitch -= randf;
                if (firstHonk) HonkAudio.volume -= 0.5f;
                if (firstHonk) firstHonk = false;
            }
            else if (PlayerDataSystem.currentStage > Stage)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    private float GetPathLength()
    {
        float[] lengths = Path.path.cumulativeLengthAtEachVertex;
        float length = lengths[lengths.Length - 1];
        return length;
    }
    
    public void RestoreEgg()
    {
        StartCoroutine(MoveToNest());
    }

    public void ShowEgg(float timeDelay)
    {
        StartCoroutine(ShowEggRoutine(timeDelay));
        IdleAudio.Play();
    }

    IEnumerator ShowEggRoutine(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        Egg.gameObject.SetActive(true);
        Egg.transform.localScale = Vector3.zero;
        Egg.transform.GetChild(0).localScale = Vector3.zero;
        ShowAudio.Play();

        float currentScale = 0f;
        while (ShowEggScale - currentScale > 0.01)
        {
            currentScale = Mathf.Lerp(currentScale, ShowEggScale, 0.5f * Time.deltaTime);
            Egg.transform.GetChild(0).localScale = currentScale * Vector3.one;
            Egg.transform.localScale = currentScale * Vector3.one;
            yield return null;
        }
    }
    

    IEnumerator MoveToNest()
    {
        RestoreBegin.Invoke();
        TouchAudio.Play();

        float speed = 20f;
        float x = 0f;
        float length = GetPathLength();
        while (x < length)
        {
            Egg.transform.position = Path.path.GetPointAtDistance(x);
            Egg.transform.localScale = Vector3.Lerp(Egg.transform.localScale, Vector3.one, Time.deltaTime);
            x += Time.deltaTime * speed;
            if (x >= length) break;
            yield return null;
        }

        RestoreEnd.Invoke();
        m_IsRestored = true;
    }
}
