using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class GameManager : MonoBehaviour
{
    private CivModel.Game _game;
    public CivModel.Game Game { get { return _game; } }

    public float outerRadius = 1f;  // Outer&inner radius of hex tile.
    public float innerRadius = Mathf.Sqrt(3) / 2;       // These variables can be deleted if there are no use.

    public GameObject HextilePrefab;
    private GameObject[,] _tiles;
    public GameObject[,] Tiles { get { return _tiles; } }

    void Awake()
    {
        var factories = new IGameSchemeFactory[] 
        {
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

    // Use this for initialization
    void Start ()
    {
        InitiateMap();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitiateMap()
    {
        _tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];

        for (int i = 0; i < _game.Terrain.Width; i++)
        {
            for (int j = 0; j < _game.Terrain.Height; j++)
            {
                Vector3 pos = new Vector3(2 * i * innerRadius, -0.05f, -j * outerRadius * 1.5f);
                if (j % 2 != 0)
                {
                    pos.x -= innerRadius;
                }
                _tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
                _tiles[i, j].name = String.Format("({0},{1})", i, j);
                _tiles[i, j].GetComponent<HexTile>().point = _game.Terrain.GetPoint(i, j);
                /*
                 * TODO
                 * hextile을 생성한 후, Terrian point 클래스의 값을 이용하여
                 * Hextile의 terrains, tilebuilding을 수정해야 함.
                 */
            }
        }

    }
}
