using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource AudJump;
    [SerializeField] AudioSource AudEat;
    [SerializeField] AudioSource AudSprint;
    [SerializeField] AudioSource AudGrounded;
    [SerializeField] AudioSource AudMove;

    Coroutine m_MoveAudioRoutine;

    public void Jump()
    {
        AudJump.Play();
    }
    public void Eat()
    {
        AudEat.Play();
    }
    public void Sprint()
    {
        AudSprint.Play();
    }
    public void Grounded()
    {
        AudGrounded.Play();
    }

    public void StartMoveOnWater()
    {
        if (m_MoveAudioRoutine != null) StopCoroutine(m_MoveAudioRoutine);
        m_MoveAudioRoutine = StartCoroutine(AudioFadeCoroutine(AudMove, 0.5f));
    }

    public void StopMoveOnWater()
    {
        if (m_MoveAudioRoutine != null) StopCoroutine(m_MoveAudioRoutine);
        m_MoveAudioRoutine = StartCoroutine(AudioFadeCoroutine(AudMove, 0f));
    }

    IEnumerator AudioFadeCoroutine(AudioSource audio, float targetVolumn)
    {
        if (targetVolumn > 0f && !audio.isPlaying)
        {
            audio.Play();
        }

        float fadeTime = 0.3f;
        float time = 0f;
        float startVolumn = audio.volume;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float phase = (time / fadeTime);
            audio.volume = Mathf.Lerp(startVolumn, targetVolumn, phase);
            yield return null;
        }

        if (targetVolumn <= 0f && audio.isPlaying)
        {
            audio.Stop();
        }
    }
}
