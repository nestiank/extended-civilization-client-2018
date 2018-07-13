using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour {

	private CivModel.Game _game;
	public CivModel.Game Game { get { return _game; } }

	private static GameManager _manager = null;
	public static GameManager Instance { get { return _manager; } }

	//public float outerRadius = 1f;
	//public float innerRadius = Mathf.Sqrt(3) / 2;

	public GameObject HextilePrefab;
	private GameObject[,] _tiles;
	public GameObject[,] Tiles { get { return _tiles; } }

	public GameObject MinimaptilePrefab;
	private GameObject[,] _minimap_tiles;
	public GameObject[,] Minimap_tiles { get { return _minimap_tiles; } }

	public GameObject UnitPrefab;
	private List<GameObject> _units = new List<GameObject>();
	public List<GameObject> Units { get { return _units; } }

    //Deploy를 위한 추가

    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }

    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

    ///Deploy를 위한 추가 끝

    public Material[] materials;

	public CivModel.Terrain.Point selectedPoint;
    public HexTile selectedTile;

    public CivModel.Actor selectedActor;
    public GameObject selectedGameObject;

    public bool isThereTodos = false;

	void Awake() {
		// Singleton
		if (_manager != null) {
			Destroy(gameObject);
			return;
		}
		else {
			_manager = this;
		}

		var factories = new IGameSchemeFactory[]
		{
			new CivModel.Common.GameSchemeFactory(),
			new CivModel.Hwan.GameSchemeFactory(),
			new CivModel.Finno.GameSchemeFactory(),
			new CivModel.Quests.GameSchemeFactory(),
			new CivModel.Zap.GameSchemeFactory(),
			new CivModel.AI.GameSchemeFactory()
		};
        string[] pathStr = { "Assets", "map.txt" };
        string path = Path.Combine(pathStr);
		_game = new CivModel.Game(path, factories);
		_game.StartTurn();

	}

	// Use this for initialization
	void Start() {
		InitiateMap();
		InitiateUnit();
		InitiateMiniMap();
        InitiateTurn();
        CheckToDo();
	}

	// Update is called once per frame
	void Update() {

	}

	public void UpdateMinimap() {
		for (int i = 0; i<_game.Terrain.Width; i++) {
			for(int j = 0; j<_game.Terrain.Height; j++) {
				CivModel.Terrain.Point point = _game.Terrain.GetPoint(i, j);
				MinimapTile tile = _minimap_tiles[i, j].GetComponent<MinimapTile>();
				tile.SetPoints(point);
				tile.SetCity();
				tile.SetOwner();
			}
		}
	}

	public void UpdateMap() {
		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {
				CivModel.Terrain.Point point = _game.Terrain.GetPoint(i, j);
				HexTile tile = _tiles[i, j].GetComponent<HexTile>();
                tile.SetPoints(point);
				tile.SetTerrain();
				tile.SetBuilding();
			}
		}
	}

    public void UpdateUnit() {
        List<GameObject> unitToDelete = new List<GameObject>();

		// POSITION FIX AND DELETETION
		foreach (GameObject unitGameObject in _units) {
			CivModel.Unit unitModel = unitGameObject.GetComponent<Unit>().unitModel;
			if (unitModel.Owner == null) {
                unitToDelete.Add(unitGameObject);
			}
			else {
				if (unitModel.PlacedPoint.HasValue) {
					var pt = unitModel.PlacedPoint.Value;
					unitGameObject.GetComponent<Unit>().SetPoints(pt);
				}
			}
		}

        foreach(GameObject unit in unitToDelete) {
            _units.Remove(unit);
            Destroy(unit);
            Debug.Log("Deleted!");
        }

        unitToDelete.Clear();

		// INSERTION
		int plyrIdx = 0;
        foreach (CivModel.Player plyr in Game.Players) {
            int untIdx = 0;
            foreach (CivModel.Unit unt in plyr.Units) {
                bool isExist = false;
                foreach (GameObject unitGameObject in _units) {
                    CivModel.Unit unitModel = unitGameObject.GetComponent<Unit>().unitModel;
                    if (unt == unitModel) {
                        isExist = true;
                    }
                }
                if (isExist == false) {
                    if (unt?.PlacedPoint != null) {
                        var pt = unt.PlacedPoint.Value;
                        Vector3 pos = ModelPntToUnityPnt(pt, 1.25f);
                        GameObject unit = Instantiate(UnitPrefab, pos, Quaternion.identity);
                        unit.name = String.Format("[{0},{1}]", plyrIdx, untIdx);
                        unit.GetComponent<Unit>().SetPoints(pt, pos);
                        unit.GetComponent<Unit>().unitModel = unt;
                        _units.Add(unit);
                    }
                }
                untIdx++;
            }
            plyrIdx++;
        }
        CheckToDo();
    }

	private void InitiateMap() {
		_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];

		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {

                Vector3 pos = ModelPntToUnityPnt(i, j, -0.05f);

				_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
				_tiles[i, j].name = String.Format("({0},{1})", i, j);
                CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
                _tiles[i, j].GetComponent<HexTile>().SetPoints(pnt, pos);
			}
		}
	}

	private void InitiateMiniMap() {
		_minimap_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];

		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {

				Vector3 pos = ModelPntToUnityPnt(i, j, -200f);

				_minimap_tiles[i, j] = Instantiate(MinimaptilePrefab, pos, Quaternion.identity);
				_minimap_tiles[i, j].name = String.Format("Minimap({0},{1})", i, j);
				CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
				_minimap_tiles[i, j].GetComponent<MinimapTile>().SetPoints(pnt, pos);
			}
		}
	}

	private void InitiateUnit() {
		int plyrIdx = 0;
		foreach (CivModel.Player plyr in Game.Players) {
			int untIdx = 0;
			foreach (CivModel.Unit unt in plyr.Units) {
				if(unt?.PlacedPoint != null) {
					var pt = unt.PlacedPoint.Value;
					Vector3 pos = ModelPntToUnityPnt(pt, 1.25f);
					GameObject unit = Instantiate(UnitPrefab, pos, Quaternion.identity);
					unit.name = String.Format("[{0},{1}]", plyrIdx, untIdx);
					unit.GetComponent<Unit>().SetPoints(pt, pos);
                    unit.GetComponent<Unit>().unitModel = unt;
					_units.Add(unit);
				}
				untIdx++;
			}
			plyrIdx++;
		}
	}

    private void InitiateTurn() {
        _game.EndTurn();
        _game.StartTurn();

        foreach (Player plyr in _game.Players)
        {
            plyr.IsAIControlled = true;
        }
        _game.Players[0].IsAIControlled = false;

    }

    public void CheckToDo()
    {
        isThereTodos = false;
        // Only For Testing!
        //foreach (CivModel.Unit unit in this.Game.PlayerInTurn.Units)
        //{
        //    if(!unit.RemainAP.Equals(0)) {
        //        if(unit.SkipFlag == false) {
        //            isThereTodos = true;
        //            break;
        //        }
        //    }
        //}
    }


    public static Vector3 ModelPntToUnityPnt(CivModel.Terrain.Point pt, float yPos) {
        Vector3 unityPoint = new Vector3(2 * pt.Position.X * (Mathf.Sqrt(3) / 2), yPos, -pt.Position.Y * 1.5f);
        if ((pt.Position.Y % 2) != 0)
            unityPoint.x -= (Mathf.Sqrt(3) / 2);
        
        return unityPoint;
    }
    public static Vector3 ModelPntToUnityPnt(int i, int j, float yPos)
    {
        Vector3 unityPoint = new Vector3(2 * i * (Mathf.Sqrt(3) / 2), yPos, -j * 1.5f);
        if ((j % 2) != 0)
            unityPoint.x -= (Mathf.Sqrt(3) / 2);

        return unityPoint;
    }

    public static GameObject GetUnitGameObject(CivModel.Terrain.Point point) {
		foreach (GameObject unt in Instance.Units) {
            Unit unit = unt.GetComponent<Unit>();
			if (unit.point == point) {
                return unt;
			}
		}
        return null;
	}

    static void Focus(CivModel.Actor actor)
    {
        if (actor.PlacedPoint == null)
        {
            return;
        }
        Focus(actor.PlacedPoint.Value);
    }

    static void Focus(CivModel.Terrain.Point point)
    {
        Vector3 tilePos = GameManager.Instance.Tiles[point.Position.X, point.Position.Y].transform.position;
        float x = tilePos.x;
        float z = tilePos.z - (Camera.main.transform.position.y / Mathf.Tan(60 * Mathf.Deg2Rad));

        Camera.main.transform.position = new Vector3(x, Camera.main.transform.position.y, z);
    }

    public void FocusOnActableUnit()
    {
        foreach (CivModel.Unit unit in GameManager.Instance.Game.PlayerInTurn.Units)
        {
            if (unit.SkipFlag == false)
            {
                if(!unit.RemainAP.Equals(0))
                {
                    Focus(unit);
                    UIManager.Instance.updateSelectedInfo(unit);
                }
            }
        }
    }

    // Deploy 하는 부분
    public void DepStateEnter(Production dep, DeployPrefab deployprefab)
    {
        // State change
        if (dep == null || _inDepState) return;
        _inDepState = true;
        _deployment = dep;
        // Select deploy tile
        CivModel.Terrain terrain = Instance.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                if (dep.IsPlacable(point))
                {
                   Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
                }
            }
        }
        CivModel.Terrain.Point StartPoint = Instance.selectedPoint;
        IEnumerator _coroutine = DeployUnit(StartPoint, dep, deployprefab);
        StartCoroutine(_coroutine);
    }

    IEnumerator DeployUnit(CivModel.Terrain.Point point, Production dep, DeployPrefab deployprefab)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = Instance.selectedPoint;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (Instance.selectedTile.isFlickering)
                {
                    if (dep.IsPlacable(destPoint))
                    {
                        Game.PlayerInTurn.Deployment.Remove(dep);
                        dep.Place(destPoint);
                        DepStateExit(deployprefab);
                        GameManager.Instance.UpdateUnit();
                        GameManager.Instance.UpdateMap();
                        break;
                    }
                    else
                    {
                        DepStateExit(deployprefab);
                        break;
                    }
                }
                DepStateExit(deployprefab);
                break;
            }
            yield return null;
        }
    }



    void DepStateExit(DeployPrefab deployprefab)
    {
        _inDepState = false;
        _deployment = null;
        CivModel.Terrain terrain = Instance.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
            }
        }
        GameManager.Instance.UpdateUnit();
        GameManager.Instance.UpdateMap();
    }

}