using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;

public class GameManager : MonoBehaviour {
    private static GameManager _manager = null;
    public static GameManager I { get { return _manager; } }

    // Main Camera for Focus()
    [SerializeField]
    Camera mainCamera;
    // For camera controll
    [SerializeField]
    float cameraMoveSpeed;
    [SerializeField]
    float cameraZoomSpeed;

    // For drawing hex tile
    public float outerRadius = 1f;  // Outer&inner radius of hex tile.
    public float innerRadius;       // These variables can be deleted if there are no use.

    // Hex tile cells
    [SerializeField]
    GameObject cellPrefab;
    private GameObject[,] _cells;
    public GameObject[,] Cells { get { return _cells; } }

    // Current game
    private CivModel.Game _game;
    public CivModel.Game Game { get { return _game; } }
    // Selected actor
    private CivModel.Unit _selectedActor = null;
    public CivModel.Unit SelectedActor { get { return _selectedActor; } }

    // Variables from Presenter.cs
    public bool isThereTodos;
    private CivModel.Unit[] _standbyUnits;
    private int _standbyUnitIndex = -1;

    // Use this for initialization
    void Awake()
    {
        // Singleton
        if (_manager != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _manager = this;
        }
        // Use this when scene changing exists
        // DontDestroyOnLoad(gameObject);
        _game = new CivModel.Game(GameInfo.mapWidth, GameInfo.mapHeight, GameInfo.numOfPlayer, new GameSchemeFactory(), new IGameSchemeFactory[] { new CivModel.AI.GameSchemeFactory()});
        _game.StartTurn();

    }
    void Start() {
        // Instantiate game

        // Map tiling
        innerRadius = outerRadius * Mathf.Sqrt(3.0f) / 2;
        DrawMap();

        ProceedTurn();
    }
	
