using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.EventSystems;
using System.Linq;


public class GameManager : MonoBehaviour
{

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
    // CivModel.Unit, Unit, Additional Unit dictionary
    public Dictionary<CivModel.Unit, KeyValuePair<Unit, Unit>> UnitDic;

    // Deploy State Singleton
    public GameObject DeployRay;
    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }

    // Production Model Class Singleton
    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

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

    private Actor[] _standbyActors = null;
    private int _standbyActorIndex = -1;

    // Indicates whether Maptile or AdditionalMapTile is clicked.
    public bool isAdClicked = true;

    public Camera minimap_camera;

    static List<Quest> NewQuestQueue = new List<Quest>();
    static List<Quest> CompletedQuestsQueue = new List<Quest>();
    static List<String> AlarmedProduction = new List<String>();

    private ManagementController managementcontroller;

    public GameObject QuestVoice;
    public GameObject DoYouKnows;
    public GameObject BGMs;
    public GameObject HolySound;
    void EstimateResource() {

    }

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
			new CivModel.Hwan.GameSchemeFactory(),
			new CivModel.Finno.GameSchemeFactory(),
			new CivModel.Quests.GameSchemeFactory(),
			new CivModel.Zap.GameSchemeFactory(),
			new CivModel.AI.GameSchemeFactory()
		};

        string mapFile = Application.streamingAssetsPath + "/mapx.75.txt";
        Debug.Log(mapFile);

        string[] protoFiles =
        {
            Application.streamingAssetsPath + "/package/finno/package.xml",
            Application.streamingAssetsPath + "/package/hwan/package.xml",
            Application.streamingAssetsPath + "/package/zap/package.xml",
            Application.streamingAssetsPath + "/package/quests/package.xml",
        };
        var protoArray = protoFiles.Select(x => File.OpenText(x)).ToArray();

        _game = new CivModel.Game(mapFile, protoArray, factories);
        _game.StartTurn();

        InitiateTurn();

        InitiateMiniMap();
        InitiateMap();
        InitiateUnit();
        InitiateMinimapCamera();
        UnitAnimation.AnimationParticleObjectPool();
    }

	// Use this for initialization
	void Start() {
        CheckToDo();
        managementcontroller = ManagementController.GetManagementController();
        GameUI.Instance.updatePanel();
		Focus(_game.PlayerInTurn.Cities.First());
	}
    void Update()
    {
        if(minimap_camera.gameObject.activeSelf)
        {
            minimap_camera.gameObject.SetActive(false);
            minimap_camera.Render();
        }
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
        minimap_camera.Render();
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

        GameUI.Instance.updatePanel();

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
                    Vector3 pos = ModelPntToUnityPnt(pt, 0);
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
            int unitOwnerNumber = unit.GetComponent<Unit>().unitOwnerNumber;

            if (unitOwnerNumber == _game.Players[GameInfo.UserPlayer].PlayerNumber)
            {
                CivModel.Unit unitModel = unit.GetComponent<Unit>().unitModel;
                AlarmManager.Instance.AddAlarm(
                    Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(unitModel)).ToLower()), typeof(Sprite)) as Sprite,
                    ProductionFactoryTraits.GetName(unitModel) + " 파괴됨",
                    null,
                    0,
                    true);
            }
            UnitDic.Remove(unit.GetComponent<Unit>().unitModel);
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
                        Vector3 pos = ModelPntToUnityPnt(pt, 0);

                        UnitPrefab = UnitEnum.GetUnitGameObject(unt);

                        GameObject unit = Instantiate(UnitPrefab, pos, Quaternion.identity);
                        unit.AddComponent<Unit>();

                        unit.name = String.Format("Unit({0},{1})", plyrIdx, untIdx);
                        unit.GetComponent<Unit>().unitModel = unt;
                        unit.GetComponent<Unit>().SetPoints(pt, pos);
                        unit.GetComponent<Unit>().unitOwnerNumber = unt.Owner.PlayerNumber;
                        _units.Add(unit);
                        unit.GetComponent<Unit>().unitModel.SkipFlag = true;

                        //The earth is round.
                        Vector3 ad_pos;
                        if (pt.Position.X < _game.Terrain.Width / 2)
                            ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                        else
                            ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                        GameObject ad_unit = Instantiate(UnitPrefab, ad_pos, Quaternion.identity);
                        ad_unit.AddComponent<Unit>();
                        ad_unit.name = String.Format("AdditionalUnit({0},{1})", plyrIdx, untIdx);
                        ad_unit.GetComponent<Unit>().unitModel = unt;
                        ad_unit.GetComponent<Unit>().SetPoints(pt, ad_pos);
                        ad_unit.GetComponent<Unit>().unitOwnerNumber = unt.Owner.PlayerNumber;
                        _additional_units.Add(ad_unit);
                        ad_unit.GetComponent<Unit>().unitModel.SkipFlag = true;
                        UnitDic.Add(unt, new KeyValuePair<Unit, Unit>
                        (unit.GetComponent<Unit>(), ad_unit.GetComponent<Unit>()));
                        unit.GetComponent<Unit>().pairUnit = ad_unit.GetComponent<Unit>();
                        ad_unit.GetComponent<Unit>().pairUnit = unit.GetComponent<Unit>();
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
                        foreach (Renderer r in unitGameObject.GetComponentsInChildren<Renderer>(true))
                            r.enabled = false;

                       // unitGameObject.GetComponent<Renderer>().enabled = false;
                    }
                }
                else {
                    foreach (Renderer r in unitGameObject.GetComponentsInChildren<Renderer>(true))
                        r.enabled = true;
                    //unitGameObject.GetComponent<Renderer>().enabled = true;
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
        // Update Unit Info
        UIManager.Instance.UpdateUnitInfo();
        // Update Quest Queue
        UIController.GetUIController().MakeQuestQueue();

        GameUI.Instance.updatePanel();
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
        UnitDic =  new Dictionary<CivModel.Unit, KeyValuePair<Unit, Unit>>();
		foreach (CivModel.Player plyr in Game.Players) {
			int untIdx = 0;
			foreach (CivModel.Unit unt in plyr.Units) {
				if(unt?.PlacedPoint != null) {
					var pt = unt.PlacedPoint.Value;
					Vector3 pos = ModelPntToUnityPnt(pt, 0);
                    
                    Vector3 ad_pos;
                    if (pt.Position.X < _game.Terrain.Width / 2)
                        ad_pos = new Vector3(pos.x + Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);
                    else
                        ad_pos = new Vector3(pos.x - Mathf.Sqrt(3) * _game.Terrain.Width, pos.y, pos.z);

                    UnitPrefab = UnitEnum.GetUnitGameObject(unt);

					GameObject unit = Instantiate(UnitPrefab, pos, Quaternion.identity);
                    GameObject additional_unit = Instantiate(UnitPrefab, ad_pos, Quaternion.identity);

                    unit.AddComponent<Unit>();
                    additional_unit.AddComponent<Unit>();

                    unit.name = String.Format("Unit({0},{1})", plyrIdx, untIdx);
                    additional_unit.name = String.Format("AdditionalUnit({0},{1})", plyrIdx, untIdx);
                    unit.GetComponent<Unit>().unitModel = unt;
                    additional_unit.GetComponent<Unit>().unitModel = unt;
					unit.GetComponent<Unit>().SetPoints(pt, pos);
                    additional_unit.GetComponent<Unit>().SetPoints(pt, ad_pos);
                    unit.GetComponent<Unit>().unitOwnerNumber = unt.Owner.PlayerNumber;
                    additional_unit.GetComponent<Unit>().unitOwnerNumber = unt.Owner.PlayerNumber;
					_units.Add(unit);
                    _additional_units.Add(additional_unit);
                    UnitDic.Add( unt, new KeyValuePair<Unit, Unit>
                        (unit.GetComponent<Unit>(), additional_unit.GetComponent<Unit>()) );
                    unit.GetComponent<Unit>().pairUnit = additional_unit.GetComponent<Unit>();
                    additional_unit.GetComponent<Unit>().pairUnit = unit.GetComponent<Unit>();
                }
				untIdx++;
			}
			plyrIdx++;
		}
    }

    private void InitiateMinimapCamera()
    {
        minimap_camera.orthographicSize = 58.3f * _game.Terrain.Width / 128;
        minimap_camera.transform.position = new Vector3(110 * _game.Terrain.Width / 128, -55, -60 * _game.Terrain.Height / 80);
    }
    // Initialize Turn Attributes of Model
    // Default: Player[0] is User and the others are AI
    private void InitiateTurn() {

        // 8번 플레이어 부터 시작하기 때문에 한번 해줌
        _game.EndTurn();
        _game.StartTurn();

        foreach (Player plyr in _game.Players)
        {
            plyr.IsAIControlled = true;
        }
        _game.Players[GameInfo.UserPlayer].IsAIControlled = false;

        // Proceeds AI's Turns
        while (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        {
            // Debug.Log(GameManager.Instance.Game.PlayerNumberInTurn);
            GameManager.Instance.Game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
            GameManager.Instance.Game.EndTurn();
            GameManager.Instance.Game.StartTurn();
        }

    }

    // Check if there exists action to do to end turn.
    public void CheckToDo()
    {
        isThereTodos = false;
        
        foreach (CivModel.Unit unit in this.Game.PlayerInTurn.Units)
            if (!unit.RemainAP.Equals(0) && !unit.SkipFlag)
            {
                isThereTodos = true;
                return;
            }
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
    public static void Focus(CivModel.Actor actor)
    {
        if (actor.PlacedPoint == null)
        {
            return;
        }
        Focus(actor.PlacedPoint.Value);
    }

    // Focus Main Camera to given CivModel.Terrain.Point
    public static void Focus(CivModel.Terrain.Point point)
    {
        Vector3 tilePos = GameManager.Instance.Tiles[point.Position.X, point.Position.Y].transform.position;
        float x = tilePos.x;
        float z = tilePos.z - (Camera.main.transform.position.y / Mathf.Tan(45 * Mathf.Deg2Rad));

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
    public void FocusOnNextActableUnit()
    {
        int tryNumber = (GameManager.Instance._standbyActorIndex == -1) ? 1 : 2;

        for (int j = 0; j < tryNumber; ++j)
        {
            if (GameManager.Instance._standbyActorIndex == -1)
            {
                GameManager.Instance._standbyActors = Game.PlayerInTurn.Actors.ToArray();
            }

            int idx = GameManager.Instance._standbyActorIndex + 1;
            for (; idx < GameManager.Instance._standbyActors.Length; ++idx)
            {
                var actor = GameManager.Instance._standbyActors[idx];
                if (actor is CivModel.Unit && actor.RemainAP > 0 && !actor.SkipFlag
                    && actor.IsControllable && actor.PlacedPoint.HasValue
                    && actor.MovePath == null)
                {
                    GameManager.Instance._standbyActorIndex = idx;
                    GameManager.Instance.selectedActor = _standbyActors[idx];
                    GameManager.Instance.isThereTodos = true;
                    Focus(GameManager.Instance.selectedActor);
                    UIManager.Instance.updateSelectedInfo(GameManager.Instance.selectedActor);
                    return;
                }
            }

            GameManager.Instance.selectedActor = null;
            GameManager.Instance._standbyActorIndex = -1;
            GameManager.Instance.isThereTodos = false;
        }
    }

    public void SelectActor(Actor actor)
    {
        var actors = Game.PlayerInTurn.Actors.ToArray();
        int idx = Array.IndexOf(actors, actor);

        if (idx == -1)
        {
            GameManager.Instance.selectedActor = actor;
            Focus(GameManager.Instance.selectedActor);
        }
        if (!actor.IsControllable)
            return;

        GameManager.Instance.selectedActor = actor;
        actor.SkipFlag = false;

        _standbyActors = actors;
        _standbyActorIndex = idx;
        GameManager.Instance.isThereTodos = true;
        Focus(GameManager.Instance.selectedActor);
    }

    // Called When a Player about to Deploy Something
    // dep is a Production to deploy
    // deployprefab is a prefab of Production dep
    public void DepStateEnter(List<Production> depList)
    {
        // State change
        if (depList.First() == null || _inDepState) return;
        _inDepState = true;
        List<GameObject> PlaceablePoints = new List<GameObject>();
        // Represent Tiles which are available to place Actor
        CivModel.Terrain terrain = Instance.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                if (depList.First().IsPlacable(point))
                {
                    GameObject tile = Instance.Tiles[point.Position.X, point.Position.Y];
                    GameObject additionaltile = Instance.AdditionalTiles[point.Position.X, point.Position.Y];
                    PlaceablePoints.Add(tile);
                    PlaceablePoints.Add(additionaltile);
                    Instantiate(DeployRay, tile.transform);
                    Instantiate(DeployRay, additionaltile.transform);
                    tile.GetComponent<HexTile>().isFlickerForSelect = true;
                    additionaltile.GetComponent<HexTile>().isFlickerForSelect = true;
                    //Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
                    //Instance.AdditionalTiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
                }
            }
        }

        CivModel.Terrain.Point StartPoint = Instance.selectedPoint;
        IEnumerator _coroutine = DeployUnit(StartPoint, depList, PlaceablePoints);
        StartCoroutine(_coroutine);
    }

    IEnumerator DeployUnit(CivModel.Terrain.Point point, List<Production> depList, List<GameObject> PlaceablePoints)
    {
        while (true)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                if (Instance.selectedTile == null)
                    yield return new WaitUntil(() => Instance.selectedTile != null);
                
                CivModel.Terrain.Point destPoint = Instance.selectedPoint;

                // Flicker하고 있는 Tile을 선택했을 때
                if (Instance.selectedTile.isFlickerForSelect)
                {
                    Debug.Log("flicker");
                    if (depList.First().IsPlacable(destPoint))
                    {
                        Debug.Log("deploy");
                        foreach (Production dep in depList)
                        {
                            Game.PlayerInTurn.Deployment.Remove(dep);
                            dep.Place(destPoint);
                        }
                        DepStateExit(PlaceablePoints);
                        GameManager.Instance.UpdateUnit();
                        GameManager.Instance.UpdateMap();
                        break;
                    }
                    else
                    {
                        Debug.Log("no");
                        DepStateExit(PlaceablePoints);
                        break;
                    }
                }
                DepStateExit(PlaceablePoints);
                break;
            }
            yield return null;
        }
    }

    // Exiting Deploy State
    void DepStateExit(List<GameObject> PlaceablePoints)
    {
        _inDepState = false;
        _deployment = null;

        foreach (GameObject Point in PlaceablePoints)
        {
            int count = Point.transform.childCount;
            if (count > 2)
            {
                Point.GetComponent<HexTile>().isFlickerForSelect = true;
                Destroy(Point.transform.Find("DeployRay(Clone)").gameObject);
            }
        }
        
        /*
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                
                Instance.Tiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
                Instance.AdditionalTiles[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
            }
        }*/

        managementcontroller.MakeProductionQ();
        managementcontroller.MakeDeploymentQ();
        GameManager.Instance.UpdateUnit();
        GameManager.Instance.UpdateMap();
    }

    // Check if there exists new quest to alarm
    public void CheckNewQuest()
    {
        foreach (Quest qst in GameManager.Instance.Game.PlayerInTurn.Quests)
        {
            switch (qst.Status)
            {
                case QuestStatus.Deployed:
                    if (!NewQuestQueue.Contains(qst))
                    {
                        Sprite questPortrait = QuestInfo.GetRequesterPortraitImage(qst);
                        AlarmManager.Instance.AddAlarm(questPortrait,
                                                       qst.TextName + " 시작가능",
                                                       delegate {
                                                           UIManager.Instance.mapUI.SetActive(false);
                                                           UIManager.Instance.questUI.SetActive(true);
                                                       },
                                                       0);
                        NewQuestQueue.Add(qst);
                    }
                    break;
            }
        }
    }

    // Check if there exist quests that have finished.
    public void CheckCompletedQuest()
    {
        foreach (Quest qst in Instance.Game.PlayerInTurn.Quests)
        {
            switch (qst.Status)
            {
                case QuestStatus.Completed:
                    if (!CompletedQuestsQueue.Contains(qst))
                    {
                        Sprite questPortrait = QuestInfo.GetPortraitImage(qst);
                        UIManager.Instance.SetQuestComplete(qst);
                        UIManager.Instance.QuestComplete.SetActive(true);
                        StartCoroutine(PlayQuestSound(qst, HolySound.GetComponent<AudioSource>().clip.length));
                        //PlayQuestSoundVoice(qst);
                        AlarmManager.Instance.AddAlarm(questPortrait,
                                                       qst.TextName + " 완료됨",
                                                       delegate {
                                                           UIManager.Instance.mapUI.SetActive(false);
                                                           UIManager.Instance.questUI.SetActive(true);
                                                       },
                                                       0);
                        CompletedQuestsQueue.Add(qst);
                    }
                    break;
            }
        }

        GameUI.Instance.CheckEnd();
    }

    private int GetQuestNumber(Quest qst)
    {
        switch(QuestInfo.GetQuestName(qst))
        {
            case "hwan_main1":
                return 0;
            case "hwan_main2":
                return 1;
            case "hwan_main3":
                return 2;
            case "hwan_sub1":
                return 3;
            case "hwan_sub2":
                return 4;
            case "finno_main1":
                return 5;
            case "finno_main2":
                return 6;
            case "finno_main3":
                return 7;
            case "finno_sub1":
                return 8;
            case "finno_sub2":
                return 9;
            default:
                return 10;
        }
    }

    // Check if there exist production that have finished.
    public void CheckCompletedProduction()
    {
        foreach (Production prod in GameManager.Instance.Game.PlayerInTurn.Deployment)
        {
            if (!AlarmedProduction.Contains(ProductionFactoryTraits.GetFacPortName(prod.Factory)))
            {
                Sprite prodPortrait = Resources.Load<Sprite>("Portraits/" + ProductionFactoryTraits.GetFacPortName(prod.Factory));
                AlarmManager.Instance.AddAlarm(prodPortrait,
                                               ProductionFactoryTraits.GetFactoryName(prod.Factory) + " 배치 가능",
                                               delegate {
                                                   UIManager.Instance.mapUI.SetActive(false);
                                                   UIManager.Instance.managementUI.SetActive(true);
                                               },
                                               0);
                AlarmedProduction.Add(ProductionFactoryTraits.GetFacPortName(prod.Factory));
            }
        }
    }

    bool IsSpyNear(CivModel.Position pt)
    {
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
    bool CheckSpy(int A, int B, int C)
    {
        if (C < 0 || C >= Game.Terrain.Height)
            return false;
        if (0 <= B + (C + Math.Sign(C)) / 2 && B + (C + Math.Sign(C)) / 2 < Game.Terrain.Width)
        {
            if (Game.Terrain.GetPoint(A, B, C).Unit != null)
            {
                if (Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Hwan.Spy || Game.Terrain.GetPoint(A, B, C).Unit is CivModel.Finno.Spy)
                {
                    if (Game.Terrain.GetPoint(A, B, C).Unit.Owner != Game.PlayerInTurn)
                        return true;
                }
            }
        }
        else
        {
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

    IEnumerator PlayQuestSound(Quest qst, float delay)
    {
        PauseBGM();
        PlayHolySound();
        if (UIManager.Instance.QuestComplete.activeSelf)
        {
            yield return new WaitForSeconds(delay);
            PlayQuestSoundVoice(qst);
        }
    }


    private void PlayQuestSoundVoice(Quest qst)
    {
        if (UIManager.Instance.QuestComplete.activeSelf)
        {
            int n = GetQuestNumber(qst);
            if (n == 10) return;
            System.Random rand = new System.Random();
            int doyouknowidx = rand.Next(0, 10);
            PauseBGM();
            float dyklength = PlayDoYouKnow(doyouknowidx) - 1f;
            if (UIManager.Instance.QuestComplete.activeSelf)
            {
                switch (n)
                {
                    case 0:
                        Invoke("PlayHM1QuestVoice", dyklength);
                        break;
                    case 1:
                        Invoke("PlayHM2QuestVoice", dyklength);
                        break;
                    case 2:
                        Invoke("PlayHM3QuestVoice", dyklength);
                        break;
                    case 3:
                        Invoke("PlayHS1QuestVoice", dyklength);
                        break;
                    case 4:
                        Invoke("PlayHS2QuestVoice", dyklength);
                        break;
                    case 5:
                        Invoke("PlayFM1QuestVoice", dyklength);
                        break;
                    case 6:
                        Invoke("PlayFM2QuestVoice", dyklength);
                        break;
                    case 7:
                        Invoke("PlayFM3QuestVoice", dyklength);
                        break;
                    case 8:
                        Invoke("PlayFS1QuestVoice", dyklength);
                        break;
                    case 9:
                        Invoke("PlayFS2QuestVoice", dyklength);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PlayHolySound()
    {
        HolySound.GetComponent<AudioSource>().Play();
    }

    private float PlayDoYouKnow(int idx)
    {
        DoYouKnows.transform.GetChild(idx).GetComponent<AudioSource>().Play();
        return DoYouKnows.transform.GetChild(idx).GetComponent<AudioSource>().clip.length;
    }

    private void PlayHM1QuestVoice()
    {
        QuestVoice.transform.GetChild(0).GetComponent<AudioSource>().Play();
    }
    private void PlayHM2QuestVoice()
    {
        QuestVoice.transform.GetChild(1).GetComponent<AudioSource>().Play();
    }
    private void PlayHM3QuestVoice()
    {
        QuestVoice.transform.GetChild(2).GetComponent<AudioSource>().Play();
    }
    private void PlayHS1QuestVoice()
    {
        QuestVoice.transform.GetChild(3).GetComponent<AudioSource>().Play();
    }
    private void PlayHS2QuestVoice()
    {
        QuestVoice.transform.GetChild(4).GetComponent<AudioSource>().Play();
    }
    private void PlayFM1QuestVoice()
    {
        QuestVoice.transform.GetChild(5).GetComponent<AudioSource>().Play();
    }
    private void PlayFM2QuestVoice()
    {
        QuestVoice.transform.GetChild(6).GetComponent<AudioSource>().Play();
    }
    private void PlayFM3QuestVoice()
    {
        QuestVoice.transform.GetChild(7).GetComponent<AudioSource>().Play();
    }
    private void PlayFS1QuestVoice()
    {
        QuestVoice.transform.GetChild(8).GetComponent<AudioSource>().Play();
    }
    private void PlayFS2QuestVoice()
    {
        QuestVoice.transform.GetChild(9).GetComponent<AudioSource>().Play();
    }
    private void PauseBGM()
    {
        if (BGMs.transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
            BGMs.transform.GetChild(0).GetComponent<AudioSource>().Pause();
        if (BGMs.transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
            BGMs.transform.GetChild(1).GetComponent<AudioSource>().Pause();
    }
    public void StopQuestVoice()
    {
        for(int i = 0; i < 10; i++)
        {
            if (QuestVoice.transform.GetChild(i).GetComponent<AudioSource>().isPlaying)
                QuestVoice.transform.GetChild(i).GetComponent<AudioSource>().Stop();
        }
    }
}