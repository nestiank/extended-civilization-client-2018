using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;
using UnityEngine.EventSystems;

public class ProductablePrefab : MonoBehaviour, IPointerClickHandler {

    private static ManagementController uicontroller;
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    private int numberToProduce = 1;

    void Awake()
    {
        //Debug.Log("call SelPre");
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

    // Use this for initialization
    void Start()
    {
        uicontroller = ManagementController.GetManagementController();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject MakeItem(IProductionFactory fact)
    {
        //Debug.Log("Selection Queue Item Made");
        string nameofFactory = ProductionFactoryTraits.GetFactoryName(fact);
        unitPrt.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetFacPortName(fact))), typeof(Sprite)) as Sprite;
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = nameofFactory;
                    break;
                case "NumberOfUnits":
                    txt.text = "X " + numberToProduce.ToString();
                    break;
            }
        }
        return this.gameObject;
    }

    public GameObject MakeItem()
    {
        unitPrt.enabled = false;
        //Debug.Log("NULL Selection Queue");
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = "생산 가능 유닛 없음";
                    txt.fontSize = Screen.height / 40;
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

    public void SetButton(IProductionFactory fac)
    {
        foreach (Button but in buttons)
        {
            switch (but.name)
            {
                case "Deploy":
                    but.onClick.AddListener(delegate () { ProduceItem(fac); });
                    break;
                case "Up":
                    but.onClick.AddListener(delegate () { IncreseProduction(); });
                    break;
                case "Down":
                    but.onClick.AddListener(delegate () { DecreaseProduction(); });
                    break;
            }
        }
    }
    private void IncreseProduction()
    {
        numberToProduce++;
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "NumberOfUnits":
                    txt.text = "X " + numberToProduce.ToString();
                    break;
            }
        }
    }
    private void DecreaseProduction()
    {
        if (numberToProduce <= 1)
            return;
        numberToProduce--;
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "NumberOfUnits":
                    txt.text = "X " + numberToProduce.ToString();
                    break;
            }
        }
    }

    private void ProduceItem(IProductionFactory fac)
    {
        for (int i = 0; i < numberToProduce; i++)
        {
            GameManager.Instance.Game.PlayerInTurn.Production.AddLast(fac.Create(GameManager.Instance.Game.PlayerInTurn));
        }
        uicontroller.MakeProductionQ();
        uicontroller.MakeDeploymentQ();
    }

    public void OnPointerClick(PointerEventData clicked)
    {
        string clicked_unit_name = clicked.pointerPress.transform.GetChild(2).GetComponent<Text>().text;
        clicked.pointerPress.transform.parent.parent.parent.parent.parent.GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>().text = clicked_unit_name;
    }

    private string mkUnitDescription(string unitname)
    {
        switch(unitname)
        {
            case "이비자":
            case "Kimchi Factory":
            case "라티푼디움":
            case "환 제국 도시 연구소":
            case "성심당":
            case "환 제국 도시":
            case "미세먼지 공장":
            case "게르마늄 광산":
            case "옥타곤":
            case "Unknown: AncientFinnoLabortoryProductionFactory":
            case "자일리톨 생산지":
            case "고대 수오미 제국 도시":
            case "자경단"://환핀중복
            case "5차 산업혁명 공장"://환핀중복
            case "불가사의": //환핀중복
            case "5차 산업혁명 요새"://환핀중복
                break;
            case "EMU 궁기병":
            case "소서러":
            case "코끼리 장갑병":
            case "Autism Beam Drone":
            case "Genghis Khan":
            case "저궤도 우주 함대":
            case "유니콘 기사단":
            case "프로토 닌자":
            case "Jackie Chan":
            case "개척자"://환핀중복
            case "탈중앙화된 군인"://환핀중복
            case "스파이"://환핀중복
            case "제다이 기사단"://환핀중복
            default:
                break;
        }
        return "";
    }
        

    


}
