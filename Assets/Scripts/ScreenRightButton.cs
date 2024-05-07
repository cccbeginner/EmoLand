using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRightButton : MonoBehaviour
{
    [SerializeField] Image RoundBgImage;
    Color m_BgImageColor;

    private void Start()
    {
        var withMainPlayer =  GetComponent<WithMainPlayer>();
        withMainPlayer.OnMainPlayerJoin.AddListener(OnMainPlayerJoin);

        m_BgImageColor = RoundBgImage.color;
    }

    private void OnMainPlayerJoin()
    {
        Player.main.playerSprint.OnSprintBegin.AddListener(HoldAnime);
    }

    private void HoldAnime()
    {
        StartCoroutine(HoldAnimeRoutine());
    }

    IEnumerator HoldAnimeRoutine()
    {
        float animeTime = 1f;
        float timeNow = 0;
        float maxaTime = 0.2f;
        float maxa = 0.2f;
        float alpha;
        while (timeNow < animeTime)
        {
            timeNow += Time.deltaTime;
            timeNow = Mathf.Min(timeNow, animeTime);
            if (timeNow < maxaTime)
            {
                alpha = Mathf.SmoothStep(0, maxa, timeNow / maxaTime);
            }
            else
            {
                alpha = Mathf.SmoothStep(maxa, 0, timeNow / (animeTime - maxaTime));
            }
            m_BgImageColor.a = alpha;
            RoundBgImage.color = m_BgImageColor;
            yield return null;
        }
        m_BgImageColor.a = 0;
        RoundBgImage.color = m_BgImageColor;
    }
}
