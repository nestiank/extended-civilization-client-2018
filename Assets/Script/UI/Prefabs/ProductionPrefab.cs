using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class ProductionPrefab : MonoBehaviour {

    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    private Production production;
    private GameManager gameManager;
    private Game game;
    private int numOfUnit;

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
        SetEstimated();
    }

    public void SetEstimated()
    {
        if (production != null)
        {
            game.PlayerInTurn.EstimateResourceInputs();
            string nameofProduction = ProductionFactoryTraits.GetFactoryName(production.Factory);
            Double leftturn;
            string resultturn;

            if (production.EstimatedGoldInputing == 0 || production.EstimatedLaborInputing == 0)
            {
                //Debug.Log(production.EstimatedGoldInputing + " : " + production.EstimatedLaborInputing + " : " + production.TotalGoldCost + " : " + production.GoldInputed + " : " + production.TotalLaborCost + " : " + production.LaborInputed);
                if ((production.TotalGoldCost - production.GoldInputed) == 0 && (production.TotalLaborCost - production.LaborInputed) == 0)
                {
                    leftturn = -1f;
                }
                else
                {
                    if ((production.TotalGoldCost - production.GoldInputed) == 0)
                        leftturn = (production.TotalLaborCost - production.LaborInputed) / production.EstimatedLaborInputing;
                    else if ((production.TotalLaborCost - production.LaborInputed) == 0)
                        leftturn = (production.TotalGoldCost - production.GoldInputed) / production.EstimatedGoldInputing;
                    else
                        leftturn = -1f;
                }
            }
            else
            {
                leftturn = Math.Max(((production.TotalGoldCost - production.GoldInputed) / production.EstimatedGoldInputing), ((production.TotalLaborCost - production.LaborInputed) / production.EstimatedLaborInputing));
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
                        txt.text = "금 : 턴당 " + Convert.ToInt32(production.EstimatedGoldInputing) + " (" + Convert.ToInt32(production.GoldInputed) + "/" + Convert.ToInt32(production.TotalGoldCost).ToString() + ")"
                            + "\n노동력 : 턴당 " + Convert.ToInt32(production.EstimatedLaborInputing) + " (" + Convert.ToInt32(production.LaborInputed).ToString() + "/" + Convert.ToInt32(production.TotalLaborCost).ToString() + ")";
                        break;
                    case "NumOfUnit":
                        txt.text = "X" + this.numOfUnit;
                        break;
                }
            }

            if(production.EstimatedLaborInputing == production.LaborCapacityPerTurn && production.EstimatedGoldInputing == production.GoldCapacityPerTurn)
            {
                Image image = this.gameObject.GetComponent<Image>();
                image.color = Color.green; 
            }

            else if(production.EstimatedLaborInputing == 0 || production.EstimatedGoldInputing == 0)
            {
                Image image = this.gameObject.GetComponent<Image>();
                image.color = Color.red;
            }

            else
            {
                Image image = this.gameObject.GetComponent<Image>();
                image.color = new Color(229 / 255, 105 / 255, 25 / 255, 1);
            }
        }
    }

    public GameObject MakeItem(Production prod, int numOfUnit)
    {
        //투입될 자원을 보기 위해 필요
        gameManager = GameManager.Instance;
        game = gameManager.Game;
        this.numOfUnit = numOfUnit;

        this.production = prod;
        string nameofProduction = ProductionFactoryTraits.GetFactoryName(prod.Factory);
        unitPrt.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetFacPortName(prod.Factory)).ToLower()), typeof(Sprite)) as Sprite;
        
        SetEstimated();

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
                    txt.text = "생산 중인 유닛/건물이 없습니다\n오른쪽 탭에서 생산을 선택하세요";
                    txt.fontSize = Screen.height / 20;
                    break;
                case "UnitName":
                    txt.text = "";
                    break;
                case "Required Resource":
                    txt.text = "";
                    break;
                case "NumOfUnit":
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
        if (i == -1)
        {
            foreach (Button but in buttons)
            {
                but.enabled = false;
            }
        }
        else
        {
            LinkedListNode<Production> prod = GameManager.Instance.Game.PlayerInTurn.Production.First;
            for (int k = 0; k < i; k++)
            {
                prod = prod.Next;
            }
            foreach (Button but in buttons)
            {
                switch (but.name)
                {
                    case "Delete":
                        but.onClick.AddListener(delegate () {

                            LinkedListNode<Production> prodToDel = GameManager.Instance.Game.PlayerInTurn.Production.First;

                            while (prodToDel != null)
                            {
                                if (ProductionFactoryTraits.GetFactoryName(prodToDel.Value.Factory) == ProductionFactoryTraits.GetFactoryName(prod.Value.Factory))
                                {
                                    GameManager.Instance.Game.PlayerInTurn.Production.Remove(prodToDel);
                                    ManagementController.GetManagementController().MakeProductionQ();
                                    break;
                                }
                                else
                                {
                                    prodToDel = prodToDel.Next;
                                }
                            }
                        });
                        break;
                    case "Top":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            GameManager.Instance.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.Instance.Game.PlayerInTurn.Production.AddFirst(prod);
                            ManagementController.GetManagementController().MakeProductionQ();
                        });
                        if (GameManager.Instance.Game.PlayerInTurn.Production.First == prod)
                            but.enabled = false;
                        break;
                    case "Up":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Previous;
                            GameManager.Instance.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.Instance.Game.PlayerInTurn.Production.AddBefore(temprod, prod);
                            ManagementController.GetManagementController().MakeProductionQ();
                        });
                        if (GameManager.Instance.Game.PlayerInTurn.Production.First == prod)
                            but.enabled = false;
                        break;
                    case "Bottom":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            GameManager.Instance.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.Instance.Game.PlayerInTurn.Production.AddLast(prod);
                            ManagementController.GetManagementController().MakeProductionQ();
                        });
                        if (GameManager.Instance.Game.PlayerInTurn.Production.Last == prod)
                            but.enabled = false;
                        break;
                    case "Down":
                        but.onClick.AddListener(delegate () {
                            //Debug.Log(but.name);
                            LinkedListNode<Production> temprod = prod.Next;
                            GameManager.Instance.Game.PlayerInTurn.Production.Remove(prod);
                            GameManager.Instance.Game.PlayerInTurn.Production.AddAfter(temprod, prod);
                            ManagementController.GetManagementController().MakeProductionQ();
                        });
                        if (GameManager.Instance.Game.PlayerInTurn.Production.Last == prod)
                            but.enabled = false;
                        break;
                }
            }
        }
    }
}
