using PathCreation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EggNest : MonoBehaviour
{
    [SerializeField] GameObject Egg, Nest;
    [SerializeField] PathCreator Path;
    public bool ShowEggInit = true;
    public float ShowEggScale = 1f;
    public UnityEvent RestoreBegin;
    public UnityEvent RestoreEnd;


    private void Start()
    {
        Egg.transform.position = Path.path.GetPointAtDistance(0f);
        if (ShowEggInit) ShowEgg(0);
        else Egg.gameObject.SetActive(false);
    }
    
    public void RestoreEgg()
    {
        StartCoroutine(MoveToNest());
    }

    public void ShowEgg(float timeDelay)
    {
        StartCoroutine(ShowEggRoutine(timeDelay));
    }

    IEnumerator ShowEggRoutine(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        Egg.gameObject.SetActive(true);
        Egg.transform.localScale = Vector3.zero;
        Egg.transform.GetChild(0).localScale = Vector3.zero;

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
        float speed = 20f;
        float x = 0f;
        float[] lengths = Path.path.cumulativeLengthAtEachVertex;
        float length = lengths[lengths.Length - 1];
        while (x < length)
        {
            Egg.transform.position = Path.path.GetPointAtDistance(x);
            Egg.transform.localScale = Vector3.Lerp(Egg.transform.localScale, Vector3.one, Time.deltaTime);
            x += Time.deltaTime * speed;
            if (x >= length) break;
            yield return null;
        }
        RestoreEnd.Invoke();
    }
}
