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
    // Camera move speed
    [SerializeField]
    float cameraMoveSpeed;
    enum CameraMoveDirection { Up, Down, Left, Right }

    // Current game
    private CivModel.Game _game;
    // Selected actor
    private CivModel.Unit _selectedActor = null;

    // For unit order
    public bool isMoveState = false;
    public bool isAttackState = false;
    public bool isSkillState = false;

    // Variables from Presenter.cs
    public bool IsThereTodos;
    private CivModel.Unit[] _standbyUnits;
    private int _standbyUnitIndex = -1;

    public float outerRadius = 1f;  // Outer&inner radius of hex tile.
    public float innerRadius;       // These variables can be deleted if there are no use.

    // Hex tile cells
    public GameObject cellPrefab;
    private GameObject[,] _cells;

    // Use this for initialization
    void Start() {
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

        // Instantiate game
        _game = new CivModel.Game( GameInfo.mapWidth, GameInfo.mapHeight, GameInfo.numOfPlayer, new CivModel.Common.GameSchemeFactory());
        _game.StartTurn();
        Debug.Log(_game.PlayerNumberInTurn);

        // Map tiling
        innerRadius = outerRadius * Mathf.Sqrt(3.0f) / 2;
        DrawMap();

        SelectNextUnit();
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
                Unit unit = hit.collider.gameObject.GetComponent<HexTile>().point.Unit;
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
            CameraMove(CameraMoveDirection.Left);
        }
        else if (mousePos.x > Screen.width - 10)
        {
            CameraMove(CameraMoveDirection.Right);
        }
        if (mousePos.y < 10)
        {
            CameraMove(CameraMoveDirection.Down);
        }
        else if (mousePos.y > Screen.height - 10)
        {
            CameraMove(CameraMoveDirection.Up);
        }
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
                _cells[i, j].name = Pos2Str(i, j);
                _cells[i, j].GetComponent<HexTile>().point = _game.Terrain.GetPoint(i, j);
            }
        }
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
            Debug.Log(_standbyUnits);
            for (; idx < _standbyUnits.Length; ++idx)
            {
                var unit = _standbyUnits[idx];
                if (unit.RemainAP > 0 && !unit.SkipFlag && unit.PlacedPoint.HasValue)
                {
                    _standbyUnitIndex = idx;
                    _selectedActor = _standbyUnits[idx];
                    IsThereTodos = true;
                    Debug.Log(_selectedActor.PlacedPoint);
                    Focus();
                    return;
                }
            }

            _selectedActor = null;
            _standbyUnitIndex = -1;
            IsThereTodos = false;
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
        IsThereTodos = true;
        Focus();
    }

    // Camera focus
    void Focus()
    {
        Focus(_selectedActor);
    }
    void Focus(CivModel.Unit unit)
    {
        if (unit == null)
        {
            return;
        }
        Focus(unit.PlacedPoint.Value);
    }
    void Focus(CivModel.Terrain.Point point)
    {
        GameObject tile = GameObject.Find(Pos2Str(point.Position));
        Vector3 tilePos = tile.transform.position;
        float x = tilePos.x;
        float z = tilePos.z - (6.75f / Mathf.Tan(40 * Mathf.Deg2Rad));

        mainCamera.transform.position = new Vector3(x, 6.75f, z);
    }

    // Method that gives "(x,y)" string with input of CivModel.Position or 2 ints
    string Pos2Str(CivModel.Position pos)
    {
        return Pos2Str(pos.X, pos.Y);
    }
    string Pos2Str(int x, int y)
    {
        return "(" + x + "," + y + ")";
    }

    // Camera move
    void CameraMove(CameraMoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case CameraMoveDirection.Up:
                mainCamera.transform.Translate(Vector3.forward * cameraMoveSpeed * Time.deltaTime, Space.World);
                break;
            case CameraMoveDirection.Down:
                mainCamera.transform.Translate(Vector3.back * cameraMoveSpeed * Time.deltaTime, Space.World);
                break;
            case CameraMoveDirection.Left:
                mainCamera.transform.Translate(Vector3.left * cameraMoveSpeed * Time.deltaTime, Space.World);
                break;
            case CameraMoveDirection.Right:
                mainCamera.transform.Translate(Vector3.right * cameraMoveSpeed * Time.deltaTime, Space.World);
                break;
        }
    }
}
