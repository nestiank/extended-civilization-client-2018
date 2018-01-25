using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using CivPresenter;

public class DepPrefab : MonoBehaviour
{

    private Text unitName;
    private Image unitPrt;
    // Use this for initialization

    void Awake()
    {
        Debug.Log("call DepPre");
        unitName = gameObject.GetComponentsInChildren<Text>()[2];
        unitName.text = "초기";

        unitPrt = gameObject.GetComponentsInChildren<Image>()[0];
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject MakeItem(IProductionFactory fact)
    {
        string nameofFactory = ProductionFactoryTraits.GetFactoryName(fact);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofFactory);
        unitName.text = nameofFactory;
        return this.gameObject;
    }

    public GameObject MakeItem()
    {
        if (unitName == null)
        {
            Debug.Log("Noname");
        }
        unitName.text = "배치 가능 유닛 없음";
        unitName.fontSize = 10;
        return this.gameObject;
    }
}

