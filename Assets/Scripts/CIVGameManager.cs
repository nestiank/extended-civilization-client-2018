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

    private const float cameraSpeed = 4.0f;

    public enum DistrictSprite { None, CityCenter }
    public static Sprite[] DistrictSprites => districtSprites;
    private static Sprite[] districtSprites;

    public enum UnitSprite { None, Pioneer }
    public static Sprite[] UnitSprites => unitSprites;
    private static Sprite[] unitSprites;

    public enum TileSprite { None, Blue, Green, Red, Yellow }
    public static Sprite[] TileSprites => tileSprites;
    private static Sprite[] tileSprites;

    /*public void MoveSight(int dx, int dy)
    {
        mainCamera.transform.Translate(new Vector3(dx, dy, 0));
        //버그 있음. 움직일 시 아래로 내려갈수록 지면에 가까워짐. (rotation 문제)
    }*/

    public void Refocus()
    { 
        if (pointSelected.HasValue)
        {
            Focus(pointSelected);
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

    private GameObject cellSelected = null;
    private CivModel.Terrain.Point? pointSelected;
    private bool readyToClick = false;

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
            readyToClick = false;
        }
        else
            Debug.Log("notselected");

    }

    public void SelectCell(GameObject go)
    {
        cellSelected = go;
        pointSelected = FindCell(cellSelected);
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
    public void Skill()
    {
        if(mPresenter.SelectedActor.GetType() == typeof(Pioneer))
            mPresenter.SelectedActor.SpecialActs[0].Act(mPresenter.SelectedActor.PlacedPoint);
    }

    private void CameraChange(Vector3 vec)
    {
        mainCamera.transform.position = new Vector3(vec.x, vec.y, -20);
        Debug.Log(vec);
    }

    public int Width { get; set; }
    public int Height { get; set; }

    public GameObject cellPrefab;
    public Camera mainCamera;
    public GameObject focusObject;


    private GameObject[,] cells;

    private Presenter mPresenter;
    //private CivModel.Terrain gameMapModel;
    //private IReadOnlyList<CivModel.Player> players;


    private static GameObject gameManager;

    public Text gold, population, happiness, labor, technology, ultimate;
    private int goldnum, popnum, happynum, labnum, technum, ultnum;


    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        //System.Diagnostics.Debug.Assert(gameObject == null);
        if (gameManager == null)
        {
            gameManager = this.gameObject;
            Width = 10;
            Height = 10;
            mPresenter = new Presenter(this);
            //gameMapModel = mPresenter.Game.Terrain;
            //players = mPresenter.Game.Players;

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
            Focus(new Position { X = 5, Y = 5 }); //testcase
        }
        else
        {
            Destroy(this);
        }

        goldnum = 10000;
        popnum = 1482;
        happynum = 42;
        labnum = 0;
        technum = 124;
        ultnum = 0;
    }

    // Update is called once per frame
    void Update() {
        float delt = Time.deltaTime;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                CastRay();
                if(cellSelected != null)
                {
                    Focus(pointSelected);
                }
            }
                
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            mPresenter.CommandCancel();
        if (Input.GetKey(KeyCode.UpArrow))
        {
            mPresenter.CommandArrowKey(Direction.Up);
            mainCamera.transform.Translate(new Vector3(0, cameraSpeed * delt, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mPresenter.CommandArrowKey(Direction.Down);
            mainCamera.transform.Translate(new Vector3(0, -cameraSpeed * delt, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            mPresenter.CommandArrowKey(Direction.Left);
            mainCamera.transform.Translate(new Vector3(-cameraSpeed * delt, 0, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            mPresenter.CommandArrowKey(Direction.Right);
            mainCamera.transform.Translate(new Vector3(cameraSpeed * delt, 0, 0));
        }
        if (Input.GetKey(KeyCode.F))
            mPresenter.CommandRefocus();


        gold.text = "금: " + goldnum.ToString();
        population.text = "인구: " + popnum.ToString();
        happiness.text = "행복도: " + happynum.ToString();
        labor.text = "노동력: " + labnum.ToString();
        technology.text = "기술력: " + technum.ToString();
        ultimate.text = "궁극기: " + ultnum.ToString() + " %";

        Render(mPresenter.Game.Terrain);
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
                    pos.x += HexMatrix.innerRadius;
                }
                cells[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity);
                cells[i, j].name = String.Format("HexCell({0},{1})", i, j);
            }
        }
    }


    
    public void TurnEndSignal()
    {
        mPresenter.CommandApply();
    }
    
}
