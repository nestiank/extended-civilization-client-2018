using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class ProPrefab : MonoBehaviour {
    
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    // Use this for initialization
    void Awake()
    {
        //Debug.Log("call ProPre");
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
        unitPrt.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetFacPortName(prod.Factory)).ToLower()), typeof(Sprite)) as Sprite;
        //남은 턴 계산하는 중
        Double leftturn;
        string resultturn;
        
        if (prod.EstimatedGoldInputing == 0 || prod.EstimatedLaborInputing == 0)
        {
            //Debug.Log(prod.EstimatedGoldInputing + " : " + prod.EstimatedLaborInputing + " : " + prod.TotalGoldCost + " : " + prod.GoldInputed + " : " + prod.TotalLaborCost + " : " + prod.LaborInputed);
            if ((prod.TotalGoldCost - prod.GoldInputed) == 0 && (prod.TotalLaborCost - prod.LaborInputed) == 0)
            {
                leftturn = -1f;
            }
            else
            {
                if ((prod.TotalGoldCost - prod.GoldInputed) == 0)
                    leftturn = (prod.TotalLaborCost - prod.LaborInputed) / prod.EstimatedLaborInputing;
                else if ((prod.TotalLaborCost - prod.LaborInputed) == 0)
                    leftturn = (prod.TotalGoldCost - prod.GoldInputed) / prod.EstimatedGoldInputing;
                else
                    leftturn = -1f;
            }
        }
        else
        {
            leftturn = Math.Max(((prod.TotalGoldCost - prod.GoldInputed) / prod.EstimatedGoldInputing),((prod.TotalLaborCost - prod.LaborInputed) / prod.EstimatedLaborInputing));
        }
        if (leftturn == -1 || Double.IsInfinity(leftturn))
            resultturn = "?";
        else
        {
            //Debug.Log(leftturn);
            resultturn = Convert.ToInt32(leftturn).ToString();
        }
        //텍스트 표시
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "TurnsLeft":
                    txt.text = resultturn + "턴 이후 배치 가능.";
                    break;
                case "UnitName":
                    txt.text = nameofProduction + " ";
                    break;
                case "Required Resource":
                    txt.text = "금 : 턴당 " + Convert.ToInt32(prod.EstimatedGoldInputing) + " (" + Convert.ToInt32(prod.GoldInputed)+ "/" + Convert.ToInt32(prod.TotalGoldCost).ToString() + ")"
                        +"\n노동력 : 턴당 " + Convert.ToInt32(prod.EstimatedLaborInputing) + " (" + Convert.ToInt32(prod.LaborInputed).ToString() + "/" + Convert.ToInt32(prod.TotalLaborCost).ToString() + ")";
                    break;
            }
        }
        return this.gameObject;
    }
    //production이 비었을 때 블랭크 아이템 만들기
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
    //버튼 기능 붙이는 함수
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
                            //Debug.Log(but.name);
                            GameManager.I.Game.PlayerInTurn.Production.Remove(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        break;
                    case "Top":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            GameManager.I.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.I.Game.PlayerInTurn.Production.AddFirst(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        if (GameManager.I.Game.PlayerInTurn.Production.First == prod)
                            but.enabled = false;
                        break;
                    case "Up":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Previous;
                            GameManager.I.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.I.Game.PlayerInTurn.Production.AddBefore(temprod, prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        if (GameManager.I.Game.PlayerInTurn.Production.First == prod)
                            but.enabled = false;
                        break;
                    case "Bottom":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            GameManager.I.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.I.Game.PlayerInTurn.Production.AddLast(prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        if (GameManager.I.Game.PlayerInTurn.Production.Last == prod)
                            but.enabled = false;
                        break;
                    case "Down":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            GameManager.I.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.I.Game.PlayerInTurn.Production.AddAfter(temprod, prod);
                            ManagementUIController.GetManagementUIController().MakeProductionQ();
                        });
                        if (GameManager.I.Game.PlayerInTurn.Production.Last == prod)
                            but.enabled = false;
                        break;
                }
            }
        }
    }
}
