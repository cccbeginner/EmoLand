using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manual : MonoBehaviour
{
    [Serializable]
    public struct ManualPage
    {
        public List<GameObject> Items;
    }

    public List<ManualPage> ManualPages;
    int m_CurrentPage = -1;
    public void OnEnable()
    {
        for (int i = 0; i < ManualPages.Count; i++)
        {
            for (int j = 0; j < ManualPages[i].Items.Count; j++)
            {
                ManualPages[i].Items[j].SetActive(false);
            }
        }
        NextPage();
    }
    private void Close()
    {
        gameObject.SetActive(false);
    }
    public void NextPage()
    {
        DisablePage(m_CurrentPage);
        m_CurrentPage += 1;
        if (m_CurrentPage == ManualPages.Count)
        {
            // already view all pages
            m_CurrentPage = -1;
            Close();
        }
        else
        {
            // enter next page
            EnablePage(m_CurrentPage);
        }
    }

    private void EnablePage(int page)
    {
        if (page < 0 || page >= ManualPages.Count) return;
        for (int j = 0; j < ManualPages[page].Items.Count; j++)
        {
            ManualPages[page].Items[j].SetActive(true);
        }
    }
    private void DisablePage(int page)
    {
        if (page < 0 || page >= ManualPages.Count) return;
        for (int j = 0; j < ManualPages[page].Items.Count; j++)
        {
            ManualPages[page].Items[j].SetActive(false);
        }
    }
}
