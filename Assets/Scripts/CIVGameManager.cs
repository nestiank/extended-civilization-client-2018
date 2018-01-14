using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;

public class CIVGameManager : MonoBehaviour, IView {

    public enum DistrictSprite { None, CityCenter }
    public static Sprite[] DistrictSprites => districtSprites;
    private static Sprite[] districtSprites;

    public enum UnitSprite { None, Pioneer }
    public static Sprite[] UnitSprites => unitSprites;
    private static Sprite[] unitSprites;

    public enum TileSprite { None, Blue, Green, Red, Yellow }
    public static Sprite[] TileSprites => tileSprites;
    private static Sprite[] tileSprites;

    public void MoveSight(int dx, int dy)
    {
        mainCamera.transform.Translate(new Vector3(dx, dy, 0));
    }

    public void Refocus()
    {
    }
    public void Focus(Unit unit)
    {
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

    public void SelectCell(GameObject go)
    {
        cellSelected = go;
    }

    public void Skill()
    {
        mPresenter.CommandSpecialAct(0);
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
            Width = 100;
            Height = 100;
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
            Focus(new Position { X = 50, Y = 50 }); //testcase
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
        if (Input.GetKeyDown(KeyCode.Escape))
            mPresenter.CommandCancel();
        if (Input.GetKey(KeyCode.UpArrow))
            mPresenter.CommandArrowKey(Direction.Down);
        if (Input.GetKey(KeyCode.DownArrow))
            mPresenter.CommandArrowKey(Direction.Up);
        if (Input.GetKey(KeyCode.LeftArrow))
            mPresenter.CommandArrowKey(Direction.Left);
        if (Input.GetKey(KeyCode.RightArrow))
            mPresenter.CommandArrowKey(Direction.Right);


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
