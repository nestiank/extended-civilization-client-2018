using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CivPresenter;
using CivModel;
using CivModel.Common;

public class CIVGameManager : MonoBehaviour, IView {

    private const float cameraSpeed = 12.0f;

    public enum DistrictSprite { None, CityCenter }
    public static Sprite[] DistrictSprites => districtSprites;
    private static Sprite[] districtSprites;

    public enum UnitSprite { None, Pioneer }
    public static Sprite[] UnitSprites => unitSprites;
    private static Sprite[] unitSprites;

    public enum TileSprite { None, Blue, Green, Red, Yellow, Orange, Magenta, Gray, IronGreen, DarkPurple }
    public static Sprite[] TileSprites => tileSprites;
    private static Sprite[] tileSprites;

    public GameObject cellSelected = null;
    public CivModel.Terrain.Point? pointSelected;
    
    public int Width { get; set; }
    public int Height { get; set; }

    private Button[] cameraButtons;

    public GameObject cellPrefab;
    public Camera mainCamera;
    private Canvas cameraUI;
    public GameObject focusObject;
    public Button turnEndButton;

    private GameObject[,] cells;

    private Presenter mPresenter;
    private Game game;
    private IReadOnlyList<CivModel.Player> players;
    private Player mainPlayer;
    //private CivModel.Terrain gameMapModel;

    private static GameObject gameManager;

    public Text gold, population, happiness, labor, technology, ultimate;
    private int goldnum, popnum, happynum, labnum, technum, ultnum;

    /*public void MoveSight(int dx, int dy)
    {
        mainCamera.transform.Translate(new Vector3(dx, dy, 0));
        //버그 있음. 움직일 시 아래로 내려갈수록 지면에 가까워짐. (rotation 문제)
    }*/

    public void Refocus()
    {
        Debug.Log("Refocus Called");
        if (pointSelected.HasValue)
        {
            Focus(pointSelected);
            Debug.Log("Focusing" + pointSelected);
        }
    }
    public void Focus(Unit unit)
    {
        if(unit.PlacedPoint.HasValue)
        {
            Focus(unit.PlacedPoint.Value);
        }
    }
    public void Focus(CivModel.Terrain.Point? pt)
    {
        if(pt.HasValue)
        {
            Focus(pt.Value.Position);
        }
    }
    public void Focus(Position pos)
    {
        CameraChange(cells[pos.X, pos.Y].transform.position);
        Debug.Log(pos.X + " " + pos.Y);
    }

    public void Shutdown()
    {
        Application.Quit();
    } 

