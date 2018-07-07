using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

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

	public GameObject UnitPrefab;
	private List<GameObject> _units = new List<GameObject>();
	public List<GameObject> Units { get { return _units; } }

	public Material[] materials;

    public CivModel.Unit selectedUnit;

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
		_game = new CivModel.Game(".\\Assets\\map.txt", factories);
		_game.StartTurn();
	}

	// Use this for initialization
	void Start() {
		InitiateMap();

	}

	// Update is called once per frame
	void Update() {

	}


	private void UpdateMapUnit() {
		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {
				CivModel.Terrain.Point point = _game.Terrain.GetPoint(i, j);

				HexTile tile = _tiles[i, j].GetComponent<HexTile>();
				tile.SetTerrain();
				tile.SetBuilding();
			}
		}
        foreach (var unt in _units){
            
        }
	}

	private void InitiateMap() {
		_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];

		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {

                Vector3 pos = ModelPntToUnityPnt(i, j, -0.05f);

				_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
				_tiles[i, j].name = String.Format("Tile({0},{1})", i, j);
                CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
                _tiles[i, j].GetComponent<HexTile>().SetPoints(pnt, pos);
				if (pnt.Unit != null)
					InitiateUnit(pnt.Unit);
			}
		}
	}

	private void InitiateUnit(CivModel.Unit unit) {
		if (unit?.PlacedPoint != null) {
			var pt = unit.PlacedPoint.Value;

            Vector3 pos = ModelPntToUnityPnt(pt, 1.25f);

			GameObject unt = Instantiate(UnitPrefab, pos, Quaternion.identity);
			unt.name = String.Format("Unit[{0},{1}]", pt.Position.X, pt.Position.Y);
            unt.GetComponent<Unit>().SetPoints(pt, pos);

            _units.Add(unt);
		}
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


}
