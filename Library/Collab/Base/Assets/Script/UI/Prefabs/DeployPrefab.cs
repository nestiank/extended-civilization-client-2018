using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class DeployPrefab : MonoBehaviour
{

    public static GameObject DeployingObject;

    private Text[] textarguments;
    private Image unitPrt;
    private Button[] buttons;

    // Use this for initialization

    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }

    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

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
    void Update()
    {

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
            LinkedListNode<Production> dep = GameManager.Instance.Game.PlayerInTurn.Deployment.First;
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
            DepStateEnter(dep);
            UIManager.Instance.mapUI.SetActive(true);
            UIManager.Instance.managementUI.SetActive(false);
            UIManager.Instance.questUI.SetActive(false);
        }
        else
        {
            //Debug.Log("Error : not finished product");
            throw new AccessViolationException();
        }
    }

    public void DepStateEnter(Production dep)
    {
        // State change
        if (dep == null || _inDepState) return;
        _inDepState = true;
        _deployment = dep;
        // Select deploy tile
        CivModel.Terrain terrain = GameManager.Instance.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                if (dep.IsPlacable(point))
                {
                    GameManager.Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
                }
            }
            IEnumerator _coroutine = DeployUnit(GameManager.Instance.selectedPoint, dep);
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator DeployUnit(CivModel.Terrain.Point point, Production dep)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            // 새로운 Point 을 선택했을 때
            if (point != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    if (dep.IsPlacable(destPoint))
                    {
                       //여기에 유닛을 생성하는 걸 추가해야 함
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                    else
                    {
                        DepStateExit();
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    DepStateExit();
                }
            }
            yield return null;
        }
    }
    void DepStateExit()
    {
        _inDepState = false;
        _deployment = null;
        CivModel.Terrain terrain = GameManager.Instance.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                GameManager.Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
            }
        }
    }
}
