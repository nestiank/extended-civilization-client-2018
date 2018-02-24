using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class ProPrefab : MonoBehaviour {

    private static int unitNum = 0;
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    // Use this for initialization
    void Awake()
    {
        Debug.Log("call ProPre");
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
            switch (txt.name)
            {
                case "TurnsLeft":
                    txt.text = "?턴 이후 배치 가능.";
                    break;
                case "UnitName":
                    txt.text = nameofProduction + " " + unitNum++;
                    break;
                case "Required Resource":
                    txt.text = "금 : 턴당 " + "?" + " (" + "?" + "/" + Convert.ToInt32(prod.TotalGoldCost).ToString() + ")"
                        +"\n노동력 : 턴당 " + "?" + " (" + Convert.ToInt32(prod.LaborInputed).ToString() + "/" + Convert.ToInt32(prod.TotalLaborCost).ToString() + ")";
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
                    txt.text = "생산 중인 유닛/건물이 없습니다!\n 오른쪽의 탭에서 생산을 선택하세요!";
                    break;
                case "UnitName":
                    txt.text = "";
                    break;
                case "Required Resource":
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
    public void SetButton(int i)
    {
        if(i == -1)
        {
            foreach (Button but in buttons)
            {
                but.enabled = false;
            }
        }
        else
        {
            LinkedListNode <Production> prod = GameManager.I.Game.PlayerInTurn.Production.First;
            for(int k = 0; k < i; k++)
            {
                prod = prod.Next;
            }
            foreach (Button but in buttons)
            {
                switch (but.name)
                {
                    case "Delete":
                        but.onClick.AddListener(delegate () {
                            Debug.Log(but.name);
                            prod.List.Remove(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                    case "Top":
                        but.onClick.AddListener(delegate () {
                            Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Previous;
                            prod.List.Remove(prod);
                            temprod.List.AddFirst(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                    case "Up":
                        but.onClick.AddListener(delegate () {
                            Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Previous;
                            prod.List.Remove(prod);
                            temprod.List.AddBefore(temprod, prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                    case "Down":
                        but.onClick.AddListener(delegate () {
                            Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            prod.List.Remove(prod);
                            temprod.List.AddLast(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                    case "Bottom":
                        but.onClick.AddListener(delegate () {
                            Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            prod.List.Remove(prod);
                            temprod.List.AddAfter(temprod, prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                }
                ManagementUIController.GetManagementUIController().MakeProductionQ();
            }
        }
    }
    public static void ResetTestingNumber()
    {
        unitNum = 0;
    }
}
