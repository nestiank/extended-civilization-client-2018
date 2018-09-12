using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;

public class DeployPrefab : MonoBehaviour
{
    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;

    private GameManager gameManager;
    private Game game;
    // Use this for initialization

    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }

    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

    private int numOfUnit;

    void Awake()
    {
        //Debug.Log("call DepPre");
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
        gameManager = GameManager.Instance;
        game = gameManager.Game;
    }

    public GameObject MakeItem(Production prod, int numOfUnit)
    {
        _deployment = prod; 
        string nameofProduction = ProductionFactoryTraits.GetFactoryName(prod.Factory);
        unitPrt.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetFacPortName(prod.Factory)).ToLower()), typeof(Sprite)) as Sprite;
        this.numOfUnit = numOfUnit;
        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "UnitName":
                    txt.text = nameofProduction;
                    break;
                case "NumberOfUnits":
                    txt.text = "X" + numOfUnit;
                    break;
            }
        }

        return this.gameObject;
    }

    public GameObject MakeItem()
    {
        _deployment = null;
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
    void Update()
    {

    }

    public void SetButton()
    {
        if (_deployment == null)
        {
            foreach (Button but in buttons)
            {
                but.enabled = false;
            }
        }
        else
        {
            foreach (Button but in buttons)
            {
                switch (but.name)
                {
                    case "Deploy":
                        if (_deployment != null)
                        {
                            if (ProductionFactoryTraits.isCityBuilding(_deployment.Factory))
                            {
                                but.onClick.AddListener(delegate ()
                                {
                                    List<Production> prodToDepList = new List<Production>();

                                    LinkedListNode<Production> nodeToDep = GameManager.Instance.Game.PlayerInTurn.Deployment.First;

                                    while (nodeToDep != null)
                                    {
                                        if (ProductionFactoryTraits.GetFactoryName(nodeToDep.Value.Factory) == ProductionFactoryTraits.GetFactoryName(_deployment.Factory))
                                        {
                                            prodToDepList.Add(nodeToDep.Value);
                                        }
                                        nodeToDep = nodeToDep.Next;
                                    }

                                    DeployItem(prodToDepList);

                                });
                            }
                            else
                            {
                                but.interactable = false;
                            }
                        }
                        break;

                    case "IndividualDeploy":
                        but.onClick.AddListener(delegate () {
                            List<Production> prodToDepList = new List<Production>();
                            prodToDepList.Add(_deployment);
                            DeployItem(prodToDepList);
                        });
                        break;
                }
            }
        }
    }

    public void DeployItem(List<Production> depList)
    {
        foreach (Production dep in depList)
        {
            if (!dep.IsCompleted)
            {
                //Debug.Log("Error : not finished product");
                throw new AccessViolationException();
            }
        }
        gameManager.DepStateEnter(depList);
        UIManager.Instance.mapUI.SetActive(true);
        UIManager.Instance.managementUI.SetActive(false);
        UIManager.Instance.questUI.SetActive(false);
    }
}
