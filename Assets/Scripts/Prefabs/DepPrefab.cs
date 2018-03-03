using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class DepPrefab : MonoBehaviour {

    public static GameObject DeployingObject;

    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;
   
    // Use this for initialization

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
    }

    public GameObject MakeItem(Production prod)
    {
        string nameofProduction = ProductionFactoryTraits.GetFactoryName(prod.Factory);
        unitPrt.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetFacPortName(prod.Factory)).ToLower()), typeof(Sprite)) as Sprite;
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
        if (i == -1)
        {
            foreach (Button but in buttons)
            {
                but.enabled = false;
            }
        }
        else
        {
            LinkedListNode<Production> dep = GameManager.I.Game.PlayerInTurn.Deployment.First;
            for (int k = 0; k < i; k++)
            {
                dep = dep.Next;
            }
            foreach (Button but in buttons)
            {
                switch (but.name)
                {
                    case "Deploy":
                        but.onClick.AddListener(delegate () { DeployItem(dep.Value); DeployingObject = this.gameObject; });
                        
                        break;
                }
            }
        }
    }

    public void DeployItem(Production dep)
    {
        if (dep.IsCompleted)
        {
            PseudoFSM.I.DepStateEnter(dep);
            UIManager.I.MapUIActive();
        }
        else
        {
            //Debug.Log("Error : not finished product");
            throw new AccessViolationException();
        }
    }
}
