using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LackSampleController : MonoBehaviour
{
    public Button returnBtn;
    public GameObject itemPrefab;
    public GameObject content;
    /// <summary>
    /// ¼ÆÊýÆ÷
    /// </summary>
    private int num = 0;
    private void OnEnable()
    {
        returnBtn.onClick.AddListener(retrunDetect);
    }
    public void showItem(CleanHerdsmanListItem herdsmanItem)
    {
        foreach (var item in herdsmanItem.cleanSampleList)
        {
            if (!item.isRecongnized) 
            {
                num++;
                GameObject go = Instantiate(itemPrefab);
                go.transform.SetParent(content.transform);
                LackItem lackItem=go.GetComponent<LackItem>();
                lackItem.setInfo(num.ToString(),herdsmanItem.herdsmanName,item.sampleCode,item.earCode);
            }
        }
        
    }
    public void showBackItem(BackHerdsmanListItem herdsmanItem)
    {
        foreach (var item in herdsmanItem.cleanSampleList)
        {
            if (!item.isRecongnized)
            {
                num++;
                GameObject go = Instantiate(itemPrefab);
                go.transform.SetParent(content.transform);
                LackItem lackItem = go.GetComponent<LackItem>();
                lackItem.setInfo(num.ToString(), herdsmanItem.herdsmanName, item.sampleCode, item.earCode);
            }
        }

    }
    public void retrunDetect()
    {
      Destroy(gameObject);
       
    }
    private void OnDestroy()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i));
        }
        num = 0;
    }
}
