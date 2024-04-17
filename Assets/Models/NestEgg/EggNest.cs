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
    public UnityEvent RestoreBegin;
    public UnityEvent RestoreEnd;

    private void Start()
    {
        Egg.transform.position = Path.path.GetPointAtDistance(0f);
    }
    public void RestoreEgg()
    {
        StartCoroutine(MoveToNest());
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
