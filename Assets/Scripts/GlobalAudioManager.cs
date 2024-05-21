using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioManager : MonoBehaviour
{

    [SerializeField] AudioMixer m_GlobalAudioMixer;
    // Start is called before the first frame update
    void Start()
    {
        m_GlobalAudioMixer.SetFloat("GlobalVolume", -80f);
    }

    public void FadeInAudio()
    {
        StartCoroutine(AudioFadeCoroutine(-80, 0));
    }

    public void FadeOutAudio()
    {
        StartCoroutine(AudioFadeCoroutine(0, -80));
    }


    IEnumerator AudioFadeCoroutine(float startVolume, float targetVolume)
    {

        float fadeTime = 2f;
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float phase = (time / fadeTime);
            float curVol = Mathf.Lerp(startVolume, targetVolume, phase);

            m_GlobalAudioMixer.SetFloat("GlobalVolume", curVol);
            yield return null;
        }
    }
}
