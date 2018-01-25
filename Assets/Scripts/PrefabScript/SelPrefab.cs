using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using CivPresenter;

public class SelPrefab : MonoBehaviour {

    private Text unitName;
    private Image unitPrt;
    private Text theNumberofProduce;
	// Use this for initialization
	void Awake () {
        Debug.Log("call SelPre");
        unitName = gameObject.GetComponentsInChildren<Text>()[1];
        unitName.text = "초기";

        unitPrt = gameObject.GetComponentsInChildren<Image>()[0];

        theNumberofProduce = gameObject.GetComponentsInChildren<Text>()[2];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
	}

    public GameObject MakeItem(IProductionFactory fact)
    {
        string nameofFactory = ProductionFactoryTraits.GetFactoryName(fact);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofFactory);
        unitName.text = nameofFactory;
        theNumberofProduce.text = "X 1";
        return this.gameObject;
    }
    public GameObject MakeItem()
    {
        if(unitName == null)
        {
            Debug.Log("Fuck");
        }
        unitName.text = "생산 가능 유닛 없음";
        unitName.fontSize = 10;
        theNumberofProduce.text = "";
        return this.gameObject;
    }
}

