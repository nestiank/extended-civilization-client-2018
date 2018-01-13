using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;

public class CIVGameManager : MonoBehaviour {

    private class CIVPresenter : Presenter
    {
        public CIVPresenter(IView view) : base(view)
        {

        }
    }
    private class CIVView : IView
    {
        public void MoveSight(int dx, int dy)
        {
            throw new System.NotImplementedException();
        }

        public void Refocus()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Render()
        {

        }
    }


    public int width { get; set; }
    public int height { get; set; }

    public GameObject cellPrefab;
    private GameObject[,] cells;
    private CivModel.Terrain gameMapModel;
    private CIVPresenter mPresenter;


    private static GameObject gameManager;


    public Text gold, population, happiness, labor, technology, ultimate;
    private int goldnum, popnum, happynum, labnum, technum, ultnum;


    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);
        //System.Diagnostics.Debug.Assert(gameObject == null);
        if (gameManager == null)
        {
            gameManager = this.gameObject;
            width = 100;
            height = 100;
            mPresenter = new CIVPresenter(new CIVView());
            gameMapModel = mPresenter.Game.Terrain;
            cells = new GameObject[width, height];
            drawMap();
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
	void Update () {

        gold.text = "금: " + goldnum.ToString();
        population.text = "인구: " + popnum.ToString();
        happiness.text = "행복도: " + happynum.ToString();
        labor.text = "노동력: " + labnum.ToString();
        technology.text = "기술력: " + technum.ToString();
        ultimate.text = "궁극기: " + ultnum.ToString() + " %";
    }

    void drawMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = new Vector2(2 * i * HexMatrix.innerRadius, (-j) * HexMatrix.outerRadius * 1.5f);
                if (j % 2 != 0)
                {
                    pos.x += HexMatrix.innerRadius;
                }
                cells[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity);
            }
        }
    }
}
