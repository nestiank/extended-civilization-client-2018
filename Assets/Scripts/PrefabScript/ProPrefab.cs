using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using CivPresenter;

public class ProPrefab : MonoBehaviour
{
    private static Presenter presenter;

    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    // Use this for initialization
    void Awake()
    {
        Debug.Log("call ProPre");
        textarguments = gameObject.GetComponentsInChildren<Text>();
        foreach(Image unt in gameObject.GetComponentsInChildren<Image>())
        {
            if(unt.name == "Image")
            {
                unitPrt = unt;
            }
        }
        
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }


    public GameObject MakeItem(Production prod)
    {
        string nameofProduction = ProductionFactoryTraits.GetFactoryName(prod.Factory);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofProduction + "_portrait");
        foreach (Text txt in textarguments)
        {
            switch(txt.name)
            {
                case "TurnsLeft":
                    txt.text = "?턴 이후 배치 가능.";
                    break;
                case "UnitName":
                    txt.text = nameofProduction;
                    break;
                case "GoldPer":
                    txt.text = "금 : 턴당 " + "?" + " (" + "?" + "/" + Convert.ToInt32(prod.TotalCost).ToString() + ")";
                    break;
                case "LaborPer":
                    txt.text = "노동력 : 턴당 " + "?" + " (" + Convert.ToInt32(prod.LaborInputed).ToString() + "/" + Convert.ToInt32(prod.TotalCost).ToString() + ")";
                    break;
            }
        }
        return this.gameObject;
    }
    public GameObject MakeItem()
    {
        unitPrt.enabled = false;
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "TurnsLeft":
                    txt.text = "비었음";
                    break;
                case "UnitName":
                    txt.text = "B었음";
                    break;
            }
        }
        return this.gameObject;
    }
    public void SetButton(int i)
    {
    }
    public static void SetPresenter()
    {
        presenter = CIVGameManager.GetGameManager().GetComponent<CIVGameManager>().GetPresenter();
    }
}
