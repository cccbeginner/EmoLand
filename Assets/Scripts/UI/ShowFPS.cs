using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    float sum = 0;
    Queue<float> queue = new Queue<float>();

    public TMPro.TMP_Text Text;

    // Update is called once per frame
    void Update()
    {
        queue.Enqueue(Time.deltaTime);
        sum += Time.deltaTime;
        while (sum > 1f)
        {
            sum -= queue.Dequeue();
        }
        Text.text = queue.Count.ToString();
    }

    private void Start()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
