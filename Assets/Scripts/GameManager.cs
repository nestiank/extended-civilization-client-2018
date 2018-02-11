using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;

public class GameManager : MonoBehaviour {
    private static GameManager _manager = null;
    public static GameManager I { get { return _manager; } }

    // Current game
    private CivModel.Game _game;

    // Currently playing country


    public float outerRadius = 1f;  // Outer&inner radius of hex tile.
    public float innerRadius;       // These variables can be deleted if there are no use.

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

        // Instantiate game instance
        _game = new CivModel.Game( GameInfo.mapWidth, GameInfo.mapHeight, GameInfo.numOfPlayer, new CivModel.Common.GameSchemeFactory());

        // Map tiling
        innerRadius = outerRadius * Mathf.Sqrt(3.0f) / 2;
        _cells = new GameObject[GameInfo.mapWidth, GameInfo.mapHeight];
        DrawMap();
	}
	
	// Update is called once per frame
	void Update() {
        RenderTile(_game.Terrain);
	}

    void DrawMap()      // Instantiate hex tiles
    {
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
                _cells[i, j].name = "(" + i + "," + j + ")";
            }
        }
    }

    // Read game terrain and update hex tile resource
    void RenderTile(CivModel.Terrain terrain)
    {
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                // TODO: Make prefab component
                // _cells[i, j].GetComponent<TilePrefab>().ChangeTile(point);
                // _cells[i, j].GetComponent<TilePrefab>().BuildDistrict(point.TileBuilding);
                // _cells[i, j].GetComponent<TilePrefab>().DrawUnit(point.Unit);
            }
        }
    }
}