    public void Render(CivModel.Terrain map)
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                CivModel.Terrain.Point point = map.GetPoint(i, j);
                cells[i, j].GetComponent<TilePrefab>().ChangeTile(point);
                cells[i, j].GetComponent<TilePrefab>().BuildDistrict(point.TileBuilding);
                cells[i, j].GetComponent<TilePrefab>().DrawUnit(point.Unit);
            }
        }
    }

    public static GameObject GetGameManager()
    {
        if(gameManager == null)
        {
            throw new NullReferenceException();
        }
        else
        {
            return gameManager;
        }
    }


    public void CastRay()
    {
        cellSelected = null;
        Vector2 pos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,20f));
        
        /* 현재 버그 존재 : camera가 orthographic이 아닌 perspective일 경우 정확한 거리값을 Vector3 값의 세 번째 자리에 넣어야 함. 
        Camera가 Rotate 되어 있을 경우 계산이 필요한지 다른 방식으로 변경할지는 추후 고려*/

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);


        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name + "Castray");
            SelectCell(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("notselected");
            cellSelected = null;
            pointSelected = null;
        }

    }

    public void SelectCell(GameObject go)
    {
        cellSelected = go;
        pointSelected = FindCell(cellSelected);
        Debug.Log(pointSelected);
    }
    public CivModel.Terrain.Point FindCell(GameObject go)
    {
        if (go == null)
        {
            Debug.Log("no Gameobject");
            throw new ArgumentNullException();
        }
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (cells[i, j] == go)
                {
                    return mPresenter.Game.Terrain.GetPoint(i, j);
                }
            }
        }
        throw new ArgumentNullException();
    }
    public GameObject FindCell(CivModel.Terrain.Point pt)
    {
        return cells[pt.Position.X, pt.Position.Y];
    }
    /*public void Skill()
    {
        if(mPresenter.SelectedActor.GetType() == typeof(Pioneer))
            mPresenter.SelectedActor.SpecialActs[0].Act(mPresenter.SelectedActor.PlacedPoint);
    }*/

    private void CameraChange(Vector3 vec)
    {
        mainCamera.transform.position = new Vector3(vec.x, vec.y, -20);
        Debug.Log(vec);
    }
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (gameManager == null)
        {
            gameManager = this.gameObject;
            mPresenter = new Presenter(this);
        }
        else
        {
            Destroy(this);
        }
    }
        // Use this for initialization
     void Start()
    {
        //System.Diagnostics.Debug.Assert(gameObject == null);
        if (gameManager == this.gameObject)
        {
            game = mPresenter.Game;
            players = game.Players;
            mainPlayer = players[0];
            Width = game.Terrain.Width;
            Height = game.Terrain.Height;
            Canvas[] acanvas = FindObjectsOfType<Canvas>();
            foreach(Canvas can in acanvas)
            {
                switch(can.name)
                {
                    case "CameraUI": cameraUI = can;
                        break;
                }
            }
            cameraButtons = cameraUI.GetComponentsInChildren<Button>();
            //gameMapModel = mPresenter.Game.Terrain;

            var dss = Enum.GetValues(typeof(DistrictSprite));
            districtSprites = new Sprite[dss.Length];
            districtSprites[0] = new Sprite();
            for (int i = 1; i < dss.Length; i++)
            {
                districtSprites[i] = Resources.Load<Sprite>("District/" + dss.GetValue(i).ToString());
            }

            var uss = Enum.GetValues(typeof(UnitSprite));
            unitSprites = new Sprite[uss.Length];
            unitSprites[0] = new Sprite();
            for (int i = 1; i < uss.Length; i++)
            {
                unitSprites[i] = Resources.Load<Sprite>("Unit/" + uss.GetValue(i).ToString());
            }

            var tss = Enum.GetValues(typeof(TileSprite));
            tileSprites = new Sprite[tss.Length];
            tileSprites[0] = Resources.Load<Sprite>("Tile_Texture/None");
            for (int i = 1; i < tss.Length; i++)
            {
                tileSprites[i] = Resources.Load<Sprite>("Tile_Texture/" + tss.GetValue(i).ToString());
            }

            cells = new GameObject[Width, Height];
            DrawMap();
            ManagementUIController.PrefabsSetting();
        }
        else
        {
            Destroy(this);
        }

        goldnum = Convert.ToInt32(game.PlayerInTurn.Gold);
        popnum = 0; // not Implemented
        happynum = Convert.ToInt32(game.PlayerInTurn.Happiness);
        labnum = Convert.ToInt32(game.PlayerInTurn.Labor);
        technum = 0;  // not Implemented
        ultnum = 0; //not Implemented
        ButtonSetup();
    }

    // Update is called once per frame
    void Update()
    {
        float delt = Time.deltaTime;
        pointSelected = mPresenter.FocusedPoint;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                switch (mPresenter.State)
                {
                    case CivPresenter.Presenter.States.Normal:
                        {
                            CastRay();
                            if (cellSelected != null)
                            {
                                //Focus(pointSelected);
                            }
                            break;
                        }
                    case CivPresenter.Presenter.States.Move:
                        {
                            CastRay();
                            if (cellSelected != null)
                            {
                                mPresenter.FocusedPoint = pointSelected.Value;
                            }
                            mPresenter.CommandApply();
                            break;
                        }

                    case CivPresenter.Presenter.States.MovingAttack:
                        {
                            CastRay();
                            if (cellSelected != null)
                            {
                                mPresenter.FocusedPoint = pointSelected.Value;
                            }
                            mPresenter.CommandApply();
                            break;
                        }
                    case CivPresenter.Presenter.States.HoldingAttack:
                        {
                            CastRay();
                            if (cellSelected != null)
                            {
                                mPresenter.FocusedPoint = pointSelected.Value;
                            }
                            mPresenter.CommandApply();
                            break;
                        }

                    case CivPresenter.Presenter.States.SpecialAct:
                        {

                            break;
                        }

                    case CivPresenter.Presenter.States.Deploy:
                        {
                            CastRay();
                            if (cellSelected != null)
                            {
                                mPresenter.FocusedPoint = pointSelected.Value;
                            }
                            mPresenter.CommandApply();
                            break;
                        }
                    case CivPresenter.Presenter.States.ProductUI:
                        {


                            break;
                        }
                    case CivPresenter.Presenter.States.ProductAdd:
                        {
                            break;
                        }
                    case CivPresenter.Presenter.States.Victory:
                        {
                            break;
                        }
                    case CivPresenter.Presenter.States.Defeated:
                        {
                            break;
                        }
                    default: throw new NotImplementedException();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            mPresenter.CommandCancel();
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //mPresenter.CommandArrowKey(Direction.Up);
            mainCamera.transform.Translate(new Vector3(0, cameraSpeed * delt, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //mPresenter.CommandArrowKey(Direction.Down);
            mainCamera.transform.Translate(new Vector3(0, -cameraSpeed * delt, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //mPresenter.CommandArrowKey(Direction.Left);
            mainCamera.transform.Translate(new Vector3(-cameraSpeed * delt, 0, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //mPresenter.CommandArrowKey(Direction.Right);
            mainCamera.transform.Translate(new Vector3(cameraSpeed * delt, 0, 0));
        }
        if (Input.GetKey(KeyCode.F))
        {
            mPresenter.CommandRefocus();
            Debug.Log("refocus request");
        }
        /*if(Input.GetKey(KeyCode.S))
            //mPresenter.CommandSelect();

        if (Input.GetKey(KeyCode.D))
            //mPresenter.CommandRemove();
        if (Input.GetKey(KeyCode.M))
            //mPresenter.CommandMove();
        if (Input.GetKey(KeyCode.Z))
            //mPresenter.CommandSkip();
        if (Input.GetKey(KeyCode.Q))
            //mPresenter.CommandMovingAttack();
        if (Input.GetKey(KeyCode.W))
            //mPresenter.CommandHoldingAttack();
        if(Input.GetKey(KeyCode.P))
            //mPresenter.CommandProductUI();*/


        /*if (Input.GetKey(KeyCode.Keypad1))
            mPresenter.CommandNumeric(0);
        if (Input.GetKey(KeyCode.Keypad2))
            mPresenter.CommandNumeric(1);
        if (Input.GetKey(KeyCode.Keypad3))
            mPresenter.CommandNumeric(2);
        if (Input.GetKey(KeyCode.Keypad4))
            mPresenter.CommandNumeric(3);
        if (Input.GetKey(KeyCode.Keypad5))
            mPresenter.CommandNumeric(4);
        if (Input.GetKey(KeyCode.Keypad6))
            mPresenter.CommandNumeric(5);
        if (Input.GetKey(KeyCode.Keypad7))
            mPresenter.CommandNumeric(6);
        if (Input.GetKey(KeyCode.Keypad8))
            mPresenter.CommandNumeric(7);
        if (Input.GetKey(KeyCode.Keypad9))
            mPresenter.CommandNumeric(8);*/

        goldnum = Convert.ToInt32(game.PlayerInTurn.Gold);
        popnum = 0; // not Implemented
        happynum = Convert.ToInt32(game.PlayerInTurn.Happiness);
        labnum = Convert.ToInt32(game.PlayerInTurn.Labor);
        technum = 0;  // not Implemented
        ultnum = 0; //not Implemented

        gold.text = "금: " + goldnum.ToString();
        population.text = "인구: " + popnum.ToString();
        happiness.text = "행복도: " + happynum.ToString();
        labor.text = "노동력: " + labnum.ToString();
        technology.text = "기술력: " + technum.ToString();
        ultimate.text = "궁극기: " + ultnum.ToString() + " %";
        
        Render(mPresenter.Game.Terrain);

        Debug.Log("Turn : " + game.TurnNumber + ", Player : " + game.PlayerNumberInTurn);
        Debug.Log(mPresenter.SelectedActor);
        switch (mPresenter.State)//for debug
        {
            case CivPresenter.Presenter.States.Normal:
                {
                    Debug.Log("State : Normal");
                    break;
                }
            case CivPresenter.Presenter.States.Move:
                {
                    Debug.Log("State : Move");
                    CivModel.Terrain.Point?[] adj = mPresenter.SelectedActor.PlacedPoint.Value.Adjacents();
                    for(int i = 0; i < adj.Length; i++)
                    {
                        if(adj[i].HasValue)
                        {
                            //if(!(adj[i].Value.Type == TerrainType.Ocean || adj[i].Value.Type == TerrainType.Mount))
                            FindCell(adj[i].Value).GetComponent<TilePrefab>().MovableTile();
                        }
                    }
                    break;
                }

            case CivPresenter.Presenter.States.MovingAttack:
                {
                    Debug.Log("State : MovingAttack");
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if (mPresenter.RunningAction.IsActable(mPresenter.Game.Terrain.GetPoint(i, j)))
                            {
                                cells[i, j].GetComponent<TilePrefab>().AttackableTile();
                            }
                        }
                    }
                    break;
                }
            case CivPresenter.Presenter.States.HoldingAttack:
                {
                    Debug.Log("State : HoldingAttack");
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if (mPresenter.RunningAction.IsActable(mPresenter.Game.Terrain.GetPoint(i, j)))
                            {
                                cells[i, j].GetComponent<TilePrefab>().AttackableTile();
                            }
                        }
                    }
                    break;
                }

            case CivPresenter.Presenter.States.SpecialAct:
                {
                    Debug.Log("State : SpecialAct");
                    break;
                }

            case CivPresenter.Presenter.States.Deploy:
                {
                    Debug.Log("State : Deploy");
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if(mPresenter.DeployProduction.IsPlacable(mPresenter.Game.Terrain.GetPoint(i,j)))
                            {
                                cells[i,j].GetComponent<TilePrefab>().MovableTile();
                            }
                        }
                    }
                    break;
                }
            case CivPresenter.Presenter.States.ProductUI:
                {
                    Debug.Log("State : ProductUI");
                    break;
                }
            case CivPresenter.Presenter.States.ProductAdd:
                {
                    Debug.Log("State : ProductAdd");
                    break;
                }
            case CivPresenter.Presenter.States.Victory:
                {
                    Debug.Log("State : Victory");
                    break;
                }
            case CivPresenter.Presenter.States.Defeated:
                {
                    Debug.Log("State : Defeated");
                    break;
                }
            default:
                throw new NotImplementedException();
        }
        if (!mPresenter.IsThereTodos)
        {
            turnEndButton.GetComponentInChildren<Text>().text = "턴 종료";
        }
        else
        {
            turnEndButton.GetComponentInChildren<Text>().text = "다음 캐릭터";
        }
    }

    // HEX Tiling 
    void DrawMap()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector2 pos = new Vector2(2 * i * HexMatrix.innerRadius, (-j) * HexMatrix.outerRadius * 1.5f);
                if (j % 2 != 0)
                {
                    pos.x -= HexMatrix.innerRadius;
                }
                cells[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity);
                cells[i, j].name = String.Format("HexCell({0},{1})", i, j);
            }
        }
    }

    public Presenter GetPresenter()
    {
        return mPresenter;
    }
    
    public void TurnEndSignal()
    {
        if(!mPresenter.IsThereTodos)
        {
            Debug.Log("End_CommandApply");
            mPresenter.CommandApply();
        }
        else
        {
            Debug.Log("Todo is remaining");
            mPresenter.CommandApply();
        }
    }

    public void ButtonSetup()
    {
        foreach(Button btn in cameraButtons)
        {
            switch(btn.name)
            {
                case "Attack":
                    Debug.Log(btn.name + " onclick addlistener");
                    btn.onClick.AddListener(delegate () { AttackButtonMethod(); });
                    break;
                default:
                    Debug.Log(btn.name);break;
            }
        }   
    }

    public void AttackButtonMethod()
    {
        if (mPresenter.State == CivPresenter.Presenter.States.Normal)
        {
            Debug.Log("Attack Button");
            mPresenter.CommandMovingAttack();
            if (mPresenter.State != CivPresenter.Presenter.States.MovingAttack)
            {
                mPresenter.CommandHoldingAttack();
            }
        }
    }
}

public class ProductionFactoryTraits
{
    public static string GetFactoryName(IProductionFactory factory)
    {
        if (factory == PioneerProductionFactory.Instance)
            return "Pioneer";
        else if (factory == JediKnightProductionFactory.Instance)
            return "JediKnight";
        else
            return "null";
    }
}