	// Update is called once per frame
	void Update() {
        Render(_game.Terrain);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Focus();
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.gameObject.GetComponent<HexTile>();

                if (PseudoFSM.I.MoveState)
                {
                    if (tile.isFlickering)
                    {
                        Move(tile.point);
                    }
                }
                if (PseudoFSM.I.DepState)
                {
                    if (tile.isFlickering)
                    {
                        Deploy(tile.point, PseudoFSM.I.Deployment);
                    }
                    else
                        PseudoFSM.I.NormalStateEnter();
                }
                if (PseudoFSM.I.AttackState)
                {
                    if (tile.isFlickering)
                    {
                        if(SelectedActor.HoldingAttackAct != null && SelectedActor.HoldingAttackAct.IsActable(tile.point))
                        {
                            SelectedActor.HoldingAttackAct.Act(tile.point);
                        }
                        else if(SelectedActor.MovingAttackAct != null && SelectedActor.MovingAttackAct.IsActable(tile.point))
                        {
                            SelectedActor.MovingAttackAct.Act(tile.point);
                        }
                        else
                            Debug.Log("잘못된 공격 대상");
                    }
                    else
                        PseudoFSM.I.NormalStateEnter();
                }
                Unit unit = tile.point.Unit;
                if (unit != null)
                { 
                    SelectUnit(unit);
                }
            }
        }

        // Camera movement
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < 10)
        {
            CameraMove(Vector3.left);
        }
        else if (mousePos.x > Screen.width - 10)
        {
            CameraMove(Vector3.right);
        }
        if (mousePos.y < 10)
        {
            CameraMove(Vector3.back);
        }
        else if (mousePos.y > Screen.height - 10)
        {
            CameraMove(Vector3.forward);
        }

        CameraZoom();
    }

    // Instantiate hex tiles
    void DrawMap()
    {
        _cells = new GameObject[GameInfo.mapWidth, GameInfo.mapHeight];

        for (int i = 0; i < GameInfo.mapWidth; i++)
        {
            for (int j = 0; j < GameInfo.mapHeight; j++)
            {
                Vector3 pos = new Vector3(2 * i * innerRadius, -0.05f, -j * outerRadius * 1.5f);
                if (j % 2 != 0)
                {
                    pos.x -= innerRadius;
                }
                _cells[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity);
                _cells[i, j].name = String.Format("({0},{1})", i, j);
                _cells[i, j].GetComponent<HexTile>().point = _game.Terrain.GetPoint(i, j);
            }
        }
    }

    public void BuildBuilding(CivModel.Terrain.Point pos, TileBuilding building)
    {

    }
    // Read game terrain and update hex tile resource
    void Render(CivModel.Terrain terrain)
    {
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                // TODO: Make prefab component
                 _cells[i, j].GetComponent<HexTile>().ChangeTile();
                // _cells[i, j].GetComponent<HexTile>().BuildDistrict(point.TileBuilding);
                _cells[i, j].GetComponent<HexTile>().DrawUnit(point.Unit);
            }
        }
    }

    // Make unit array and iterate while all units consume all AP
    // From Presenter.cs
    public void SelectNextUnit()
    {
        int tryNumber = (_standbyUnitIndex == -1) ? 1 : 2;

        for (int j = 0; j < tryNumber; ++j)
        {
            Debug.Log(_standbyUnitIndex);
            if (_standbyUnitIndex == -1)
            {
                _standbyUnits = _game.PlayerInTurn.Units.ToArray();
            }

            int idx = _standbyUnitIndex + 1;
            Debug.Log(_standbyUnits.Length);
            for (; idx < _standbyUnits.Length; ++idx)
            {
                var unit = _standbyUnits[idx];
                if (unit.RemainAP > 0 && !unit.SkipFlag && unit.PlacedPoint.HasValue)
                {
                    _standbyUnitIndex = idx;
                    _selectedActor = _standbyUnits[idx];
                    isThereTodos = true;
                    Debug.Log(_selectedActor.PlacedPoint);
                    Focus();
                    return;
                }
            }

            _selectedActor = null;
            _standbyUnitIndex = -1;
            isThereTodos = false;
        }
    }
    void SelectUnit(Unit unit)
    {
        Debug.Log(unit.ToString() + " selected");
        var units = _game.PlayerInTurn.Units.ToArray();
        int idx = Array.IndexOf(units, unit);

        if (idx == -1)
            return;

        _selectedActor = unit;
        unit.SkipFlag = false;

        _standbyUnits = units;
        _standbyUnitIndex = idx;
        isThereTodos = true;
        Focus();
    }
    public void ProceedTurn()
    {
        if (_game.IsInsideTurn)
            _game.EndTurn();
        _game.StartTurn();

        SelectNextUnit();
        if (_selectedActor == null)
        {
            if (_game.PlayerInTurn.Cities.FirstOrDefault() is CivModel.CityBase)
            {
                CityBase city = _game.PlayerInTurn.Cities.FirstOrDefault();
                if (city.PlacedPoint is CivModel.Terrain.Point)
                    Focus(city.PlacedPoint.Value);
            }
        }
        PseudoFSM.I.NormalStateEnter();
    }

    // Camera focus
    void Focus()
    {
        Focus(_selectedActor);
    }
    void Focus(CivModel.Unit unit)
    {
        if (unit.PlacedPoint == null)
        {
            return;
        }
        Focus(unit.PlacedPoint.Value);
    }
    void Focus(CivModel.Terrain.Point point)
    {
        GameObject tile = _cells[point.Position.X, point.Position.Y];
        Vector3 tilePos = tile.transform.position;
        float x = tilePos.x;
        float z = tilePos.z - (6.75f / Mathf.Tan(40 * Mathf.Deg2Rad));

        mainCamera.transform.position = new Vector3(x, 6.75f, z);
    }

    // Camera controlls
    void CameraMove(Vector3 vec)
    {
        mainCamera.transform.Translate(vec * cameraMoveSpeed * Time.deltaTime, Space.World);
    }
    void CameraZoom()
    {
        Vector2 vec2 = Input.mouseScrollDelta;
        Vector3 vec3 = new Vector3(vec2.x, 0, vec2.y);
        mainCamera.transform.Translate(vec3 * cameraZoomSpeed * Time.deltaTime, Space.Self);
    } 

    // Move _selectedActor
    void Move(CivModel.Terrain.Point point)
    {
        _selectedActor.MoveAct.Act(point);
        PseudoFSM.I.NormalStateEnter();
    }

    void Deploy(CivModel.Terrain.Point point, Production dep)
    {
        dep.Place(point);
        Game.PlayerInTurn.Deployment.Remove(dep);
        PseudoFSM.I.NormalStateEnter();
    }
}

public static class ProductionFactoryTraits
{
    public static string GetFactoryName(CivModel.IProductionFactory Factory)
    {
        char[] sep = { '.' };
        string name = Factory.ToString().Split(sep)[2];
        string result;
        switch(name)
        {
            case "PioneerProductionFactory":
                result = "개척자";
                break;
            case "JediKnightProductionFactory":
                result = "제다이 기사";
                break;
            case "CityCenterProductionFactory":
                result = "도심부";
                break;
            case "FakeKnightProductionFactory":
                result = "가짜 기사(테스팅)";
                break;
            case "FactoryBuildingProductionFactory":
                result = "공장";
                break;
            case "LaboratoryBuildingProductionFactory":
                result = "연구소";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
    public static string GetName(CivModel.Unit unit)
    {
        char[] sep = { '.' };
        string name = unit.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "Pioneer":
                result = "개척자";
                break;
            case "JediKnight":
                result = "제다이 기사";
                break;
            case "CityCenter":
                result = "도심부";
                break;
            case "FakeKnight":
                result = "가짜 기사(테스팅)";
                break;
            case "FactoryBuilding":
                result = "공장";
                break;
            case "LabortoryBuilding":
                result = "연구소";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
}