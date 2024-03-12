using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    [SerializeField]
    Image m_Icon;

    public Button button
    {
        get
        {
            return GetComponent<Button>();
        }
    }
    private Animation m_Animation;

    private void OnEnable()
    {
        m_Animation = GetComponent<Animation>();
        m_Animation.playAutomatically = false;
        m_Animation["ButtonAppear"].speed = 2;
        m_Animation.Play("ButtonAppear");
    }

    public void Discard()
    {
        m_Animation["ButtonDisappear"].speed = 2;
        m_Animation.Play("ButtonDisappear");
        StartCoroutine(DisappearAfterSec());
    }

    public void SetIconSprite(Sprite icon)
    {
        m_Icon.sprite = icon;
    }
    public void SetColor(Color color)
    {
        m_Icon.color = color;
    }

    IEnumerator DisappearAfterSec()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
