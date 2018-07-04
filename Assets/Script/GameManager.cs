using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class GameManager : MonoBehaviour {

	private CivModel.Game _game;
	public CivModel.Game Game { get { return _game; } }

	private static GameManager _manager = null;
	public static GameManager I { get { return _manager; } }

	public float outerRadius = 1f;  // Outer&inner radius of hex tile.
	public float innerRadius = Mathf.Sqrt(3) / 2;       // These variables can be deleted if there are no use.

	public GameObject HextilePrefab;
	private GameObject[,] _tiles;
	public GameObject[,] Tiles { get { return _tiles; } }

	public GameObject UnitPrefab;

	public Material[] materials;

	public Renderer rend;

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

	private void InitiateMap() {
		_tiles = new GameObject[_game.Terrain.Width, _game.Terrain.Height];

		for (int i = 0; i < _game.Terrain.Width; i++) {
			for (int j = 0; j < _game.Terrain.Height; j++) {
				Vector3 pos = new Vector3(2 * i * innerRadius, -0.05f, -j * outerRadius * 1.5f);
				if (j % 2 != 0) {
					pos.x -= innerRadius;
				}
				_tiles[i, j] = Instantiate(HextilePrefab, pos, Quaternion.identity);
				_tiles[i, j].name = String.Format("Tile({0},{1})", i, j);
				CivModel.Terrain.Point pnt = _game.Terrain.GetPoint(i, j);
				_tiles[i, j].GetComponent<HexTile>().point = pnt;
				if (pnt.Unit != null)
					InitiateUnit(_game.Terrain.GetPoint(i, j).Unit);
			}
		}
	}

	private void InitiateUnit(CivModel.Unit unit) {
		if (unit?.PlacedPoint != null) {
			var pt = unit.PlacedPoint.Value;
			Vector3 pos = new Vector3(2 * pt.Position.X * innerRadius, 1.25f, -pt.Position.Y * outerRadius * 1.5f);
			if ((pt.Position.Y % 2) != 0) {
				pos.x -= innerRadius;
			}
			GameObject unt = Instantiate(UnitPrefab, pos, Quaternion.identity);
			unt.name = String.Format("Unit[{0},{1}]", pt.Position.X, pt.Position.Y);
			unt.GetComponent<Unit>().point = pt;
			foreach (Material m in GameManager.I.materials) {
				if (m == GameManager.I.materials[(int)UnitEnum.UnitToEnum(unit)]) {
					unt.GetComponent<Renderer>().material = m;
				}
			}
		}
	}

}
