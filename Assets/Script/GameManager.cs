using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour {

    // Game Model Instance Singleton
	private CivModel.Game _game;
	public CivModel.Game Game { get { return _game; } }

    // GameManager Class Instance Singleton
	private static GameManager _manager = null;
	public static GameManager Instance { get { return _manager; } }

    // Tiles Instance Singleton
	public GameObject HextilePrefab;
	private GameObject[,] _tiles;
	public GameObject[,] Tiles { get { return _tiles; } }

    private GameObject[,] _additional_tiles;
    public GameObject[,] AdditionalTiles { get { return _additional_tiles; } }

    // Minimap Tiles Instance Singleton
	public GameObject MinimaptilePrefab;
	private GameObject[,] _minimap_tiles;
	public GameObject[,] Minimap_tiles { get { return _minimap_tiles; } }

    // List of Units
	public GameObject UnitPrefab;
	private List<GameObject> _units = new List<GameObject>();
	public List<GameObject> Units { get { return _units; } }
    private List<GameObject> _additional_units = new List<GameObject>();
    public List<GameObject> Additional_Units { get { return _additional_units; } }

    // Deploy State Singleton
    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }

    // Production Model Class Singleton
    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

    // List of Unit Materials. Stored in Unity Editor
    public Material[] materials;

    // Class Variable which stores CivModel.Terrain.Point Component of Selected Point
	public CivModel.Terrain.Point selectedPoint;
    // Class Variable which stores HexTile Component of Selected Tile
    public HexTile selectedTile;

    // Class Variable which stores CivModel.Actor Component of Selected Tile
    // First Click -> Unit
    // Second Click -> Tile
    public CivModel.Actor selectedActor;

    // Class Variable which stores Gameobject of Selected Tile
    // First Click -> Unit
    // Second Click -> Tile
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

    public void UpdateMap()
    {
        for (int i = 0; i < _game.Terrain.Width; i++)
        {
            for (int j = 0; j < _game.Terrain.Height; j++)
            {
                CivModel.Terrain.Point point = _game.Terrain.GetPoint(i, j);
                HexTile tile = _tiles[i, j].GetComponent<HexTile>();
                tile.SetPoints(point);
                tile.SetTerrain();
                tile.SetBuilding();
                // The earth is round.
                HexTile additional_tile = _additional_tiles[i, j].GetComponent<HexTile>();
                Vector3 pos = ModelPntToUnityPnt(point, -0.05f);
                Vector3 ad_pos;
                if (point.Position.X < _game.Terrain.Width / 2)
                    ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                else
                    ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                additional_tile.SetPoints(point, ad_pos);
                additional_tile.SetTerrain();
                additional_tile.SetBuilding();
            }
            // Check if there exists quest that has completed.
            CheckCompletedQuest();
        }
    }

    // Unit Postion Fix, Deletion and Insertion
    public void UpdateUnit() {
        List<GameObject> unitToDelete = new List<GameObject>();
		// POSITION FIX AND DELETETION
		foreach (GameObject unitGameObject in _units) {
            GameObject ad_unitGameObject = _additional_units.Find(x => x.name.Equals("Additional" + unitGameObject.name));
			CivModel.Unit unitModel = unitGameObject.GetComponent<Unit>().unitModel;
			if (unitModel.Owner == null) {
                unitToDelete.Add(unitGameObject);
			}
			else {
				if (unitModel.PlacedPoint.HasValue) {
					var pt = unitModel.PlacedPoint.Value;
					unitGameObject.GetComponent<Unit>().SetPoints(pt);
                    Vector3 pos = ModelPntToUnityPnt(pt, 1.25f);
                    Vector3 ad_pos;
                    if (pt.Position.X < _game.Terrain.Width / 2)
                        ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                    else
                        ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                    ad_unitGameObject.GetComponent<Unit>().SetPoints(pt, ad_pos);
                }
			}
		}
        foreach(GameObject unit in unitToDelete) {
            _units.Remove(unit);
            Destroy(unit);
            GameObject additionalUnitToDelete = _additional_units.Find(x => x.name.Equals("Additional" + unit.name));
            _additional_units.Remove(additionalUnitToDelete);
            Destroy(additionalUnitToDelete);
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
                        unit.name = String.Format("Unit({0},{1})", plyrIdx, untIdx);
                        unit.GetComponent<Unit>().SetPoints(pt, pos);
                        unit.GetComponent<Unit>().unitModel = unt;
                        _units.Add(unit);

                        //The earth is round.
                        Vector3 ad_pos;
                        if (pt.Position.X < _game.Terrain.Width / 2)
                            ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                        else
                            ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                        GameObject ad_unit = Instantiate(UnitPrefab, ad_pos, Quaternion.identity);
                        ad_unit.name = String.Format("AdditionalUnit({0},{1})", plyrIdx, untIdx);
                        ad_unit.GetComponent<Unit>().SetPoints(pt, ad_pos);
                        unit.GetComponent<Unit>().unitModel = unt;
                        _additional_units.Add(ad_unit);
                    }
                }
                
                untIdx++;
            }
            plyrIdx++;
        }
        // VISIBILITY SET
        foreach (GameObject unitGameObject in _units) {
            if ((unitGameObject.GetComponent<Unit>().unitModel is CivModel.Hwan.Spy) || (unitGameObject.GetComponent<Unit>().unitModel is CivModel.Finno.Spy)) {
                if (unitGameObject.GetComponent<Unit>().unitModel.Owner != Game.PlayerInTurn) {
                    if (!IsSpyNear(unitGameObject.GetComponent<Unit>().unitModel.PlacedPoint.Value.Position)) {
                        unitGameObject.GetComponent<Renderer>().enabled = false;
                    }
                }
                else {
                    unitGameObject.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        //The earth is round.   
        foreach (GameObject ad_unitGameObject in _additional_units)
        {
            if (ad_unitGameObject.GetComponent<Unit>().unitModel is CivModel.Hwan.Spy || ad_unitGameObject.GetComponent<Unit>().unitModel is CivModel.Finno.Spy)
            {
                if (ad_unitGameObject.GetComponent<Unit>().unitModel.Owner != Game.PlayerInTurn)
                    if (!IsSpyNear(ad_unitGameObject.GetComponent<Unit>().unitModel.PlacedPoint.Value.Position))
                        ad_unitGameObject.GetComponent<Renderer>().enabled = false;
                    else
                        ad_unitGameObject.GetComponent<Renderer>().enabled = true;
            }
        }
        // Check if there exists action to do to end turn.
        CheckToDo();
        // Check if there exists quest that has completed.
        CheckCompletedQuest();
    }

    bool IsSpyNear(CivModel.Position pt) {
        int A = pt.A;
        int B = pt.B;
        int C = pt.C;
        // in range 1
        if (CheckSpy(A + 1, B - 1, C))
            return true;
        if (CheckSpy(A + 1, B, C - 1))
            return true;
        if (CheckSpy(A, B + 1, C - 1))
            return true;
        if (CheckSpy(A - 1, B + 1, C))
            return true;
        if (CheckSpy(A - 1, B, C + 1))
            return true;
        if (CheckSpy(A, B - 1, C + 1))
            return true;

        // in range 2
        if (CheckSpy(A + 2, B - 2, C))
            return true;
        if (CheckSpy(A + 2, B - 1, C - 1))
            return true;
        if (CheckSpy(A + 2, B, C - 2))
            return true;
        if (CheckSpy(A + 1, B + 1, C - 2))
            return true;
        if (CheckSpy(A, B + 2, C - 2))
            return true;
        if (CheckSpy(A - 1, B + 2, C - 1))
            return true;
        if (CheckSpy(A - 2, B + 2, C))
            return true;
        if (CheckSpy(A - 2, B + 1, C + 1))
            return true;
        if (CheckSpy(A - 2, B, C + 2))
            return true;
        if (CheckSpy(A - 1, B - 1, C + 2))
            return true;
        if (CheckSpy(A, B - 2, C + 2))
            return true;
        if (CheckSpy(A + 1, B - 2, C + 1))
            return true;

        // in range 3
        if (CheckSpy(A + 3, B - 3, C))
            return true;
        if (CheckSpy(A + 3, B - 2, C - 1))
            return true;
        if (CheckSpy(A + 3, B - 1, C - 2))
            return true;
        if (CheckSpy(A + 3, B, C - 3))
            return true;
        if (CheckSpy(A + 2, B + 1, C - 3))
            return true;
        if (CheckSpy(A + 1, B + 2, C - 3))
            return true;
        if (CheckSpy(A, B + 3, C - 3))
            return true;
        if (CheckSpy(A - 1, B + 3, C - 2))
            return true;
        if (CheckSpy(A - 2, B + 3, C - 1))
            return true;
        if (CheckSpy(A - 3, B + 3, C))
            return true;
        if (CheckSpy(A - 3, B + 2, C + 1))
            return true;
        if (CheckSpy(A - 3, B + 1, C + 2))
            return true;
        if (CheckSpy(A - 3, B, C + 3))
            return true;
        if (CheckSpy(A - 2, B - 1, C + 3))
            return true;
        if (CheckSpy(A - 1, B - 2, C + 3))
            return true;
        if (CheckSpy(A, B - 3, C + 3))
            return true;
        if (CheckSpy(A + 1, B - 3, C + 2))
            return true;
        if (CheckSpy(A + 2, B - 3, C + 1))
            return true;

        return false;
    }

    // Check spy by point
    bool CheckSpy(int A, int B, int C){
        if (C < 0 || C >= Game.Terrain.Height)
            return false;
        if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < Game.Terrain.Width) {
            if(Game.Terrain.GetPoint(A,B,C).Unit != null) {
                if (Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Hwan.Spy || Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Finno.Spy) {
                    if (Game.Terrain.GetPoint(A, B, C).Unit.Owner != Game.PlayerInTurn)
                        return true;
                }
            }
        }
        else {
            A = A - Game.Terrain.Width;
            B = B + Game.Terrain.Width;
            if (Game.Terrain.GetPoint(A, B, C).Unit != null)
            {
                if (Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Hwan.Spy || Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Finno.Spy)
                {
                    if (Game.Terrain.GetPoint(A, B, C).Unit.Owner != Game.PlayerInTurn)
                        return true;
                }
            }
        }

        return false;
    }


    // Initialize Tile Map
	private void InitiateMap() {
		_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];
        _additional_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];
		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {

                Vector3 pos = ModelPntToUnityPnt(i, j, -0.05f);

				_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
				_tiles[i, j].name = String.Format("Tile({0},{1})", i, j);
                CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
                _tiles[i, j].GetComponent<HexTile>().SetPoints(pnt, pos);
			}
		}

        // The earth is round.
        for(int i = 0; i < _game.Terrain.Width; i++)
            for(int j = 0; j < _game.Terrain.Height; j++)
            {
                if(i < _game.Terrain.Width / 2)
                {
                    Vector3 pos = ModelPntToUnityPnt(i + _game.Terrain.Width, j, -0.05f);
                    _additional_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
                    _additional_tiles[i, j].name = String.Format("AdditionalTile({0},{1})", i, j);
                    CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
                    _additional_tiles[i, j].GetComponent<HexTile>().SetPoints(pnt, pos);
                }
                else
                {
                    Vector3 pos = ModelPntToUnityPnt(i - _game.Terrain.Width, j, -0.05f);
                    _additional_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
                    _additional_tiles[i, j].name = String.Format("AdditionalTile({0},{1})", i, j);
                    CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
                    _additional_tiles[i, j].GetComponent<HexTile>().SetPoints(pnt, pos);
                }
            }
    }

    // Initialize Minimap Tile
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

    // Initialize Units of Model
	private void InitiateUnit() {
		int plyrIdx = 0;
		foreach (CivModel.Player plyr in Game.Players) {
			int untIdx = 0;
			foreach (CivModel.Unit unt in plyr.Units) {
				if(unt?.PlacedPoint != null) {
					var pt = unt.PlacedPoint.Value;
					Vector3 pos = ModelPntToUnityPnt(pt, 1.25f);
                    
                    Vector3 ad_pos;
                    if (pt.Position.X < _game.Terrain.Width / 2)
                        ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                    else
                        ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                    
					GameObject unit = Instantiate(UnitPrefab, pos, Quaternion.identity);
                    GameObject additional_unit = Instantiate(UnitPrefab, ad_pos, Quaternion.identity);
					unit.name = String.Format("Unit({0},{1})", plyrIdx, untIdx);
                    additional_unit.name = String.Format("AdditionalUnit({0},{1})", plyrIdx, untIdx);
					unit.GetComponent<Unit>().SetPoints(pt, pos);
                    additional_unit.GetComponent<Unit>().SetPoints(pt, ad_pos);
                    unit.GetComponent<Unit>().unitModel = unt;
                    additional_unit.GetComponent<Unit>().unitModel = unt;
					_units.Add(unit);
                    _additional_units.Add(additional_unit);
				}
				untIdx++;
			}
			plyrIdx++;
		}
	}

    // Initialize Turn Attributes of Model
    // Default: Player[0] is User and the others are AI
    private void InitiateTurn() {
        _game.EndTurn();
        _game.StartTurn();

        foreach (Player plyr in _game.Players)
        {
            plyr.IsAIControlled = true;
        }
        _game.Players[0].IsAIControlled = false;

        // Finno 플레이시 주석 해제 (GameUI.cs 도 같이 수정 요망)
        // _game.Players[1].IsAIControlled = false;

    }

    // Check if there exists action to do to end turn.
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

    // Input: CivModel.Terrain.Point and Position of Y
    // Output: New Vector3 Instance for unity transform of given Input
    public static Vector3 ModelPntToUnityPnt(CivModel.Terrain.Point pt, float yPos) {
        Vector3 unityPoint = new Vector3(2 * pt.Position.X * (Mathf.Sqrt(3) / 2), yPos, -pt.Position.Y * 1.5f);
        if ((pt.Position.Y % 2) != 0)
            unityPoint.x -= (Mathf.Sqrt(3) / 2);
        
        return unityPoint;
    }

    // Input: Logical Coordinate X, Y and Position of Y
    // Output: New Vector3 Instance for unity transform of given Input
    public static Vector3 ModelPntToUnityPnt(int i, int j, float yPos)
    {
        Vector3 unityPoint = new Vector3(2 * i * (Mathf.Sqrt(3) / 2), yPos, -j * 1.5f);
        if ((j % 2) != 0)
            unityPoint.x -= (Mathf.Sqrt(3) / 2);

        return unityPoint;
    }
    // Get a GameObject of the given CivModel.Terrain.Point which represents a Unit
    // Null if unit does not exist
    public static GameObject GetUnitGameObject(CivModel.Terrain.Point point) {
		foreach (GameObject unt in Instance.Units) {
            Unit unit = unt.GetComponent<Unit>();
			if (unit.point == point) {
                return unt;
			}
		}
        return null;
	}

    // Focus Main Camera to given CivModel.Actor
    static void Focus(CivModel.Actor actor)
    {
        if (actor.PlacedPoint == null)
        {
            return;
        }
        Focus(actor.PlacedPoint.Value);
    }

    // Focus Main Camera to given CivModel.Terrain.Point
    static void Focus(CivModel.Terrain.Point point)
    {
        Vector3 tilePos = GameManager.Instance.Tiles[point.Position.X, point.Position.Y].transform.position;
        float x = tilePos.x;
        float z = tilePos.z - (Camera.main.transform.position.y / Mathf.Tan(60 * Mathf.Deg2Rad));

        Camera.main.transform.position = new Vector3(x, Camera.main.transform.position.y, z);
    }

    // Check a Unit which needs action and focus the main camera onto it.
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

    // Called When a Player about to Deploy Something
    // dep is a Production to deploy
    // deployprefab is a prefab of Production dep
    public void DepStateEnter(Production dep, DeployPrefab deployprefab)
    {
        // State change
        if (dep == null || _inDepState) return;
        _inDepState = true;
        _deployment = dep;

        // Represent Tiles which are available to place Actor
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

    // Exiting Deploy State
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

    // Check if there exist quest that have finished.
    public void CheckCompletedQuest()
    {
        List<Quest> AlarmedQuests = new List<Quest>();
        foreach (Quest qst in GameManager.Instance.Game.PlayerInTurn.Quests)
        {
            switch (qst.Status)
            {
                case QuestStatus.Completed:
                    if (!AlarmedQuests.Contains(qst))
                    {
                        Sprite questPortrait = QuestInfo.GetPortraitImage(qst);
                        AlarmManager.Instance.AddAlarm(questPortrait,
                                                       qst.Name + " 완료됨",
                                                       delegate {
                                                           UIManager.Instance.mapUI.SetActive(false);
                                                           UIManager.Instance.questUI.SetActive(true);
                                                       },
                                                       0);
                        AlarmedQuests.Add(qst);
                    }
                    break;
            }
        }
    }

    // Check if there exist production that have finished.
    public void CheckCompletedProduction()
    {
        List<Production> AlarmedProduction = new List<Production>();

        foreach (Production prod in GameManager.Instance.Game.PlayerInTurn.Deployment)
        {
            if (!AlarmedProduction.Contains(prod))
            {
                Sprite prodPortrait = Resources.Load<Sprite>("Portraits/" + ProductionFactoryTraits.GetFacPortName(prod.Factory));
                AlarmManager.Instance.AddAlarm(prodPortrait,
                                               ProductionFactoryTraits.GetFactoryName(prod.Factory) + " 배치 가능",
                                               delegate {
                                                   UIManager.Instance.mapUI.SetActive(false);
                                                   UIManager.Instance.managementUI.SetActive(true);
                                               },
                                               0);
                AlarmedProduction.Add(prod);
            }
        }

        // If you do not want to re-alarm production, Comment this line.
        AlarmedProduction.Clear();
    }


}