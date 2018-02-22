using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class DepPrefab : MonoBehaviour {

    private static ManagementUIController uicontroller;

    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    // Use this for initialization

    void Awake()
    {
        Debug.Log("call DepPre");
        textarguments = gameObject.GetComponentsInChildren<Text>();
        foreach (Image unt in gameObject.GetComponentsInChildren<Image>())
        {
            if (unt.name == "Portrait")
            {
                unitPrt = unt;
            }
        }
        buttons = gameObject.GetComponentsInChildren<Button>();
    }

    void Start()
    {
        uicontroller = ManagementUIController.GetManagementUIController();
    }

    public GameObject MakeItem(Production prod)
    {
        string nameofProduction = ProductionFactoryTraits.GetFactoryName(prod.Factory);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofProduction + "_portrait");
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = nameofProduction;
                    break;
                case "NumberOfUnits":
                    txt.text = "X 1";
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
                case "UnitName":
                    txt.text = "비었음";
                    break;
                case "NumberOfUnits":
                    txt.text = "";
                    break;
            }
        }
        foreach (Button but in buttons)
        {
            but.gameObject.SetActive(false);
        }
        return this.gameObject;
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void SetButton(int i)
    {
    }
}
