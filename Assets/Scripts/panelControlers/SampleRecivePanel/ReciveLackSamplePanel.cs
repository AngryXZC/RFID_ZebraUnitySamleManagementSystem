using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class ReciveLackSamplePanel : MonoBehaviour
{
    public Text title;
    /// <summary>
    /// ������������Ϣ
    /// </summary>
    private InstitutionInfo data;
    /// <summary>
    /// ����������Ϣ
    /// </summary>
    private HerdsmanListItem herdsmanData;
    public GameObject listItem;
    public Transform content;
    public Button returnBtn;
    private void OnEnable()
    {
        returnBtn.onClick.AddListener(() => onReturnBtnOnclick());
    }
    /// <summary>
    /// �����鿴���
    /// </summary>
    /// <param name="res"></param>
    public void initData(InstitutionInfo res)
    {

        int number = 0;
        data = res;
        Debug.Log(JsonMapper.ToJson(data));
        if (data.herdsmanList.Count > 0)
        {
            foreach (var item in data.herdsmanList)
            {
                //if (item.lackSampleNum > 0)
                //{
                foreach (var te in item.tagCodeList)
                {
                    if (!te.isRecongnized)
                    {
                        number++;
                        GameObject go = Instantiate(listItem);
                        
                        ReciveLackItem reciveLackItem = go.GetComponent<ReciveLackItem>();
                        reciveLackItem.setInfo(number.ToString(), item.herdsmanName, te.tagCode);
                        go.transform.SetParent(content);
                        go.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                //}

            }

        }


        number = 0;
    }

    public void initHerdmanData(HerdsmanListItem data, WatchEnum watchEnum)
    {
        herdsmanData = data;
        int number = 0;
        //�鿴��������ȱ����Ʒ
        if (watchEnum == WatchEnum.LackSample)
        {
            foreach (var te in herdsmanData.tagCodeList)
            {

                if (!te.isRecongnized)
                {
                    number++;
                    GameObject go = Instantiate(listItem);

                    ReciveLackItem reciveLackItem = go.GetComponent<ReciveLackItem>();
                    reciveLackItem.setInfo(number.ToString(), herdsmanData.herdsmanName, te.tagCode);
                    go.transform.SetParent(content);
                    go.transform.localScale = new Vector3(1, 1, 1);
                }


            }
        }
        //�鿴���������޶�������
        if (watchEnum == WatchEnum.NoearCodeSample)
        {
            foreach (var te in herdsmanData.noEarCodeList)
            {

                number++;
                GameObject go = Instantiate(listItem);

                ReciveLackItem reciveLackItem = go.GetComponent<ReciveLackItem>();
                reciveLackItem.setInfo(number.ToString(), herdsmanData.herdsmanName, te);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        number = 0;
    }
    private void OnDisable()
    {
        data = null;
        herdsmanData = null;
        returnBtn.onClick.RemoveAllListeners();
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

    }
    public void onReturnBtnOnclick()
    {
        gameObject.SetActive(false);
        GangwayDoorDevice device = GangwayDoorDevice.getInstance();
        if (!device.IsInventory)
        {
            device.StartInventory();
        }
    }
}
