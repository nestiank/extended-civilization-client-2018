using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using System.Linq;
using UnityEngine.EventSystems;

public class ProductablePrefab : MonoBehaviour, IPointerClickHandler {

    private static ManagementController uicontroller;
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
    private int numberToProduce = 1;

    private IProductionFactory actorFactory;

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
        this.actorFactory = fact;
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
                    but.onClick.AddListener(delegate () { ProduceItem(fac); GameManager.Instance.CheckCompletedQuest(); });
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
        clicked.pointerPress.transform.parent.parent.parent.parent.parent.GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>().text
               = ProductionFactoryTraits.GetActorDescription(actorFactory);
    }

}
