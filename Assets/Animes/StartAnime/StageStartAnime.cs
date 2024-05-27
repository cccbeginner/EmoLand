using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StageStartAnime : MonoBehaviour
{
    [SerializeField] GameObject AnimeCamera;
    [SerializeField] GameObject VPlayer;
    [SerializeField] Vector3 VPlayerForce;

    private void Start()
    {
        DisableAnime();
    }
    public void ShootVPlayer()
    {
        VPlayer.GetComponent<Rigidbody>().AddForce(VPlayerForce, ForceMode.Impulse);
    }

    public void SlowMotion(float totalTime)
    {
        StartCoroutine(SlowMotionRoutine(totalTime));
    }
    public void StartAnimeIfBegin()
    {
        if (PlayerDataSystem.currentStage == 0)
        {
            StartCoroutine(AnimeRoutine());
        }
    }

    void EnableAnime()
    {
        GetComponent<PlayableDirector>().Play();
        ThirdPersonCamera.main.gameObject.SetActive(false);
        AnimeCamera.SetActive(true);
    }
    void DisableAnime()
    {
        ThirdPersonCamera.main.gameObject.SetActive(true);
        AnimeCamera.SetActive(false);
    }

    IEnumerator AnimeRoutine()
    {
        EnableAnime();

        float time = 0;
        while (time <= GetComponent<PlayableDirector>().duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        DisableAnime();
    }
    IEnumerator SlowMotionRoutine(float totalTime)
    {
        Time.timeScale = 0.25f;
        yield return new WaitForSeconds(totalTime);
        Time.timeScale = 1;
    }
}
