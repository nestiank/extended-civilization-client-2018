using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;
using System.Threading.Tasks;

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
    public IQuestObserver QuestObserver;

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
        //_game = new CivModel.Game(GameInfo.mapWidth, GameInfo.mapHeight, GameInfo.numOfPlayer, new GameSchemeFactory()/*, new IGameSchemeFactory[] { new CivModel.AI.GameSchemeFactory()}*/);
        var factories = new IGameSchemeFactory[] {
            new CivModel.Common.GameSchemeFactory(),
            new CivModel.Hwan.GameSchemeFactory(),
            new CivModel.Finno.GameSchemeFactory(),
            new CivModel.Quests.GameSchemeFactory(),
            new CivModel.Zap.GameSchemeFactory(),
            new CivModel.AI.GameSchemeFactory()
        };
        _game = new CivModel.Game(".\\Assets\\map.txt", factories);
        _game.StartTurn();

    }
    void Start() {
        // Instantiate game

        // Map tiling
        innerRadius = outerRadius * Mathf.Sqrt(3.0f) / 2;

        ObserverSet();
        DrawMap();

        _game.Players[1].IsAIControlled = true;
        
        ProceedTurn();
    }
	
	// Update is called once per frame
	void Update() {
        Render(_game.Terrain);
        Debug.Log(Game.PlayerNumberInTurn);
        //Debug.Log("Gold:" + _game.PlayerInTurn.Gold + "(+" + _game.PlayerInTurn.GoldIncome +")");
        //Debug.Log("Pop:" + _game.PlayerInTurn.Population);
        //Debug.Log("Happ:" + _game.PlayerInTurn.Happiness);
        //Debug.Log("Prod:" + _game.PlayerInTurn.Labor);
        //Debug.Log("Tech:" + _game.PlayerInTurn.Research);
        if (CheckVictory())
        {
            UIManager.I.GameEnd();
        }
        else
        {
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
                            //공격 가능한 놈이 있을 때
                            if (SelectedActor.MovingAttackAct != null && SelectedActor.MovingAttackAct.IsActable(tile.point))
                            {
                                SelectedActor.MovingAttackAct.Act(tile.point);
                                PseudoFSM.I.NormalStateEnter();
                            }
                            else
                            {
                                Move(tile.point);
                            }
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
                            if (SelectedActor.HoldingAttackAct != null && SelectedActor.HoldingAttackAct.IsActable(tile.point))
                            {
                                SelectedActor.HoldingAttackAct.Act(tile.point);
                                PseudoFSM.I.NormalStateEnter();
                            }
                            else
                            {
                                ////Debug.Log("잘못된 공격 대상");
                                PseudoFSM.I.NormalStateEnter();
                            }
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

    /*public void BuildBuilding(CivModel.Terrain.Point pos, TileBuilding building)
    {

    }*/
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
                _cells[i, j].GetComponent<HexTile>().BuildDistrict(point.TileBuilding);
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
            //Debug.Log(_standbyUnitIndex);
            if (_standbyUnitIndex == -1)
            {
                _standbyUnits = _game.PlayerInTurn.Units.ToArray();
            }

            int idx = _standbyUnitIndex + 1;
            //Debug.Log(_standbyUnits.Length);
            for (; idx < _standbyUnits.Length; ++idx)
            {
                var unit = _standbyUnits[idx];
                if (unit.RemainAP > 0 && !unit.SkipFlag && unit.PlacedPoint.HasValue)
                {
                    _standbyUnitIndex = idx;
                    _selectedActor = _standbyUnits[idx];
                    isThereTodos = true;
                    //Debug.Log(_selectedActor.PlacedPoint);
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
        //Debug.Log(unit.ToString() + " selected");
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
    public async Task ProceedTurn()
    {
        while (true)
        {
            if (_game.IsInsideTurn)
                _game.EndTurn();
            _game.StartTurn();

            if (_game.PlayerInTurn.IsAIControlled)
            {
                await _game.PlayerInTurn.DoAITurnAction();
            }
            else
            {
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
                UIManager.I.MakeUnitInfo();
                break;
            }
        }
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
        UIManager.I.MakeUnitInfo();
    }

    void Deploy(CivModel.Terrain.Point point, Production dep)
    {
        dep.Place(point);
        Game.PlayerInTurn.Deployment.Remove(dep);
        PseudoFSM.I.NormalStateEnter();
        UIManager.I.MakeUnitInfo();
    }
    //승리 확인 함수(From Presenter)
    private bool CheckVictory()
    {
        var survivors = Game.Players.Where(player => !player.IsDefeated);
        if (survivors.Count() <= 1)
        {
            PseudoFSM.I.NormalStateEnter();
            return true;
        }
        return false;
    }
    private void ObserverSet()
    {
        QuestObserver = new CivQuestObserver();
        _game.QuestObservable.AddObserver(QuestObserver);
    }
}
public class CivQuestObserver : IQuestObserver
{
    public void QuestAccepted(Quest quest)
    {
        return;
    }

    public void QuestCompleted(Quest quest)
    {
        UIManager.I.ShowQuestEnd(quest);
    }

    public void QuestGivenup(Quest quest)
    {
        return;
    }
}
public static class ProductionFactoryTraits
{
    public static string GetFactoryName(CivModel.IProductionFactory Factory)
    {
        char[] sep = { '.' };
        string name = Factory.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "PioneerProductionFactory":
                result = "개척자";
                break;
            case "JediKnightProductionFactory":
                result = "제다이 기사";
                break;
            case "FakeKnightProductionFactory":
                result = "가짜 기사(테스팅)";
                break;
            case "BrainwashedEMUKnightProductionFactory":
                result = "세뇌된 에뮤 기사";
                break;
            case "DecentralizedMilitaryProductionFactory":
                result = "탈중앙화된 군인";
                break;
            case "JackieChanProductionFactory":
                result = "재키 찬";
                break;
            case "LEOSpaceArmadaProductionFactory":
                result = "저궤도 우주 함대";
                break;
            case "ProtoNinjaProductionFactory":
                result = "프로토-닌자";
                break;
            case "UnicornOrderProductionFactory":
                result = "유니콘 기사단";
                break;
            case "SpyProductionFactory":
                result = "스파이";
                break;
            case "AncientSorcererProductionFactory":
                result = "고대 소서러";
                break;
            case "AutismBeamDroneFactory":
                result = "O-ti-ism 빔 드론";
                break;
            case "ElephantCavalryProductionFactory":
                result = "코끼리 기병";
                break;
            case "EMUHorseArcherProductionFactory":
                result = "에뮤 궁기병";
                break;
            case "GenghisKhanProductionFactory":
                result = "징기즈 칸";
                break;
            case "ArmedDivisionProductionFactory":
                result = "기갑사단";
                break;
            case "InfantryDivisionProductionFactory":
                result = "보병사단";
                break;
            case "PadawanProductionFactory":
                result = "파다완";
                break;
            case "ZapNinjaProductionFactory":
                result = "닌자";
                break;
            case "CityCenterProductionFactory":
                result = "도심부";
                break;
            case "HwanEmpireCityProductionFactory":
                result = "환 제국 도시";
                break;
            case "HwanEmpireFIRFortressProductionFactory":
                result = "환 제국 4차 산업 요새";
                break;
            case "HwanEmpireCityCentralLabProductionFactory":
                result = "환 제국 도시 연구소";
                break;
            case "HwanEmpireFIRFactoryProductionFactory":
                result = "환 제국 4차 산업 요새";
                break;
            case "HwanEmpireIbizaProductionFactory":
                result = "환 제국 이비자";
                break;
            case "HwanEmpireKimchiFactoryProductionFactory":
                result = "환 제국 김치 군수공장";
                break;
            case "HwanEmpireLatifundiumProductionFactory":
                result = "환 제국 라티푼디움";
                break;
            case "AncientFinnoFineDustFactoryProductionFactory":
                result = "고대 수오미 제국 미세먼지 공장";
                break;
            case "AncientFinnoFIRFortressProductionFactory":
                result = "고대 수오미 제국 4차 산업 요새";
                break;
            case "AncientFinnoGermaniumMineProductionFactory":
                result = "고대 수오미 제국 게르마늄 광산";
                break;
            case "AncientFinnoOctagonProductionFactory":
                result = "고대 수오미 제국 옥타곤";
                break;
            case "FinnoEmpireCityProductionFactory":
                result = "고대 수오미 제국 도시";
                break;
            case "CasinoProductionFactory":
                result = "카지노";
                break;
            case "FIRFortressProductionFactory":
                result = "4차 산업 요새";
                break;
            case "ZapFactoryBuildingProductionFactory":
                result = "공장";
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
    public static string GetFacPortName(CivModel.IProductionFactory Factory)
    {
        char[] sep = { '.' };
        string name = Factory.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "PioneerProductionFactory":
                result = "Pioneer";
                break;
            case "JediKnightProductionFactory":
                result = "JediKnight";
                break;
            case "CityCenterProductionFactory":
                result = "CityCenter";
                break;
            case "FakeKnightProductionFactory":
                result = "JediKnight";
                break;
            case "FactoryBuildingProductionFactory":
                result = "Factory";
                break;
            case "LaboratoryBuildingProductionFactory":
                result = "Laboratory";
                break;
            case "BrainwashedEMUKnightProductionFactory":
                result = "BrainwashedEMUKnight";
                break;
            case "DecentralizedMilitaryProductionFactory":
                result = "DecentralizedMilitary";
                break;
            case "JackieChanProductionFactory":
                result = "JackieChan";
                break;
            case "LEOSpaceArmadaProductionFactory":
                result = "LEOSpaceArmada";
                break;
            case "ProtoNinjaProductionFactory":
                result = "ProtoNinja";
                break;
            case "UnicornOrderProductionFactory":
                result = "UnicornOrder";
                break;
            case "SpyProductionFactory":
                result = "Spy";
                break;
            case "AncientSorcererProductionFactory":
                result = "AncientSorcerer";
                break;
            case "AutismBeamDroneFactory":
                result = "AutismBeamDrone";
                break;
            case "ElephantCavalryProductionFactory":
                result = "ElephantCavalry";
                break;
            case "EMUHorseArcherProductionFactory":
                result = "EMUHorseArcher";
                break;
            case "GenghisKhanProductionFactory":
                result = "GenghisKhan";
                break;
            case "ArmedDivisionProductionFactory":
                result = "ArmedDivision";
                break;
            case "InfantryDivisionProductionFactory":
                result = "InfantryDivision";
                break;
            case "PadawanProductionFactory":
                result = "Padawan";
                break;
            case "ZapNinjaProductionFactory":
                result = "ZapNinja";
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
            case "LaboratoryBuilding":
                result = "연구소";
                break;
            case "BrainwashedEMUKnight":
                result = "세뇌된 에뮤 기사";
                break;
            case "DecentralizedMilitary":
                result = "탈중앙화된 군인";
                break;
            case "JackieChan":
                result = "재키 찬";
                break;
            case "LEOSpaceArmada":
                result = "저궤도 우주 함대";
                break;
            case "ProtoNinja":
                result = "프로토-닌자";
                break;
            case "UnicornOrder":
                result = "유니콘 기사단";
                break;
            case "Spy":
                result = "스파이";
                break;
            case "AncientSorcerer":
                result = "고대 소서러";
                break;
            case "AutismBeamDrone":
                result = "O-ti-ism 빔 드론";
                break;
            case "ElephantCavalry":
                result = "코끼리 기병";
                break;
            case "EMUHorseArcher":
                result = "에뮤 궁기병";
                break;
            case "GenghisKhan":
                result = "징기즈 칸";
                break;
            case "ArmedDivision":
                result = "기갑사단";
                break;
            case "InfantryDivision":
                result = "보병사단";
                break;
            case "Padawan":
                result = "파다완";
                break;
            case "ZapNinja":
                result = "닌자";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
}

public static class ParseQuest
{
    public static string GetQuestName(Quest qst)
    {
        string rlatmxmfld;
        switch (qst.Name)
        {
            case "개꿀잼 퀘스트":
                rlatmxmfld = "hwan_main2";
                break;
            case "불가사의 - 오티즘 빔 반사 어레이":
                rlatmxmfld = "hwan_main1";
                break;
            case "첩보 - 크툴루 계획":
                rlatmxmfld = "hwan_main2";
                break;
            case "불가사의- 이집트 캉덤":
                rlatmxmfld = "hwan_main3";
                break;
            case "[전쟁 동맹] - 에뮤 연방":
                rlatmxmfld = "finno_main1";
                break;
            case "[불가사의] - 아틀란티스":
                rlatmxmfld = "finno_main2";
                break;
            case "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙":
                rlatmxmfld = "finno_main3";
                break;
            case "군사 동맹 - 궤도 장악권":
                rlatmxmfld = "hwan_sub1";
                break;
            case "건물 기증 - 모아이 포스 필드":
                rlatmxmfld = "hwan_sub2";
                break;
            case "불가사의 - 성간 에너지":
                rlatmxmfld = "finno_sub1";
                break;
            case "불가사의 - 유전 연구학":
                rlatmxmfld = "finno_sub2";
                break;
            default:
                rlatmxmfld = "hwan_main1";
                break;
        }
        return rlatmxmfld;
    }
}